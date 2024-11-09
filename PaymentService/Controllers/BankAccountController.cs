using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons.Helpers;
using PaymentService.Services.BankAccounts;
using PaymentService.Services.BankAccounts.Schemas;

namespace PaymentService.Controllers
{
    public class BankAccountController(IBankAccountService bankAccountService) : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
        
        /// <summary>
        /// Get bank accounts
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<ActionResult> AddBankAccount([FromBody] BankAccountDto bankAccount)
        {
            try
            {
                var response = await _bankAccountService.AddBankAccountAsync(bankAccount);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get bank accounts
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        public async Task<ActionResult> UpdateBankAccount([FromBody] BankAccountDto bankAccount)
        {
            try
            {
                var response = await _bankAccountService.UpdateBankAccountAsync(bankAccount);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Delete bank account
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{bankAccountId}")]
        public async Task<ActionResult> DeleteBankAccount(Guid bankAccountId)
        {
            try
            {
                var response = await _bankAccountService.DeleteBankAccountAsync(bankAccountId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get bank account
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{bankAccountId}")]
        public async Task<ActionResult> GetBankAccount(Guid bankAccountId)
        {
            try
            {
                var response = await _bankAccountService.GetBankAccountAsync(bankAccountId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}