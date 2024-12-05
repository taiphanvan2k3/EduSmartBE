using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using UserService.Commons;
using UserService.GrpcServices;
using UserService.Services.Users.Schemas;

namespace UserService.Services.Grpc
{
    public interface IGrpcPaymentService
    {
        public Task<ResponseInfo> GetMonthlyPaymentInfo();
    }

    public class GrpcPaymentService : BaseService, IGrpcPaymentService
    {
        private readonly string _serviceName = nameof(GrpcPaymentService);
        private readonly GrpcChannel _channel;

        public GrpcPaymentService(IServiceProvider serviceProvider, ILogger<GrpcPaymentService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:PaymentService:GrpcUrl"]);
        }

        public async Task<ResponseInfo> GetMonthlyPaymentInfo()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, methodName);
                var responseInfo = new ResponseInfo();
                var client = new Payment.PaymentClient(_channel);
                var res = await client.GetMonthlyPaymentInfoAsync(new Empty());

                if (res.IsSuccess)
                {
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                    responseInfo.Message = res.Message;
                    responseInfo.Data.Add("monthlyDrawingRequests", res.MonthlyDrawingRequests.Select(x => new MonthlyData
                    {
                        Month = x.Month,
                        Amount = x.Amount
                    }).ToList());
                    responseInfo.Data.Add("monthlyPurchaseCourses", res.MonthlyPurchaseCourses.Select(x => new MonthlyData
                    {
                        Month = x.Month,
                        Amount = x.Amount
                    }).ToList());
                    responseInfo.Data.Add("monthlyRevenues", res.MonthlyRevenues.Select(x => new MonthlyData
                    {
                        Month = x.Month,
                        Amount = x.Amount
                    }).ToList());
                    responseInfo.Data.Add("monthlyProfits", res.MonthlyProfits.Select(x => new MonthlyData
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