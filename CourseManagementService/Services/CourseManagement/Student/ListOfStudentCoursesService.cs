using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IListOfStudentCoursesService
    {
        public Task<List<CourseDetailDto>> GetEnrolledCourses();
    }

    public class ListOfStudentCoursesService : BaseService, IListOfStudentCoursesService
    {
        private readonly string _serviceName = nameof(ListOfStudentCoursesService);

        public async Task<List<CourseDetailDto>> GetEnrolledCourses()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);

                var courses = await _context.CourseEnrollments
                    .Where(x => x.StudentId == _appStateService.UserInfo.UserId)
                    .Select(c => new CourseDetailDto()
                    {
                        Id = c.Course.Id,
                        Name = c.Course.Name,
                        BriefDescription = c.Course.BriefDescription,
                        DetailedDescription = c.Course.DetailedDescription,
                        ThumbnailURL = c.Course.ThumbnailURL,
                        TotalStudents = c.Course.Enrollments.Count,
                        TeacherId = c.Course.TeacherId
                    })
                    .ToListAsync();

                var studentIds = courses.Select(x => x.TeacherId).Distinct().ToList();


                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return courses;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }
    }
}