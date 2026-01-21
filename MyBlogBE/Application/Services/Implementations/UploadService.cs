using Application.Dtos;
using Application.Helpers;
using Application.Loggers;
using Application.Services.Interfaces;
using Application.Settings;
using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Imagekit.Sdk;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations;

public class UploadService : IUploadService
{
    private readonly MyBlogLogger _logger;
    private readonly UploadSettings _uploadSettings;
    private readonly ImagekitClient imagekit;
    private readonly IUnitOfWork _unitOfWork;

    public UploadService(
        IUnitOfWork unitOfWork,
        MyBlogLogger logger,
        IOptions<UploadSettings> options
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _uploadSettings = options.Value;
        imagekit = new ImagekitClient(
            _uploadSettings.PublicKey,
            _uploadSettings.PrivateKey,
            _uploadSettings.UrlEndpoint
        );
    }

    public async Task<List<string>> UploadImageAsync(IFileStream[] files)
    {
        List<ImageDto> res = [];

        foreach (var file in files)
            res.Add(await UploadAsync(file));

        _unitOfWork.Pictures.AddRange(
            res.Select(r => new Picture { PublicId = r.PublicId, Link = r.Link })
        );

        await _unitOfWork.SaveChangesAsync();

        return res.Select(r => r.Link).ToList();
    }

    public async Task DeleteImageAsync(List<string> fileIds)
    {
        foreach (var fileId in fileIds)
        {
            await imagekit.DeleteFileAsync(fileId);
        }
    }

    private async Task<ImageDto> UploadAsync(IFileStream file)
    {
        await _logger.LogInfo("UploadService", $"Uploading image {file.FileName}");

        using var ms = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(ms);
        byte[] fileBytes = ms.ToArray();

        FileCreateRequest request = new()
        {
            file = fileBytes,
            fileName = file.FileName,
            folder = "uploads",
        };

        Result resp = await imagekit.UploadAsync(request);

        if (resp.HttpStatusCode == 200)
        {
            await _logger.LogInfo("UploadService", $"Upload successfully {file.FileName}");
        }
        else
        {
            await _logger.LogError("UploadService", $"Upload failed {file.FileName}");
        }

        return resp.HttpStatusCode == 200
            ? new ImageDto { PublicId = resp.fileId, Link = resp.url }
            : null!;
    }
}
