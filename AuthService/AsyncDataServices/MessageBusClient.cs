using System.Text;
using System.Text.Json;
using AuthService.Dtos;
using RabbitMQ.Client;

namespace AuthService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishUserCreated(UserCreatedDto userCreatedDto);
        void PublishUserUpdated(UserPublishedDto userUpdatedDto);
        void PublishUserDeleted(UserReadDto userDeletedDto);
    }

    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
                
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

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger",
                            routingKey: "",
                            basicProperties: null,
                            body: body);
            Console.WriteLine($"--> We have sent {message}");
        }

        public void PublishUserCreated(UserCreatedDto userCreatedDto)
        {
            var message = JsonSerializer.Serialize(new
            {
                type = "UserCreatedEvent",
                data = userCreatedDto
            });

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending");
            }  
        }

        public void PublishUserUpdated(UserPublishedDto userUpdatedDto)
        {
            var message = JsonSerializer.Serialize(new
            {
                type = "UserUpdatedEvent",
                data = userUpdatedDto
            });

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending");
            }
        }

        public void PublishUserDeleted(UserReadDto userDeletedDto)
        {
            var message = JsonSerializer.Serialize(new
            {
                type = "UserDeletedEvent",
                data = userDeletedDto
            });

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending");
            }
        }
    }
}