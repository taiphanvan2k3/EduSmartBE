using AuthService.Commons;
using AuthService.GrpcServices;
using Grpc.Net.Client;

namespace AuthService.Services.Grpc
{
    public interface IGrpcUserService
    {
        public Task<ResponseInfo> DeleteAccount(int userId);
    }

    public class GrpcUserService : BaseService, IGrpcUserService
    {
        private readonly GrpcChannel _channel;

        public GrpcUserService(IServiceProvider serviceProvider, ILogger<GrpcUserService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:UserService:GrpcUrl"]);
        }

        public async Task<ResponseInfo> DeleteAccount(int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var client = new User.UserClient(_channel);
                MessageResponse response = await client.DeleteUserAsync(new UserRequest { Id = userId });

                responseInfo.Message = response.Message;
                responseInfo.StatusCode = response.StatusCode;

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}