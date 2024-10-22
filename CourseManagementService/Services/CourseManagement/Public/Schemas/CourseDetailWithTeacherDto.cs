using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class CourseDetailWithTeacherDto : CourseDto, ICourseWithTeacher
    {
        public TeacherDetail Teacher { get; set; }
    }
}