using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseManagementService.Common;
using CourseManagementService.Services.Medias.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CourseManagementService.Services.Medias
{
    public class MinioVideoService(IServiceProvider serviceProvider,
        IStorageService storageService,
        ILogger<MinioVideoService> logger) : BaseService(serviceProvider, logger), IVideoService
    {
        private readonly IStorageService _storageService = storageService;

        public async Task<ResponseInfo> UploadVideoAsync(IFormFile file)
        {
            var responseInfo = new ResponseInfo();
            var method = GetActualAsyncMethodName();

            if (file == null || file.Length == 0)
            {
                responseInfo.Message = "File is empty";
                return responseInfo;
            }

            try
            {
                using var stream = file.OpenReadStream();
                var fileUrl = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

                responseInfo.Message = "Upload video successfully";
                responseInfo.Data.Add("baseUrlWithoutSAS", fileUrl);

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> UploadVideoFromLocalAsync(VideoUploadInfo videoUploadInfo)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var fileUrl = await _storageService.UploadFileFromLocalAsync(
                    videoUploadInfo.LocalPath, 
                    videoUploadInfo.ResourceId.ToString(), 
                    "video/mp4"
                );

                responseInfo.Message = "Upload video successfully";
                responseInfo.Data.Add("baseUrlWithoutSAS", fileUrl);

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> UploadVideoChunkByChunkAsync(VideoUploadInfo videoUploadInfo)
        {
            // For MinIO, the SDK internally manages multipart chunks for large files.
            return await UploadVideoFromLocalAsync(videoUploadInfo);
        }

        public string GetVideoURLWithSAS(string baseBlobURL, int expireTimeInMinutes = 60)
        {
            if (string.IsNullOrEmpty(baseBlobURL)) return baseBlobURL;
            try
            {
                var uri = new Uri(baseBlobURL);
                var objectName = uri.Segments.Last().Trim('/');
                return _storageService.GetPresignedUrlAsync(objectName, expireTimeInMinutes).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error generating presigned URL for {Url}", baseBlobURL);
                return baseBlobURL;
            }
        }

        public async Task<ResponseInfo> DeleteVideoAsync(string blobURL)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var uri = new Uri(blobURL);
                var objectName = uri.Segments.Last().Trim('/');

                await _storageService.DeleteFileAsync(objectName);

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}
