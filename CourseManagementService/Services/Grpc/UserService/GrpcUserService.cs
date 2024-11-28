using CourseManagementService.GrpcServices;
using Grpc.Net.Client;

namespace CourseManagementService.Services.Grpc.UserService
{
    public interface IGrpcUserService
    {
        public Task<bool> CheckUserExist(int teacherId);
        public Task<ListOfUsersResponse> GetListOfUsers(List<int> userIds);
        public Task<ListOfUsersResponse> GetListOfTeachers(List<int> teacherIds);
        public Task<ListOfUsersResponse> GetListOfStudents(List<int> studentIds);
        public Task<ListOfUsersResponse> GetTeachersByName(string name);
    }

    public class GrpcUserService : BaseService, IGrpcUserService
    {
        private readonly string _serviceName = nameof(GrpcUserService);
        private readonly GrpcChannel _channel;

        public GrpcUserService(IServiceProvider serviceProvider, ILogger<GrpcUserService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:UserService:GrpcUrl"]);
        }

        public async Task<bool> CheckUserExist(int teacherId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new User.UserClient(_channel);
                return (await client.CheckUserExistAsync(new UserRequest { Id = teacherId })).IsExist;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }

        public async Task<ListOfUsersResponse> GetListOfUsers(List<int> userIds)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new User.UserClient(_channel);

                var response = await client.GetListOfUsersAsync(new UsersRequest { Ids = { userIds } });

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, methodName);
                return response;
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