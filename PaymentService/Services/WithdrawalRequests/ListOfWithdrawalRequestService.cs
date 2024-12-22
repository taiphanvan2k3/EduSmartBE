using Microsoft.EntityFrameworkCore;
using PaymentService.Commons.Helpers;
using PaymentService.Commons.Schemas;
using PaymentService.Extensions;
using PaymentService.Services.WithdrawalRequests.Schemas;

namespace PaymentService.Services.WithdrawalRequests
{
    public interface IListOfWithdrawalRequestService
    {
        /// <summary>
        /// Get withdrawal requests
        /// <para>Author: ManhTD - Created at: 9/11/2024</para>
        /// <para>Author: TaiPV - Updated at: 22/12/2024</para>
        /// </summary>
        /// <returns></returns>
        Task<PaginatedList<WithdrawalRequestForAdmin>> GetWithdrawalRequestsAsync(WithdrawalRequestSearchConditionForAdmin searchCondition);

        /// <summary>
        /// Get withdrawal requests by user id
        /// <para>Author: ManhTD - Created at: 9/11/2024</para>
        /// <para>Author: TaiPV - Updated at: 22/12/2024</para>
        /// </summary>
        /// <returns></returns>
        Task<PaginatedList<WithdrawalRequestDto>> GetWithdrawalRequestsByUserIdAsync(int userId,
            WithdrawalRequestSearchCondition searchCondition);
    }

    public class ListOfWithdrawalRequestService(IServiceProvider serviceProvider,
        ILogger<ListOfWithdrawalRequestService> logger) : BaseService(serviceProvider, logger), IListOfWithdrawalRequestService
    {
        public async Task<PaginatedList<WithdrawalRequestForAdmin>> GetWithdrawalRequestsAsync(WithdrawalRequestSearchConditionForAdmin searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[ListOfWithdrawalRequestService] [{Method}] Start", methodName);
                searchCondition.SearchInput = searchCondition.SearchInput.Trim().ToLower();
                searchCondition.FromDate = searchCondition.FromDate?.ToUniversalTime();
                searchCondition.ToDate = searchCondition.ToDate?.ToUniversalTime();

                var withdrawalRequestsQuery = _context.WithdrawalRequests
                    .Where(x =>
                        string.IsNullOrEmpty(searchCondition.SearchInput) ||
                        x.CreatorInfo.Email.Contains(searchCondition.SearchInput) ||
                        x.CreatorInfo.Username.Contains(searchCondition.SearchInput) ||
                        EF.Functions.ILike(x.CreatorInfo.FullName, $"%{searchCondition.SearchInput}%")
                    )
                    .Where(x =>
                        (!searchCondition.FromDate.HasValue || x.RequestedAt >= searchCondition.FromDate) &&
                        (!searchCondition.ToDate.HasValue || x.RequestedAt <= searchCondition.ToDate)
                    )
                    .OrderByDescending(x => x.RequestedAt)
                    .Select(x => new WithdrawalRequestForAdmin()
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        CreatorInfo = x.CreatorInfo,
                        Amount = x.Amount,
                        Currency = x.Currency.ToString(),
                        AmountInBaseCurrency = Utils.ConvertToBaseCurrency(x.Amount, x.Currency),
                        BankAccountId = x.BankAccountId,
                        Status = x.Status,
                        RequestedAt = x.RequestedAt,
                        ApprovedAt = x.ApprovedAt,
                        BankAccountNumber = x.BankAccount.AccountNumber,
                        BankName = x.BankAccount.Bank.ShortName,
                        BankAccountName = x.BankAccount.AccountName
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

        public async Task<PaginatedList<WithdrawalRequestDto>> GetWithdrawalRequestsByUserIdAsync(int userId, WithdrawalRequestSearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[ListOfWithdrawalRequestService] [{Method}] Start", methodName);

                searchCondition.FromDate = searchCondition.FromDate?.ToUniversalTime();
                searchCondition.ToDate = searchCondition.ToDate?.ToUniversalTime();

                var withdrawalRequestsQuery = _context.WithdrawalRequests
                    .Where(x => x.UserId == userId)
                    .Where(x =>
                        (!searchCondition.FromDate.HasValue || x.RequestedAt >= searchCondition.FromDate) &&
                        (!searchCondition.ToDate.HasValue || x.RequestedAt <= searchCondition.ToDate)
                    )
                    .OrderByDescending(x => x.RequestedAt)
                    .Select(x => new WithdrawalRequestDto()
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Amount = x.Amount,
                        Currency = x.Currency.ToString(),
                        BankAccountId = x.BankAccountId,
                        Status = x.Status,
                        RequestedAt = x.RequestedAt,
                        ApprovedAt = x.ApprovedAt,
                        BankAccountNumber = x.BankAccount.Bank.Name,
                        BankName = x.BankAccount.AccountName,
                        BankAccountName = x.BankAccount.AccountName
                    });

                var paginatedWithdrawalRequests = await withdrawalRequestsQuery
                    .ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

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