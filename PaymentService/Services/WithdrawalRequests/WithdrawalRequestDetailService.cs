using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Enumerations;
using PaymentService.Services.WithdrawalRequests.Schemas;
using TblWithdrawalRequest = PaymentService.Databases.Schemas.WithdrawalRequest;
using TblTeacherEarning = PaymentService.Databases.Schemas.TeacherEarning;
using PaymentService.Commons.Schemas;
using PaymentService.Commons.Helpers;
using PaymentService.Services.Sepay.Schemas;
using TblPaymentTransaction = PaymentService.Databases.Schemas.PaymentTransaction;

namespace PaymentService.Services.WithdrawalRequests
{
    public interface IWithdrawalRequestDetailService
    {
        /// <summary>
        /// Get withdrawal request by id
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseInfo> GetWithdrawalRequestAsync(Guid id);

        /// <summary>
        /// Create a new withdrawal request
        /// <para>Author: ManhTD - Created at: 9/11/2024</para>
        /// <para>Author: TaiPV - Updated at: 22/12/2024</para>
        /// </summary>
        /// <param name="withdrawalRequest"></param>
        /// <returns></returns>
        Task<ResponseInfo> CreateWithdrawalRequestAsync(WithdrawalRequestPost withdrawalRequest);

        /// <summary>
        /// Update withdrawal request status
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="withdrawalRequestDto"></param>
        /// <returns></returns>
        Task<ResponseInfo> UpdateWithdrawalRequestStatusAsync(WithdrawalRequestDto withdrawalRequestDto);

        /// <summary>
        /// Delete withdrawal request
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para>
        /// </summary>
        /// <param name="id">Id of withdrawal request</param>
        /// <returns></returns>
        Task<ResponseInfo> DeleteWithdrawalRequestAsync(Guid id);

        /// <summary>
        /// Admin get payment info of withdrawal request
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/12/23</para>
        /// </summary>
        /// <param name="id">Id of withdrawal request</param>
        /// <returns></returns>
        Task<ResponseInfo> GetPaymentInfoOfWithdrawalRequestAsync(Guid id);
    }

    public class WithdrawalRequestDetailService(IServiceProvider serviceProvider,
        ILogger<WithdrawalRequestDetailService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IWithdrawalRequestDetailService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> CreateWithdrawalRequestAsync(WithdrawalRequestPost withdrawalRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var currentUser = GetCurrentUser();

                var teacherEarning = await _context.TeacherEarnings
                    .FirstOrDefaultAsync(e => e.UserId == currentUser.UserId);

                if (teacherEarning is null)
                {
                    teacherEarning = new TblTeacherEarning
                    {
                        UserId = currentUser.UserId,
                        CurrentBalance = 0,
                        TotalWithdrawn = 0
                    };

                    await _context.TeacherEarnings.AddAsync(teacherEarning);
                    await _context.SaveChangesAsync();
                    response.Message = "No teacher earning found. Created new teacher earning";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    var totalPendingWithdrawalInBaseCurrency = await _context.WithdrawalRequests
                        .Where(w => w.UserId == currentUser.UserId && w.Status == RequestStatus.Pending)
                        .SumAsync(w => w.Amount * (w.Currency == CurrencyType.VND ? 1 : Constants.EXCHANGE_RATE_USD_TO_VND));
                    var withdrawalAmountInBaseCurrency = Utils.ConvertToBaseCurrency(withdrawalRequest.Amount, withdrawalRequest.CurrencyType);

                    if (teacherEarning.CurrentBalance < withdrawalAmountInBaseCurrency
                        || teacherEarning.CurrentBalance - totalPendingWithdrawalInBaseCurrency < withdrawalAmountInBaseCurrency)
                    {
                        response.Message = "Not enough balance";
                        response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                    else
                    {
                        var withdrawalRequestEntity = new TblWithdrawalRequest
                        {
                            Id = Guid.NewGuid(),
                            UserId = currentUser.UserId,
                            Amount = withdrawalRequest.Amount,
                            BankAccountId = withdrawalRequest.BankAccountId,
                            Status = RequestStatus.Pending,
                            RequestedAt = DateTime.Now,
                            CreatorInfo = new CreatorInfo()
                            {
                                Username = currentUser.UserName,
                                Email = currentUser.Email,
                                FullName = currentUser.FullName
                            },
                            Currency = withdrawalRequest.CurrencyType
                        };

                        await _context.WithdrawalRequests.AddAsync(withdrawalRequestEntity);
                        await _context.SaveChangesAsync();

                        response.Data.Add("id", withdrawalRequestEntity.Id);
                    }
                }
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[WithdrawalRequestService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> GetWithdrawalRequestAsync(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var withdrawalRequest = _context.WithdrawalRequests.FirstOrDefault(x => x.Id == id);
                if (withdrawalRequest == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "WithdrawalRequest not found";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                response.Data.Add("withdrawalRequest", withdrawalRequest);
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[WithdrawalRequestService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateWithdrawalRequestStatusAsync(WithdrawalRequestDto withdrawalRequestDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var withdrawalRequest = await _context.WithdrawalRequests
                    .FirstOrDefaultAsync(x => x.Id == withdrawalRequestDto.Id);
                if (withdrawalRequest == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "WithdrawalRequest not found";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return response;
                }

                if (withdrawalRequest.UserId != withdrawalRequest.UserId)
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You don't have permission to update this withdrawalRequest";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return response;
                }

                withdrawalRequest.Status = withdrawalRequest.Status;
                withdrawalRequest.ApprovedAt = DateTime.Now;

                _context.WithdrawalRequests.Update(withdrawalRequest);
                await _context.SaveChangesAsync();

                response.Message = "Update withdrawalRequest status successfully";
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[WithdrawalRequestService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> DeleteWithdrawalRequestAsync(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var withdrawalRequest = _context.WithdrawalRequests.FirstOrDefault(x => x.Id == id);
                if (withdrawalRequest == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "WithdrawalRequest not found";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                var currentUser = GetCurrentUser();
                if (withdrawalRequest.UserId != currentUser.UserId || !currentUser.Roles.Contains("Admin"))
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You don't have permission to delete this withdrawalRequest";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                _context.WithdrawalRequests.Remove(withdrawalRequest);
                _context.SaveChanges();

                _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[WithdrawalRequestService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetPaymentInfoOfWithdrawalRequestAsync(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var withdrawalRequestIdString = $"\"withdrawalRequestId\":\"{id}\"";

                var relatedInfo = await _context.PaymentTransactions
                    .Where(p => p.RelatedInformation.Contains(withdrawalRequestIdString))
                    .Select(p => p.RelatedInformation)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(relatedInfo))
                {
                    return CreateResponseInfo("qrCode", JsonSerializerUtils.Deserialize<WithdrawalRelatedInfo>(relatedInfo).QRCode);
                }

                var withdrawalRequest = await _context.WithdrawalRequests
                    .Where(w => w.Id == id)
                    .Select(w => new
                    {
                        w.BankAccount.Bank.Bin,
                        w.BankAccount.AccountNumber,
                        w.BankAccount.AccountName,
                        Amount = Utils.ConvertToBaseCurrency(w.Amount, w.Currency),
                        ReceiverId = w.UserId
                    })
                    .FirstOrDefaultAsync();

                if (withdrawalRequest == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Withdrawal request not found");
                }

                var transactionOrder = $"SEVQR{Utils.GenerateRandomString(7)}";
                var qrData = new PaymentQRData()
                {
                    Bin = withdrawalRequest.Bin,
                    AccountNumber = withdrawalRequest.AccountNumber,
                    AccountName = withdrawalRequest.AccountName,
                    Amount = withdrawalRequest.Amount,
                    TransactionOrder = transactionOrder
                };

                var currentUser = GetCurrentUser();
                var qrCode = Utils.GenerateQRCode(qrData);
                relatedInfo = JsonSerializerUtils.Serialize(new WithdrawalRelatedInfo()
                {
                    WithdrawalRequestId = id,
                    QRCode = qrCode
                });

                var paymentTransactionEntity = new TblPaymentTransaction()
                {
                    Id = Guid.NewGuid(),
                    Code = transactionOrder,
                    UserId = currentUser.UserId,
                    PaymentMethod = PaymentMethod.Sepay,
                    Amount = withdrawalRequest.Amount,
                    Currency = CurrencyType.VND,
                    OrderStatus = OrderStatus.New,
                    TransactionType = TransactionType.WithdrawalRequest,
                    RelatedInformation = relatedInfo,
                    CreatedAt = DateTime.Now,
                    CreatorInfo = new CreatorInfo()
                    {
                        Username = currentUser.UserName,
                        Email = currentUser.Email,
                        FullName = currentUser.FullName
                    },
                    ReceiverId = withdrawalRequest.ReceiverId,
                };

                await _context.PaymentTransactions.AddAsync(paymentTransactionEntity);
                await _context.SaveChangesAsync();

                return CreateResponseInfo("qrCode", qrCode);
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
    }
}