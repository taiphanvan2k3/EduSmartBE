using System.Text.Json;
using AutoMapper;
using UserService.Databases.Schemas;
using UserService.Dtos;
using UserService.Services.Users;
using UserService.Services.Users.Schemas;

namespace UserService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }

    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.UserPublished:
                    AddUser(message);
                    break;
                default:
                    break;
            }
        }

        private static EventType DetermineEvent(string notifcationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Cho phép không phân biệt chữ hoa và chữ thường
            };

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifcationMessage, options);

            switch (eventType.Type)
            {
                case "UserCreatedEvent":
                    Console.WriteLine("--> User Published Event Detected");
                    return EventType.UserPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private async void AddUser(string userPublishedMessage)
        {
            try
            {
                var userPublishedDto = JsonSerializer.Deserialize<UserPublishedDto>(
                    JsonDocument.Parse(userPublishedMessage).RootElement.GetProperty("data").ToString());

                // Chuyển đổi chuỗi CreateAt thành DateTime
                DateTime createAt;
                if (DateTime.TryParseExact(userPublishedDto.CreateAt, 
                                        "yyyy-MM-dd HH:mm:ss", 
                                        System.Globalization.CultureInfo.InvariantCulture, 
                                        System.Globalization.DateTimeStyles.None, 
                                        out createAt))
                {
                    var user = _mapper.Map<UserDto>(userPublishedDto);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await _userService.AddUser(user);
                    }
                }
                else
                {
                    Console.WriteLine("--> Invalid DateTime format in CreateAt");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not add user to database: {e.Message}");
            }
        }
    }
    enum EventType
    {
        UserPublished,
        Undetermined
    }
}