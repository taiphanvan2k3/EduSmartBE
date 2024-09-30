using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Commons;
using UserService.Services.Users.Schemas;
using TblUser = UserService.Databases.Schemas.User;
using TblUserInfo = UserService.Databases.Schemas.UserInfo;
using TblUserRole = UserService.Databases.Schemas.UserRole;

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
        /// Update active status of user
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 29/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateActiveStatus(int userId, bool isActive);

        /// <summary>
        /// Update last login of user
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 29/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateLastLogin(int userId, DateTimeOffset lastLogin);

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
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[UserService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var isExist = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (isExist)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "User is already exist";

                    _logger.LogInformation("[UserService] [{Method}] End", methodName);
                    return response;
                }

                var userEntity = _mapper.Map<TblUser>(user);
                userEntity.UserInfo = new TblUserInfo()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AvatarURL = user.AvatarUrl,
                    Phone = null
                };

                userEntity.UserRoles = await _context.Roles.Where(r => user.Roles.Contains(r.Name))
                    .Select(r => new TblUserRole()
                    {
                        RoleId = r.Id
                    })
                    .ToListAsync();

                await _context.Users.AddAsync(userEntity);
                await _context.SaveChangesAsync();

                response.Message = "Add user successfully";
                _logger.LogInformation("[UserService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[UserService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateActiveStatus(int userId, bool isActive)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[UserService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var userFromDB = await _context.Users.FindAsync(userId);
                if (userFromDB == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "User not found";

                    _logger.LogInformation("[UserService] [{Method}] End", methodName);
                    return response;
                }

                userFromDB.IsActive = isActive;
                await _context.SaveChangesAsync();

                response.Message = "Update active status successfully";
                _logger.LogInformation("[UserService] [{Method}] End", methodName);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[UserService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateLastLogin(int userId, DateTimeOffset lastLogin)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[UserService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var userFromDB = await _context.Users.FindAsync(userId);
                if (userFromDB == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "User not found";

                    _logger.LogInformation("[UserService] [{Method}] End", methodName);
                    return response;
                }

                userFromDB.LastLogin = lastLogin;
                await _context.SaveChangesAsync();

                response.Message = "Update last login successfully";
                _logger.LogInformation("[UserService] [{Method}] End", methodName);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[UserService] [{Method}] Error", methodName);
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