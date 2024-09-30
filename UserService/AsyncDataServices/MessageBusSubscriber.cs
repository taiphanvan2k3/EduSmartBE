
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserService.EventProcessing;

namespace UserService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                Console.WriteLine("--> [MessageBusSubscriber] [InitializeRabbitMQ]");
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
                        Thread.Sleep(millisecondsTimeout: 2000);
                    }
                }

                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine("--> Listening on the message bus");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> [MessageBusSubscriber] [InitializeRabbitMQ] [Exception]: {e.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ModuleHandle, eventArgs) =>
                {
                    Console.WriteLine("--> Event received");

                    var body = eventArgs.Body;
                    Console.WriteLine("[UserService] [MessageBusSubscriber] [ExecuteAsync] [body]: " + body);
                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine("[UserService] [MessageBusSubscriber] [ExecuteAsync] [notificationMessage]: " + notificationMessage);

                    _eventProcessor.ProcessEvent(notificationMessage);
                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                };

                _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> [MessageBusSubscriber] [ExecuteAsync] [Exception]: {e.Message}");
            }

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection shutdown");
        }

        public override void Dispose()
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
            }

            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
            }

            // Thêm GC.SuppressFinalize để ngăn Finalizer chạy sau khi đã Dispose
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}