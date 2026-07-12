using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons.Helpers;
using PaymentService.Services.BankAccounts;
using PaymentService.Services.BankAccounts.Schemas;

namespace PaymentService.Controllers
{
    [Filters.Auth(Roles = "Admin,Teacher")]
    [Route("payment-service/api/bank-accounts")]
    [ApiController]
    public class BankAccountController(IBankAccountService bankAccountService) : BaseController
    {
        private readonly IBankAccountService _bankAccountService = bankAccountService
            ?? throw new ArgumentNullException(nameof(bankAccountService));

        /// <summary>
        /// Get all bank accounts of the current user
        /// <para>Created at: 2026/07/12</para>
        /// </summary>
        [HttpGet("my")]
        public async Task<ActionResult> GetMyBankAccounts()
        {
            var response = await _bankAccountService.GetMyBankAccountsAsync();
            return HandleResponseInfo(response, resourceName: "bankAccounts");
        }

        /// <summary>
        /// Get bank account
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// <para>Modified at: 2024/11/19</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of bank account</param>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetBankAccount(Guid id)
        {
            var response = await _bankAccountService.GetBankAccountAsync(id);
            return HandleResponseInfo(response, resourceName: "bankAccount");
        }

        /// <summary>
        /// Add bank account
        /// <para>Created at: 2024/11/09</para>
        /// <para>Created by ManhTD</para>
        /// <para>Modified at: 2024/11/19</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddBankAccount([FromBody] BankAccountCreateUpdateDto bankAccountCreateDto)
        {
            var response = await _bankAccountService.AddBankAccountAsync(bankAccountCreateDto);
            return HandleResponseInfo(response, resourceName: "bankAccount");
        }

        /// <summary>
        /// Update bank account
        /// <para>Created at: 2024/11/09</para>
        /// <para>Created by ManhTD</para>
        /// <para>Modified at: 2024/11/19</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBankAccount(Guid id, [FromBody] BankAccountCreateUpdateDto bankAccountUpdate)
        {
            var response = await _bankAccountService.UpdateBankAccountAsync(id, bankAccountUpdate);
            return HandleResponseInfo(response, resourceName: "bankAccount");
        }

        /// <summary>
        /// Delete bank account
        /// <para>Created at: 2024/11/09</para>
        /// <para>Created by ManhTD</para>
        /// <para>Modified at: 2024/11/19</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of bank account</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBankAccount(Guid id)
        {
            var response = await _bankAccountService.DeleteBankAccountAsync(id);
            return HandleResponseInfo(response, resourceId: id.ToString());
        }
    }
}