using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
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
                    Amount = (decimal)request.Amount,
                    Currency = currencyType,
                    PaymentMethod = PaymentMethod.Sepay,
                    TransactionType = TransactionType.BuyCourse,
                    OrderStatus = OrderStatus.New,
                    RelatedInformation = request.RelatedInfo
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
                    .Where(x => x.TransactionType == TransactionType.DrawingRequest
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