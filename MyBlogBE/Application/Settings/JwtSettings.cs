namespace Application.Settings;

/// <summary>
/// Configuration settings for JWT token generation and validation.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// The secret key used to sign JWT tokens.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// The issuer of the JWT token.
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// The audience of the JWT token.
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// Duration of the access token in minutes.
    /// </summary>
    public int AccessTokenDurationMinutes { get; set; }

    /// <summary>
    /// Duration of the refresh token in days.
    /// </summary>
    public int RefreshTokenDurationDays { get; set; }
}
