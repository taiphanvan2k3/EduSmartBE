using CourseManagementService.Common.Schemas;
using CourseManagementService.GrpcServices;
using CourseManagementService.Services.Cache;
using Grpc.Net.Client;

namespace CourseManagementService.Services.Grpc.UserService
{
    public interface IGrpcUserService
    {
        public Task<bool> CheckUserExist(int teacherId);
        public Task<UserDetail> GetUserInfo(int userId);
        public Task<UserDetail> GetUserInfoWithRole(int userId);
        public Task<List<UserDetail>> GetListOfUsers(List<int> userIds);
        public Task<ListOfUsersResponse> GetListOfTeachers(List<int> teacherIds);
        public Task<ListOfUsersResponse> GetListOfStudents(List<int> studentIds);
        public Task<ListOfUsersResponse> GetTeachersByName(string name);
    }

    public class GrpcUserService : BaseService, IGrpcUserService
    {
        private readonly string _serviceName = nameof(GrpcUserService);
        private readonly GrpcChannel _channel;
        private readonly ICacheService _cacheService;

        public GrpcUserService(IServiceProvider serviceProvider, ILogger<GrpcUserService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:UserService:GrpcUrl"]);
            _cacheService = serviceProvider.GetService<ICacheService>()
                ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));
        }

        public async Task<bool> CheckUserExist(int teacherId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var client = new User.UserClient(_channel);

                var isExist = (await client.CheckUserExistAsync(new UserRequest { Id = teacherId })).IsExist;

                LogInfo("End", methodName);
                return isExist;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<UserDetail> GetUserInfo(int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var cachedUser = _cacheService.GetData<UserDetail>(CacheManager.User.Key(userId));
                if (cachedUser != null)
                {
                    return cachedUser;
                }

                var client = new User.UserClient(_channel);

                var response = await client.GetListOfUsersAsync(new UsersRequest { Ids = { new List<int> { userId } } });
                if (response.Users.Count > 0)
                {
                    var user = response.Users[0];
                    var userDetail = new UserDetail
                    {
                        Id = user.Id,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Email = user.Email,
                        AvatarURL = user.AvatarURL
                    };
                    _cacheService.SetData(CacheManager.User.Key(user.Id), userDetail, DateTimeOffset.Now.AddMinutes(CacheManager.User.ExpireTimeInMinutes));

                    return userDetail;
                }

                LogInfo("End", methodName);
                return null;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }

        public async Task<UserDetail> GetUserInfoWithRole(int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var cachedUser = _cacheService.GetData<UserDetail>(CacheManager.User.Key(userId, withRole: true));
                if (cachedUser != null)
                {
                    return cachedUser;
                }

                var client = new User.UserClient(_channel);

                var user = await client.GetUserInfoWithRoleAsync(new UserRequest { Id = userId });
                if (user != null)
                {
                    var userDetail = new UserDetail
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Email = user.Email,
                        AvatarURL = user.AvatarURL,
                        Roles = user.Roles
                    };
                    _cacheService.SetData(CacheManager.User.Key(user.Id, withRole: true), userDetail, DateTimeOffset.Now.AddMinutes(CacheManager.User.ExpireTimeInMinutes));

                    return userDetail;
                }

                LogInfo("End", methodName);
                return null;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }

        public async Task<List<UserDetail>> GetListOfUsers(List<int> userIds)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new User.UserClient(_channel);

                List<UserDetail> users = [];
                List<int> notCachedUserIds = [];

                userIds = userIds.Distinct().ToList();
                var cacheKeys = userIds.Select(userId => CacheManager.User.Key(userId, withRole: true)).ToList();
                var cachedUsers = await _cacheService.GetMultipleDataAsync<UserDetail>(cacheKeys);

                for (int i = 0; i < userIds.Count; i++)
                {
                    if (cachedUsers[i] != null)
                    {
                        users.Add(cachedUsers[i]);
                    }
                    else
                    {
                        notCachedUserIds.Add(userIds[i]);
                    }
                }

                var response = await client.GetListOfUsersAsync(new UsersRequest { Ids = { notCachedUserIds } });
                if (response.Users.Count > 0)
                {
                    foreach (var user in response.Users)
                    {
                        var userDetail = new UserDetail
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            FullName = $"{user.FirstName} {user.LastName}",
                            Email = user.Email,
                            AvatarURL = user.AvatarURL,
                            Roles = user.Roles
                        };

                        users.Add(userDetail);
                        _cacheService.SetData(CacheManager.User.Key(user.Id, withRole: true), userDetail,
                            DateTimeOffset.Now.AddMinutes(CacheManager.User.ExpireTimeInMinutes));
                    }
                }

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, methodName);
                return users;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<ListOfUsersResponse> GetListOfTeachers(List<int> teacherIds)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new User.UserClient(_channel);

                var response = await client.GetListOfTeachersAsync(new UsersRequest { Ids = { teacherIds } });

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<ListOfUsersResponse> GetListOfStudents(List<int> studentIds)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new User.UserClient(_channel);
                var response = await client.GetListOfStudentsAsync(new UsersRequest { Ids = { studentIds } });
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<ListOfUsersResponse> GetTeachersByName(string name)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new User.UserClient(_channel);
                var response = await client.GetTeachersByNameAsync(new UserRequest { Name = name });
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }
    }
}