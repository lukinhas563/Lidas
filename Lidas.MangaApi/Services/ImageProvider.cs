using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Lidas.MangaApi.Config;
using Microsoft.Extensions.Options;
using System.Net;

namespace Lidas.MangaApi.Services;

public class ImageProvider
{
    private Cloudinary _cloudinary { get; set; }
    public ImageProvider(IOptions<CloudinarySettings> options)
    {
        var settings = options.Value;
        _cloudinary = new Cloudinary(new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret));
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty.");
        }

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            PublicId = Guid.NewGuid().ToString(),
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Error uploading file.");
        }

        return uploadResult.SecureUrl.ToString();
    }
}
