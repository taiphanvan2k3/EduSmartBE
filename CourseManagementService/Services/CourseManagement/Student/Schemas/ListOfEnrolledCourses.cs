using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.CourseManagement.Student.Schemas
{
    public class ListOfEnrolledCourses
    {
        public UserDetail UserInfo { get; set; }

        public List<EnrolledCourseInfo> Courses { get; set; }
    }
}