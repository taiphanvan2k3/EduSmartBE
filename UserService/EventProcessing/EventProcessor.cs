using AutoMapper;
using Newtonsoft.Json;
using UserService.EventData;
using UserService.Services.Users;
using UserService.Services.Users.Schemas;

namespace UserService.EventProcessing
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

    public partial class EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper) : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory
            ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));

        public async Task ProcessEvent(string payload)
        {
            try
            {
                var decodePayload = JsonConvert.DeserializeObject<MessagePayload<dynamic>>(payload);
                switch (decodePayload.Type)
                {
                    case EventType.UserCreated:
                        var userPublished = JsonConvert.DeserializeObject<UserCreatedEventData>(decodePayload.Data.ToString());
                        await AddUser(userPublished);
                        break;
                    case EventType.UserActivated:
                        var userActivated = JsonConvert.DeserializeObject<UserActivatedEventData>(decodePayload.Data.ToString());
                        await UpdateActiveStatus(userActivated);
                        break;
                    case EventType.UserLastLoginUpdated:
                        var userLastLoginUpdated = JsonConvert.DeserializeObject<UserLastLoginUpdatedEventData>(decodePayload.Data.ToString());
                        await UpdateLastLogin(userLastLoginUpdated);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not process event: {e.Message}");
                throw;
            }
        }
    }
}