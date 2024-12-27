using System.Threading.Channels;

namespace CourseManagementService.BackgroundServices
{
    public interface INotificationQueue<T>
    {
        Task EnqueueAsync(T item, CancellationToken cancellationToken = default);
        Task<T> DequeueAsync(CancellationToken cancellationToken = default);
    }

    public class NotificationQueue<T> : INotificationQueue<T>
    {
        private readonly Channel<T> _channel;

        public NotificationQueue(int capacity)
        {
            // Cấu hình Channel với dung lượng và tùy chọn
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait // Chờ nếu hàng đợi đầy
            };
            _channel = Channel.CreateBounded<T>(options);
        }

        public async Task EnqueueAsync(T item, CancellationToken cancellationToken = default)
        {
            await _channel.Writer.WriteAsync(item, cancellationToken);
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}