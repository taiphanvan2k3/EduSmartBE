using CourseManagementService.Common;

namespace CourseManagementService.Services.BookmarkManagement.Schemas
{
    public class Bookmark
    {
        public Guid LessonId { get; set; }

        public string LessonName { get; set; }

        public LookupDto LessonType { get; set; }
    }
}