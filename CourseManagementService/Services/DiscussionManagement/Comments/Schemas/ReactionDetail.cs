using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class ReactionDetail
    {
        public UserDetail User { get; set; }

        public string Type { get; set; }
    }
}