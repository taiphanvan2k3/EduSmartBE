using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Enumerations;
using PaymentService.Services.PaymentTransactions.Course.Schemas;

namespace PaymentService.Services.PaymentTransactions.Course
{
    public interface ICoursePaymentDetailService
    {
        public Task<ResponseInfo> BuyCourse(CoursePaymentRequest coursePaymentRequest);
    }

    public class CoursePaymentDetailService(IServiceProvider serviceProvider, ILogger<CoursePaymentDetailService> logger)
        : BaseService(serviceProvider, logger), ICoursePaymentDetailService
    {
        public async Task<ResponseInfo> BuyCourse(CoursePaymentRequest coursePaymentRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                await _context.PaymentTransactions.Where(p => p.Id == coursePaymentRequest.TransactionId)
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.OrderStatus, OrderStatus.SUCCESS)
                        .SetProperty(x => x.CompletedAt, DateTimeOffset.UtcNow));

                return responseInfo;
            }
            catch (Exception e)
            {
                await _context.PaymentTransactions.Where(p => p.Id == coursePaymentRequest.TransactionId)
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.OrderStatus, OrderStatus.FAILED)
                        .SetProperty(x => x.Error, e.Message)
                        .SetProperty(x => x.CompletedAt, DateTimeOffset.UtcNow));
                LogError(e, methodName);
                throw;
            }
        }
    }
}