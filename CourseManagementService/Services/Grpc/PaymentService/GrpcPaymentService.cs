using CourseManagementService.Common;
using CourseManagementService.GrpcServices;
using CourseManagementService.Services.Grpc.PaymentService.Schemas;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using CurrencyEnumGrpc = CourseManagementService.GrpcServices.Currency;

namespace CourseManagementService.Services.Grpc.PaymentService
{
    public interface IGrpcPaymentService
    {
        public Task<ResponseInfo> GetAdminBankAccountAsync();
        public Task<ResponseInfo> CreateCoursePaymentTransactionAsync(PaymentTransactionData data);
    }

    public class GrpcPaymentService : BaseService, IGrpcPaymentService
    {
        private readonly GrpcChannel _channel;

        public GrpcPaymentService(IServiceProvider serviceProvider, ILogger<GrpcUserService> logger) : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:PaymentService:GrpcUrl"]);
        }

        public async Task<ResponseInfo> GetAdminBankAccountAsync()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var client = new Payment.PaymentClient(_channel);
                var adminBankResponse = await client.GetAdminBankAccountAsync(new Empty());

                var responseInfo = new ResponseInfo();
                if (adminBankResponse.IsSuccess)
                {
                    responseInfo.Data.Add("adminAccount", new BankAccountDto()
                    {
                        AccountNumber = adminBankResponse.AccountNumber,
                        AccountName = adminBankResponse.AccountName,
                        Bin = adminBankResponse.BankBin,
                        BankName = adminBankResponse.BankName
                    });
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                }

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateCoursePaymentTransactionAsync(PaymentTransactionData data)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var client = new Payment.PaymentClient(_channel);

                var request = new CoursePaymentTransactionRequest
                {
                    Code = data.TransactionCode,
                    UserId = data.UserId,
                    Amount = data.Amount,
                    RelatedInfo = data.RelatedInfo,
                    Currency = ConvertStringToCurrencyEnum(data.CurrencyCode)
                };

                if (request.Currency == CurrencyEnumGrpc.Unknown)
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    return responseInfo;
                }

                var transactionResponse = await client.CreateCoursePaymentTransactionAsync(request);
                if (transactionResponse.IsSuccess)
                {
                    responseInfo.Data.Add("transactionId", transactionResponse.TransactionId);
                    responseInfo.Data.Add("exchangeRate", transactionResponse.ExchangeRate.Rate);
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                }

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        private static CurrencyEnumGrpc ConvertStringToCurrencyEnum(string currencyCode)
        {
            return currencyCode switch
            {
                "USD" => CurrencyEnumGrpc.Usd,
                "VND" => CurrencyEnumGrpc.Vnd,
                _ => CurrencyEnumGrpc.Unknown
            };
        }
    }
}