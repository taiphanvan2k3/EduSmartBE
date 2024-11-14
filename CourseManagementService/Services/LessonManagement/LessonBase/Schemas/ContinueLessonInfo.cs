using CourseManagementService.Enumerations;

namespace CourseManagementService.Services.LessonManagement.LessonBase.Schemas
{
    public class ContinueLessonInfo
    {
        public Guid LessonId { get; set; }

        public LessonType LessonType { get; set; }

        public long TimeSpent { get; set; }
    }
}