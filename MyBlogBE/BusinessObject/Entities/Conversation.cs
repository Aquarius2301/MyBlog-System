using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Entities;

public class Conversation
{
    /// <summary>
    /// Unique identifier for the conversation.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the first participant in the conversation.
    /// The order of participants is not significant; this is just a convention for storage.
    /// When querying, both Account1Id and Account2Id should be considered to find conversations involving a specific account.
    /// </summary>
    [Required]
    public Guid Account1Id { get; set; }

    /// <summary>
    /// ID of the second participant in the conversation.
    /// The order of participants is not significant; this is just a convention for storage.
    /// When querying, both Account1Id and Account2Id should be considered to find conversations involving a specific account.
    /// </summary>
    [Required]
    public Guid Account2Id { get; set; }

    /// <summary>
    /// Timestamp when the conversation was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the conversation was last updated (e.g., when a new message is sent).
    /// </summary>
    public DateTime? UpdatedAt { get; set; } = null;

    /// <summary>
    /// Timestamp when the conversation was deleted. This is used for soft deletion; if this field is not null, the conversation is considered deleted and should be excluded from active queries.
    /// </summary>
    public DateTime? DeletedAt { get; set; } = null;

    #region Navigation Properties

    /// <summary>
    /// Navigation property for the first participant's account. This allows access to the account details of the first participant in the conversation.
    /// </summary>
    public Account Account1 { get; set; } = null!;

    /// <summary>
    /// Navigation property for the second participant's account. This allows access to the account details of the second participant in the conversation.
    /// </summary>
    public Account Account2 { get; set; } = null!;

    /// <summary>
    /// Navigation property for the messages in this conversation. This collection will contain all messages that belong to this conversation, ordered by their creation time. When querying conversations, this collection can be used to retrieve the associated messages.
    /// </summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    #endregion
}
