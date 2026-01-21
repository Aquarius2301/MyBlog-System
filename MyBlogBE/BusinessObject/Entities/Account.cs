using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BusinessObject.Enums;

namespace BusinessObject.Entities;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier for the account.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Username used for login. Maximum length 20 characters.
    /// </summary>
    [Required, MaxLength(20)]
    public string Username { get; set; } = null!;

    /// <summary>
    /// Display name of the user. Maximum length 50 characters.
    /// </summary>
    [Required, MaxLength(50)]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// User's date of birth.
    /// </summary>
    [Required]
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// ID of the profile picture associated with the account.
    /// </summary>
    public Guid? PictureId { get; set; } = null;

    /// <summary>
    /// Email address of the user.
    /// </summary>
    [Required]
    public string Email { get; set; } = null!;

    /// <summary>
    /// Verification code sent to the user's email.
    /// </summary>
    public string? EmailVerifiedCode { get; set; } = null;

    /// <summary>
    /// Type of verification process used for the account.
    /// </summary>
    public VerificationType? VerificationType { get; set; } = null;

    /// <summary>
    /// Expiration time of the email verification code.
    /// </summary>
    public DateTime? EmailVerifiedCodeExpiry { get; set; } = null;

    /// <summary>
    /// Hashed password of the account. Not serialized in JSON responses.
    /// </summary>
    [Required, JsonIgnore]
    public string HashedPassword { get; set; } = null!;

    /// <summary>
    /// Refresh token for renewing access tokens.
    /// </summary>
    public string? RefreshToken { get; set; } = null;

    /// <summary>
    /// Current access token for the account.
    /// </summary>
    public string? AccessToken { get; set; } = null;

    /// <summary>
    /// Expiration time of the refresh token.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; } = null;

    /// <summary>
    /// Scheduled time for self-removal (soft delete) of the account.
    /// </summary>
    public DateTime? SelfRemoveTime { get; set; } = null;

    /// <summary>
    /// Current status of the account.
    /// </summary>
    [Required]
    public string Status { get; set; } = StatusType.Active.Code;

    /// <summary>
    /// Preferred language of the user.
    /// </summary>
    [Required]
    public string Language { get; set; } = LanguageType.English.Code;

    /// <summary>
    /// Timestamp when the account was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last update to the account.
    /// </summary>
    public DateTime? UpdatedAt { get; set; } = null;

    /// <summary>
    /// Timestamp when the account was soft-deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; } = null;

    #region Navigation Properties

    /// <summary>
    /// Profile picture associated with the account.
    /// </summary>
    public Picture? Picture { get; set; } = null;

    /// <summary>
    /// Posts created by the account.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    /// <summary>
    /// Comments made by the account.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// Likes on posts by the account.
    /// </summary>
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    /// <summary>
    /// Likes on comments by the account.
    /// </summary>
    public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    /// <summary>
    /// Users who follow this account.
    /// </summary>
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();

    /// <summary>
    /// Users that this account is following.
    /// </summary>
    public ICollection<Follow> Following { get; set; } = new List<Follow>();

    #endregion
}
