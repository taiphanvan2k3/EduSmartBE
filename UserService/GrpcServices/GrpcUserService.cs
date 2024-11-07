using System.Runtime.CompilerServices;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserService.Commons;
using UserService.Databases;

namespace UserService.GrpcServices
{
    // User cũng nằm trong thư mục UserService.GrpcServices nên ta không cần import lại namespace
    public class GrpcUserService(DataContext context, ILogger<GrpcUserService> logger) : User.UserBase
    {
        private readonly DataContext _context = context
            ?? throw new ArgumentNullException(nameof(context));

        private readonly ILogger<GrpcUserService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public override async Task<CheckUserExistResponse> CheckUserExist(UserRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcUserService] [{Method}] Start", methodName);

                var user = await _context.Users.FindAsync(request.Id);

                _logger.LogInformation("[GrpcUserService] [{Method}] End", methodName);
                return new CheckUserExistResponse
                {
                    IsExist = user != null
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcUserService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<UserResponse> GetUserInfo(UserRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcUserService] [{Method}] Start", methodName);

                var user = await _context.Users
                    .Include(u => u.UserInfo)
                    .FirstOrDefaultAsync(u => u.Id == request.Id)
                    ?? throw new RpcException(new Status(StatusCode.NotFound, "User not found"));


                _logger.LogInformation("[GrpcUserService] [{Method}] End", methodName);
                return new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.UserInfo.FirstName,
                    LastName = user.UserInfo.LastName,
                    Email = user.Email,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcUserService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<ListOfUsersResponse> GetListOfTeachers(UsersRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcUserService] [{Method}] Start", methodName);

                var users = await _context.Users
                    .Where(u => request.Ids.Contains(u.Id) &&
                        u.UserRoles.Any(role => role.Role.Name == Constants.TEACHER_ROLE))
                    .Select(u => new SimpleUserResponse()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        FullName = $"{u.UserInfo.FirstName} {u.UserInfo.LastName}",
                        AvatarURL = u.UserInfo.AvatarURL,
                        Email = u.Email
                    })
                    .ToListAsync();

                var listOfUsers = new ListOfUsersResponse();
                listOfUsers.Users.AddRange(users);

                _logger.LogInformation("[GrpcUserService] [{Method}] End", methodName);
                return listOfUsers;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcUserService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<ListOfUsersResponse> GetTeachersByName(UserRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                const int MAX_TEACHERS = 5;

                // TODO: Optimize by using Redis Cache
                _logger.LogInformation("[GrpcUserService] [{Method}] Start", methodName);
                var users = await _context.Users
                    .Where(u => u.UserRoles.Any(role => role.Role.Name == Constants.TEACHER_ROLE))
                    .Where(u => EF.Functions.ILike(u.UserInfo.FirstName + " " + u.UserInfo.LastName, $"%{request.Name}%"))
                    .OrderByDescending(u => u.UserInfo.LastName)
                    .Select(u => new SimpleUserResponse()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        FullName = $"{u.UserInfo.FirstName} {u.UserInfo.LastName}",
                        AvatarURL = u.UserInfo.AvatarURL,
                        Email = u.Email
                    })
                    .Take(MAX_TEACHERS)
                    .ToListAsync();

                var listOfUsers = new ListOfUsersResponse();
                listOfUsers.Users.AddRange(users);

                _logger.LogInformation("[GrpcUserService] [{Method}] End", methodName);
                return listOfUsers;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcUserService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<MessageResponse> DeleteUser(UserRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcUserService] [{Method}] Start", methodName);

                var user = await _context.Users.FindAsync(request.Id)
                    ?? throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[GrpcUserService] [{Method}] End", methodName);
                return new MessageResponse
                {
                    Message = "User deleted successfully",
                    StatusCode = 200
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcUserService] [{Method}] Error", methodName);
                return new MessageResponse
                {
                    Message = $"Error when deleting user. Details: {e.InnerException?.Message ?? e.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}