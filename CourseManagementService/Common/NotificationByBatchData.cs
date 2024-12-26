using CourseManagementService.Services.NotificationManagement.Schemas;

namespace CourseManagementService.Common
{
    public class NotificationByBatchData
    {
        public string BatchId { get; set; }

        public List<int> UserIdsInBatch { get; set; }

        public NotificationCreateDto NotificationCreateDto { get; set; }
    }
}