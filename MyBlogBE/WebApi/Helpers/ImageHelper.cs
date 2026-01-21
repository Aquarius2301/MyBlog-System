using Application.Services.Interfaces;

namespace WebApi.Helpers;

public class ImageHelper : IFileStream
{
    private readonly IFormFile _formFile;

    public ImageHelper(IFormFile formFile) => _formFile = formFile;

    public string FileName => _formFile.FileName;
    public string ContentType => _formFile.ContentType;
    public long Length => _formFile.Length;

    public Stream OpenReadStream() => _formFile.OpenReadStream();
}
