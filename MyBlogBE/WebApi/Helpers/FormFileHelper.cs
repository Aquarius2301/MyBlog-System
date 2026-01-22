using Application.Services.Interfaces;

namespace WebApi.Helpers;

/// <summary>
/// Helper class to convert IFormFile to IFileStream
/// </summary>
public class FormFileHelper : IFileStream
{
    private readonly IFormFile _formFile;

    public FormFileHelper(IFormFile formFile) => _formFile = formFile;

    public string FileName => $"{Guid.NewGuid()}";
    public string ContentType => _formFile.ContentType;
    public long Length => _formFile.Length;

    public Stream OpenReadStream() => _formFile.OpenReadStream();
}
