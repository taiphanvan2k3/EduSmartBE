using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CourseManagement.Student
{
    public interface IListOfStudentCoursesService
    {
        public Task<List<CourseDetailWithTeacherDto>> GetEnrolledCourses();
    }

    public class ListOfStudentCoursesService(IServiceProvider serviceProvider, ILogger<ListOfStudentCoursesService> logger)
        : BaseService(serviceProvider, logger), IListOfStudentCoursesService
    {
        private readonly string _serviceName = nameof(ListOfStudentCoursesService);
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IGrpcUserService"));

        public async Task<List<CourseDetailWithTeacherDto>> GetEnrolledCourses()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);

                var courses = await _context.CourseEnrollments
                    .Where(x => x.StudentId == _appStateService.UserInfo.UserId)
                    .OrderByDescending(x => x.EnrollmentDate)
                    .Select(c => new CourseDetailWithTeacherDto()
                    {
                        Id = c.Course.Id,
                        Name = c.Course.Name,
                        BriefDescription = c.Course.BriefDescription,
                        DetailedDescription = c.Course.DetailedDescription,
                        ThumbnailURL = c.Course.ThumbnailURL,
                        TotalStudents = c.Course.Enrollments.Count,
                        Teacher = new TeacherDetail()
                        {
                            Id = c.Course.TeacherId
                        }
                    })
                    .ToListAsync();

                var studentIds = courses.Select(x => x.Teacher.Id).Distinct().ToList();
                var teachersDict = courses.GroupBy(x => x.Teacher.Id)
                    .ToDictionary(g => g.Key, g => g.First().Teacher);

                // Get teachers' info from UserService
                var teachers = await _grpcUserService.GetListOfTeachers(studentIds);
                foreach (var teacher in teachers.Users)
                {
                    if (teachersDict.TryGetValue(teacher.Id, out var teacherDetail))
                    {
                        teacherDetail.FullName = teacher.FullName;
                        teacherDetail.AvatarURL = teacher.AvatarURL;
                    }
                }

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