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
        /// Get bank by id
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> GetBankAsync(int bankId);

        /// <summary>
        /// Get list of banks
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetBanksAsync();

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
        public Task<ResponseInfo> DeleteBankAsync(int bankId);
    }

    public class BankService(IServiceProvider serviceProvider,
        ILogger<BankService> logger,
        IMapper mapper) : BaseService(serviceProvider, logger), IBankService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ResponseInfo> GetBankAsync(int bankId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == bankId);
                if (bank == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return response;
                }

                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                response.Data.Add("bank", bank);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetBanksAsync()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var banks = await _context.Banks.ToListAsync();
                response.Data.Add("banks", banks);

                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

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

                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateBankAsync(BankDto bank)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bankEntity = await _context.Banks.FirstOrDefaultAsync(b => b.Id == bank.Id);
                if (bankEntity == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return response;
                }

                bankEntity.Name = bank.Name;

                _context.Banks.Update(bankEntity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[PaymentService] [{Method}] Error", methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteBankAsync(int bankId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[PaymentService] [{Method}] Start", methodName);
                var response = new ResponseInfo();

                var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id == bankId);
                if (bank == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bank is not exist";

                    _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
                    return response;
                }

                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[PaymentService] [{Method}] End", methodName);
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