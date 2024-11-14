using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using CourseManagementService.Common;
using CourseManagementService.Services.Medias.Schemas;
using CourseManagementService.Settings;
using Microsoft.Extensions.Options;

namespace CourseManagementService.Services.Medias
{
    public interface IVideoService
    {
        /// <summary>
        /// Upload video to Azure Blob Storage by IFormFile
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/01</para>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UploadVideoAsync(IFormFile file);

        /// <summary>
        /// Upload video to Azure Blob Storage by local path
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="videoUploadInfo"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UploadVideoFromLocalAsync(VideoUploadInfo videoUploadInfo);

        /// <summary>
        /// Upload video to Azure Blob Storage by small chunks
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/14</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UploadVideoChunkByChunkAsync(VideoUploadInfo videoUploadInfo);

        /// <summary>
        /// Get video URL with the shared access signature (SAS)<br/>
        /// Client only can use this url to access video in Azure Blob Storage
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/01</para>
        /// </summary>
        /// <param name="baseBlobURL"></param>
        /// <param name="expireTimeInMinutes"></param>
        /// <returns></returns>
        public string GetVideoURLWithSAS(string baseBlobURL, int expireTimeInMinutes = 60);

        /// <summary>
        /// Delete video from Azure Blob Storage
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para> 
        /// </summary>
        /// <param name="blobURL">Example: https://edusmartblob.blob.core.windows.net/coursesdata/0640adc6-e5e0-475a-832d-6414c83af5c9</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteVideoAsync(string blobURL);
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

                var options = GetBlobUploadOptions(file.ContentType);
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, options);

                responseInfo.Message = "Upload video successfully";
                responseInfo.Data.Add("baseUrlWithoutSAS", blobClient.Uri.ToString());

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

                var connectionTimeout = TimeSpan.FromMinutes(10); // Timeout dài hơn cho các tệp lớn
                var blobServiceClient = CreateBlobServiceClient(azureBlobStorageSetting.Value.ConnectionString,
                    connectionTimeout);

                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(videoUploadInfo.ResourceId.ToString());

                var options = GetBlobUploadOptions("video/mp4", maximumConcurrency: 8, maximumTransferSize: 8 * 1024 * 1024);

                using var stream = new FileStream(videoUploadInfo.LocalPath, FileMode.Open);
                await blobClient.UploadAsync(stream, options);

                responseInfo.Message = "Upload video successfully";
                responseInfo.Data.Add("baseUrlWithoutSAS", blobClient.Uri.ToString());

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, GetActualAsyncMethodName());
                throw;
            }
        }

        public async Task<ResponseInfo> UploadVideoChunkByChunkAsync(VideoUploadInfo videoUploadInfo)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var connectionTimeout = TimeSpan.FromMinutes(5);
                var blobServiceClient = CreateBlobServiceClient(azureBlobStorageSetting.Value.ConnectionString,
                    connectionTimeout);

                var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
                var blockBlobClient = containerClient.GetBlockBlobClient(videoUploadInfo.ResourceId.ToString());

                var blockSize = 4 * 1024 * 1024; // 4MB
                long totalFileSize = new FileInfo(videoUploadInfo.LocalPath).Length;
                int totalBlocks = (int)Math.Ceiling((double)totalFileSize / blockSize);
                int uploadedBlockCount = 0;

                IProgress<double> progressPercentage = new Progress<double>(percentage =>
                {
                    LogInfo($"Upload file of Course {videoUploadInfo.ResourceId} with {percentage:F2}%", method);
                });

                using var fileStream = new FileStream(videoUploadInfo.LocalPath, FileMode.Open);
                var tasks = new List<Task>();
                var blockIds = new List<string>();
                int blockIndex = 0;

                while (fileStream.Position < fileStream.Length)
                {
                    var remainingBytes = fileStream.Length - fileStream.Position;
                    var bytesToRead = Math.Min(blockSize, remainingBytes);
                    var buffer = new byte[bytesToRead];
                    await fileStream.ReadAsync(buffer, 0, buffer.Length);

                    var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockIndex.ToString("d6")));
                    blockIds.Add(blockId);

                    tasks.Add(UploadBlockAsync(blockBlobClient, blockId, buffer, () =>
                    {
                        Interlocked.Increment(ref uploadedBlockCount);
                        progressPercentage.Report((double)uploadedBlockCount / totalBlocks * 100);
                    }));

                    blockIndex++;
                }

                await Task.WhenAll(tasks);

                var blockList = await blockBlobClient.GetBlockListAsync(BlockListTypes.All);
                var uncommittedBlocks = blockList.Value.UncommittedBlocks.ToList();
                var blockNames = uncommittedBlocks.Select(b => b.Name).ToList();

                await blockBlobClient.CommitBlockListAsync(blockNames);

                responseInfo.Message = "Upload video successfully";
                responseInfo.Data.Add("baseUrlWithoutSAS", blockBlobClient.Uri.ToString());

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, GetActualAsyncMethodName());
                throw;
            }
        }

        public string GetVideoURLWithSAS(string baseBlobURL, int expireTimeInMinutes = 60)
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

        public async Task<ResponseInfo> DeleteVideoAsync(string blobURL)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobUri = new Uri(blobURL);
                var segments = blobUri.Segments;

                // Ví dụ: https://edusmartblob.blob.core.windows.net/coursesdata/0640adc6-e5e0-475a-832d-6414c83af5c9
                // thì segments = ["/", "coursesdata/", "0640adc6-e5e0-475a-832d-6414c83af5c9"]
                if (segments.Length < 3)
                {
                    responseInfo.Message = "Invalid blob URL";
                    return responseInfo;
                }

                var blobClient = containerClient.GetBlobClient(blobName: segments.Last());

                await blobClient.DeleteIfExistsAsync();

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, GetActualAsyncMethodName());
                throw;
            }
        }

        private static BlobUploadOptions GetBlobUploadOptions(string contentType,
            int maximumConcurrency = 4, int maximumTransferSize = 4 * 1024 * 1024)
        {
            return new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                TransferOptions = new StorageTransferOptions
                {
                    MaximumConcurrency = maximumConcurrency,
                    MaximumTransferSize = maximumTransferSize
                }
            };
        }

        private static BlobServiceClient CreateBlobServiceClient(string connectionString, TimeSpan connectionTimeout)
        {
            var options = new BlobClientOptions
            {
                Retry =
                {
                    NetworkTimeout = connectionTimeout
                }
            };
            return new BlobServiceClient(connectionString, options);
        }

        // Phương thức để upload từng block
        private async static Task UploadBlockAsync(BlockBlobClient blobClient, string blockId, byte[] blockData, Action onBlockUploaded)
        {
            using var blockStream = new MemoryStream(blockData);
            await blobClient.StageBlockAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId)), blockStream);
            onBlockUploaded?.Invoke(); // Cập nhật tiến trình sau khi upload xong block
        }
    }
}