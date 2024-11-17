using Microsoft.AspNetCore.Mvc;
using PaymentService.Services.Sepay.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        [HttpPost("sepay")]
        public IActionResult SepayWebhook([FromBody] SepayRequest request)
        {
            return Ok(request);
        }
    }
}