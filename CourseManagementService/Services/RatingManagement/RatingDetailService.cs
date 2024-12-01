using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.RatingManagement.Schemas;
using TblLessonRating = CourseManagementService.Database.Schemas.LessonRating;
using TblCourseRating = CourseManagementService.Database.Schemas.CourseRating;

namespace CourseManagementService.Services.RatingManagement
{
    public interface IRatingDetailService
    {
        /// <summary>
        /// Create a course rating
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="courseRatingCreate">Course rating information</param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateCourseRating(CourseRatingCreateDto courseRatingCreate);

        /// <summary>
        /// Update a course rating
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseRatingUpdate"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateCourseRating(CourseRatingUpdateDto courseRatingUpdate);

        /// <summary>
        /// Create a lesson rating
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="lessonRatingCreateDto">Lesson rating information</param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateLessonRating(LessonRatingCreateDto lessonRatingCreateDto);
    }

    public class RatingDetailService(IServiceProvider serviceProvider, ILogger<RatingDetailService> logger)
        : BaseService(serviceProvider, logger), IRatingDetailService
    {
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly IMapper _mapper = serviceProvider.GetService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));

        public async Task<ResponseInfo> CreateCourseRating(CourseRatingCreateDto courseRatingCreate)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var coursePermissionResponse = await _lessonBaseDetailService.CheckCoursePermission(courseRatingCreate.CourseId,
                    currentUser.UserId);
                if (!coursePermissionResponse.IsSuccess)
                {
                    return coursePermissionResponse;
                }

                var courseRatingEntity = _mapper.Map<TblCourseRating>(courseRatingCreate);
                courseRatingEntity.UserId = currentUser.UserId;

                await _context.CourseRatings.AddAsync(courseRatingEntity);
                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("courseRatingId", courseRatingEntity.Id);

                return responseInfo;
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

        public async Task<ResponseInfo> CreateLessonRating(LessonRatingCreateDto lessonRatingCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var lessonPermissionResponse = await _lessonBaseDetailService.CheckLessonPermission(lessonRatingCreateDto.LessonId, currentUser.UserId);
                if (!lessonPermissionResponse.IsSuccess)
                {
                    return lessonPermissionResponse;
                }

                var lessonRatingEntity = _mapper.Map<TblLessonRating>(lessonRatingCreateDto);
                lessonRatingEntity.StudentId = currentUser.UserId;

                await _context.LessonRatings.AddAsync(lessonRatingEntity);
                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("lessonRatingId", lessonRatingEntity.Id);

                return responseInfo;
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

        public async Task<ResponseInfo> UpdateCourseRating(CourseRatingUpdateDto courseRatingUpdate)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var coursePermissionResponse = await _lessonBaseDetailService.CheckCoursePermission(courseRatingUpdate.CourseId,
                    currentUser.UserId);
                if (!coursePermissionResponse.IsSuccess)
                {
                    return coursePermissionResponse;
                }

                var courseRatingEntity = await _context.CourseRatings.FindAsync(courseRatingUpdate.Id);
                if (courseRatingEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Course rating not found");
                }

                if (courseRatingEntity.UserId != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no permission to update this rating");
                }

                courseRatingEntity.Comment = courseRatingUpdate.Comment;
                courseRatingEntity.Rating = courseRatingUpdate.Rating;

                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                var courseRatingDto = _mapper.Map<CourseRatingDetail>(courseRatingEntity);

                responseInfo.Data.Add("courseRating", courseRatingDto);
                return responseInfo;
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