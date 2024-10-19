using Grpc.Net.Client;
using UserService.GrpcServices;
using UserService.Services.Users.Schemas;

namespace UserService.Services.Grpc
{
    public interface IGrpcAuthService
    {
        public Task<bool> SaveUserProfile(UserInfo userInfo);
    }

    public class GrpcAuthService : BaseService, IGrpcAuthService
    {
        private readonly string _serviceName = nameof(GrpcAuthService);
        private readonly GrpcChannel _channel;

        public GrpcAuthService(IServiceProvider serviceProvider, ILogger<GrpcAuthService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:AuthService:GrpcUrl"]);
        }

        public async Task<bool> SaveUserProfile(UserInfo userInfo)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var client = new Auth.AuthClient(_channel);
                var res = await client.SaveUserProfileAsync(new SaveUserProfileRequest
                {
                    Id = userInfo.UserId,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    AvatarURL = userInfo.AvatarURL,
                    Phone = userInfo.Phone,
                    Gender = userInfo.Gender
                });
                return res.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }
    }
}