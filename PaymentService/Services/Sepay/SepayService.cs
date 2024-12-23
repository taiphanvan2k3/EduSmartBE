using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Enumerations;
using PaymentService.Hubs;
using PaymentService.Services.PaymentTransactions.Course;
using PaymentService.Services.PaymentTransactions.Course.Schemas;
using PaymentService.Services.PaymentTransactions.Shared.Schemas;
using PaymentService.Services.Sepay.Schemas;
using PaymentService.Services.WithdrawalRequests.Schemas;
using TblPaymentTransaction = PaymentService.Databases.Schemas.PaymentTransaction;

namespace PaymentService.Services.Sepay
{
    public interface ISepayService
    {
        /// <summary>
        /// Handle Sepay payment request
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/21</para>
        /// </summary>
        /// <param name="sepayRequest">Content of Sepay request</param>
        /// <returns></returns>
        public Task<ResponseInfo> HandlePaymentRequest(SepayRequest sepayRequest);

        /// <summary>
        /// Bank money to teacher
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="sepayWithdrawalRequest"></param>
        /// <returns></returns>
        public Task<ResponseInfo> SaveWithdrawalTransaction(SepayWithdrawalRequest sepayWithdrawalRequest);

    }

    public class SepayService(IServiceProvider serviceProvider, ILogger<SepayService> logger)
        : BaseService(serviceProvider, logger), ISepayService
    {
        private readonly ICoursePaymentDetailService _coursePaymentDetailService = serviceProvider.GetRequiredService<ICoursePaymentDetailService>()
            ?? throw new InvalidOperationException(ServiceInjectionError(nameof(ICoursePaymentDetailService)));
        private readonly IHubContext<NotificationHub> _hubContext = serviceProvider.GetRequiredService<IHubContext<NotificationHub>>()
            ?? throw new ArgumentNullException(ServiceInjectionError(nameof(IHubContext<NotificationHub>)));

        public async Task<ResponseInfo> HandlePaymentRequest(SepayRequest sepayRequest)
        {
            var methodName = GetActualAsyncMethodName();
            TblPaymentTransaction paymentTransaction = null;
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                paymentTransaction = await _context.PaymentTransactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Code == sepayRequest.PaymentContent
                        && p.PaymentMethod == PaymentMethod.Sepay);

                if (paymentTransaction == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "NotFound", "Payment transaction not found");
                }

                if (paymentTransaction.OrderStatus == OrderStatus.SUCCESS)
                {
                    responseInfo = CreateEarlyResponseInfo(StatusCodes.Status400BadRequest,
                        "InvalidStatus", "Payment transaction is already completed");

                    await NotifyClient(paymentTransaction.UserId.ToString(),
                        paymentTransaction.TransactionType.ToString(), responseInfo);
                    return responseInfo;
                }

                var expectedAmount = paymentTransaction.Amount;
                if (paymentTransaction.Currency == CurrencyType.USD)
                {
                    expectedAmount *= Constants.EXCHANGE_RATE_USD_TO_VND;
                }

                if (sepayRequest.TransferAmount != expectedAmount)
                {
                    responseInfo = CreateEarlyResponseInfo(StatusCodes.Status400BadRequest,
                        "InvalidAmount", "Transfer amount is not correct");
                }
                else
                {
                    // Tiền vào tài khoản Admin
                    if (sepayRequest.TransactionType == SepayTransactionType.In)
                    {
                        responseInfo = await HandleTransactionInByType(sepayRequest, paymentTransaction);
                    }
                    else
                    {
                        // TODO: Admin chuyển tiền vào tài khoản khác
                    }
                }

                await _hubContext.Clients.User(paymentTransaction.UserId.ToString())
                    .SendAsync($"{paymentTransaction.TransactionType}Completed", responseInfo);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);

                if (paymentTransaction != null)
                {
                    var responseInfo = CreateEarlyResponseInfo(StatusCodes.Status500InternalServerError,
                        "InternalServerError", e.InnerException?.Message ?? e.Message);

                    await NotifyClient(paymentTransaction.UserId.ToString(),
                        paymentTransaction.TransactionType.ToString(), responseInfo);
                }
                throw;
            }
        }

        public async Task<ResponseInfo> SaveWithdrawalTransaction(SepayWithdrawalRequest sepayWithdrawalRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var paymentTransaction = await _context.PaymentTransactions
                    .FirstOrDefaultAsync(x => x.Code == sepayWithdrawalRequest.PaymentContent);

                if (paymentTransaction == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "NotFound", "Payment transaction not found");
                }

                if (paymentTransaction.OrderStatus == OrderStatus.SUCCESS)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "InvalidStatus",
                        "Payment transaction is already completed");
                }

                paymentTransaction.OrderStatus = OrderStatus.SUCCESS;
                paymentTransaction.CompletedAt = DateTimeOffset.UtcNow;

                var withdrawalRequestId = JsonSerializerUtils.Deserialize<WithdrawalRelatedInfo>(paymentTransaction.RelatedInformation).WithdrawalRequestId;
                var withdrawalRequest = await _context.WithdrawalRequests
                    .FirstOrDefaultAsync(x => x.Id == withdrawalRequestId);

                withdrawalRequest.Status = RequestStatus.Approved;
                withdrawalRequest.ApprovedAt = paymentTransaction.CompletedAt;
                withdrawalRequest.ApprovedBy = paymentTransaction.UserId;

                var teacherEarning = await _context.TeacherEarnings
                    .FirstOrDefaultAsync(x => x.UserId == withdrawalRequest.UserId);

                if (teacherEarning != null)
                {
                    teacherEarning.CurrentBalance -= withdrawalRequest.Amount;
                    teacherEarning.TotalWithdrawn += withdrawalRequest.Amount;
                }

                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Withdrawal transaction completed"
                };

                await NotifyClient(paymentTransaction.UserId.ToString(), paymentTransaction.TransactionType.ToString(), responseInfo);
                return responseInfo;
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

        private async Task<ResponseInfo> HandleTransactionInByType(SepayRequest sepayRequest, TblPaymentTransaction paymentTransaction)
        {
            var responseInfo = new ResponseInfo();
            switch (paymentTransaction.TransactionType)
            {
                case TransactionType.BuyCourse:
                    var decodedRelatedInfo = JsonSerializerUtils.Deserialize<RelatedInfo.BuyCourse>(paymentTransaction.RelatedInformation);

                    // Giờ của Sepay là giờ của Việt Nam => Đang ở sẵn múi +7 rồi
                    DateTimeOffset paymentDate = DateTimeOffset.Parse($"{sepayRequest.TransactionDate} +07:00").ToUniversalTime();

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
                    responseInfo.Data.Add("courseId", decodedRelatedInfo.CourseId);
                    responseInfo.Data.Add("buyerId", paymentTransaction.UserId);
                    break;
            }

            return responseInfo;
        }

        private async Task NotifyClient(string userId, string transactionType, ResponseInfo response)
        {
            await _hubContext.Clients.User(userId).SendAsync($"{transactionType}Completed", response);
        }
    }
}