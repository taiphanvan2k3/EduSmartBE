using AutoMapper;
using UserService.Commons;
using UserService.Databases.Schemas;
using UserService.Services.Users.Schemas;

namespace UserService.Services.Users
{
    public interface IUserService
    {
        /// <summary>
        /// Add user to database
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 18/09/2024</para>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<ResponseInfo> AddUser(UserDto user);

        /// <summary>
        /// Update user to database
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 18/09/2024</para>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateUser(UserDto user);

        /// <summary>
        /// Delete user from database
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 18/09/2024</para>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteUser(UserDto user);
    }

    public class UserService(IServiceProvider serviceProvider, 
        ILogger<UserService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IUserService
    {

        private readonly IMapper _mapper = mapper
            ?? throw new ArgumentNullException(nameof(mapper));
        public async Task<ResponseInfo> AddUser(UserDto user)
        {
            try
            {
                _logger.LogInformation("[UserService] [AddUser] Start");
                var response = new ResponseInfo();
                var userEntity = new User()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    LastLogin = user.LastLogin.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(user.LastLogin, DateTimeKind.Utc)
                        : user.LastLogin.ToUniversalTime(),
                    LastLogout = user.LastLogout.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(user.LastLogout, DateTimeKind.Utc)
                        : user.LastLogout.ToUniversalTime(),
                    IsOnline = user.IsOnline,
                    IsActive = user.IsActive
                };
                await _context.Users.AddAsync(userEntity);
                await _context.SaveChangesAsync();
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Add user successfully";
                _logger.LogInformation("[UserService] [AddUser] End");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AddUser");
                throw;
            }
        }

        public Task<ResponseInfo> DeleteUser(UserDto user)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseInfo> UpdateUser(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}