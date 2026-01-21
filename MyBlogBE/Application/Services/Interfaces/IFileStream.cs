namespace Application.Services.Interfaces;

/// <summary>
/// Represents a file stream with metadata.
/// </summary>
public interface IFileStream
{
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }
    Stream OpenReadStream();
}
