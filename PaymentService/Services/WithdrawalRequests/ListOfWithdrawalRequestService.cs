using AutoMapper;
using PaymentService.Commons;
using PaymentService.Commons.Schemas;
using PaymentService.Extensions;
using PaymentService.Services.WithdrawalRequests.Schemas;

namespace PaymentService.Services.WithdrawalRequests
{
    public interface IListOfWithdrawalRequestService
    {

        /// <summary>
        /// Get withdrawal requests
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<PaginatedList<WithdrawalRequestDto>> GetWithdrawalRequestsAsync(SearchCondition searchCondition);

        /// <summary>
        /// Get withdrawal requests by user id
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<PaginatedList<WithdrawalRequestDto>> GetWithdrawalRequestsByUserIdAsync(int userId, SearchCondition searchCondition);
    }

    public class ListOfWithdrawalRequestService(IServiceProvider serviceProvider,
        ILogger<ListOfWithdrawalRequestService> logger) : BaseService(serviceProvider, logger), IListOfWithdrawalRequestService
    {
        public async Task<PaginatedList<WithdrawalRequestDto>> GetWithdrawalRequestsAsync(SearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[ListOfWithdrawalRequestService] [{Method}] Start", methodName);
                searchCondition.SearchInput = searchCondition.SearchInput.Trim();

                var withdrawalRequestsQuery = _context.WithdrawalRequests
                    .Where(x =>
                        string.IsNullOrEmpty(searchCondition.SearchInput) ||
                        x.BankAccount.AccountName.Contains(searchCondition.SearchInput))
                    .Select(x => new WithdrawalRequestDto()
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Amount = x.Amount,
                        BankAccountId = x.BankAccountId,
                        Status = x.Status,
                        RequestedAt = x.RequestedAt,
                        ApprovedAt = x.ApprovedAt

                    });

                var paginatedWithdrawalRequests = await withdrawalRequestsQuery.ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                _logger.LogInformation("[ListOfWithdrawalRequestService] [{Method}] End", methodName);
                return paginatedWithdrawalRequests;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ListOfWithdrawalRequestService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<PaginatedList<WithdrawalRequestDto>> GetWithdrawalRequestsByUserIdAsync(int userId, SearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[ListOfWithdrawalRequestService] [{Method}] Start", methodName);
                searchCondition.SearchInput = searchCondition.SearchInput.Trim();

                var withdrawalRequestsQuery = _context.WithdrawalRequests
                    .Where(x =>
                        x.UserId == userId)
                    .Select(x => new WithdrawalRequestDto()
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Amount = x.Amount,
                        BankAccountId = x.BankAccountId,
                        Status = x.Status,
                        RequestedAt = x.RequestedAt,
                        ApprovedAt = x.ApprovedAt
                    });

                var paginatedWithdrawalRequests = await withdrawalRequestsQuery.ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                _logger.LogInformation("[ListOfWithdrawalRequestService] [{Method}] End", methodName);
                return paginatedWithdrawalRequests;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ListOfWithdrawalRequestService] [{Method}] Error", methodName);
                throw;
            }
        }
    }
}