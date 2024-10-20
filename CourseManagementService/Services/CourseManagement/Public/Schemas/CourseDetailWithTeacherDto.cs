using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class CourseDetailWithTeacherDto : CourseDto
    {
        public TeacherDetail Teacher { get; set; }
    }
}