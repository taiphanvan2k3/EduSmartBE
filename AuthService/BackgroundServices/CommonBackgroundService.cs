using System.Threading.Channels;
using AuthService.Commons;
using AuthService.Services.Auth;

namespace AuthService.BackgroundServices
{
    public class CommonProducer(Channel<BackgroundJobData> channel)
    {
        private readonly Channel<BackgroundJobData> _channel = channel;

        public async Task EnqueueDataAsync(BackgroundJobData channel)
        {
            await _channel.Writer.WriteAsync(channel);
        }
    }

    public class CommonBackgroundService(Channel<BackgroundJobData> channel, IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly Channel<BackgroundJobData> _channel = channel;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var data in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                switch (data.JobType)
                {
                    case BackgroundJobType.SAVE_REFRESH_TOKEN:
                        await SaveRefreshToken(scope.ServiceProvider.GetRequiredService<ITokenService>(), data.Data);
                        break;
                    case BackgroundJobType.RENEW_REFRESH_TOKEN:
                        await RenewRefreshToken(scope.ServiceProvider.GetRequiredService<ITokenService>(), data.Data);
                        break;
                    default:
                        throw new ArgumentException("Unsupported job type.");
                }
            }
        }

        private static async Task SaveRefreshToken(ITokenService tokenService, Dictionary<string, dynamic> data)
        {
            var refreshToken = data["refreshToken"] as string;
            var userId = data["userId"] as int? ?? 0;
            var ipAddress = data["ipAddress"] as string;
            await tokenService.SaveRefreshToken(userId, refreshToken, ipAddress);
        }

        private static async Task RenewRefreshToken(ITokenService tokenService, Dictionary<string, dynamic> data)
        {
            var oldRefreshToken = data["oldRefreshToken"] as string;
            var newRefreshToken = data["newRefreshToken"] as string;
            var userId = data["userId"] as int? ?? 0;
            var ipAddress = data["ipAddress"] as string;
            await tokenService.RenewRefreshToken(userId, oldRefreshToken, newRefreshToken, ipAddress);
        }
    }
}