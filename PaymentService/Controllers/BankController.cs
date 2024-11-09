using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Services.Banks;
using PaymentService.Services.Banks.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/banks")]
    [ApiController]
    public class BankController(IBankService bankService) : ControllerBase
    {
        private readonly IBankService _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
        
        /// <summary>
        /// Get banks
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<BankDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBanks()
        {
            try
            {
                var banks = await _bankService.GetBanksAsync();
                return Ok(banks);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Add bank
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
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
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
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
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{bankId}")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteBank(Guid bankId)
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
        /// <para>Author: ManhTD</para>
        /// <para>Created at: 9/11/2024</para>
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{bankId}")]
        [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetBank(Guid bankId)
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