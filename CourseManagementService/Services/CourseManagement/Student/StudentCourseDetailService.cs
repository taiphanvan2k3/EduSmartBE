using CourseManagementService.Common;
using CourseManagementService.Services.CourseManagement.Student.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IStudentCourseDetailService
    {
        public Task<bool> IsCourseEnrolled(Guid courseId);
        public Task<ResponseInfo> EnrollCourse(EnrollCourseRequest request);
    }

    public class StudentCourseDetailService(IServiceProvider serviceProvider, ILogger<StudentCourseDetailService> logger)
        : BaseService(serviceProvider, logger), IStudentCourseDetailService
    {
        public Task<ResponseInfo> EnrollCourse(EnrollCourseRequest request)
        {
            throw new NotImplementedException();
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
    }
}