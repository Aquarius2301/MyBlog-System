using Application.Services.Interfaces;

namespace Application.Dtos;

public class UploadRequest
{
    public IFileStream[] Pictures { get; set; } = [];
}

public class ImageDto
{
    public string PublicId { get; set; } = null!;
    public string Link { get; set; } = null!;
}
