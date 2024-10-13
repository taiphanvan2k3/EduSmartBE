using AuthService.Commons.Helpers;
using AuthService.Services.Account;
using AuthService.Services.Account.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/auth/account")]
    [ApiController]
    public class AccountController(IAccountDetailService accountDetailService) : ControllerBase
    {
        private readonly IAccountDetailService _accountDetailService = accountDetailService
            ?? throw new ArgumentNullException(nameof(accountDetailService));

        [HttpPut("{userId}/activate")]
        public async Task<IActionResult> ActivateUser([FromRoute] int userId, [FromBody] UserActivationContent activationContent)
        {
            try
            {
                var responseInfo = await _accountDetailService.ActivateUser(userId, activationContent.IsActive);
                if (responseInfo.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new
                    {
                        userId,
                        isActive = activationContent.IsActive
                    });
                }

                return StatusCode(responseInfo.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    responseInfo.StatusCode, responseInfo.Error, responseInfo.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}