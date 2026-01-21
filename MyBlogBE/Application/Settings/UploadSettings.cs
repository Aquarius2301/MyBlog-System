namespace Application.Settings;

/// <summary>
/// Configuration settings for ImageKit integration.
/// </summary>
public class UploadSettings
{
    /// <summary>
    /// The public key for ImageKit.
    /// </summary>
    public string PublicKey { get; set; } = null!;

    /// <summary>
    /// The private key for ImageKit.
    /// </summary>
    public string PrivateKey { get; set; } = null!;

    /// <summary>
    /// The URL endpoint for ImageKit.
    /// </summary>
    public string UrlEndpoint { get; set; } = null!;
}
