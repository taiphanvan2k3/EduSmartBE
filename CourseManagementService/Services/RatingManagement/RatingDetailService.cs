using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.RatingManagement.Schemas;
using TblLessonRating = CourseManagementService.Database.Schemas.LessonRating;
using TblCourseRating = CourseManagementService.Database.Schemas.CourseRating;
using Microsoft.EntityFrameworkCore;

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
        /// Delete a course rating
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        /// <param name="courseRatingId">Id of the rating</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteCourseRating(Guid courseId, Guid courseRatingId);

        /// <summary>
        /// Get lesson rating of a student
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Id of the lesson</param>
        public Task<ResponseInfo> GetLessonRatingOfStudent(Guid lessonId);

        /// <summary>
        /// Create a lesson rating
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="lessonRatingCreateDto">Lesson rating information</param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateLessonRating(LessonRatingCreateDto lessonRatingCreateDto);

        /// <summary>
        /// Update a lesson rating
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="lessonRatingUpdateDto">Lesson rating information</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateLessonRating(LessonRatingUpdateDto lessonRatingUpdateDto);

        /// <summary>
        /// Delete a lesson rating
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> DeleteLessonRating(Guid lessonId, Guid lessonRatingId);

        /// <summary>
        /// Get overall lesson rating of a lesson (Teacher only)
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetOverallLessonRating(Guid lessonId);
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

        public async Task<ResponseInfo> GetLessonRatingOfStudent(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var lessonPermissionResponse = await _lessonBaseDetailService.CheckLessonPermission(lessonId, currentUser.UserId);
                if (!lessonPermissionResponse.IsSuccess)
                {
                    return lessonPermissionResponse;
                }

                var lessonRatingEntity = await _context.LessonRatings
                    .FirstOrDefaultAsync(x => x.LessonId == lessonId && x.StudentId == currentUser.UserId);
                if (lessonRatingEntity != null)
                {
                    var lessonRatingDto = _mapper.Map<LessonRatingDetail>(lessonRatingEntity);
                    return new ResponseInfo("myLessonRating", lessonRatingDto);
                }
                else
                {
                    return new ResponseInfo("myLessonRating", null);
                }
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

        public async Task<ResponseInfo> GetOverallLessonRating(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                var overallLessonRating = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .SelectMany(l => l.Ratings)
                    .GroupBy(r => r.LessonId)
                    .Select(r => new LessonRatingOverall()
                    {
                        LessonId = r.Key,
                        LessonName = r.First().Lesson.Title,
                        LikeCount = r.Count(x => x.IsLike),
                        DislikeCount = r.Count(x => !x.IsLike),
                    })
                    .FirstOrDefaultAsync();

                if (overallLessonRating == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
                }

                return new ResponseInfo(resource: "overallLessonRating", data: overallLessonRating);
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

        public async Task<ResponseInfo> UpdateLessonRating(LessonRatingUpdateDto lessonRatingUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var lessonPermissionResponse = await _lessonBaseDetailService.CheckLessonPermission(lessonRatingUpdateDto.LessonId,
                    currentUser.UserId);
                if (!lessonPermissionResponse.IsSuccess)
                {
                    return lessonPermissionResponse;
                }

                var lessonRatingEntity = await _context.LessonRatings.FindAsync(lessonRatingUpdateDto.Id);
                if (lessonRatingEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson rating not found");
                }

                if (lessonRatingEntity.StudentId != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no permission to update this rating");
                }

                lessonRatingEntity.Comment = lessonRatingUpdateDto.Comment;
                lessonRatingEntity.IsLike = lessonRatingUpdateDto.IsLike;

                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                var lessonRatingDto = _mapper.Map<LessonRatingDetail>(lessonRatingEntity);

                responseInfo.Data.Add("lessonRating", lessonRatingDto);
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

        public async Task<ResponseInfo> DeleteCourseRating(Guid courseId, Guid courseRatingId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var coursePermissionResponse = await _lessonBaseDetailService.CheckCoursePermission(courseId,
                    currentUser.UserId);
                if (!coursePermissionResponse.IsSuccess)
                {
                    return coursePermissionResponse;
                }

                var courseRatingEntity = await _context.CourseRatings.FindAsync(courseRatingId);
                if (courseRatingEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Course rating not found");
                }

                if (courseRatingEntity.UserId != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no permission to delete this rating");
                }

                _context.CourseRatings.Remove(courseRatingEntity);
                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo("courseRatingId", courseRatingId);
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

        public async Task<ResponseInfo> DeleteLessonRating(Guid lessonId, Guid lessonRatingId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var lessonPermissionResponse = await _lessonBaseDetailService.CheckLessonPermission(lessonId,
                    currentUser.UserId);
                if (!lessonPermissionResponse.IsSuccess)
                {
                    return lessonPermissionResponse;
                }

                var lessonRatingEntity = await _context.LessonRatings.FindAsync(lessonRatingId);
                if (lessonRatingEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson rating not found");
                }

                if (lessonRatingEntity.StudentId != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no permission to delete this rating");
                }

                _context.LessonRatings.Remove(lessonRatingEntity);
                await _context.SaveChangesAsync();

                return new ResponseInfo(resource: "lessonRatingId", lessonRatingId);
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