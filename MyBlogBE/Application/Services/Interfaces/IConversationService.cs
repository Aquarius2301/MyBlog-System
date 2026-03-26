using BusinessObject.Entities;

namespace Application.Services.Interfaces;

public interface IConversationService
{
    /// <summary>
    /// Creates a new conversation between the current user and the specified receiver. If a conversation already exists between the two users, it returns the existing conversation instead of creating a new one.
    /// </summary>
    /// <param name="receiverId">The ID of the user to start a conversation with.</param>
    /// <returns>The conversation response.</returns>
    Task<ConversationsResponse> CreateConversationAsync(Guid receiverId);

    Task<List<ConversationsResponse>> GetConversationsListAsync();
    Task<(List<MessagesResponse>, DateTime?)> GetMessagesListAsync(
        Guid conversationId,
        DateTime? cursor,
        int pageSize
    );

    Task ReadMessagesAsync(Guid conversationId);

    Task HideMessagesAsync(Guid messageId);

    Task SendMessagesAsync(Guid conversationId, string content);

    Task DeleteMessagesAsync(Guid messageId);
}
