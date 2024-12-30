using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Commons.Schemas;
using PaymentService.Databases;
using PaymentService.Enumerations;
using System.Runtime.CompilerServices;
using TblPaymentTransaction = PaymentService.Databases.Schemas.PaymentTransaction;

namespace PaymentService.GrpcServices
{
    public class GrpcPaymentService(DataContext context, ILogger<GrpcPaymentService> logger) : Payment.PaymentBase
    {
        private readonly DataContext _context = context
            ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<GrpcPaymentService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public override Task<ExchangeRate> GetExchangeRate(Empty request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcPaymentService] [{Method}] Start", methodName);

                var exchangeRate = GetExchangeRate();

                _logger.LogInformation("[GrpcPaymentService] [{Method}] End", methodName);
                return Task.FromResult(exchangeRate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcPaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<BankAccountResponse> GetAdminBankAccount(Empty request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcPaymentService] [{Method}] Start", methodName);

                var bankAccount = await _context.BankAccounts
                    .Where(x => x.IsAdminAccount && x.IsPrimary)
                    .Select(x => new
                    {
                        x.AccountNumber,
                        x.AccountName,
                        x.Bank.Bin,
                        x.Bank.Name
                    })
                    .FirstOrDefaultAsync();

                var response = new BankAccountResponse();
                if (bankAccount == null)
                {
                    response.IsSuccess = false;
                }
                else
                {
                    response.IsSuccess = true;
                    response.AccountNumber = bankAccount.AccountNumber;
                    response.AccountName = bankAccount.AccountName;
                    response.BankBin = bankAccount.Bin;
                    response.BankName = bankAccount.Name;
                }

                _logger.LogInformation("[GrpcPaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcPaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<CoursePaymentTransactionResponse> CreateCoursePaymentTransaction(CoursePaymentTransactionRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcPaymentService] [{Method}] Start", methodName);

                var currencyType = System.Enum.Parse<CurrencyType>(request.Currency.ToString().ToUpper());
                var coursePaymentTransaction = new TblPaymentTransaction
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code,
                    UserId = request.UserId,
                    CreatorInfo = new CreatorInfo()
                    {
                        Username = request.Username,
                        FullName = request.FullName,
                        Email = request.Email
                    },
                    Amount = (decimal)request.Amount,
                    Currency = currencyType,
                    PaymentMethod = PaymentMethod.Sepay,
                    TransactionType = TransactionType.BuyCourse,
                    OrderStatus = OrderStatus.New,
                    RelatedInformation = request.RelatedInfo,
                    ReceiverId = request.ReceiverId
                };

                // Cancel các transaction cũ đối với khi mua cùng 1 course
                await _context.PaymentTransactions
                    .Where(x => x.UserId == request.UserId && x.PaymentMethod == PaymentMethod.Sepay
                        && x.OrderStatus == OrderStatus.New
                        && x.RelatedInformation == request.RelatedInfo)
                    .ExecuteDeleteAsync();

                await _context.PaymentTransactions.AddAsync(coursePaymentTransaction);
                await _context.SaveChangesAsync();

                var response = new CoursePaymentTransactionResponse
                {
                    IsSuccess = true,
                    TransactionId = coursePaymentTransaction.Id.ToString(),
                    ExchangeRate = GetExchangeRate()
                };

                _logger.LogInformation("[GrpcPaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcPaymentService] [{Method}] Error", methodName);

                return new CoursePaymentTransactionResponse
                {
                    IsSuccess = false
                };
            }
        }

        public override async Task<MonthlyDataPaymentResponse> GetMonthlyPaymentInfo(Empty request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcPaymentService] [{Method}] Start", methodName);

                var response = new MonthlyDataPaymentResponse();

                var monthlyDrawingRequests = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.WithdrawalRequest
                        && x.OrderStatus == OrderStatus.SUCCESS
                        && x.CreatedAt.Year == DateTime.Now.Year)
                    .GroupBy(x => x.CreatedAt.Month)
                    .Select(x => new MonthlyDataPayment
                    {
                        Month = x.Key,
                        Amount = x.Count()
                    }).ToListAsync();
                var monthlyPurchaseCourses = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.BuyCourse
                        && x.OrderStatus == OrderStatus.SUCCESS
                        && x.CreatedAt.Year == DateTime.Now.Year)
                    .GroupBy(x => x.CreatedAt.Month)
                    .Select(x => new MonthlyDataPayment
                    {
                        Month = x.Key,
                        Amount = x.Count()
                    }).ToListAsync();

                var monthlyRevenues = await _context.PaymentTransactions
                    .Where(x => x.TransactionType == TransactionType.BuyCourse
                        && x.OrderStatus == OrderStatus.SUCCESS
                        && x.CreatedAt.Year == DateTime.Now.Year)
                    .GroupBy(x => x.CreatedAt.Month)
                    .Select(x => new MonthlyDataPayment
                    {
                        Month = x.Key,
                        Amount = (long)x.Sum(x => x.Currency == CurrencyType.VND ? x.Amount : x.Amount * Constants.EXCHANGE_RATE_USD_TO_VND)
                    }).ToListAsync();

                var monthlyProfits = monthlyRevenues
                    .Select(x => new MonthlyDataPayment
                    {
                        Month = x.Month,
                        Amount = x.Amount - 217000
                    }).ToList();

                response.MonthlyDrawingRequests.AddRange(monthlyDrawingRequests);
                response.MonthlyPurchaseCourses.AddRange(monthlyPurchaseCourses);
                response.MonthlyRevenues.AddRange(monthlyRevenues);
                response.MonthlyProfits.AddRange(monthlyProfits);
                response.IsSuccess = true;
                response.Message = "Get monthly payment info successfully";

                _logger.LogInformation("[GrpcPaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcPaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<StorageInfoResponse> GetCurrentStoringAmount(UserInfo userInfo, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcPaymentService] [{Method}] Start", methodName);

                var storageInfo = await _context.StorageInfos
                    .Where(x => x.UserId == userInfo.Id)
                    .Select(x => new
                    {
                        x.MaximumStorage,
                        x.UsedStorage
                    })
                    .FirstOrDefaultAsync();

                var response = new StorageInfoResponse();
                if (storageInfo == null)
                {
                    response.IsSuccess = true;
                    response.MaximumStorage = Constants.FREE_MAXIMUM_STORAGE_AMOUNT;
                    response.UsedStorage = 0;
                }
                else
                {
                    response.IsSuccess = true;
                    response.MaximumStorage = storageInfo.MaximumStorage;
                    response.UsedStorage = storageInfo.UsedStorage;
                }

                _logger.LogInformation("[GrpcPaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcPaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<CompletedCourses> GetAchievementURLInCompletedCourses(CompletedCourses completedCourses, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcPaymentService] [{Method}] Start", methodName);

                var completedCourseResult = new CompletedCourses
                {
                    UserId = completedCourses.UserId,
                };

                foreach (var completedCourse in completedCourses.CompletedCourseInfo)
                {
                    var course = await _context.StudentAchievements
                        .Where(x => x.CourseId == Guid.Parse(completedCourse.Id) && x.StudentId == completedCourses.UserId)
                        .Select(x => new CompletedCourseInfo
                        {
                            Id = x.CourseId.ToString(),
                            Name = completedCourse.Name,
                            AchievementURL = x.AchievementURL
                        })
                        .FirstOrDefaultAsync();

                    if (course != null)
                    {
                        completedCourseResult.CompletedCourseInfo.Add(course);
                    }
                    else
                    {
                        completedCourseResult.CompletedCourseInfo.Add(completedCourse);
                    }
                }
                completedCourseResult.IsSuccess = true;

                _logger.LogInformation("[GrpcPaymentService] [{Method}] End", methodName);
                return completedCourseResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcPaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        private static ExchangeRate GetExchangeRate()
        {
            return new ExchangeRate
            {
                FromCurrency = Currency.Usd.ToString().ToUpper(),
                ToCurrency = Currency.Vnd.ToString().ToUpper(),
                Rate = Constants.EXCHANGE_RATE_USD_TO_VND
            };
        }
    }
}