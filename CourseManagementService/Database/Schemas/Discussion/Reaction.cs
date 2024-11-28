namespace CourseManagementService.Database.Schemas.Discussion
{
    public class Reaction
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public string Type { get; set; }
    }
}