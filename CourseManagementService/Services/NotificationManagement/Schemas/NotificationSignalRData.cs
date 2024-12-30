using CourseManagementService.Database.Schemas.NotificationEntities;

namespace CourseManagementService.Services.NotificationManagement.Schemas
{
    public class NotificationSignalRData
    {
        public SenderInfo SenderInfo { get; set; }

        public string NotificationType { get; set; }

        public string RelatedEntityId { get; set; }

        public string RelatedEntityType { get; set; }

        public Guid CourseId { get; set; }

        public string MetaData { get; set; }
    }
}