using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

/// <summary>
/// Represents a post created by an account, which can include content, pictures, comments, and likes.
/// </summary>
public class Post
{
    /// <summary>
    /// Unique identifier for the post.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the account who created the post.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Unique link or slug for the post.
    /// </summary>
    [Required]
    public string Link { get; set; } = null!;

    /// <summary>
    /// The main content of the post.
    /// </summary>
    [Required]
    public string Content { get; set; } = null!;

    /// <summary>
    /// Timestamp when the post was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last update to the post.
    /// </summary>
    public DateTime? UpdatedAt { get; set; } = null;

    /// <summary>
    /// Timestamp when the post was soft-deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; } = null;

    #region Navigation Properties

    /// <summary>
    /// The account who created the post.
    /// </summary>
    public Account Account { get; set; } = null!;

    /// <summary>
    /// Collection of pictures associated with this comment.
    /// </summary>
    public ICollection<Picture> Pictures { get; set; } = null!;

    /// <summary>
    /// Collection of comments on the post.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Collection of likes on the post.
    /// </summary>
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    #endregion
}
