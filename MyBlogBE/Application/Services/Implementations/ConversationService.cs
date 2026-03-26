using Application.Dtos;
using Application.Exceptions;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObject.Entities;
using DataAccess.Extensions;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementations;

public class ConversationService : IConversationService
{
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private Guid AccountId => _jwtService.GetAccountInfo().Id;
    private readonly IMapper _mapper;

    public ConversationService(IJwtService jwtService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ConversationsResponse> CreateConversationAsync(Guid receiverId)
    {
        if (receiverId == AccountId)
        {
            throw new BadRequestException("CannotStartConversationWithYourself.");
        }

        // Check if a conversation already exists between the two users
        var existingConversation = await _unitOfWork
            .Conversations.ReadOnly()
            .Where(a =>
                (a.Account1Id == AccountId && a.Account2Id == receiverId)
                || (a.Account1Id == receiverId && a.Account2Id == AccountId)
            )
            .ProjectTo<ConversationsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            )
            .FirstOrDefaultAsync();

        if (existingConversation != null)
        {
            return existingConversation;
        }

        // Create a new conversation
        var newConversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Account1Id = AccountId,
            Account2Id = receiverId,
            CreatedAt = DateTime.UtcNow,
        };

        _unitOfWork.Conversations.Add(newConversation);
        await _unitOfWork.SaveChangesAsync();

        return await CreateConversationAsync(receiverId);
    }

    public async Task<List<ConversationsResponse>> GetConversationsListAsync()
    {
        var query = _unitOfWork
            .Conversations.ReadOnly()
            .Where(c => c.Account1Id == AccountId || c.Account2Id == AccountId)
            // Order conversations by the latest message's CreatedAt, if there is no message, use conversation's CreatedAt
            .OrderByDescending(c =>
                c.Messages.Where(m => m.DeletedAt == null && !m.IsHidden)
                    .Max(m => (DateTime?)m.CreatedAt)
                ?? c.CreatedAt
            )
            .ProjectTo<ConversationsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            );

        var result = await query.ToListAsync();

        // foreach (var item in result)
        // {
        //     item.LastMessage ??= new LastMessageResponse
        //     {
        //         Content = string.Empty,
        //         CreatedAt = DateTime.MinValue,
        //     };
        // }

        return result;
    }

    public async Task<(List<MessagesResponse>, DateTime?)> GetMessagesListAsync(
        Guid conversationId,
        DateTime? cursor,
        int pageSize = 20
    )
    {
        var query = _unitOfWork
            .Messages.ReadOnly()
            .Where(m =>
                m.ConversationId == conversationId
                && (
                    (m.SenderId == AccountId && !m.IsHidden)
                    || (m.SenderId != AccountId && !m.IsReceiverHidden)
                ) // If sender hides the message, only receiver still see it; if receiver hides the message, only sender still see it.
                // If both sender and receiver hide the message, no one can see it.
                && m.DeletedAt == null
            );

        if (cursor.HasValue)
        {
            query = query.Where(m => m.CreatedAt < cursor.Value);
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ProjectTo<MessagesResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            )
            .Take(pageSize + 1)
            .ToListAsync();

        Console.WriteLine(
            $"Fetched {messages.Count} messages for conversation {conversationId} with cursor {cursor}"
        );

        var nextCursor =
            messages.Count > pageSize ? messages[pageSize - 1].CreatedAt : (DateTime?)null;

        return (messages.Take(pageSize).ToList(), nextCursor);
    }

    public async Task ReadMessagesAsync(Guid conversationId)
    {
        // Only update messages that are not read and sent by the other user to avoid unnecessary updates
        var unreadMessages = await _unitOfWork
            .Messages.GetQuery()
            .Where(m => m.ConversationId == conversationId && !m.IsRead && m.SenderId != AccountId)
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task HideMessagesAsync(Guid messageId)
    {
        var message =
            await _unitOfWork
                .Messages.GetQuery()
                .Where(m => m.Id == messageId)
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException("MessageNotFound.");

        // If the current user is the sender, hide the message for the sender; otherwise, hide it for the receiver
        if (message.SenderId == AccountId)
        {
            message.IsHidden = true;
        }
        else
        {
            message.IsReceiverHidden = true;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SendMessagesAsync(Guid conversationId, string content)
    {
        var conversation =
            await _unitOfWork
                .Conversations.GetQuery()
                .Where(c => c.Id == conversationId && c.DeletedAt == null)
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException("ConversationNotFound.");

        if (conversation.Account1Id != AccountId && conversation.Account2Id != AccountId)
        {
            throw new BadRequestException("ConversationNotFound.");
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = AccountId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
        };

        _unitOfWork.Messages.Add(message);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteMessagesAsync(Guid messageId)
    {
        var message =
            await _unitOfWork
                .Messages.GetQuery()
                .Where(c => c.Id == messageId && c.SenderId == AccountId && c.DeletedAt == null)
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException("MessageNotFound.");

        _unitOfWork.Messages.Remove(message);
        await _unitOfWork.SaveChangesAsync();
    }
}
