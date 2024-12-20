using CourseManagementService.Common;
using CourseManagementService.GrpcServices;
using CourseManagementService.Services.Grpc.PaymentService.Schemas;
using CourseManagementService.Services.Grpc.UserService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using CurrencyEnumGrpc = CourseManagementService.GrpcServices.Currency;
using UserInfoGrpc = CourseManagementService.GrpcServices.UserInfo;

namespace CourseManagementService.Services.Grpc.PaymentService
{
    public interface IGrpcPaymentService
    {
        /// <summary>
        /// Get admin bank account information
        /// <para>Created at: 2024/11/19</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        public Task<ResponseInfo> GetAdminBankAccountAsync();

        /// <summary>
        /// Create course payment transaction (Handle after user paid for course)
        /// <para>Created at: 2024/11/19</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> CreateCoursePaymentTransactionAsync(PaymentTransactionData data);

        /// <summary>
        /// Get current storage information
        /// <para>Created at: 2024/12/08</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetCurrentStorageInfo(int userId);
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
                    UserId = data.CreatedBy.Id,
                    Username = data.CreatedBy.Username,
                    FullName = data.CreatedBy.FullName,
                    Email = data.CreatedBy.Email,
                    Amount = data.Amount,
                    RelatedInfo = data.RelatedInfo,
                    Currency = ConvertStringToCurrencyEnum(data.CurrencyCode),
                    ReceiverId = data.ReceiverId,
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

        public async Task<ResponseInfo> GetCurrentStorageInfo(int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var client = new Payment.PaymentClient(_channel);
                var storageInfoResponse = await client.GetCurrentStoringAmountAsync(new UserInfoGrpc { Id = userId });

                if (storageInfoResponse.IsSuccess)
                {
                    responseInfo.Data.Add("storageInfo", new StorageInfo
                    {
                        MaximumStorage = storageInfoResponse.MaximumStorage,
                        UsedStorage = storageInfoResponse.UsedStorage
                    });
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                    responseInfo.Message = storageInfoResponse.Message;
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