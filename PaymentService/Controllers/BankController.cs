using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Services.Banks;
using PaymentService.Services.Banks.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/banks")]
    [ApiController]
    public class BankController(IBankService bankService) : BaseController
    {
        private readonly IBankService _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));

        /// <summary>
        /// Get banks
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<BankDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBanks()
        {
            ResponseInfo banks = await _bankService.GetBanksAsync();
            return HandleResponseInfo(banks, resourceName: "banks");
        }

        /// <summary>
        /// Add bank
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddBank([FromBody] BankDto bank)
        {
            try
            {
                var response = await _bankService.AddBankAsync(bank);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Update bank
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by by ManhTD</para>
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateBank([FromBody] BankDto bank)
        {
            try
            {
                var response = await _bankService.UpdateBankAsync(bank);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Delete bank
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by by ManhTD</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{bankId}")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteBank(int bankId)
        {
            try
            {
                var response = await _bankService.DeleteBankAsync(bankId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get bank
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{bankId}")]
        [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBank(int bankId)
        {
            try
            {
                var bank = await _bankService.GetBankAsync(bankId);
                return Ok(bank);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}