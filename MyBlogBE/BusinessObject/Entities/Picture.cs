namespace BusinessObject.Entities;

public class Picture
{
    /// <summary>
    /// Unique identifier for the picture.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the account who owns this picture.
    ///</summary>
    public Guid? AccountId { get; set; } = null;

    /// <summary>
    /// The ID of the comment this picture belongs to.
    /// </summary>
    public Guid? CommentId { get; set; } = null;

    /// <summary>
    /// The ID of the post this picture belongs to.
    /// </summary>
    public Guid? PostId { get; set; } = null;

    /// <summary>
    /// Public ID to the picture.
    /// </summary>
    public string PublicId { get; set; } = null!;

    /// <summary>
    /// URL to the picture.
    /// </summary>
    public string Link { get; set; } = null!;

    #region Navigation Properties

    /// <summary>
    /// The account that owns this picture.
    /// </summary>
    public Account? Account { get; set; } = null;

    /// <summary>
    /// The comment associated with this picture.
    /// </summary>
    public Comment? Comment { get; set; } = null;

    /// <summary>
    /// The post associated with this picture.
    /// </summary>
    public Post? Post { get; set; } = null;

    #endregion
}
