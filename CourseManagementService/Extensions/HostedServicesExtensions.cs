using System.Threading.Channels;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;

namespace CourseManagementService.Extensions
{
    public static class HostedServicesExtensions
    {
        public static IServiceCollection AddCustomHostedServices(this IServiceCollection services)
        {
            SettingForCommonBackgroundService(services);
            SettingForNotificationBackgroundService(services);
            return services;
        }

        private static void SettingForCommonBackgroundService(IServiceCollection services)
        {
            var commonChannel = Channel.CreateUnbounded<BackgroundJobData>();
            services.AddSingleton(commonChannel);
            services.AddHostedService<CommonBackgroundService>();
            services.AddSingleton<CommonProducer>();
        }

        private static void SettingForNotificationBackgroundService(IServiceCollection services)
        {
            // Sau chỉ cần đổi kiểu dữ liệu của NotificationQueue và NotificationByBatchBackgroundService là có thể sử dụng cho các loại background service khác
            services.AddSingleton<INotificationQueue<NotificationByBatchData>>(new NotificationQueue<NotificationByBatchData>(100));
            services.AddHostedService<NotificationByBatchBackgroundService>();
        }
    }
}