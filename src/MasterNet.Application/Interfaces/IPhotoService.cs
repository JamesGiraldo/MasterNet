using MasterNet.Application.Photos.PhotosUploadResult;
using Microsoft.AspNetCore.Http;

namespace MasterNet.Application.Interfaces;

public interface IPhotoService
{
    Task<PhotosUploadResult> AddPhoto(IFormFile file);
    Task<string> DeletePhoto(string publicId);
}