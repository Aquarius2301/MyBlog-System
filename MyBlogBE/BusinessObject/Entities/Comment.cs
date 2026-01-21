using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

/// <summary>
/// Represents a comment made by a user on a post, including replies and associated media.
/// </summary>
public class Comment
{
    /// <summary>
    /// Unique identifier for the comment.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the account who created the comment.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Optional ID of the parent comment if this comment is a reply.
    /// </summary>
    public Guid? ParentCommentId { get; set; } = null;

    /// <summary>
    /// Optional ID of the account being replied to.
    /// </summary>
    public Guid? ReplyAccountId { get; set; } = null;

    /// <summary>
    /// ID of the post that this comment belongs to.
    /// </summary>
    [Required]
    public Guid PostId { get; set; }

    /// <summary>
    /// Content of the comment.
    /// </summary>
    [Required]
    public string Content { get; set; } = null!;

    /// <summary>
    /// Timestamp when the comment was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last update to the comment.
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Timestamp when the comment was soft-deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; } = null;

    #region Navigation Properties

    /// <summary>
    /// The account who created the comment.
    /// </summary>
    public Account Account { get; set; } = null!;

    /// <summary>
    /// The account being replied to, if this is a reply comment.
    /// </summary>
    public Account? ReplyAccount { get; set; } = null;

    /// <summary>
    /// Parent comment if this comment is a reply.
    /// </summary>
    public Comment? ParentComment { get; set; } = null;

    /// <summary>
    /// The post that this comment belongs to.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    /// Collection of pictures associated with this comment.
    /// </summary>
    public ICollection<Picture> Pictures { get; set; } = null!;

    /// <summary>
    /// Collection of likes on this comment.
    /// </summary>
    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    /// <summary>
    /// Collection of replies on this comment.
    /// </summary>
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    #endregion
}
