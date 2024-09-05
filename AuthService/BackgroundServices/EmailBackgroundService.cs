using System.Threading.Channels;
using AuthService.Services.MailSender;
using AuthService.Services.MailSender.Schemas;

namespace AuthService.BackgroundServices
{
    /// <summary>
    /// Producer này sẽ làm trung gian giữa các service và background service
    /// Khi có mail cần gửi, các service khác sẽ gọi EnqueueMailAsync để gửi một dữ liệu vào channel
    /// và background service sẽ lắng nghe dữ liệu từ channel đó và đi xử lý công việc của nó tại ExecuteAsync
    /// </summary>
    /// <param name="mailChannel"></param>
    public class MailProducer(Channel<MailBodyBase> mailChannel)
    {
        private readonly Channel<MailBodyBase> _mailChannel = mailChannel;

        public async Task EnqueueMailAsync(MailBodyBase mailBody)
        {
            await _mailChannel.Writer.WriteAsync(mailBody);
        }
    }

    public class EmailBackgroundService(Channel<MailBodyBase> mailChannel, IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly Channel<MailBodyBase> _mailChannel = mailChannel;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Sẽ có 1 thread chạy vào đây và lắng nghe dữ liệu từ channel gủi đến
            // Mỗi khi có dữ liệu mới được đưa vào channel, nó sẽ lấy ra và đi xử lý việc gửi mail
            // Khi gửi xong mail, sẽ chờ đến khi có dữ liệu mới trong channel
            await foreach (var mailBody in _mailChannel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var mailService = scope.ServiceProvider.GetRequiredService<ISendMailService>();
                switch (mailBody)
                {
                    case ConfirmMailBody confirmMailBody:
                        await mailService.SendMailConfirmAccount(confirmMailBody);
                        break;
                    default:
                        throw new ArgumentException("Unsupported mail body type.");
                }
            }
        }
    }
}