using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

/// <summary>
/// Represents a like on a post by a user.
/// Each account can like a specific post only once.
/// </summary>
public class PostLike
{
    /// <summary>
    /// Unique identifier for the post like record.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the post being liked.
    /// </summary>
    [Required]
    public Guid PostId { get; set; }

    /// <summary>
    /// The ID of the account who liked the post.
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
    /// The post associated with this like.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    /// The account who liked the post.
    /// </summary>
    public Account Account { get; set; } = null!;

    #endregion
}
