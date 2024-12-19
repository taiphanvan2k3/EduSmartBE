using CourseManagementService.Common;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Services.CourseManagement.Student.Schemas
{
    public class EnrolledCourseInfo : ICourseWithTeacher
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailURL { get; set; }

        public string Description { get; set; }

        public int TotalLessons { get; set; }

        public int TotalStudents { get; set; }

        public int CompletedLessons { get; set; }

        public long TimeSpent { get; set; }

        public LookupDto VisibilityStatus { get; set; }

        public TeacherDetail Teacher { get; set; }
    }
}