namespace Application.Settings;

/// <summary>
/// Base configuration settings for the application.
/// </summary>
public class BaseSettings
{
    /// <summary>
    /// Length of tokens used for various operations (e.g., email verification, password reset).
    /// </summary>
    public int TokenLength { get; set; }

    /// <summary>
    /// Expiry time for tokens in minutes.
    /// </summary>
    public int TokenExpiryMinutes { get; set; }

    /// <summary>
    /// Duration in days before a self-removal request is executed.
    /// </summary>
    public int SelfRemoveDurationDays { get; set; }

    /// <summary>
    /// Configuration settings for Cloudinary integration.
    /// </summary>
    public UploadSettings UploadSettings { get; set; } = null!;

    /// <summary>
    /// Configuration settings for JWT token generation and validation.
    /// </summary>
    public JwtSettings JwtSettings { get; set; } = null!;

    /// <summary>
    /// Configuration settings for sending emails.
    /// </summary>
    public EmailSettings EmailSettings { get; set; } = null!;
}
