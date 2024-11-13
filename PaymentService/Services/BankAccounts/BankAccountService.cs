using AutoMapper;
using PaymentService.Commons;
using PaymentService.Services.BankAccounts.Schemas;
using TblBankAccount = PaymentService.Databases.Schemas.BankAccount;

namespace PaymentService.Services.BankAccounts
{
    public interface IBankAccountService
    {
        /// <summary>
        /// Add a new bank account
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        public Task<ResponseInfo> AddBankAccountAsync(BankAccountDto bankAccount);

        /// <summary>
        /// Update bank account information
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateBankAccountAsync(BankAccountDto bankAccount);

        /// <summary>
        /// Delete a bank account
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteBankAccountAsync(Guid bankAccountId);

        /// <summary>
        /// Get bank account by id
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> GetBankAccountAsync(Guid bankAccountId);
    }

    public class BankAccountService(IServiceProvider serviceProvider,
        ILogger<BankAccountService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IBankAccountService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public Task<ResponseInfo> AddBankAccountAsync(BankAccountDto bankAccount)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var isExist = _context.BankAccounts.Any(b => b.AccountNumber == bankAccount.AccountNumber);
                if (isExist)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is already exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                var bankAccountEntity = _mapper.Map<TblBankAccount>(bankAccount);

                _context.BankAccounts.Add(bankAccountEntity);
                _context.SaveChanges();

                response.Message = "Add bank account successfully";
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> UpdateBankAccountAsync(BankAccountDto bankAccount)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bankAccountEntity = _context.BankAccounts.FirstOrDefault(b => b.Id == bankAccount.Id);
                if (bankAccountEntity == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank account is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                bankAccountEntity.AccountName = bankAccount.AccountName;
                bankAccountEntity.AccountNumber = bankAccount.AccountNumber;
                bankAccountEntity.BankId = bankAccount.BankId;
                bankAccountEntity.IsPrimary = bankAccount.IsPrimary;

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