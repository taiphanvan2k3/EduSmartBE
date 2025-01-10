using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AuthService.AsyncDataServices
{
    public interface IMessagePublisher
    {
        void PublishMessage<T>(string eventType, T message);
    }

    public class MessageBusProvider : IMessagePublisher
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        public MessageBusProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                Console.WriteLine("--> [MessageBusProvider] [InitializeRabbitMQ]");
                Console.WriteLine($"--> Host: {_configuration["RabbitMQ:Host"]} and Port: {_configuration["RabbitMQ:Port"]}");
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"],
                    Port = int.Parse(_configuration["RabbitMQ:Port"]),
                    UserName = _configuration["RabbitMQ:Username"],
                    Password = _configuration["RabbitMQ:Password"]
                };


                var connected = false;
                var retryCount = 5;

                while (!connected && retryCount > 0)
                {
                    try
                    {
                        _connection = factory.CreateConnection();
                        connected = true;
                        Console.WriteLine("--> [MessageBusSubscriber] [InitializeRabbitMQ] [CreateConnection] [Success]");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"--> [MessageBusSubscriber] [InitializeRabbitMQ] [RetryCount]: {retryCount} and [Exception]: {e.Message}");
                        retryCount--;
                        Thread.Sleep(millisecondsTimeout: 500);
                    }
                }

                _channel = _connection.CreateModel();

                // Khai báo cách gửi message, sẽ sử dụng Topic để tránh bị message đi đến sai chỗ
                _channel.ExchangeDeclare("main_exchange", ExchangeType.Topic);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the MessageBus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        public void PublishMessage<T>(string eventType, T message)
        {
            string payload = JsonSerializer.Serialize(new
            {
                type = eventType,
                data = message
            });

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(eventType, payload);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending");
            }
        }

        private void SendMessage(string routingKey, string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            // Dùng thêm exchange, routingKey để đến đúng chỗ subscriber
            _channel.BasicPublish(exchange: "main_exchange",
                routingKey: routingKey,
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> We have sent {message}");
        }
    }
}
