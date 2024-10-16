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
                        Avatar = u.UserInfo.AvatarURL
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
    }
}