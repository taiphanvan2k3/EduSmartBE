namespace CourseManagementService.Database.Schemas.Discussion
{
    public class Comment : BaseEntity
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public int VotersCount { get; set; }

        public Guid? ParentId { get; set; }

        public bool IsApproved { get; set; }

        public CommentType Type { get; set; }

        public virtual ICollection<Comment> Replies { get; set; }

        public virtual ICollection<Reaction> Reactions { get; set; }
    }
}