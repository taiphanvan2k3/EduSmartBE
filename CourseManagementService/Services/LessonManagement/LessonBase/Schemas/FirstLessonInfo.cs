using CourseManagementService.Common;

namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class FirstLessonInfo
    {
        public Guid Id { get; set; }

        public LookupDto LessonType { get; set; }
    }
}