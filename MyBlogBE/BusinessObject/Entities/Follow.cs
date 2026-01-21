using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

/// <summary>
/// Represents a following relationship between two accounts.
/// Each account can follow another account only once.
/// </summary>
public class Follow
{
    /// <summary>
    /// Unique identifier for the follow record.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the account who is following another account.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// The ID of the account being followed.
    /// </summary>
    [Required]
    public Guid FollowingId { get; set; }

    #region Navigation Properties

    /// <summary>
    /// The account who is following.
    /// </summary>
    public Account Account { get; set; } = null!;

    /// <summary>
    /// The account being followed.
    /// </summary>
    public Account Following { get; set; } = null!;

    #endregion
}
