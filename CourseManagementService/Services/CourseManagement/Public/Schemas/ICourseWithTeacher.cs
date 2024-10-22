using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public interface ICourseWithTeacher
    {
        public TeacherDetail Teacher { get; set; }
    }
}