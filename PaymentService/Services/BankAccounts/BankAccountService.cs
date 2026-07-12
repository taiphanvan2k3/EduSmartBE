using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Services.BankAccounts.Schemas;
using TblBankAccount = PaymentService.Databases.Schemas.BankAccount;

namespace PaymentService.Services.BankAccounts
{
    public interface IBankAccountService
    {
        /// <summary>
        /// Get bank account by id
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetBankAccountAsync(Guid bankAccountId);

        /// <summary>
        /// Get all bank accounts of the current user
        /// <para>Author: TaiPV - Created at: 2026/07/12</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetMyBankAccountsAsync();

        /// <summary>
        /// Add a new bank account
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para>
        /// </summary>
        public Task<ResponseInfo> AddBankAccountAsync(BankAccountCreateUpdateDto bankAccountCreateDto);

        /// <summary>
        /// Update bank account information
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para> 
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateBankAccountAsync(Guid id, BankAccountCreateUpdateDto bankAccountUpdateDto);

        /// <summary>
        /// Delete a bank account
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteBankAccountAsync(Guid bankAccountId);
    }

    public class BankAccountService(IServiceProvider serviceProvider,
        ILogger<BankAccountService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IBankAccountService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> GetBankAccountAsync(Guid bankAccountId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var response = new ResponseInfo();

                var bankAccount = await _context.BankAccounts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == bankAccountId);
                var currentUser = GetCurrentUser();

                if (bankAccount == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return response;
                }

                if (!currentUser.IsAdmin && bankAccount.UserId != currentUser.UserId)
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You are not authorized to view this bank account";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return response;
                }

                var bankAccountDto = _mapper.Map<BankAccountDto>(bankAccount);
                response.Data.Add("bankAccount", bankAccountDto);

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetMyBankAccountsAsync()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();
                var response = new ResponseInfo();

                var bankAccounts = await _context.BankAccounts
                    .AsNoTracking()
                    .Include(b => b.Bank)
                    .Where(b => b.UserId == currentUser.UserId)
                    .OrderByDescending(b => b.IsPrimary)
                    .ToListAsync();

                var bankAccountDtos = _mapper.Map<List<BankAccountDto>>(bankAccounts);
                response.Data.Add("bankAccounts", bankAccountDtos);

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> AddBankAccountAsync(BankAccountCreateUpdateDto bankAccountCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var response = new ResponseInfo();

                var isExist = await _context.BankAccounts.AnyAsync(b =>
                    b.AccountNumber == bankAccountCreateDto.AccountNumber);

                if (isExist)
                {
                    response.Error = "BankExists";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is already exist";

                    LogInfo("End", methodName);
                    return response;
                }

                var bankAccountEntity = _mapper.Map<TblBankAccount>(bankAccountCreateDto);
                if (bankAccountEntity == null)
                {
                    response.Error = "MappingFailed";
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Failed to map bank account";

                    LogInfo("End", methodName);
                    return response;
                }

                var currentUser = GetCurrentUser();
                bankAccountEntity.UserId = currentUser.UserId;
                bankAccountEntity.IsAdminAccount = currentUser.IsAdmin;

                var isExistPrimary = await _context.BankAccounts.AnyAsync(b =>
                    b.UserId == currentUser.UserId && b.IsPrimary);
                if (bankAccountEntity.IsPrimary && isExistPrimary)
                {
                    response.Error = "PrimaryAccountExists";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Primary bank account is already exist";

                    LogInfo("End", methodName);
                    return response;
                }

                await _context.BankAccounts.AddAsync(bankAccountEntity);
                await _context.SaveChangesAsync();

                var bankAccountDto = _mapper.Map<BankAccountDto>(bankAccountEntity);

                response.Data.Add("bankAccount", bankAccountDto);
                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateBankAccountAsync(Guid id, BankAccountCreateUpdateDto bankAccountUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var response = new ResponseInfo();

                var currentUser = GetCurrentUser();

                var bankAccountEntity = await _context.BankAccounts.FindAsync(id);
                if (bankAccountEntity == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    LogInfo("End", methodName);
                    return response;
                }

                if (currentUser.UserId != bankAccountEntity.UserId)
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You are not authorized to update this bank account";

                    LogInfo("End", methodName);
                    return response;
                }

                bankAccountEntity.AccountName = bankAccountUpdateDto.AccountName;
                bankAccountEntity.AccountNumber = bankAccountUpdateDto.AccountNumber;
                bankAccountEntity.BankId = bankAccountUpdateDto.BankId;
                bankAccountEntity.IsPrimary = bankAccountUpdateDto.IsPrimary;

                if (bankAccountEntity.IsPrimary)
                {
                    var existedPrimary = await _context.BankAccounts
                        .FirstOrDefaultAsync(b => b.UserId == currentUser.UserId && b.IsPrimary && b.Id != id);

                    if (existedPrimary != null)
                    {
                        existedPrimary.IsPrimary = false;
                    }
                }

                await _context.SaveChangesAsync();

                var bankAccountDto = _mapper.Map<BankAccountDto>(bankAccountEntity);
                response.Data.Add("bankAccount", bankAccountDto);

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteBankAccountAsync(Guid bankAccountId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();
                var currentUser = GetCurrentUser();

                var bankAccount = _context.BankAccounts.FirstOrDefault(b => b.Id == bankAccountId);
                if (bankAccount == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    LogInfo("End", methodName);
                    return response;
                }

                if (!currentUser.IsAdmin && bankAccount.UserId != currentUser.UserId)
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You are not authorized to delete this bank account";

                    LogInfo("End", methodName);
                    return response;
                }

                _context.BankAccounts.Remove(bankAccount);
                await _context.SaveChangesAsync();

                response.Data.Add("bankAccountId", bankAccountId);

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }
    }
}