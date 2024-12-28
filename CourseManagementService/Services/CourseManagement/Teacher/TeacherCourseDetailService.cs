using AutoMapper;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Database.Schemas.NotificationEntities;
using CourseManagementService.Enumerations;
using CourseManagementService.Extensions;
using CourseManagementService.GrpcServices;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc.UserService;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using CourseManagementService.Services.Medias;
using CourseManagementService.Services.NotificationManagement.Schemas;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using TblCourse = CourseManagementService.Database.Schemas.Course;
using TblCourseTag = CourseManagementService.Database.Schemas.CourseTag;

namespace CourseManagementService.Services.CourseManagement.Teacher
{
    public interface ITeacherCourseDetailService
    {
        /// <summary>
        /// Get teaching analysis of current teacher
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/17</para>
        /// </summary>
        /// <returns></returns>
        public Task<TeachingAnalysis> GetTeachingAnalysis();

        /// <summary>
        /// Get student enrollments of a course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/17</para>
        /// </summary>
        /// <param name="paramsSearch">Search parameters</param>
        /// <returns></returns>
        public Task<PaginatedList<StudentEnrollmentOverall>> GetStudentEnrollments(ParamsSearch paramsSearch);

        /// <summary>
        /// Get student enrollments of a course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/17</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <param name="paramsSearch">Search parameters</param>
        /// <returns></returns>
        public Task<PaginatedList<StudentEnrollmentInCourse>> GetStudentEnrollmentsByCourse(Guid courseId, ParamsSearch paramsSearch);

        /// <summary>
        /// Create a new course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/02</para>
        /// </summary>
        /// <param name="courseCreateDto">Course information to create</param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateCourse(CourseCreateDto courseCreateDto);

        /// <summary>
        /// Create a new course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/06</para>
        /// </summary>
        /// <param name="id">Id of the course</param>
        /// <param name="courseUpdateDto">Course information to update</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateCourse(Guid id, CourseUpdateDto courseUpdateDto);

        /// <summary>
        /// Update course thumbnail
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/26</para>
        /// </summary>
        /// <param name="id"> Id of the course</param>
        /// <param name="thumbnailURL">URL of the thumbnail</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateCourseThumbnail(Guid id, string thumbnailURL);

        /// <summary>
        /// Update preview video of the course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/15</para>
        /// </summary>
        /// <param name="courseId"> Id of the course</param>
        /// <param name="videoUrl">URL of the preview video</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdatePreviewVideo(Guid courseId, string videoUrl);

        /// <summary>
        /// Update order of chapters and lessons in a course
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <param name="courseOrderSetting">Chapter orders and lesson orders</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateCourseOrderSettings(Guid courseId, CourseOrderSetting courseOrderSetting);

        /// <summary>
        /// Delete a course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/06</para>
        /// </summary>
        /// <param name="id">Id of the course</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteCourse(Guid id);

        /// <summary>
        /// Get teacher id of a course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/12/03</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        public Task<int> GetTeacherIdOfCourse(Guid courseId);
    }

    public class TeacherCourseDetailService(IServiceProvider serviceProvider, ILogger<TeacherCourseDetailService> logger)
        : BaseService(serviceProvider, logger), ITeacherCourseDetailService
    {
        private readonly string _serviceName = nameof(TeacherCourseDetailService);
        private readonly IConfiguration _configuration = serviceProvider.GetRequiredService<IConfiguration>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
        private readonly IPhotoService _photoService = serviceProvider.GetRequiredService<IPhotoService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IPhotoService"));
        private readonly IVideoService _videoService = serviceProvider.GetRequiredService<IVideoService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IVideoService"));
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IMapper"));
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetRequiredService<IGrpcUserService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IGrpcUserService"));
        private readonly CommonProducer _commonProducer = serviceProvider.GetRequiredService<CommonProducer>()
            ?? throw new InvalidOperationException(ServiceInjectionError("CommonProducer"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));

        public async Task<TeachingAnalysis> GetTeachingAnalysis()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var teachingAnalysis = new TeachingAnalysis();

                var currentUser = GetCurrentUser();
                var cacheKey = CacheManager.TeachingAnalysis.Key(currentUser.UserId);
                var cachedData = _cacheService.GetData<TeachingAnalysis>(cacheKey);

                if (cachedData != null)
                {
                    LogInfo("End", methodName);
                    return cachedData;
                }

                var ownedCourseQuantity = await _context.Courses
                    .Where(x => x.TeacherId == currentUser.UserId)
                    .CountAsync();

                var studentQuantity = await _context.CourseEnrollments
                    .Where(x => x.Course.TeacherId == currentUser.UserId)
                    .Select(x => x.StudentId)
                    .Distinct()
                    .CountAsync();

                teachingAnalysis.CourseQuantity = ownedCourseQuantity;
                teachingAnalysis.StudentQuantity = studentQuantity;

                _cacheService.SetData(cacheKey, teachingAnalysis,
                    DateTimeOffset.Now.AddMinutes(CacheManager.TeachingAnalysis.ExpireTimeInMinutes));

                LogInfo("End", methodName);
                return teachingAnalysis;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<PaginatedList<StudentEnrollmentOverall>> GetStudentEnrollments(ParamsSearch paramsSearch)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var cacheKey = CacheManager.StudentEnrollments.Overall.Key(currentUser.UserId, paramsSearch.CurrentPage, paramsSearch.PageSize);
                var cachedData = _cacheService.GetData<PaginatedList<StudentEnrollmentOverall>>(cacheKey);
                if (cachedData != null)
                {
                    LogInfo("End", methodName);
                    return cachedData;
                }

                var studentEnrollments = await _context.CourseEnrollments
                    .Where(x => x.Course.TeacherId == currentUser.UserId)
                    .GroupBy(x => x.StudentId)
                    .Select(g => new StudentEnrollmentOverall
                    {
                        StudentId = g.Key,
                        EnrollmentCount = g.Count()
                    })
                    .ToPaginatedListAsync(paramsSearch.CurrentPage, paramsSearch.PageSize);

                await FillStudentInfo(studentEnrollments.Items);

                _cacheService.SetData(cacheKey, studentEnrollments,
                    DateTimeOffset.Now.AddMinutes(CacheManager.StudentEnrollments.Overall.ExpireTimeInMinutes));

                LogInfo("End", methodName);
                return studentEnrollments;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<PaginatedList<StudentEnrollmentInCourse>> GetStudentEnrollmentsByCourse(Guid courseId, ParamsSearch paramsSearch)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var cacheKey = CacheManager.StudentEnrollments.InCourse.Key(courseId, paramsSearch.CurrentPage, paramsSearch.PageSize);
                var cachedData = _cacheService.GetData<PaginatedList<StudentEnrollmentInCourse>>(cacheKey);
                if (cachedData != null)
                {
                    LogInfo("End", methodName);
                    return cachedData;
                }

                var studentEnrollments = await _context.CourseEnrollments
                    .Where(x => x.CourseId == courseId && x.Course.TeacherId == currentUser.UserId)
                    .Select(x => new StudentEnrollmentInCourse()
                    {
                        StudentId = x.StudentId,
                        EnrollmentDate = x.EnrollmentDate,
                        LeaveDate = x.LeaveDate,

                        // TODO: Check whether the student is still can access the course
                        IsActive = true
                    })
                    .ToPaginatedListAsync(paramsSearch.CurrentPage, paramsSearch.PageSize);

                await FillStudentInfo(studentEnrollments.Items);

                _cacheService.SetData(cacheKey, studentEnrollments,
                    DateTimeOffset.Now.AddMinutes(CacheManager.StudentEnrollments.InCourse.ExpireTimeInMinutes));

                LogInfo("End", methodName);
                return studentEnrollments;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateCourse(CourseCreateDto courseCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                var response = new ResponseInfo();
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);

                using var dbContextForCategory = _dbContextFactory.CreateDbContext();
                using var dbContextForCurrency = _dbContextFactory.CreateDbContext();

                var categoryTask = dbContextForCategory.Categories.FindAsync(courseCreateDto.CategoryId).AsTask();
                var currencyTask = dbContextForCurrency.Currencies.FindAsync((int)courseCreateDto.Currency).AsTask();

                var isValidSelectedTagsTask = IsValidSelectedTags(courseCreateDto.TagIds);
                var isExistTeacherTask = _grpcUserService.CheckUserExist(_appStateService.UserInfo.UserId);
                await Task.WhenAll(categoryTask, currencyTask, isValidSelectedTagsTask, isExistTeacherTask);

                var errorMessages = new List<string>();
                if (categoryTask.Result == null)
                {
                    errorMessages.Add("Category does not exist");
                }

                if (!isExistTeacherTask.Result)
                {
                    errorMessages.Add("Teacher does not exist");
                }

                if (currencyTask.Result == null)
                {
                    errorMessages.Add("Currency does not exist");
                }

                if (!isValidSelectedTagsTask.Result)
                {
                    errorMessages.Add("Invalid selected tags");
                }

                if (errorMessages.Count > 0)
                {
                    response.Error = "InvalidData";
                    response.Message = string.Join(", ", errorMessages);
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var newCourse = new TblCourse
                {
                    Name = courseCreateDto.Name,
                    Description = courseCreateDto.Description,
                    CoreValues = courseCreateDto.CoreValues,
                    Prerequisites = courseCreateDto.Prerequisites,
                    ThumbnailURL = courseCreateDto.Thumbnail != null
                        ? Constants.IN_PROGRESS_THUMBNAIL
                        : Constants.DEFAULT_COURSE_THUMBNAIL,
                    Price = courseCreateDto.Price,
                    Type = courseCreateDto.Type,
                    TeacherId = _appStateService.UserInfo.UserId,
                    CategoryId = courseCreateDto.CategoryId,
                    CurrencyId = (int)courseCreateDto.Currency,
                    Tags = courseCreateDto.TagIds
                        .Select(x => new TblCourseTag { TagId = x })
                        .ToList(),
                    IsPublished = courseCreateDto.IsPublished
                };

                await _context.Courses.AddAsync(newCourse);
                await _context.SaveChangesAsync();

                if (courseCreateDto.Thumbnail != null)
                {
                    await StartUploadImageToCloudinaryJob(courseCreateDto.Thumbnail, newCourse.Id);
                }

                if (courseCreateDto.PreviewVideo != null)
                {
                    await StartUploadPreviewVideoJob(courseCreateDto.PreviewVideo, newCourse.Id);
                }

                var courseDto = _mapper.Map<CourseDto>(newCourse);
                courseDto.Tags = newCourse.Tags.Select(x => new LookupDto
                {
                    Id = x.TagId.ToString(),
                })
                .ToList();
                courseDto.CurrencyCode = currencyTask.Result.Code;

                // Clear the cache of owned courses
                ClearOwnedCoursesCache(_appStateService.UserInfo.UserId);
                _cacheService.RemoveData(CacheManager.TeachingAnalysis.Key(_appStateService.UserInfo.UserId));

                response.Data.Add("Course", courseDto);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateCourse(Guid id, CourseUpdateDto courseUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var response = new ResponseInfo();

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    response.Error = "NotFound";
                    response.Message = "Course does not exist";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                using var dbContextForCategory = _dbContextFactory.CreateDbContext();
                using var dbContextForCurrency = _dbContextFactory.CreateDbContext();

                var categoryTask = dbContextForCategory.Categories.FindAsync(courseUpdateDto.CategoryId).AsTask();
                var currencyTask = dbContextForCurrency.Currencies.FindAsync((int)courseUpdateDto.Currency).AsTask();

                var isValidSelectedTagsTask = IsValidSelectedTags(courseUpdateDto.TagIds);
                await Task.WhenAll(categoryTask, currencyTask, isValidSelectedTagsTask);

                var errorMessages = new List<string>();

                if (categoryTask.Result == null)
                {
                    errorMessages.Add("Category does not exist");
                }

                if (currencyTask.Result == null)
                {
                    errorMessages.Add("Currency does not exist");
                }

                if (!isValidSelectedTagsTask.Result)
                {
                    errorMessages.Add("Invalid selected tags");
                }

                if (errorMessages.Count > 0)
                {
                    response.Error = "InvalidData";
                    response.Message = string.Join(", ", errorMessages);
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                course.Name = courseUpdateDto.Name;
                course.Description = courseUpdateDto.Description;
                course.CoreValues = courseUpdateDto.CoreValues;
                course.Prerequisites = courseUpdateDto.Prerequisites;
                course.Price = courseUpdateDto.Price;
                course.Type = courseUpdateDto.Type;
                course.CategoryId = courseUpdateDto.CategoryId;
                course.CurrencyId = (int)courseUpdateDto.Currency;
                course.IsPublished = courseUpdateDto.IsPublished;

                if (courseUpdateDto.Thumbnail != null)
                {
                    if (course.ThumbnailURL.StartsWith(Constants.CLOUDINARY_URL_PREFIX)
                        && course.ThumbnailURL != Constants.DEFAULT_COURSE_THUMBNAIL)
                    {
                        await StartDeleteImageFromCloudinaryJob(course.ThumbnailURL);
                    }

                    course.ThumbnailURL = Constants.IN_PROGRESS_THUMBNAIL;
                    await StartUploadImageToCloudinaryJob(courseUpdateDto.Thumbnail, course.Id);
                }

                if (courseUpdateDto.PreviewVideo != null)
                {
                    if (course.PreviewVideoURL != null && course.PreviewVideoURL.StartsWith(Constants.AZURE_STORAGE_URL_PREFIX))
                    {
                        await _videoService.DeleteVideoAsync(course.PreviewVideoURL);
                    }
                    await StartUploadPreviewVideoJob(courseUpdateDto.PreviewVideo, course.Id);
                }

                await _context.CourseTags.Where(x => x.CourseId == id).ExecuteDeleteAsync();
                course.Tags = courseUpdateDto.TagIds
                    .Select(x => new TblCourseTag { TagId = x })
                    .ToList();

                await _context.SaveChangesAsync();

                // Clear the cache of owned courses
                // TODO: Clear cache for public course detail API in a background job
                ClearOwnedCoursesCache(_appStateService.UserInfo.UserId);

                // Clear cache for public course detail API
                await _cacheService.RemoveDataByPattern(CacheManager.CourseDetail.GetPrefixKey(courseId: id));

                var courseDto = _mapper.Map<CourseDto>(course);
                courseDto.Tags = course.Tags.Select(x => new LookupDto
                {
                    Id = x.TagId.ToString()
                })
                .ToList();

                courseDto.CurrencyCode = currencyTask.Result.Code;
                response.Data.Add("Course", courseDto);

                await NotifyWhenCourseUpdated(id);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateCourseThumbnail(Guid id, string thumbnailURL)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var response = new ResponseInfo();

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    response.Error = "NotFound";
                    response.Message = "Course does not exist";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                course.ThumbnailURL = thumbnailURL;
                await _context.SaveChangesAsync();

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdatePreviewVideo(Guid courseId, string videoURL)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var response = new ResponseInfo();

                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    response.Error = "NotFound";
                    response.Message = "Course does not exist";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                course.PreviewVideoURL = videoURL;
                await _context.SaveChangesAsync();

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateCourseOrderSettings(Guid courseId, CourseOrderSetting courseOrderSetting)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();
                ResponseInfo response = await CanModifyCourse(courseId, currentUser.UserId);

                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return response;
                }

                var chapterInputs = courseOrderSetting.ChapterOrders;
                var lessonInputs = courseOrderSetting.ChapterOrders.SelectMany(x => x.LessonOrders).ToList();

                // Kiểm tra có bị duplicate không
                var chapterInputDistinct = chapterInputs.Distinct().Select(x => x.Id).ToList();
                var lessonInputDistinct = lessonInputs.Distinct().Select(x => x.Id).ToList();

                if (chapterInputs.Count != chapterInputDistinct.Count
                    || lessonInputs.Count != lessonInputDistinct.Count)
                {
                    response.Error = "InvalidData";
                    response.Message = "Duplicate order detected";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var chapterEntities = await _context.Chapters
                    .Where(chapter => chapter.CourseId == courseId && chapterInputDistinct.Contains(chapter.Id))
                    .ToListAsync();

                if (chapterEntities.Count != chapterInputDistinct.Count)
                {
                    response.Error = "InvalidData";
                    response.Message = "Some chapters do not belong to the course";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var lessonEntities = await _context.Lessons
                    .Where(lesson => lesson.Chapter.CourseId == courseId && lessonInputDistinct.Contains(lesson.Id))
                    .ToListAsync();

                if (lessonEntities.Count != lessonInputDistinct.Count)
                {
                    response.Error = "InvalidData";
                    response.Message = "Some lessons do not belong to the course";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                // Update order for chapters
                Dictionary<Guid, int> chapterOrderDict = chapterInputs.ToDictionary(x => x.Id, x => x.Order);
                foreach (var chapter in chapterEntities)
                {
                    if (chapter.Order != chapterOrderDict[chapter.Id])
                    {
                        chapter.Order = chapterOrderDict[chapter.Id];
                    }
                }

                // Update order and chapterId for lessons
                Dictionary<Guid, LessonOrder> lessonOrderDict = lessonInputs.ToDictionary(x => x.Id);
                foreach (var lesson in lessonEntities)
                {
                    if (lesson.Order != lessonOrderDict[lesson.Id].Order
                        || lesson.ChapterId != lessonOrderDict[lesson.Id].ChapterId)
                    {
                        lesson.Order = lessonOrderDict[lesson.Id].Order;
                        lesson.ChapterId = lessonOrderDict[lesson.Id].ChapterId;
                    }
                }

                await _context.SaveChangesAsync();

                ClearOwnedCoursesCache(currentUser.UserId);
                response.Data.Add("courseOrderSetting", courseOrderSetting);

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteCourse(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var response = new ResponseInfo();

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    response.Error = "NotFound";
                    response.Message = "Course does not exist";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                if (course.TeacherId != _appStateService.UserInfo.UserId)
                {
                    response.Error = "Unauthorized";
                    response.Message = "Unauthorized access";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var isExistEnrollment = await _context.CourseEnrollments.AnyAsync(x => x.CourseId == id);
                if (isExistEnrollment)
                {
                    response.Error = "DeleteNotAllowed";
                    response.Message = "Course has enrollments";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                await _context.Courses.Where(x => x.Id == id).ExecuteDeleteAsync();

                // Clear the cache of owned courses
                ClearOwnedCoursesCache(_appStateService.UserInfo.UserId);
                _cacheService.RemoveData(CacheManager.TeachingAnalysis.Key(_appStateService.UserInfo.UserId));

                response.Message = "Course deleted successfully";
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<int> GetTeacherIdOfCourse(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var cachedTeacherId = _cacheService.GetData<int>(CacheManager.TeacherIdOfCourse.KeyFromCourseId(courseId));
                if (cachedTeacherId != 0)
                {
                    return cachedTeacherId;
                }

                var teacherId = await _context.Courses
                    .Where(d => d.Id == courseId)
                    .Select(d => d.TeacherId)
                    .FirstOrDefaultAsync();

                _cacheService.SetData(CacheManager.TeacherIdOfCourse.KeyFromCourseId(courseId), teacherId,
                    DateTimeOffset.Now.AddMinutes(CacheManager.TeacherIdOfCourse.ExpireTimeInMinutes));

                LogInfo("End", methodName);
                return teacherId;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        private async Task<bool> CheckIfTeacherExists(int teacherId)
        {
            var userServiceGrpcURL = _configuration.GetValue<string>("ExternalServices:UserService:GrpcUrl");
            var channel = GrpcChannel.ForAddress(userServiceGrpcURL);
            var client = new User.UserClient(channel);

            return (await client.CheckUserExistAsync(new UserRequest { Id = teacherId })).IsExist;
        }

        private async Task<bool> IsValidSelectedTags(List<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return true;
            }

            using var dbContext = _dbContextFactory.CreateDbContext();
            var matchingTagCount = await dbContext.Tags
                .AsNoTracking()
                .Where(x => tagIds.Contains(x.Id))
                .CountAsync();

            return matchingTagCount == tagIds.Count;
        }

        private async Task StartUploadImageToCloudinaryJob(IFormFile file, Guid courseId)
        {
            // Save the file to the local storage
            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UPLOAD_FOLDER_NAME);
            Utils.CreateUploadFolderIfNotExist(uploadFolderPath);
            await Utils.SaveFileLocally(uploadFolderPath, $"{courseId}.jpg", file);

            // Start the background job to upload the file to Cloudinary
            var backgroundJobData = new BackgroundJobData
            {
                JobType = BackgroundJobType.UpdateCourseThumbnail,
                Data = new Dictionary<string, dynamic>
                {
                    { "FilePath", $"{uploadFolderPath}/{courseId}.jpg" },
                    { "CourseId", courseId }
                }
            };

            await _commonProducer.EnqueueDataAsync(backgroundJobData);
        }

        private async Task StartUploadPreviewVideoJob(IFormFile file, Guid courseId)
        {
            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UPLOAD_FOLDER_NAME);
            Utils.CreateUploadFolderIfNotExist(uploadFolderPath);
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{courseId}{fileExtension}";
            var filePath = await Utils.SaveFileLocally(uploadFolderPath, fileName, file);

            var backgroundJobData = new BackgroundJobData
            {
                JobType = BackgroundJobType.UpdateCoursePreviewVideo,
                Data = new Dictionary<string, dynamic>
                {
                    { "CourseId", courseId },
                    { "FilePath", filePath }
                }
            };
            await _commonProducer.EnqueueDataAsync(backgroundJobData);
        }

        private async Task StartDeleteImageFromCloudinaryJob(string publicURL)
        {
            var publicId = Utils.ExtractPublicId(publicURL);
            if (string.IsNullOrEmpty(publicId))
            {
                return;
            }

            var backgroundJobData = new BackgroundJobData
            {
                JobType = BackgroundJobType.DeleteCourseThumbnail,
                Data = new Dictionary<string, dynamic>
                {
                    { "PublicId", publicId }
                }
            };

            await _commonProducer.EnqueueDataAsync(backgroundJobData);
        }

        private async Task<ResponseInfo> CanModifyCourse(Guid courseId, int teacherId)
        {
            var responseInfo = new ResponseInfo();
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                responseInfo.Error = "NotFound";
                responseInfo.Message = "Course does not exist";
                responseInfo.StatusCode = StatusCodes.Status404NotFound;
                return responseInfo;
            }

            if (course.TeacherId != teacherId)
            {
                responseInfo.Error = "Unauthorized";
                responseInfo.Message = "Unauthorized access";
                responseInfo.StatusCode = StatusCodes.Status401Unauthorized;
                return responseInfo;
            }

            return responseInfo;
        }

        private async Task FillStudentInfo<T>(List<T> studentEnrollments) where T : StudentEnrollmentDto
        {
            var studentIds = studentEnrollments.Select(x => x.StudentId).ToList();
            var students = await _grpcUserService.GetListOfStudents(studentIds);
            Dictionary<int, SimpleUserResponse> studentDict = students.Users.ToDictionary(x => x.Id);

            foreach (var studentEnrollment in studentEnrollments.Where(x => studentDict.ContainsKey(x.StudentId)))
            {
                var student = studentDict[studentEnrollment.StudentId];
                studentEnrollment.Username = student.UserName;
                studentEnrollment.FullName = student.FullName;
                studentEnrollment.Email = student.Email;
                studentEnrollment.AvatarURL = student.AvatarURL;
            }
        }

        private void ClearOwnedCoursesCache(int userId)
        {
            var prefixOwnedCoursesCacheKey = CacheManager.OwnedCourses.PrefixKey(userId);
            _cacheService.RemoveDataByPatternUsingLuaScript(prefixOwnedCoursesCacheKey);
        }

        private async Task NotifyWhenCourseUpdated(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                var notificationCreateDto = new NotificationCreateDto()
                {
                    Type = NotificationType.CourseDetailModification,
                    RelatedEntityId = courseId.ToString(),
                    RelatedEntityType = RelatedEntityType.Course,
                    CourseId = courseId,
                    MetaData = []
                };

                await _commonProducer.EnqueueDataAsync(new BackgroundJobData()
                {
                    JobType = BackgroundJobType.CreateNotification,
                    Data = new Dictionary<string, dynamic>()
                    {
                        { "notificationCreateDto", notificationCreateDto }
                    }
                });
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }
    }
}