
using System.Text;
using PaymentService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly ILogger<MessageBusSubscriber> _logger;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor, ILogger<MessageBusSubscriber> logger)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _logger = logger;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                _logger.LogInformation("--> [MessageBusSubscriber] [InitializeRabbitMQ]");
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
                        _logger.LogInformation("--> [MessageBusSubscriber] [InitializeRabbitMQ] [CreateConnection] [Success]");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "--> [MessageBusSubscriber] [InitializeRabbitMQ] [CreateConnection] [Exception]");
                        retryCount--;
                        Thread.Sleep(millisecondsTimeout: 2000);
                    }
                }

                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                _logger.LogInformation("--> Listening on the message bus");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "--> [MessageBusSubscriber] [InitializeRabbitMQ] [Exception]");
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
                    _logger.LogInformation("--> Event received");

                    var body = eventArgs.Body;
                    _logger.LogInformation("--> [MessageBusSubscriber] [ExecuteAsync] [body]: {body}", body);

                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                    _logger.LogInformation("--> [MessageBusSubscriber] [ExecuteAsync] [notificationMessage]: {notificationMessage}", notificationMessage);

                    _eventProcessor.ProcessEvent(notificationMessage);
                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                };

                _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "--> [MessageBusSubscriber] [ExecuteAsync] [Exception]: {message}", e.Message);
            }

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("--> [MessageBusSubscriber] [RabbitMQ_ConnectionShutdown]");
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