using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Enumerations;
using PaymentService.Services.PaymentTransactions.Course;
using PaymentService.Services.PaymentTransactions.Course.Schemas;
using PaymentService.Services.PaymentTransactions.Schemas;
using PaymentService.Services.Sepay.Schemas;
using TblPaymentTransaction = PaymentService.Databases.Schemas.PaymentTransaction;

namespace PaymentService.Services.Sepay
{
    public interface ISepayService
    {
        public Task<ResponseInfo> HandlePaymentRequest(SepayRequest sepayRequest);
    }

    public class SepayService(IServiceProvider serviceProvider, ILogger<SepayService> logger)
        : BaseService(serviceProvider, logger), ISepayService
    {
        private readonly ICoursePaymentDetailService _coursePaymentDetailService = serviceProvider.GetRequiredService<ICoursePaymentDetailService>()
            ?? throw new InvalidOperationException(ServiceInjectionError(nameof(ICoursePaymentDetailService)));

        public async Task<ResponseInfo> HandlePaymentRequest(SepayRequest sepayRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var paymentTransaction = await _context.PaymentTransactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Code == sepayRequest.PaymentContent
                        && p.PaymentMethod == PaymentMethod.Sepay
                        && p.OrderStatus == OrderStatus.New);

                if (paymentTransaction == null)
                {
                    responseInfo.Message = "Payment transaction not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                var expectedAmount = paymentTransaction.Amount;
                if (paymentTransaction.Currency == CurrencyType.USD)
                {
                    expectedAmount *= Constants.EXCHANGE_RATE_USD_TO_VND;
                }

                if (sepayRequest.TransferAmount != expectedAmount)
                {
                    responseInfo.Message = "Transfer amount is not correct";
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    return responseInfo;
                }

                // Tiền vào tài khoản Admin
                if (sepayRequest.TransactionType == SepayTransactionType.In)
                {
                    responseInfo = await HandleTransactionInByType(sepayRequest, paymentTransaction);
                }
                else
                {
                    // TODO: Admin chuyển tiền vào tài khoản khác
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

        private async Task<ResponseInfo> HandleTransactionInByType(SepayRequest sepayRequest, TblPaymentTransaction paymentTransaction)
        {
            var responseInfo = new ResponseInfo();
            switch (paymentTransaction.TransactionType)
            {
                case TransactionType.BuyCourse:
                    var decodedRelatedInfo = JsonSerializerUtils.Deserialize<RelatedInfo.BuyCourse>(paymentTransaction.RelatedInformation);

                    // Giờ của Sepay là giờ của Việt Nam => Đang ở sẵn múi +7 rồi
                    DateTimeOffset paymentDate = DateTime.Parse(sepayRequest.TransactionDate).ToUniversalTime();

                    var coursePaymentRequest = new CoursePaymentRequest()
                    {
                        TransactionId = paymentTransaction.Id,
                        CourseId = decodedRelatedInfo.CourseId,
                        TeacherId = decodedRelatedInfo.TeacherId,
                        BuyerId = paymentTransaction.UserId,
                        Amount = sepayRequest.TransferAmount,
                        PaymentDate = paymentDate
                    };

                    responseInfo = await _coursePaymentDetailService.BuyCourse(coursePaymentRequest);
                    break;
            }

            return responseInfo;
        }
    }
}