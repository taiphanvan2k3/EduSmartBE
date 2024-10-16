using CourseManagementService.GrpcServices;
using Grpc.Net.Client;

namespace CourseManagementService.Services.Grpc
{
    public interface IGrpcUserService
    {
        public Task<bool> CheckIfTeacherExists(int teacherId);
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

        public async Task<bool> CheckIfTeacherExists(int teacherId)
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
    }

}