using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionDetail: DiscussionInfo
    {
        public UserDetail User { get; set; }

        public string Content { get; set; }

        public Guid LessonId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}