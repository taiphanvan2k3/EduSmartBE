using System.Threading.Channels;
using CourseManagementService.Common;
using CourseManagementService.Services.CourseManagement.Teacher;
using CourseManagementService.Services.Medias;
using CourseManagementService.Services.Medias.Schemas;

namespace CourseManagementService.BackgroundServices
{
    public class CommonProducer(Channel<BackgroundJobData> channel)
    {
        private readonly Channel<BackgroundJobData> _channel = channel;

        public async Task EnqueueDataAsync(BackgroundJobData channel)
        {
            await _channel.Writer.WriteAsync(channel);
        }
    }

    public class CommonBackgroundService(Channel<BackgroundJobData> channel, IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly Channel<BackgroundJobData> _channel = channel;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var data in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                switch (data.JobType)
                {
                    case BackgroundJobType.UploadImageToCloudinary:
                        var teacherCourseDetailService = scope.ServiceProvider.GetRequiredService<ITeacherCourseDetailService>();
                        await UploadImageToCloudinaryAsync(scope.ServiceProvider.GetRequiredService<IPhotoService>(), 
                            teacherCourseDetailService, data.Data);
                        break;
                    case BackgroundJobType.DeleteImageFromCloudinary:
                        await DeleteImageFromCloudinaryAsync(scope.ServiceProvider.GetRequiredService<IPhotoService>(), data.Data);
                        break;
                    default:
                        throw new ArgumentException("Unsupported job type.");
                }
            }
        }

        private static async Task UploadImageToCloudinaryAsync(IPhotoService photoService,
            ITeacherCourseDetailService teacherCourseDetailService,
            Dictionary<string, dynamic> data)
        {
            try
            {
                var courseId = data["CourseId"] as Guid? ?? Guid.Empty;
                var localImagePath = data["FilePath"] as string;
                var imageUploadInfo = new ImageUploadInfo(courseId, localImagePath);

                var uploadResult = await photoService.UploadImageFromLocalAsync(imageUploadInfo);
                await teacherCourseDetailService.UpdateCourseThumbnail(courseId, uploadResult.SecureUrl.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Upload image to cloudinary failed: " + e.InnerException?.Message ?? e.Message);
            }
            finally
            {
                var filePath = data["FilePath"] as string;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private static async Task DeleteImageFromCloudinaryAsync(IPhotoService photoService, Dictionary<string, dynamic> data)
        {
            try
            {
                var publicId = data["PublicId"] as string;
                await photoService.DeletePhotoAsync(publicId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Upload image to cloudinary failed: " + e.InnerException?.Message ?? e.Message);
            }
        }
    }
}