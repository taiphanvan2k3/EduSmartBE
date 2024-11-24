using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CourseManagement.Student.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IStudentCourseDetailService
    {
        public Task<bool> IsCourseEnrolled(Guid courseId);
        public Task<ResponseInfo> UpdateCourseStatus(Guid courseId, SingleUpdateVisibilityStatus request);
    }

    public class StudentCourseDetailService(IServiceProvider serviceProvider, ILogger<StudentCourseDetailService> logger)
        : BaseService(serviceProvider, logger), IStudentCourseDetailService
    {
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("ICacheService"));

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