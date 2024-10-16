using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Teacher
{
    public interface IListOfTeacherCoursesService
    {
        /// <summary>
        /// Get all courses that teacher created
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/07</para>
        /// </summary>
        public Task<List<CourseDetailDto>> GetOwnCourses();
    }

    public class ListOfTeacherCoursesService(IServiceProvider serviceProvider, ILogger<TeacherCourseDetailService> logger) 
        : BaseService(serviceProvider, logger), IListOfTeacherCoursesService
    {
        private readonly string _serviceName = nameof(ListOfTeacherCoursesService);

        public async Task<List<CourseDetailDto>> GetOwnCourses()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);

                List<CourseDetailDto> courses = await _context.Courses
                    .Where(x => x.TeacherId == _appStateService.UserInfo.UserId)
                    .Select(x => new CourseDetailDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        BriefDescription = x.BriefDescription,
                        DetailedDescription = x.DetailedDescription,
                        ThumbnailURL = x.ThumbnailURL,
                        Price = x.Price,
                        Type = x.Type,
                        TotalStudents = x.Enrollments.Count
                    })
                    .ToListAsync();

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, methodName);
                return courses;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }
    }
}