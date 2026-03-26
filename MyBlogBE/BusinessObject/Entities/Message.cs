using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

public class Message
{
    /// <summary>
    /// Unique identifier for the message. This is the primary key for the Message entity.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the conversation to which this message belongs. This is a foreign key referencing the Conversation entity. It indicates which conversation this message is part of.
    /// </summary>
    [Required]
    public Guid ConversationId { get; set; }

    /// <summary>
    /// ID of the account that sent this message. This is a foreign key referencing the Account entity. It indicates which account sent this message.
    /// </summary>
    [Required]
    public Guid SenderId { get; set; }

    /// <summary>
    /// The content of the message.
    /// </summary>
    [Required]
    public string Content { get; set; } = null!;

    /// <summary>
    /// Indicates whether the message has been read by the receiver.
    /// </summary>
    [Required]
    public bool IsRead { get; set; } = true;

    /// <summary>
    /// Indicates whether the message is hidden.
    /// </summary>
    [Required]
    public bool IsHidden { get; set; } = false;

    /// <summary>
    /// Indicates whether the message is hidden by the receiver.
    /// </summary>
    [Required]
    public bool IsReceiverHidden { get; set; } = false;

    /// <summary>
    /// Timestamp when the message was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the message was deleted. This is used for soft deletion; if this field is not null, the message is considered deleted and should be excluded from active queries.
    /// </summary>
    public DateTime? DeletedAt { get; set; } = null;

    #region Navigation Properties

    /// <summary>
    /// Navigation property for the conversation to which this message belongs. This allows access to the details of the conversation, such as the participants and other messages in the same conversation.
    /// </summary>
    public Conversation Conversation { get; set; } = null!;

    /// <summary>
    /// Navigation property for the account that sent this message. This allows access to the details of the sender, such as their username and profile picture.
    /// </summary>
    public Account Sender { get; set; } = null!;

    #endregion
}
