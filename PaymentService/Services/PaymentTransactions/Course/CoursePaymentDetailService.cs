using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Enumerations;
using PaymentService.Services.Grpc.CourseService;
using PaymentService.Services.Grpc.CourseService.Schemas;
using PaymentService.Services.PaymentTransactions.Course.Schemas;

namespace PaymentService.Services.PaymentTransactions.Course
{
    public interface ICoursePaymentDetailService
    {
        /// <summary>
        /// Process buying course from Sepay webhook
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/21</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> BuyCourse(CoursePaymentRequest coursePaymentRequest);
    }

    public class CoursePaymentDetailService(IServiceProvider serviceProvider, ILogger<CoursePaymentDetailService> logger)
        : BaseService(serviceProvider, logger), ICoursePaymentDetailService
    {
        private readonly IGrpcCourseService _grpcCourseService = serviceProvider.GetService<IGrpcCourseService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IGrpcCourseService"));

        public async Task<ResponseInfo> BuyCourse(CoursePaymentRequest coursePaymentRequest)
        {
            string methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                ResponseInfo responseInfo = await _grpcCourseService.EnrollCourse(new CourseEnrollmentDto
                {
                    CourseId = coursePaymentRequest.CourseId.ToString(),
                    StudentId = coursePaymentRequest.BuyerId,
                    EnrollmentDate = coursePaymentRequest.PaymentDate
                });

                if (responseInfo.StatusCode == StatusCodes.Status200OK)
                {
                    await _context.PaymentTransactions.Where(p => p.Id == coursePaymentRequest.TransactionId)
                        .ExecuteUpdateAsync(p => p.SetProperty(x => x.OrderStatus, OrderStatus.SUCCESS)
                            .SetProperty(x => x.CompletedAt, DateTimeOffset.UtcNow));
                }
                else
                {
                    await _context.PaymentTransactions.Where(p => p.Id == coursePaymentRequest.TransactionId)
                        .ExecuteUpdateAsync(p => p.SetProperty(x => x.OrderStatus, OrderStatus.FAILED)
                            .SetProperty(x => x.Error, responseInfo.Message)
                            .SetProperty(x => x.CompletedAt, DateTimeOffset.UtcNow));
                }

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
