using CourseManagementService.Common;

namespace CourseManagementService.Services.DiscussionManagement.Discussions.Schemas
{
    public class DiscussionInfo
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public bool IsAnswered { get; set; }

        public LookupDto Type { get; set; }
    }
}