using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Services.Banks.Schemas;
using TblBank = PaymentService.Databases.Schemas.Bank;

namespace PaymentService.Services.Banks
{
    public interface IBankService
    {
        /// <summary>
        /// Add a new bank
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        public Task<ResponseInfo> AddBankAsync(BankDto bank);

        /// <summary>
        /// Update bank information
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateBankAsync(BankDto bank);

        /// <summary>
        /// Delete a bank
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteBankAsync(Guid bankId);

        /// <summary>
        /// Get bank by id
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> GetBankAsync(Guid bankId);

        /// <summary>
        /// Get list of banks
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetBanksAsync();
    }

    public class BankService(IServiceProvider serviceProvider,
        ILogger<BankService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IBankService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> AddBankAsync(BankDto bank)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var isExist = await _context.Banks.AnyAsync(b => b.Name == bank.Name);
                if (isExist)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is already exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return response;
                }

                var bankEntity = _mapper.Map<TblBank>(bank);

                await _context.Banks.AddAsync(bankEntity);
                await _context.SaveChangesAsync();

                response.Message = "Add bank successfully";
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> UpdateBankAsync(BankDto bank)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bankEntity = _context.Banks.FirstOrDefault(b => b.Id == bank.Id);
                if (bankEntity == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                bankEntity.Name = bank.Name;

                _context.Banks.Update(bankEntity);
                _context.SaveChanges();

                response.Message = "Update bank successfully";
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> DeleteBankAsync(Guid bankId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bank = _context.Banks.FirstOrDefault(b => b.Id == bankId);
                if (bank == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                _context.Banks.Remove(bank);
                _context.SaveChanges();

                response.Message = "Delete bank successfully";
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> GetBankAsync(Guid bankId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bank = _context.Banks.FirstOrDefault(b => b.Id == bankId);
                if (bank == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return Task.FromResult(response);
                }

                response.Data.Add("bank", bank);
                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public Task<ResponseInfo> GetBanksAsync()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var banks = _context.Banks.ToList();
                response.Data.Add("banks", banks);

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