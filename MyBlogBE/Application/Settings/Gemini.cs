using System;

namespace Application.Settings;

public class Gemini
{
    /// <summary>
    /// The model name for the Gemini API.
    /// </summary>
    public string Model { get; set; } = null!;

    /// <summary>
    /// The API key for accessing the Gemini service.
    /// </summary>
    public string ApiKey { get; set; } = null!;
}
