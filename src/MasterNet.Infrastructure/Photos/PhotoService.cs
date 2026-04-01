using MasterNet.Application.Interfaces;
using MasterNet.Application.Photos.PhotosUploadResult;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using CloudinaryDotNet.Actions;

namespace MasterNet.Infrastructure.Photos;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CLOUDINARY_CLOUD_NAME,
            config.Value.CLOUDINARY_API_KEY,
            config.Value.CLOUDINARY_API_SECRET
        );
        _cloudinary = new Cloudinary(account);
    }
    public async Task<PhotosUploadResult> AddPhoto(IFormFile file)
    {
        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill"),
                Folder = "masternet-courses"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error is not null) throw new Exception(uploadResult.Error.Message);

            return new PhotosUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
            };
        }

        throw new Exception("Failed to upload photo");
    }

    public async Task<string> DeletePhoto(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null) throw new Exception(result.Error.Message);
        return result.Result == "ok"
                ? result.Result
                : throw new Exception("Failed to delete photo");
    }
}