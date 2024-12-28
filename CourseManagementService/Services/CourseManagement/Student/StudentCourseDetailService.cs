using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Student.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase;
using Microsoft.EntityFrameworkCore;
using TblLessonTracking = CourseManagementService.Database.Schemas.LessonTracking;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IStudentCourseDetailService
    {
        /// <summary>
        /// Check if the current user is enrolled in the course
        /// <para>Created at: 2024/12/19</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="courseId">Id of the course</param>
        public Task<bool> IsCourseEnrolled(Guid courseId);

        /// <summary>
        /// Update the visibility status of the course
        /// <para>Created at: 2024/12/24</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> UpdateCourseStatus(Guid courseId, SingleUpdateVisibilityStatus request);

        /// <summary>
        /// Unlock the first lesson of the course
        /// <para>Created at: 2024/12/24</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task UnlockFirstLesson(Guid courseId, int userId);

        /// <summary>
        /// Get list of unlocked lessons in a course of a student
        /// <para>Created at: 2024/12/24</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetUnlockedLessons(Guid courseId);
    }

    public class StudentCourseDetailService(IServiceProvider serviceProvider, ILogger<StudentCourseDetailService> logger)
        : BaseService(serviceProvider, logger), IStudentCourseDetailService
    {
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("ICacheService"));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("ILessonBaseDetailService"));

        public async Task<ResponseInfo> GetUnlockedLessons(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canAccessCourse = await _context.CourseEnrollments
                    .Where(x => x.CourseId == courseId && x.StudentId == currentUser.UserId)
                    .Select(x => new
                    {
                        IsLeft = x.LeaveDate.HasValue,
                    })
                    .FirstOrDefaultAsync();

                if (canAccessCourse == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden,
                        "You are not enrolled in this course");
                }

                if (canAccessCourse.IsLeft)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden,
                        "Cannot access course that you have left");
                }

                var unlockedLessons = await _lessonBaseDetailService.GetUnlockedLessons(courseId, currentUser.UserId);
                return CreateResponseInfo("unlockedLessons", unlockedLessons);
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

        public async Task<bool> IsCourseEnrolled(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser() ?? throw new UnauthorizedAccessException("Unauthorized access");

                var isEnrolled = await _context.CourseEnrollments
                    .AnyAsync(x => x.CourseId == courseId && x.StudentId == currentUser.UserId);

                LogInfo("End", methodName);
                return isEnrolled;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task UnlockFirstLesson(Guid courseId, int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var firstLesson = await _lessonBaseDetailService.GetFirstLessonInfo(courseId);
                var lessonTrackingEntity = new TblLessonTracking()
                {
                    LessonId = firstLesson.Id,
                    StudentId = userId,
                    CourseId = courseId,
                    IsCompleted = false,
                };

                await _context.LessonTrackings.AddAsync(lessonTrackingEntity);
                await _context.SaveChangesAsync();

                LogInfo("End", methodName);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateCourseStatus(Guid courseId, SingleUpdateVisibilityStatus request)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var currentUser = GetCurrentUser();

                var course = await _context.CourseEnrollments
                    .Where(c => c.CourseId == courseId && c.StudentId == currentUser.UserId && !c.LeaveDate.HasValue)
                    .FirstOrDefaultAsync();

                if (course == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest,
                        "Cannot update status of course that you are not currently enrolled");
                }

                if (course.VisibilityStatus == request.VisibilityStatus)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Visibility status is same");
                }

                course.VisibilityStatus = request.VisibilityStatus;
                await _context.SaveChangesAsync();


                responseInfo.Message = "Update course status successfully";
                responseInfo.Data.Add("currentStatus", Utils.GetEnumName(request.VisibilityStatus));

                _cacheService.RemoveData(CacheManager.CourseProgressOfOtherUser.Key(currentUser.UserId));
                _cacheService.RemoveData(CacheManager.EnrolledCourses.Key(currentUser.UserId));

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}