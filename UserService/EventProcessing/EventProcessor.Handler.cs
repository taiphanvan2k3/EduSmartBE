using UserService.EventData;
using UserService.Services.Users;
using UserService.Services.Users.Schemas;

namespace UserService.EventProcessing
{
    public partial class EventProcessor : IEventProcessor
    {
        private async Task AddUser(UserCreatedEventData userPublished)
        {
            try
            {
                var user = _mapper.Map<UserDto>(userPublished);
                using var scope = _serviceScopeFactory.CreateScope();
                var _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await _userService.AddUser(user);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not add user to database: {e.Message}");
            }
        }

        private async Task UpdateActiveStatus(ActiveStatusEventData userActivated)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await _userService.UpdateActiveStatus(userActivated.UserId, userActivated.IsActive);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not update active status of user: {e.Message}");
            }
        }

        private async Task UpdateLastLogin(UserLastLoginUpdatedEventData userLastLoginUpdated)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await _userService.UpdateLastLogin(userLastLoginUpdated.UserId, userLastLoginUpdated.LastLogin);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not update last login of user: {e.Message}");
            }
        }
    }
}