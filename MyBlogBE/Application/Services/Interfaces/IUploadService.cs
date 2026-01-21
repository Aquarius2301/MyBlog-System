namespace Application.Services.Interfaces;

public interface IUploadService
{
    Task<List<string>> UploadImageAsync(IFileStream[] files);

    Task DeleteImageAsync(List<string> fileIds);
}
