using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CourseManagementService.Services.Medias.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CourseManagementService.Services.Medias
{
    public class MinioPhotoService(IServiceProvider serviceProvider,
        IStorageService storageService,
        ILogger<MinioPhotoService> logger) : BaseService(serviceProvider, logger), IPhotoService
    {
        private readonly IStorageService _storageService = storageService;

        public async Task<ImageUploadResult> UploadImageFromLocalAsync(ImageUploadInfo imageUploadInfo)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var objectId = Guid.NewGuid().ToString();

                var fileUrl = await _storageService.UploadFileFromLocalAsync(
                    imageUploadInfo.LocalImagePath,
                    objectId,
                    "image/jpeg"
                );

                var uploadResult = new ImageUploadResult
                {
                    Url = new Uri(fileUrl),
                    SecureUrl = new Uri(fileUrl),
                    PublicId = objectId
                };

                LogInfo("End", methodName);
                return uploadResult;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty");
                }

                var objectId = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                using var stream = file.OpenReadStream();
                var fileUrl = await _storageService.UploadFileAsync(stream, objectId, file.ContentType);

                var uploadResult = new ImageUploadResult
                {
                    Url = new Uri(fileUrl),
                    SecureUrl = new Uri(fileUrl),
                    PublicId = objectId
                };

                LogInfo("End", methodName);
                return uploadResult;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw new Exception(e.Message);
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                await _storageService.DeleteFileAsync(publicId);

                var deletionResult = new DeletionResult
                {
                    Result = "ok"
                };

                LogInfo("End", methodName);
                return deletionResult;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw new Exception(e.Message);
            }
        }
    }
}
