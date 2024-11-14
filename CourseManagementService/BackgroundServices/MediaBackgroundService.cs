using System.Threading.Channels;
using CourseManagementService.Common;
using CourseManagementService.Services.CourseManagement.Teacher;
using CourseManagementService.Services.LessonManagement.VideoLesson;
using CourseManagementService.Services.Medias;
using CourseManagementService.Services.Medias.Schemas;

namespace CourseManagementService.BackgroundServices
{
    public class MediaProducer(Channel<BackgroundJobData> channel)
    {
        private readonly Channel<BackgroundJobData> _channel = channel;

        public async Task EnqueueDataAsync(BackgroundJobData channel)
        {
            await _channel.Writer.WriteAsync(channel);
        }
    }

    public class MediaBackgroundService(Channel<BackgroundJobData> channel, IServiceProvider serviceProvider) : BackgroundService
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
                    case BackgroundJobType.UpdateCourseThumbnail:
                        var teacherCourseDetailService = scope.ServiceProvider.GetRequiredService<ITeacherCourseDetailService>();
                        await UpdateCourseThumbnailAsync(scope.ServiceProvider.GetRequiredService<IPhotoService>(),
                            teacherCourseDetailService, data.Data);
                        break;
                    case BackgroundJobType.UpdateCoursePreviewVideo:
                        await UpdatePreviewVideoAsync(scope.ServiceProvider.GetRequiredService<IVideoService>(),
                            scope.ServiceProvider.GetRequiredService<ITeacherCourseDetailService>(), data.Data);
                        break;
                    case BackgroundJobType.DeleteCourseThumbnail:
                        await DeleteCourseThumbnailAsync(scope.ServiceProvider.GetRequiredService<IPhotoService>(), data.Data);
                        break;
                    case BackgroundJobType.UpdateLessonVideo:
                        await UpdateLessonVideoAsync(scope.ServiceProvider.GetRequiredService<IVideoLessonDetailService>(),
                            data.Data);
                        break;
                    case BackgroundJobType.UpdateLessonThumbnail:
                        await UpdateLessonThumbnailAsync(scope.ServiceProvider.GetRequiredService<IVideoLessonDetailService>(),
                            data.Data);
                        break;
                    default:
                        throw new ArgumentException("Unsupported job type.");
                }
            }
        }

        private static async Task UpdateCourseThumbnailAsync(IPhotoService photoService,
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

        private static async Task UpdatePreviewVideoAsync(IVideoService videoService,
            ITeacherCourseDetailService teacherCourseDetailService,
            Dictionary<string, dynamic> data)
        {
            try
            {
                var courseId = data["CourseId"] as Guid? ?? Guid.Empty;
                var localImagePath = data["FilePath"] as string;
                var videoUploadInfo = new VideoUploadInfo(courseId, localImagePath);

                var uploadResult = await videoService.UploadVideoChunkByChunkAsync(videoUploadInfo);
                var videoUrl = uploadResult.Data["baseUrlWithoutSAS"] as string;
                await teacherCourseDetailService.UpdatePreviewVideo(courseId, videoUrl);
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

        private static async Task DeleteCourseThumbnailAsync(IPhotoService photoService, Dictionary<string, dynamic> data)
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

        private static async Task UpdateLessonVideoAsync(IVideoLessonDetailService lessonDetailService, Dictionary<string, dynamic> data)
        {
            try
            {
                var lessonId = data["VideoLessonId"] as Guid? ?? Guid.Empty;
                var filePath = data["FilePath"] as string;
                await lessonDetailService.UpdateLessonVideoUrl(lessonId, filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Create video lesson failed: " + e.InnerException?.Message ?? e.Message);
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

        private static async Task UpdateLessonThumbnailAsync(IVideoLessonDetailService videoLessonDetailService,
            Dictionary<string, dynamic> data)
        {
            try
            {
                var lessonId = data["VideoLessonId"] as Guid? ?? Guid.Empty;
                var localImagePath = data["FilePath"] as string;
                await videoLessonDetailService.UpdateLessonThumbnail(lessonId, localImagePath);
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
    }
}