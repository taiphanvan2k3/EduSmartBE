using CourseManagementService.Common;
using CourseManagementService.Services.CourseManagement.Student.Schemas;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IStudentCourseDetailService
    {
        public Task<ResponseInfo> EnrollCourse(EnrollCourseRequest request);
    }

    public class StudentCourseDetailService(IServiceProvider serviceProvider, ILogger<StudentCourseDetailService> logger)
        : BaseService(serviceProvider, logger), IStudentCourseDetailService
    {
        public Task<ResponseInfo> EnrollCourse(EnrollCourseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}