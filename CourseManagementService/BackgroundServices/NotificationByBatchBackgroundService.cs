using CourseManagementService.Common;
using CourseManagementService.Services.NotificationManagement;

namespace CourseManagementService.BackgroundServices
{
    public class NotificationByBatchBackgroundService(INotificationQueue<NotificationByBatchData> notificationQueue, IServiceProvider serviceProvider)
        : BackgroundService
    {
        private readonly INotificationQueue<NotificationByBatchData> _notificationQueue = notificationQueue
            ?? throw new ArgumentNullException(nameof(notificationQueue));
        private readonly IServiceProvider _serviceProvider = serviceProvider
            ?? throw new ArgumentNullException(nameof(serviceProvider));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                NotificationByBatchData notificationBatchData = await _notificationQueue.DequeueAsync(stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var notificationDetailService = scope.ServiceProvider.GetRequiredService<INotificationDetailService>();
                await notificationDetailService.NotifyUsersInCourseByBatch(notificationBatchData);
            }
        }
    }
}