using CourseManagementService.Database.Schemas.NotificationEntities;
using CourseManagementService.Enumerations;

namespace CourseManagementService.Services.NotificationManagement.Schemas
{
    public class NotificationCreateDto
    {
        public int ReceiverId { get; set; }

        public SenderInfo SenderInfo { get; set; }

        public NotificationType Type { get; set; }

        public string RelatedEntityId { get; set; }

        public RelatedEntityType RelatedEntityType { get; set; }

        public Guid CourseId { get; set; }

        public Dictionary<string, string> MetaData { get; set; }
    }
}