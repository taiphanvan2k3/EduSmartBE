using CourseManagementService.Database.Schemas.NotificationEntities;

namespace CourseManagementService.Services.NotificationManagement.Schemas
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public SenderInfo SenderInfo { get; set; }

        public string Type { get; set; }

        public Guid RelatedEntityId { get; set; }

        public string RelatedEntityType { get; set; }

        public Guid CourseId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public dynamic MetaData { get; set; }
    }
}