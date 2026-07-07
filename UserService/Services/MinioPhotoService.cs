using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UserService.Services
{
    public class MinioPhotoService(IServiceProvider serviceProvider,
        IStorageService storageService,
        ILogger<MinioPhotoService> logger) : BaseService(serviceProvider, logger), IPhotoService
    {
        private readonly IStorageService _storageService = storageService;

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
