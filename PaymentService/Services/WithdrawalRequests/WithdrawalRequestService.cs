using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Services.WithdrawalRequests.Schemas;
using TblWithdrawalRequest = PaymentService.Databases.Schemas.WithdrawalRequest;

namespace PaymentService.Services.WithdrawalRequests
{
    public interface IWithdrawalRequestService
    {
        /// <summary>
        /// Add withdrawal request
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="withdrawalRequest"></param>
        /// <returns></returns>
        Task<ResponseInfo> AddWithdrawalRequestAsync(WithdrawalRequestDto withdrawalRequest);

        /// <summary>
        /// Get withdrawal request by id
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseInfo> GetWithdrawalRequestAsync(Guid id);

        /// <summary>
        /// Update withdrawal request status
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="withdrawalRequest"></param>
        /// <returns></returns>
        Task<ResponseInfo> UpdateWithdrawalRequestStatusAsync(WithdrawalRequestDto withdrawalRequest);

        /// <summary>
        /// Delete withdrawal request
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para>
        /// </summary>
        /// <param name="id">Id of withdrawal request</param>
        /// <returns></returns>
        Task<ResponseInfo> DeleteWithdrawalRequestAsync(Guid id);

        /// <summary>
        /// Bank money to teacher
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<ResponseInfo> BankMoneyToTeacherAsync(Guid id, int userId, int amount);
    }

    public class WithdrawalRequestService(IServiceProvider serviceProvider,
        ILogger<WithdrawalRequestService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IWithdrawalRequestService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> AddWithdrawalRequestAsync(WithdrawalRequestDto withdrawalRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var withdrawalRequestEntity = _mapper.Map<TblWithdrawalRequest>(withdrawalRequest);

                await _context.WithdrawalRequests.AddAsync(withdrawalRequestEntity);
                await _context.SaveChangesAsync();

                response.Message = "Add withdrawalRequest successfully";
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

        public Task<ResponseInfo> UpdateWithdrawalRequestStatusAsync(WithdrawalRequestDto withdrawalRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var withdrawalRequestQuery = _context.WithdrawalRequests.FirstOrDefault(x => x.Id == withdrawalRequest.Id);
                if (withdrawalRequestQuery == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "WithdrawalRequest not found";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                if (withdrawalRequestQuery.UserId != withdrawalRequest.UserId)
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You don't have permission to update this withdrawalRequest";

                    _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                withdrawalRequestQuery.Status = withdrawalRequest.Status;
                withdrawalRequestQuery.ApprovedAt = DateTime.Now;

                _context.WithdrawalRequests.Update(withdrawalRequestQuery);
                _context.SaveChanges();

                response.Message = "Update withdrawalRequest status successfully";
                _logger.LogInformation("[WithdrawalRequestService] [{Method}] End", methodName);
                return Task.FromResult(response);
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

        public Task<ResponseInfo> BankMoneyToTeacherAsync(Guid id, int userId, int amount)
        {
            throw new NotImplementedException();
        }
    }
}