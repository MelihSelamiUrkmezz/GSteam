using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GameService.DTOs;
using Microsoft.Extensions.Logging;

namespace GameService.Services;


public class FileService : IFileService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<FileService> _logger;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        var acc = new Account(
            configuration["Cloudinary:cloudName"],
            configuration["Cloudinary:apiKey"],
            configuration["Cloudinary:apiSecret"]
        );
        _cloudinary = new Cloudinary(acc);
        _logger = logger;
    }

    public async Task<string> UploadVideo(IFormFile File)
    {
        var uploadResult = new VideoUploadResult();
        if (File.Length > 0)
        {
            using var stream = File.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(File.FileName,stream),
                Folder = "g-steam_microservices"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
            string videoUrl = _cloudinary.Api.UrlVideoUp.BuildUrl(uploadResult.PublicId);
            return videoUrl;
        }
        return "";
        
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file was provided for upload");
                return null;
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Width(800)
                    .Height(600)
                    .Crop("fill")
                    .Quality("auto"),
                Folder = "g-steam_microservices"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            
            if (uploadResult.Error != null)
            {
                _logger.LogError("Cloudinary upload error: {Error}", uploadResult.Error.Message);
                return null;
            }

            return uploadResult.SecureUrl.ToString().Replace("/v1/", "/v1737328778/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to Cloudinary");
            throw;
        }
    }

    public async Task<string> UploadZip(IFormFile file)
    {
        var uploadResult = new RawUploadResult();
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "g-steam_microservices"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
            
            return uploadResult.SecureUrl.ToString();
        }
        return "";
    }

    public async Task<string> DownloadGame(string publicId)
    {   
        var result = await _cloudinary.GetResourceAsync(publicId);
        var getUrl = _cloudinary.Api.UrlImgUp
            .Transform(new Transformation().Quality("auto").FetchFormat("auto"))
            .BuildUrl(result.PublicId);
        return getUrl;
    }
}