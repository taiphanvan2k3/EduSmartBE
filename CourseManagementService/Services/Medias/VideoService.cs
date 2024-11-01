using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
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
        IOptions<AzureBlobStorageSetting> azureBlobStorageSetting,
        ILogger<VideoService> logger) : BaseService(serviceProvider, logger), IVideoService
    {
        private readonly string _containerName = azureBlobStorageSetting.Value.ContainerName;
        private readonly BlobServiceClient _blobServiceClient = new(azureBlobStorageSetting.Value.ConnectionString);

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
                responseInfo.Data.Add("videoSasUrl", GenerateSASToken(blobClient.Uri.ToString()));

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        private string GenerateSASToken(string baseBlobURL, int expireTimeInMinutes = 60)
        {
            var blobClient = new BlobClient(new Uri(baseBlobURL));
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b", // b: blob, sb: blob snapshot, c: container,...
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expireTimeInMinutes)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, azureBlobStorageSetting.Value.Key1)).ToString();
            return $"{blobClient.Uri}?{sasToken}";
        }
    }
}