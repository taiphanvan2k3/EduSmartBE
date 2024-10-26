using AutoMapper;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.GrpcServices;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc;
using CourseManagementService.Services.Medias;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TblCourse = CourseManagementService.Database.Schemas.Course;
using TblCourseTag = CourseManagementService.Database.Schemas.CourseTag;

namespace CourseManagementService.Services.CourseManagement.Teacher
{
    public interface ITeacherCourseDetailService
    {
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
        /// Delete a course
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/06</para>
        /// </summary>
        /// <param name="id">Id of the course</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteCourse(Guid id);
    }

    public class TeacherCourseDetailService(IServiceProvider serviceProvider, ILogger<TeacherCourseDetailService> logger)
        : BaseService(serviceProvider, logger), ITeacherCourseDetailService
    {
        private readonly string _serviceName = nameof(TeacherCourseDetailService);
        private readonly IConfiguration _configuration = serviceProvider.GetRequiredService<IConfiguration>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
        private readonly IPhotoService _photoService = serviceProvider.GetRequiredService<IPhotoService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IPhotoService"));
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IMapper"));
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetRequiredService<IGrpcUserService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IGrpcUserService"));
        private readonly CommonProducer _commonProducer = serviceProvider.GetRequiredService<CommonProducer>()
            ?? throw new InvalidOperationException(ServiceInjectionError("CommonProducer"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));

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
                var isExistTeacherTask = _grpcUserService.CheckIfTeacherExists(_appStateService.UserInfo.UserId);
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
                    BriefDescription = courseCreateDto.BriefDescription,
                    DetailedDescription = courseCreateDto.DetailedDescription,
                    ThumbnailURL = courseCreateDto.Thumbnail != null
                        ? Constants.INPROGRESS_THUMBNAIL
                        : Constants.DEFAULT_COURSE_THUMBNAIL,
                    Price = courseCreateDto.Price,
                    Type = courseCreateDto.Type,
                    TeacherId = _appStateService.UserInfo.UserId,
                    CategoryId = courseCreateDto.CategoryId,
                    CurrencyId = (int)courseCreateDto.Currency,
                    Tags = courseCreateDto.TagIds
                        .Select(x => new TblCourseTag { TagId = x })
                        .ToList()
                };

                await _context.Courses.AddAsync(newCourse);
                await _context.SaveChangesAsync();

                // Clear the cache of owned courses
                ClearOwnedCoursesCache(_appStateService.UserInfo.UserId);

                if (courseCreateDto.Thumbnail != null)
                {
                    await StartUploadImageToCloudinaryJob(courseCreateDto.Thumbnail, newCourse.Id);
                }

                var courseDto = _mapper.Map<CourseDto>(newCourse);
                courseDto.Tags = newCourse.Tags.Select(x => new LookupDto
                {
                    Id = x.TagId.ToString(),
                })
                .ToList();
                courseDto.CurrencyCode = currencyTask.Result.Code;

                response.Data.Add("course", courseDto);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
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

                response.Message = "Course deleted successfully";
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
                course.BriefDescription = courseUpdateDto.BriefDescription;
                course.DetailedDescription = courseUpdateDto.DetailedDescription;
                course.ThumbnailURL = courseUpdateDto.ThumbnailURL;
                course.Price = courseUpdateDto.Price;
                course.Type = courseUpdateDto.Type;
                course.CategoryId = courseUpdateDto.CategoryId;
                course.CurrencyId = (int)courseUpdateDto.Currency;

                await _context.CourseTags.Where(x => x.CourseId == id).ExecuteDeleteAsync();
                course.Tags = courseUpdateDto.TagIds
                    .Select(x => new TblCourseTag { TagId = x })
                    .ToList();

                await _context.SaveChangesAsync();

                // Clear the cache of owned courses
                ClearOwnedCoursesCache(_appStateService.UserInfo.UserId);

                var courseDto = _mapper.Map<CourseDto>(course);
                courseDto.Tags = course.Tags.Select(x => new LookupDto
                {
                    Id = x.TagId.ToString()
                })
                .ToList();

                courseDto.CurrencyCode = currencyTask.Result.Code;
                response.Data.Add("course", courseDto);
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
                JobType = BackgroundJobType.UploadImageToCloudinary,
                Data = new Dictionary<string, dynamic>
                {
                    { "FilePath", $"{uploadFolderPath}/{courseId}.jpg" },
                    { "CourseId", courseId }
                }
            };

            await _commonProducer.EnqueueDataAsync(backgroundJobData);
        }

        private void ClearOwnedCoursesCache(int userId)
        {
            var cacheKey = CacheManager.OwnedCourses.Key(userId);
            _cacheService.RemoveData(cacheKey);
        }
    }
}