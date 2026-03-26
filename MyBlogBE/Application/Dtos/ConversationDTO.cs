using Application.Dtos;

public class LastMessageResponse
{
    public Guid MessageId { get; set; }
    public bool IsOwner { get; set; }
    public string Content { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ConversationsResponse
{
    public Guid ConversationId { get; set; }
    public AccountNameResponse Account { get; set; } = null!;
    public LastMessageResponse? LastMessage { get; set; } = null;
}

public class MessagesResponse
{
    public Guid MessageId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
