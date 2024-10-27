using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CourseManagementService.Services.Medias.Schemas;
using CourseManagementService.Settings;
using Microsoft.Extensions.Options;

namespace CourseManagementService.Services.Medias
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadImageFromLocalAsync(ImageUploadInfo imageUploadInfo);

        /// <summary>
        /// Add photo to cloudinary
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 09/10/2024</para>
        /// </summary>
        /// <returns>ImageUploadResult</returns>
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        /// <summary>
        /// Delete photo from cloudinary
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 09/10/2024</para>
        /// </summary>
        /// <returns>DeletionResult</returns>
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }

    public class PhotoService : BaseService, IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySetting> config, IServiceProvider serviceProvider, ILogger<PhotoService> logger, IMapper mapper)
            : base(serviceProvider, logger)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ImageUploadResult> UploadImageFromLocalAsync(ImageUploadInfo imageUploadInfo)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var uploadResult = new ImageUploadResult();

                var fileStream = new FileStream(imageUploadInfo.LocalImagePath, FileMode.Open, FileAccess.Read);
                if (fileStream != null)
                {
                    using var stream = fileStream;
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageUploadInfo.CouseId.ToString(), stream),
                        Transformation = new Transformation().Quality(80).FetchFormat("auto")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }

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
                var uploadResult = new ImageUploadResult();

                if (file != null && file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Quality(80).FetchFormat("auto")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }

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
                var deletionParams = new DeletionParams(publicId);
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
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