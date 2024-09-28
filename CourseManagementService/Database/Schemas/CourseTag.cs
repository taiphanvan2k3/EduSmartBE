namespace CourseManagementService.Database.Schemas
{
    public class CourseTag
    {
        public int Id { get; set; }

        public Guid CourseId { get; set; }

        public int TagId { get; set; }

        public virtual Course Course { get; set; }

        public virtual Tag Tag { get; set; }
    }
}