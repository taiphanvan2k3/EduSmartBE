using System.Threading.Channels;
using AuthService.BackgroundServices;
using AuthService.Services.MailSender.Schemas;

namespace AuthService.Extensions
{
    public static class HostedServicesExtensions
    {
        public static IServiceCollection AddCustomHostedServices(this IServiceCollection services)
        {
            SettingForEmailBackgroundService(services);
            return services;
        }

        private static void SettingForEmailBackgroundService(IServiceCollection services)
        {
            var mailChannel = Channel.CreateUnbounded<MailBodyBase>();
            services.AddSingleton(mailChannel);
            services.AddHostedService<EmailBackgroundService>();
            services.AddSingleton<MailProducer>();
        }
    }
}