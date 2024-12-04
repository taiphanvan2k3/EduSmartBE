using CourseManagementService.Common.Schemas;

namespace CourseManagementService.Services.DiscussionManagement.Comments.Schemas
{
    public class CommentDetail
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public UserDetail CreatedBy { get; set; }

        public Guid DiscussionId { get; set; }

        public Guid? ParentId { get; set; }

        public UserDetail MentionedUser { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsApproved { get; set; }

        public bool IsDelFlag { get; set; }

        public int ReplyCount { get; set; }

        public ReactionsInfo Reactions { get; set; }

        public bool HasReacted { get; set; }

        public string ReactionType { get; set; }
    }

    public class ReactionsInfo
    {
        public int Count { get; set; }

        public List<string> Types { get; set; }
    }
}