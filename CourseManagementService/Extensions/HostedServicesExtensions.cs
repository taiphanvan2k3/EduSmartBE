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
            return services;
        }

        private static void SettingForCommonBackgroundService(IServiceCollection services)
        {
            var commonChannel = Channel.CreateUnbounded<BackgroundJobData>();
            services.AddSingleton(commonChannel);
            services.AddHostedService<MediaBackgroundService>();
            services.AddSingleton<MediaProducer>();
        }
    }
}