using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.CourseManagement.Teacher.Schemas
{
    public class TeacherDetail : UserDetail
    {
        public new string FullName { get; set; }
    }
}