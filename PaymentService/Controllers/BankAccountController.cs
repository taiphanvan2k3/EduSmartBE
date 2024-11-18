using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons.Helpers;
using PaymentService.Services.BankAccounts;
using PaymentService.Services.BankAccounts.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/bank-accounts")]
    [ApiController]
    public class BankAccountController(IBankAccountService bankAccountService) : BaseController
    {
        private readonly IBankAccountService _bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));

        /// <summary>
        /// Get bank accounts
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth]
        [HttpPost]
        [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddBankAccount([FromBody] BankAccountCreateDto bankAccountCreateDto)
        {
            var response = await _bankAccountService.AddBankAccountAsync(bankAccountCreateDto);
            return HandleResponseInfo(response, resourceName: "bankAccount");
        }

        /// <summary>
        /// Get bank accounts
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
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
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
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
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
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