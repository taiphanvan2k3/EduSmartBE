using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using UserService.Commons;
using UserService.GrpcServices;
using UserService.Services.Users.Schemas;

namespace UserService.Services.Grpc
{
    public interface IGrpcCourseService
    {
        public Task<ResponseInfo> GetMonthlyDiscussions();
    }

    public class GrpcCourseService : BaseService, IGrpcCourseService
    {
        private readonly string _serviceName = nameof(GrpcCourseService);
        private readonly GrpcChannel _channel;

        public GrpcCourseService(IServiceProvider serviceProvider, ILogger<GrpcCourseService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:CourseService:GrpcUrl"]);
        }

        public async Task<ResponseInfo> GetMonthlyDiscussions()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var responseInfo = new ResponseInfo();
                var client = new Course.CourseClient(_channel);
                var res = await client.GetMonthlyDiscussionsAsync(new Empty());

                if (res.IsSuccess)
                {
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                    responseInfo.Message = res.Message;
                    responseInfo.Data.Add("listOfMonthlyDiscussions", res.MonthlyDataCourse.Select(x => new MonthlyData
                    {
                        Month = x.Month,
                        Amount = x.Amount
                    }).ToList());
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = res.Message;
                }

                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, methodName);
                throw;
            }
        }
    }
}