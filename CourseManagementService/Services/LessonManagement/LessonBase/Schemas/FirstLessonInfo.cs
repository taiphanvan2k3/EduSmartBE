using CourseManagementService.Common;

namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class LessonInfoBase
    {
        public Guid Id { get; set; }

        public LookupDto LessonType { get; set; }
    }
}