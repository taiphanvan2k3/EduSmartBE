using AutoMapper;
using Newtonsoft.Json;
using PaymentService.EventData;

namespace PaymentService.EventProcessing
{
    public interface IEventProcessor
    {
        /// <summary>
        /// Process the event of RabbitMQ
        /// <para>Author1: ManhTD</para>
        /// <para>Author2: TaiPV</para>
        /// <para>Created: 19/09/2024</para>
        /// <para>Updated: 29/09/2024</para>
        /// </summary>
        /// <returns></returns>
        Task ProcessEvent(string message);
    }

    public partial class EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper,
        ILogger<EventProcessor> logger) : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory
            ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<EventProcessor> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        public async Task ProcessEvent(string payload)
        {
            try
            {
                // Ở đây đôi lúc sẽ nhận nhiều data của các event khác nhau, nên cần phải xác định event type để xử lý
                var decodePayload = JsonConvert.DeserializeObject<MessagePayload<dynamic>>(payload);
                switch (decodePayload.Type)
                {
                    case EventType.StorageInfoCreated:
                        var storageInfo = JsonConvert.DeserializeObject<StorageInfoCreatedEventData>(decodePayload.Data.ToString());
                        await CreateStorageInfo(storageInfo);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing event");
                throw;
            }
        }
    }
}