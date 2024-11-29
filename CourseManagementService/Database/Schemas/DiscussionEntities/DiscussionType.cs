namespace CourseManagementService.Database.Schemas.DiscussionEntities
{
    public class DiscussionType : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Discussion> Discussions { get; set; }
    }
}