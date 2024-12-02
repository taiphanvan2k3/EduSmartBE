using CourseManagementService.Common;
using CourseManagementService.Services.ChapterManagement.Schemas;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class LessonWithDiscussion
    {
        public LookupDto Lesson { get; set; }

        public SimpleChapterInfo Chapter { get; set; }

        public DiscussionInfo Discussion { get; set; }
    }
}