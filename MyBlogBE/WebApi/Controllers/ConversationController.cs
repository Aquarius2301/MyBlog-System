using Application.Dtos;
using Application.Exceptions;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationController : BaseController
{
    private readonly IConversationService _service;

    public ConversationController(IConversationService service)
    {
        _service = service;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetConversationsList()
    {
        var messages = await _service.GetConversationsListAsync();

        return HandleResponse(Success(messages));
    }

    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetMessagesList(Guid id, [FromQuery] PaginationRequest request)
    {
        var messages = await _service.GetMessagesListAsync(id, request.Cursor, request.PageSize);

        return HandleResponse(
            Success(
                new PaginationResponse
                {
                    Items = messages.Item1,
                    Cursor = messages.Item2,
                    PageSize = request.PageSize,
                }
            )
        );
    }

    [HttpPost("{receiverId}")]
    public async Task<IActionResult> CreateConversation(Guid receiverId)
    {
        var res = await _service.CreateConversationAsync(receiverId);

        return HandleResponse(Success(res));
    }

    [HttpPut("{conversationId}")]
    public async Task<IActionResult> ReadMessage(Guid conversationId)
    {
        await _service.ReadMessagesAsync(conversationId);

        return HandleResponse(Success<object>(null));
    }

    [HttpPost("{conversationId}/send")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] string request)
    {
        if (string.IsNullOrEmpty(request))
        {
            throw new BadRequestException("MessageContentCannotBeEmpty.");
        }

        await _service.SendMessagesAsync(conversationId, request);

        return HandleResponse(Success<object>(null));
    }

    [HttpDelete("{messageId}/hide")]
    public async Task<IActionResult> HideMessage(Guid messageId)
    {
        await _service.HideMessagesAsync(messageId);

        return HandleResponse(Success<object>(null));
    }

    [HttpDelete("{messageId}/delete")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        await _service.DeleteMessagesAsync(messageId);

        return HandleResponse(Success<object>(null));
    }
}
