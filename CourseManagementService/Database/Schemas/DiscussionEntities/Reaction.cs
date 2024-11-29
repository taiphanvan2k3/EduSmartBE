namespace CourseManagementService.Database.Schemas.DiscussionEntities
{
    public class Reaction
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public string Type { get; set; }

        public Guid CommentId { get; set; }

        public Comment Comment { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}