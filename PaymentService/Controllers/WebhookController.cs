using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons.Helpers;
using PaymentService.Services.Sepay;
using PaymentService.Services.Sepay.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/webhook")]
    [ApiController]
    public class WebhookController(ISepayService sepayService, IConfiguration configuration) : BaseController
    {
        private readonly ISepayService _sepayService = sepayService
            ?? throw new ArgumentNullException(nameof(sepayService));
        private readonly IConfiguration _configuration = configuration
            ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        /// Handle Sepay webhook
        /// <para>Created at: 2024/11/21</para>
        /// <para>Created by TaiPV</para> 
        /// </summary>
        [HttpPost("sepay")]
        public async Task<IActionResult> SepayWebhook([FromBody] SepayRequest request)
        {
            var sepayAPIKey = _configuration["Sepay:WebhookAPIKey"];
            var authorizationHeader = HttpContext.Request.Headers.Authorization;
            if (authorizationHeader.Count == 0 || authorizationHeader.ToString().Replace("Apikey ", "") != sepayAPIKey)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ErrorResponseHelper.GetContentOfAnyError(StatusCodes.Status401Unauthorized,
                    "Unauthorized", "Missing or invalid Authorization header"));
            }

            var responseInfo = await _sepayService.HandlePaymentRequest(request);
            return HandleResponseInfoNoResource(responseInfo);
        }

        [HttpPost("sepay/receive")]
        public IActionResult ReceiveWebhook([FromBody] SepayWithdrawlRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Invalid request body");
                }

                _sepayService.SaveWithdrawalTransaction(request);

                return Ok(new { message = "Webhook received and processed successfully" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status200OK, e.Message);
            }
        }
    }
}