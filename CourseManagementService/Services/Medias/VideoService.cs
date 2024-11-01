using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CourseManagementService.Common;
using CourseManagementService.Settings;
using Microsoft.Extensions.Options;

namespace CourseManagementService.Services.Medias
{
    public interface IVideoService
    {
        public Task<ResponseInfo> UploadVideoAsync(IFormFile file);
    }

    public class VideoService(IServiceProvider serviceProvider,
        IOptions<AzureBlobStoragSetting> azureBlobStoragSetting,
        ILogger<VideoService> logger) : BaseService(serviceProvider, logger), IVideoService
    {
        private readonly string _containerName = azureBlobStoragSetting.Value.ContainerName;
        private readonly BlobServiceClient _blobServiceClient = new(azureBlobStoragSetting.Value.ConnectionString);

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
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(file.FileName);

                var blobHttpHeaders = new BlobHttpHeaders { ContentType = "video/mp4" };
                var options = new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders,
                    TransferOptions = new StorageTransferOptions
                    {
                        MaximumConcurrency = 4, // Số luồng đồng thời, bạn có thể điều chỉnh để tối ưu
                        MaximumTransferSize = 4 * 1024 * 1024 // Kích thước chunk tối đa 4 MB
                    }
                };

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, options);

                responseInfo.Message = "Upload video successfully";
                responseInfo.Data.Add("videoUrl", blobClient.Uri.ToString());

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