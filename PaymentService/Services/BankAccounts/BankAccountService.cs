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

        /// <summary>
        /// Get bank account by id
        /// <para>Author: ManhTD - Created at: 2024/11/09</para>
        /// <para>Author: TaiPV - Updated at: 2024/11/19</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetBankAccountAsync(Guid bankAccountId);
    }

    public class BankAccountService(IServiceProvider serviceProvider,
        ILogger<BankAccountService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IBankAccountService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

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
                var currentUser = 

                await _context.BankAccounts.AddAsync(bankAccountEntity);
                await _context.SaveChangesAsync();

                LogInfo("End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> UpdateBankAccountAsync(Guid id, BankAccountCreateUpdateDto bankAccountUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bankAccountEntity = _context.BankAccounts.FirstOrDefault(b => b.Id == id);
                if (bankAccountEntity == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                bankAccountEntity.AccountName = bankAccountUpdateDto.AccountName;
                bankAccountEntity.AccountNumber = bankAccountUpdateDto.AccountNumber;
                bankAccountEntity.BankId = bankAccountUpdateDto.BankId;
                bankAccountEntity.IsPrimary = bankAccountUpdateDto.IsPrimary;

                _context.BankAccounts.Update(bankAccountEntity);
                _context.SaveChanges();

                response.Message = "Update bank account successfully";
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> DeleteBankAccountAsync(Guid bankAccountId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bankAccount = _context.BankAccounts.FirstOrDefault(b => b.Id == bankAccountId);
                if (bankAccount == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                _context.BankAccounts.Remove(bankAccount);
                _context.SaveChanges();

                response.Message = "Delete bank account successfully";
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> GetBankAccountAsync(Guid bankAccountId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bankAccount = _context.BankAccounts.FirstOrDefault(b => b.Id == bankAccountId);
                if (bankAccount == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                response.Data.Add("bankAccount", bankAccount);
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }
    }
}