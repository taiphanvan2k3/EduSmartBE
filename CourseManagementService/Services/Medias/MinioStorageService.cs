using System;
using System.IO;
using System.Threading.Tasks;
using CourseManagementService.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace CourseManagementService.Services.Medias
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSetting _minioSetting;

        public MinioStorageService(IOptions<MinioSetting> minioSetting)
        {
            _minioSetting = minioSetting.Value;

            var client = new MinioClient()
                .WithEndpoint(_minioSetting.Host)
                .WithCredentials(_minioSetting.AccessKey, _minioSetting.SecretKey);

            if (_minioSetting.SSL)
            {
                client = client.WithSSL();
            }

            _minioClient = client.Build();
        }

        private async Task EnsureBucketExistsAsync()
        {
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(_minioSetting.Bucket);
            bool exists = await _minioClient.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);
            if (!exists)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(_minioSetting.Bucket);
                await _minioClient.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
            }
        }

        public async Task<string> UploadFileAsync(Stream stream, string objectName, string contentType)
        {
            await EnsureBucketExistsAsync();

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_minioSetting.Bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            return $"{(_minioSetting.SSL ? "https" : "http")}://{_minioSetting.Host}/{_minioSetting.Bucket}/{objectName}";
        }

        public async Task<string> UploadFileFromLocalAsync(string localPath, string objectName, string contentType)
        {
            await EnsureBucketExistsAsync();

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_minioSetting.Bucket)
                .WithObject(objectName)
                .WithFileName(localPath)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            return $"{(_minioSetting.SSL ? "https" : "http")}://{_minioSetting.Host}/{_minioSetting.Bucket}/{objectName}";
        }

        public async Task<string> GetPresignedUrlAsync(string objectName, int expireTimeInMinutes = 60)
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_minioSetting.Bucket)
                .WithObject(objectName)
                .WithExpiry(expireTimeInMinutes * 60);

            return await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
        }

        public async Task DeleteFileAsync(string objectName)
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_minioSetting.Bucket)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(args).ConfigureAwait(false);
        }
    }
}
