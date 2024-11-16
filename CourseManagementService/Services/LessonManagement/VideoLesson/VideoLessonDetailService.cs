using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.LessonManagement.VideoLesson.Schemas;
using CourseManagementService.Services.Medias;
using CourseManagementService.Services.Medias.Schemas;
using Microsoft.EntityFrameworkCore;
using TblLesson = CourseManagementService.Database.Schemas.Lesson;
using TblVideoLesson = CourseManagementService.Database.Schemas.VideoLesson;

namespace CourseManagementService.Services.LessonManagement.VideoLesson
{
    public interface IVideoLessonDetailService
    {
        /// <summary>
        /// Get video lesson by id
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="lessonUd">Id of the lesson (not id of video lesson)</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetVideoLessonById(Guid lessonUd);

        /// <summary>
        /// Create a video lesson (Video is still uploading)
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="videoLessonInfo"></param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateVideoLesson(VideoLessonCreateUpdateDto videoLessonInfo);

        /// <summary>
        /// Update video lesson's video URL after video is uploaded<br/>
        /// This method is called by the background job
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="lessonId"></param>
        /// <param name="localVideoPath"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateLessonVideoUrl(Guid lessonId, string localVideoPath);

        /// <summary>
        /// Update video lesson's thumbnail after thumbnail is uploaded<br/>
        /// This method is called by the background job
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="lessonId"></param>
        /// <param name="localThumbnailPath"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateLessonThumbnail(Guid lessonId, string localThumbnailPath);

        /// <summary>
        /// Update video lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="lessonId"></param>
        /// <param name="videoLessonInfo"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateVideoLesson(Guid lessonId, VideoLessonCreateUpdateDto videoLessonInfo);

        /// <summary>
        /// Delete video lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/03</para>
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteVideoLesson(Guid lessonId);
    }

    public class VideoLessonDetailService(IServiceProvider serviceProvider,
        ILogger<VideoLessonDetailService> logger) : BaseService(serviceProvider, logger), IVideoLessonDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));
        private readonly MediaProducer _mediaProducer = serviceProvider.GetService<MediaProducer>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(MediaProducer)));
        private readonly IChapterDetailService _chapterDetailService = serviceProvider.GetService<IChapterDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IChapterDetailService)));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly IPhotoService _photoService = serviceProvider.GetService<IPhotoService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IPhotoService)));
        private readonly IVideoService _videoService = serviceProvider.GetService<IVideoService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IVideoService)));

        public async Task<ResponseInfo> GetVideoLessonById(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var courseId = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => l.Chapter.CourseId)
                    .FirstOrDefaultAsync();

                if (courseId == Guid.Empty)
                {
                    responseInfo.Error = "NotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanAccessCourseMaterial(courseId: courseId, currentUser.UserId))
                {
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You don't have permission to access this lesson";
                    return responseInfo;
                }

                var videoLesson = await _context.VideoLessons
                    .Where(v => v.LessonId == lessonId)
                    .ProjectTo<VideoLessonDetail>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (videoLesson == null)
                {
                    responseInfo.Error = "NotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                videoLesson.VideoURLWithSAS = _videoService.GetVideoURLWithSAS(videoLesson.BaseBlobURL);
                (Guid? previousLessonId, Guid? nextLessonId) = await _lessonBaseDetailService.GetPreviousAndNextLessonId(
                     videoLesson.ChapterOrder, videoLesson.LessonOrder);

                videoLesson.PreviousLessonId = previousLessonId;
                videoLesson.NextLessonId = nextLessonId;

                responseInfo.Data.Add("lesson", videoLesson);
                LogInfo("End", methodName);

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateVideoLesson(VideoLessonCreateUpdateDto videoLessonInfo)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                if (!await _chapterDetailService.IsExistingChapter(videoLessonInfo.ChapterId))
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Chapter not found";
                    return responseInfo;
                }

                int videoLessonOrder = await _context.Lessons
                    .Where(l => l.ChapterId == videoLessonInfo.ChapterId)
                    .MaxAsync(l => (int?)l.Order) ?? 0;

                var currentUser = GetCurrentUser();
                var lessonEntity = new TblLesson
                {
                    Id = Guid.NewGuid(),
                    Title = videoLessonInfo.Title,
                    Description = videoLessonInfo.Description,
                    ChapterId = videoLessonInfo.ChapterId,
                    LessonType = LessonType.Video,
                    IsPublished = videoLessonInfo.IsPublished,
                    IsCommentAllowed = videoLessonInfo.IsCommentAllowed,
                    IsRatingAllowed = videoLessonInfo.IsRatingAllowed,
                    CreatedBy = currentUser.UserId,
                    DurationInSeconds = videoLessonInfo.VideoDurationInSeconds ?? 0,
                    Order = videoLessonOrder + 1
                };

                var videoLessonEntity = new TblVideoLesson()
                {
                    Id = Guid.NewGuid(),
                    BaseBlobURL = "",
                    ThumbnailURL = Constants.IN_PROGRESS_THUMBNAIL,
                    UploadStatus = UploadStatus.InProgress,
                    Lesson = lessonEntity
                };

                await _context.VideoLessons.AddAsync(videoLessonEntity);
                await _context.SaveChangesAsync();

                var videoLessonDetail = _mapper.Map<VideoLessonDetail>(videoLessonEntity);
                responseInfo.Data.Add("Lesson", videoLessonDetail);

                if (videoLessonInfo.Thumbnail != null)
                {
                    await StartUploadImageToCloudinaryJob(videoLessonInfo.Thumbnail, videoLessonEntity.Id);
                }
                await StartUploadVideoJob(videoLessonInfo.Video, videoLessonEntity.Id);
                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateVideoLesson(Guid lessonId, VideoLessonCreateUpdateDto videoLessonInfo)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var courseId = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => l.Chapter.CourseId)
                    .FirstOrDefaultAsync();

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanModifyCourseMaterial(courseId: courseId, currentUser.UserId))
                {
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You don't have permission to modify this lesson";
                    return responseInfo;
                }

                var videoLessonEntity = await _context.VideoLessons
                    .Include(v => v.Lesson)
                    .FirstOrDefaultAsync(v => v.LessonId == lessonId);

                if (videoLessonEntity == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                videoLessonEntity.Lesson.Title = videoLessonInfo.Title;
                videoLessonEntity.Lesson.Description = videoLessonInfo.Description;
                videoLessonEntity.Lesson.IsPublished = videoLessonInfo.IsPublished;
                videoLessonEntity.Lesson.IsCommentAllowed = videoLessonInfo.IsCommentAllowed;
                videoLessonEntity.Lesson.IsRatingAllowed = videoLessonInfo.IsRatingAllowed;

                var deletedResources = new Dictionary<string, string>()
                {
                    { "ThumbnailURL", videoLessonEntity.ThumbnailURL },
                    { "VideoURL", videoLessonEntity.BaseBlobURL }
                };

                if (videoLessonInfo.Thumbnail != null)
                {
                    await StartUploadImageToCloudinaryJob(videoLessonInfo.Thumbnail, videoLessonEntity.Id);
                }
                else
                {
                    deletedResources["ThumbnailURL"] = "";
                }

                if (videoLessonInfo.Video != null)
                {
                    videoLessonEntity.Lesson.DurationInSeconds = videoLessonInfo.VideoDurationInSeconds ?? 0;
                    await StartUploadVideoJob(videoLessonInfo.Video, videoLessonEntity.Id);
                }
                else
                {
                    deletedResources["VideoURL"] = "";
                }

                await DeleteLessonResources(deletedResources["ThumbnailURL"], deletedResources["VideoURL"]);
                await _context.SaveChangesAsync();

                responseInfo.Message = "Update video lesson successfully";

                var videoLessonDetail = _mapper.Map<VideoLessonDetail>(videoLessonEntity);
                responseInfo.Data.Add("Lesson", videoLessonDetail);
                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        // Background job
        public async Task<ResponseInfo> UpdateLessonVideoUrl(Guid videoLessonId, string localVideoPath)
        {
            var methodName = GetActualAsyncMethodName();
            TblVideoLesson videoLessonEntity = null;
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                videoLessonEntity = await _context.VideoLessons
                    .Include(v => v.Lesson)
                    .FirstOrDefaultAsync(v => v.Id == videoLessonId)
                    ?? throw new InvalidDataException("Lesson not found");

                LogInfo("Begin upload video", methodName);
                var uploadResponse = await _videoService.UploadVideoFromLocalAsync(new VideoUploadInfo(
                    videoLessonEntity.LessonId, localVideoPath));

                videoLessonEntity.BaseBlobURL = uploadResponse.Data["baseUrlWithoutSAS"];
                videoLessonEntity.UploadStatus = UploadStatus.Completed;

                try
                {
                    videoLessonEntity.Lesson.DurationInSeconds = await Utils.GetDurationOfVideo(localVideoPath);
                }
                catch (Exception e)
                {
                    LogError(e, "UpdateLessonVideoUrl->GetDurationOfVideo");
                }
                await _context.SaveChangesAsync();

                responseInfo.Message = "Update video lesson successfully";
                return responseInfo;
            }
            catch (Exception e)
            {
                if (videoLessonEntity != null)
                {
                    videoLessonEntity.UploadStatus = UploadStatus.Failed;
                    await _context.SaveChangesAsync();
                }

                LogError(e, GetActualAsyncMethodName());
                throw;
            }
        }

        // Background job
        public async Task<ResponseInfo> UpdateLessonThumbnail(Guid videoLessonId, string localThumbnailPath)
        {
            var methodName = GetActualAsyncMethodName();
            TblVideoLesson videoLessonEntity = null;
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                videoLessonEntity = await _context.VideoLessons
                    .FirstOrDefaultAsync(v => v.Id == videoLessonId)
                    ?? throw new InvalidDataException("Lesson not found");

                var uploadResponse = await _photoService
                    .UploadImageFromLocalAsync(new ImageUploadInfo(videoLessonEntity.LessonId, localThumbnailPath));

                videoLessonEntity.ThumbnailURL = uploadResponse.SecureUrl.ToString();
                await _context.SaveChangesAsync();

                responseInfo.Message = "Update video lesson successfully";
                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                if (videoLessonEntity != null)
                {
                    videoLessonEntity.UploadStatus = UploadStatus.Failed;
                    await _context.SaveChangesAsync();
                }
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteVideoLesson(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var courseId = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => l.Chapter.CourseId)
                    .FirstOrDefaultAsync();

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanModifyCourseMaterial(courseId: courseId, currentUser.UserId))
                {
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You don't have permission to access this lesson";
                    return responseInfo;
                }

                var deletedResources = await _context.VideoLessons
                    .Where(v => v.LessonId == lessonId)
                    .Select(v => new
                    {
                        v.ThumbnailURL,
                        VideoURL = v.BaseBlobURL
                    })
                    .FirstOrDefaultAsync();

                await DeleteLessonResources(deletedResources.ThumbnailURL, deletedResources.VideoURL);
                await _context.Lessons.Where(l => l.Id == lessonId).ExecuteDeleteAsync();

                responseInfo.Message = "Delete video lesson successfully";
                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        private async Task StartUploadImageToCloudinaryJob(IFormFile file, Guid videoLessonId)
        {
            // Save the file to the local storage
            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UPLOAD_FOLDER_NAME);
            Utils.CreateUploadFolderIfNotExist(uploadFolderPath);
            var fileExtension = Path.GetExtension(file.FileName);
            var filePath = await Utils.SaveFileLocally(uploadFolderPath, $"{videoLessonId}.{fileExtension}", file);

            // Start the background job to upload the file to Cloudinary
            var backgroundJobData = new BackgroundJobData
            {
                JobType = BackgroundJobType.UpdateLessonThumbnail,
                Data = new Dictionary<string, dynamic>
                {
                    { "FilePath", filePath },
                    { "VideoLessonId", videoLessonId }
                }
            };

            await _mediaProducer.EnqueueDataAsync(backgroundJobData);
        }

        private async Task StartUploadVideoJob(IFormFile file, Guid videoLessonId)
        {
            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UPLOAD_FOLDER_NAME);
            Utils.CreateUploadFolderIfNotExist(uploadFolderPath);
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{videoLessonId}{fileExtension}";
            var filePath = await Utils.SaveFileLocally(uploadFolderPath, fileName, file);

            var backgroundJobData = new BackgroundJobData
            {
                JobType = BackgroundJobType.UpdateLessonVideo,
                Data = new Dictionary<string, dynamic>
                {
                    { "VideoLessonId", videoLessonId },
                    { "FilePath", filePath }
                }
            };
            await _mediaProducer.EnqueueDataAsync(backgroundJobData);
        }

        private async Task DeleteLessonResources(string thumbnailURL, string videoURL)
        {
            var tasks = new List<Task>();
            if (!string.IsNullOrEmpty(thumbnailURL)
                && thumbnailURL != Constants.IN_PROGRESS_THUMBNAIL
                && thumbnailURL.StartsWith(Constants.CLOUDINARY_URL_PREFIX))
            {
                tasks.Add(_photoService.DeletePhotoAsync(thumbnailURL));
            }

            if (!string.IsNullOrEmpty(videoURL))
            {
                tasks.Add(_videoService.DeleteVideoAsync(videoURL));
            }

            await Task.WhenAll(tasks);
        }
    }
}