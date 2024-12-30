using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PaymentService.Commons;
using PaymentService.Commons.Schemas;
using PaymentService.Enumerations;
using PaymentService.Extensions;
using PaymentService.Services.Grpc.CourseService;
using PaymentService.Services.TeacherEarnings.Schemas;

namespace PaymentService.Services.TeacherEarnings
{
    public interface ITeacherEarningService
    {
        Task<decimal> GetRevenueAllCoursesOfTeacherAsync(int currencyType);

        Task<List<RevenueCourse>> GetListOfRevenueAllCoursesOfTeacherAsync(int currencyType);

        Task<decimal> GetTotalWithdrawalAmountOfTeacherAsync();

        Task<List<RevenueInMonth>> GetRevenueInYearAsync(int year, int currencyType);

        /// <summary>
        /// Get course payment history in all courses of teacher
        /// <para>Created at: 2024/12/20</para>
        /// <para>Created by TaiPV</para>
        /// </summary>
        Task<ResponseInfo> GetCoursePaymentHistoryAsync(ParamsSearch paramsSearch);
    }

    public class TeacherEarningService(IServiceProvider serviceProvider,
        ILogger<TeacherEarningService> logger) : BaseService(serviceProvider, logger), ITeacherEarningService
    {
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
                    .GroupBy(x => x.RelatedInfo.CourseId)
                    .Select(g => new RevenueCourse
                    {
                        Amount = g.Sum(x => x.Amount),
                        Currency = g.FirstOrDefault().Currency,
                        RelatedInfo = g.FirstOrDefault().RelatedInfo
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

        public async Task<List<RevenueInMonth>> GetRevenueInYearAsync(int year, int currencyType)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[TeacherEarningService] [{Method}] Start", methodName);
                var currentUser = GetCurrentUser();
                var userId = currentUser?.UserId ?? 0;

                var currencyTypeEnum = Enum.Parse<CurrencyType>(currencyType.ToString());
                var transactions = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.BuyCourse
                        && x.OrderStatus == OrderStatus.SUCCESS
                        && x.RelatedInformation.Contains($"\"teacherId\":{userId}")
                        && x.CreatedAt.Year == year)
                    .Select(x => new
                    {
                        Amount = x.Currency == currencyTypeEnum
                            ? x.Amount
                            : (currencyTypeEnum == CurrencyType.VND
                                ? x.Amount * Constants.EXCHANGE_RATE_USD_TO_VND
                                : x.Amount / Constants.EXCHANGE_RATE_USD_TO_VND),
                        Month = x.CreatedAt.Month
                    })
                    .ToListAsync();

                var revenueByMonth = new List<RevenueInMonth>();

                for (int month = 1; month <= 12; month++)
                {
                    var monthName = new DateTime(year, month, 1).ToString("MMMM", CultureInfo.InvariantCulture);
                    var monthlyRevenue = new RevenueInMonth
                    {
                        Month = monthName,
                        Currency = currencyTypeEnum,
                        Amount = transactions.Where(x => x.Month == month).Sum(x => x.Amount)
                    };

                    revenueByMonth.Add(monthlyRevenue);
                }

                _logger.LogInformation("[TeacherEarningService] [{Method}] End", methodName);
                return revenueByMonth;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[TeacherEarningService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetCoursePaymentHistoryAsync(ParamsSearch paramsSearch)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();
                var currentUser = GetCurrentUser();

                var enrollmentHistory = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.BuyCourse
                        && x.OrderStatus == OrderStatus.SUCCESS
                        && x.ReceiverId == currentUser.UserId)
                    .OrderByDescending(x => x.CompletedAt)
                    .Select(x => new
                    {
                        x.Code,
                        x.UserId,
                        x.CreatorInfo,
                        x.Amount,
                        x.Currency,
                        RelatedInfo = JsonConvert.DeserializeObject<RelatedInfo>(x.RelatedInformation),
                        EnrollmentAt = x.CompletedAt ?? x.CreatedAt
                    })
                    .Select(x => new CoursePaymentHistory
                    {
                        PaymentCode = x.Code,
                        Amount = x.Amount,
                        Currency = x.Currency.ToString(),
                        StudentInfo = new UserDetail()
                        {
                            Id = x.UserId,
                            Username = x.CreatorInfo.Username,
                            FullName = x.CreatorInfo.FullName,
                            Email = x.CreatorInfo.Email,
                            AvatarURL = x.CreatorInfo.AvatarURL
                        },
                        CourseInfo = new CourseDetail()
                        {
                            Id = x.RelatedInfo.CourseId
                        },
                        EnrollmentDate = x.EnrollmentAt
                    })
                    .ToPaginatedListAsync(paramsSearch.CurrentPage, paramsSearch.PageSize);

                await FillCourseInfo(enrollmentHistory.Items, responseInfo);
                responseInfo.Data.Add("enrollmentHistories", enrollmentHistory);
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

        private async Task FillCourseInfo(List<CoursePaymentHistory> enrollmentHistories, ResponseInfo responseInfo)
        {
            var courseIds = enrollmentHistories.Select(x => x.CourseInfo.Id).Distinct().ToList();
            var grpcResponse = await _grpcCourseService.GetCoursesByIds(courseIds);

            if (grpcResponse.IsSuccess)
            {
                var coursesDict = (grpcResponse.Data["courses"] as List<CourseDetail>).ToDictionary(x => x.Id, x => x);
                foreach (var enrollmentHistory in enrollmentHistories)
                {
                    if (coursesDict.TryGetValue(enrollmentHistory.CourseInfo.Id, out var courseDetail))
                    {
                        enrollmentHistory.CourseInfo.Name = courseDetail.Name;
                    }
                }
            }
            else
            {
                responseInfo.StatusCode = grpcResponse.StatusCode;
                responseInfo.Message = grpcResponse.Message;
            }
        }
    }
}