using System.Reflection.Metadata;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PaymentService.Commons;
using PaymentService.Enumerations;
using PaymentService.Services.Grpc.CourseService;
using PaymentService.Services.TeacherEarnings.Schemas;

namespace PaymentService.Services.TeacherEarnings
{
    public interface ITeacherEarningService
    {
        Task<decimal> GetRevenueAllCoursesOfTeacherAsync(int currencyType);

        Task<List<RevenueCourse>> GetListOfRevenueAllCoursesOfTeacherAsync(int currencyType);

        Task<decimal> GetTotalWithdrawalAmountOfTeacherAsync();
    }

    public class TeacherEarningService(IServiceProvider serviceProvider,
        ILogger<TeacherEarningService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), ITeacherEarningService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IGrpcCourseService _grpcCourseService = serviceProvider.GetService<IGrpcCourseService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("IGrpcCourseService"));

        public async Task<decimal> GetRevenueAllCoursesOfTeacherAsync(int currencyType)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[TeacherEarningService] [{Method}] Start", methodName);
                var currentUser = GetCurrentUser();
                var userId = currentUser?.UserId ?? 0;

                var currencyTypeEnum = Enum.Parse<CurrencyType>(currencyType.ToString());
                var totalEarnings = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.BuyCourse && x.OrderStatus == OrderStatus.SUCCESS && x.RelatedInformation.Contains($"\"teacherId\":{userId}"))
                    .Select(x => new
                    {
                        Amount = x.Currency == currencyTypeEnum
                            ? x.Amount
                            : (currencyTypeEnum == CurrencyType.VND
                                ? x.Amount * Constants.EXCHANGE_RATE_USD_TO_VND
                                : x.Amount / Constants.EXCHANGE_RATE_USD_TO_VND)
                    })
                    .SumAsync(x => x.Amount);

                _logger.LogInformation("[TeacherEarningService] [{Method}] End", methodName);
                return totalEarnings;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TeacherEarningService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<List<RevenueCourse>> GetListOfRevenueAllCoursesOfTeacherAsync(int currencyType)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[TeacherEarningService] [{Method}] Start", methodName);
                var responseInfo = new ResponseInfo();
                var currentUser = GetCurrentUser();
                var userId = currentUser?.UserId ?? 0;

                var currencyTypeEnum = Enum.Parse<CurrencyType>(currencyType.ToString());
                var listOfTotalEarning = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.BuyCourse && x.OrderStatus == OrderStatus.SUCCESS && x.RelatedInformation.Contains($"\"teacherId\":{userId}"))
                    .Select(x => new
                    {
                        x.Amount,
                        x.Currency,
                        x.RelatedInformation
                    })
                    .ToListAsync();

                var listOfRevenueCourse = listOfTotalEarning
                    .Select(x => new RevenueCourse
                    {
                        Amount = x.Currency == currencyTypeEnum
                            ? x.Amount
                            : (currencyTypeEnum == CurrencyType.VND
                                ? x.Amount * Constants.EXCHANGE_RATE_USD_TO_VND
                                : x.Amount / Constants.EXCHANGE_RATE_USD_TO_VND),
                        Currency = x.Currency,
                        RelatedInfo = JsonConvert.DeserializeObject<RelatedInfo>(x.RelatedInformation)
                    })
                    .GroupBy(x => x.RelatedInfo)
                    .Select(g => new RevenueCourse
                    {
                        Amount = g.Sum(x => x.Amount),
                        Currency = g.FirstOrDefault().Currency,
                        RelatedInfo = g.Key,
                    })
                    .ToList();

                var responseInfoGrpc = await _grpcCourseService.GetInfoCourseByIds(listOfRevenueCourse);

                _logger.LogInformation("[TeacherEarningService] [{Method}] End", methodName);
                return responseInfoGrpc.Data["listOfTotalEarning"];
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TeacherEarningService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<decimal> GetTotalWithdrawalAmountOfTeacherAsync()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[TeacherEarningService] [{Method}] Start", methodName);
                var currentUser = GetCurrentUser();
                var userId = currentUser?.UserId ?? 0;

                var totalWithdrawalAmount = _context.TeacherEarnings
                    .Where(x => x.UserId == userId)
                    .Select(x => x.TotalWithdrawn)
                    .FirstOrDefault();

                _logger.LogInformation("[TeacherEarningService] [{Method}] End", methodName);
                return Task.FromResult(totalWithdrawalAmount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TeacherEarningService] [{Method}] Error", methodName);
                throw;
            }
        }
    }
}