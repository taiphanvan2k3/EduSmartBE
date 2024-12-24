namespace CourseManagementService.Database.Schemas.NotificationEntities
{
    public class NotificationBellClickLog : BaseEntity
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public DateTimeOffset LastClickedAt { get; set; }
    }
}