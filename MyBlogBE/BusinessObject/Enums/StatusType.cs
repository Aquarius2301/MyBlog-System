namespace BusinessObject.Enums;

public sealed record StatusType(string Code)
{
    /// <summary>
    /// The account is inactive and cannot log in or perform any actions.
    /// </summary>
    public static readonly StatusType Inactive = new("inactive");

    /// <summary>
    /// The account is active and can log in, post content, and comment.
    /// </summary>
    public static readonly StatusType Active = new("active");

    /// <summary>
    /// The account is suspended (can log in and view content but cannot post and comment).
    /// </summary>
    public static readonly StatusType Suspended = new("suspended");
}
