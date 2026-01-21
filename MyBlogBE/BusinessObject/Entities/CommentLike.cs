using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

/// <summary>
/// Represents a like on a comment by a user. Each account can like a comment only once.
/// </summary>
public class CommentLike
{
    /// <summary>
    /// Unique identifier for the comment like.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the comment being liked.
    /// </summary>
    [Required]
    public Guid CommentId { get; set; }

    /// <summary>
    /// The ID of the account who liked the comment.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Timestamp when the like was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    #region Navigation Properties

    /// <summary>
    /// The comment associated with this like.
    /// </summary>
    public Comment Comment { get; set; } = null!;

    /// <summary>
    /// The account who liked the comment.
    /// </summary>
    public Account Account { get; set; } = null!;

    #endregion
}
