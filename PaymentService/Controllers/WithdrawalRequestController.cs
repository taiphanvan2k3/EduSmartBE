using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Commons.Schemas;
using PaymentService.Services.WithdrawalRequests;
using PaymentService.Services.WithdrawalRequests.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/withdrawal-requests")]
    [ApiController]
    public class WithdrawalRequestController(IListOfWithdrawalRequestService listOfWithdrawalRequestService,
        IWithdrawalRequestService withdrawalRequestService) : ControllerBase
    {
        private readonly IListOfWithdrawalRequestService _listOfWithdrawalRequestService = listOfWithdrawalRequestService
            ?? throw new ArgumentNullException(nameof(listOfWithdrawalRequestService));

        private readonly IWithdrawalRequestService _withdrawalRequestService = withdrawalRequestService
            ?? throw new ArgumentNullException(nameof(withdrawalRequestService));

        /// <summary>
        /// Get withdrawal requests
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal requests</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<WithdrawalRequestDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWithdrawalRequests([FromQuery] SearchCondition searchCondition)
        {
            try
            {
                var withdrawalRequests = await _listOfWithdrawalRequestService.GetWithdrawalRequestsAsync(searchCondition);
                return Ok(withdrawalRequests);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get withdrawal requests by user id
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal requests</response>
        /// <response code="500">Internal server error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="400">Bad request</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpGet("my-withdrawal-requests")]
        [ProducesResponseType(typeof(PaginatedList<WithdrawalRequestDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWithdrawalRequestsByUserId([FromQuery] SearchCondition searchCondition)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                var withdrawalRequests = await _listOfWithdrawalRequestService.GetWithdrawalRequestsByUserIdAsync(int.Parse(userId), searchCondition);
                return Ok(withdrawalRequests);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Add withdrawal request
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by by ManhTD</para>
        /// </summary>
        /// <param name="withdrawalRequest"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddWithdrawalRequest([FromBody] WithdrawalRequestPost withdrawalRequest)
        {
            try
            {
                var response = await _withdrawalRequestService.AddWithdrawalRequestAsync(withdrawalRequest);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get withdrawal request by id
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WithdrawalRequestDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWithdrawalRequest(Guid id)
        {
            try
            {
                var withdrawalRequest = await _withdrawalRequestService.GetWithdrawalRequestAsync(id);
                return Ok(withdrawalRequest);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Update withdrawal request status
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by by by ManhTD</para>
        /// </summary>
        /// <param name="withdrawalRequest"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Admin")]
        [HttpPut("status")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateWithdrawalRequestStatus([FromBody] WithdrawalRequestDto withdrawalRequest)
        {
            try
            {
                var response = await _withdrawalRequestService.UpdateWithdrawalRequestStatusAsync(withdrawalRequest);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Delete withdrawal request
        /// <para>Created at: 9/11/2024</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Withdrawal request</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteWithdrawalRequest(Guid id)
        {
            try
            {
                var response = await _withdrawalRequestService.DeleteWithdrawalRequestAsync(id);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}