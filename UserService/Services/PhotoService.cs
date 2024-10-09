using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using UserService.Settings;

namespace UserService.Services
{
    public interface IPhotoService
    {
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
        private readonly ILogger<PhotoService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        public PhotoService(IOptions<CloudinarySettings> config, IServiceProvider serviceProvider, ILogger<PhotoService> logger, IMapper mapper)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
             _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PhotoService] [{Method}] Start", methodName);
                var uploadResult = new ImageUploadResult();

                if(file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Quality(80).FetchFormat("auto")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }

                _logger.LogInformation("[PhotoService] [{Method}] End", methodName);
                return uploadResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PhotoService] [{Method}] Error", methodName);
                throw new Exception(e.Message);
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}