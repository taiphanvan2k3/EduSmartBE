using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Services.CourseManagement.Public.Schemas
{
    public class CourseSearchItem : ICourseWithTeacher
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailURL { get; set; }

        public TeacherDetail Teacher { get; set; }
    }
}