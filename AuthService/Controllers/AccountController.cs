using AuthService.Commons.Helpers;
using AuthService.Services.Account;
using AuthService.Services.Account.Schemas;
using AuthService.Services.Otp.Schemas.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("auth-service/api/account")]
    [ApiController]
    public class AccountController(IAccountDetailService accountDetailService) : ControllerBase
    {
        private readonly IAccountDetailService _accountDetailService = accountDetailService
            ?? throw new ArgumentNullException(nameof(accountDetailService));

        /// <summary>
        /// Active/Inactive user
        /// <para>Created at: 2024/10/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     This API is only used for the admin role (in Admin page)
        ///     
        /// </remarks>
        [HttpPut("{userId}/activate")]
        [Filters.Auth(Roles = "Admin")]
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

        /// <summary>
        /// Change password
        /// <para>Created at: 2024/10/17</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="changePasswordContent"></param>
        /// <returns></returns>
        [HttpPut("password")]
        [Filters.Auth]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordContent changePasswordContent)
        {
            try
            {
                var responseInfo = await _accountDetailService.ChangePassword(changePasswordContent);
                if (responseInfo.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new
                    {
                        message = responseInfo.Message
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

        /// <summary>
        /// Send email to verify the account deletion
        /// <para>Created at: 2024/10/17</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpPost("delete-account-verification")]
        [Filters.Auth]
        public async Task<IActionResult> VerifyAccountDeletion()
        {
            try
            {
                var responseInfo = await _accountDetailService.SendEmailDeleteAccount();
                if (responseInfo.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new { message = responseInfo.Message });
                }
                return StatusCode(responseInfo.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    responseInfo.StatusCode, responseInfo.Error, responseInfo.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Delete account permanently
        /// <para>Created at: 2024/10/17</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpDelete]
        [Filters.Auth]
        public async Task<IActionResult> DeleteAccount([FromBody] OtpWrapperBase otpWrapperBase)
        {
            try
            {
                var responseInfo = await _accountDetailService.DeleteAccount(otpWrapperBase);
                if (responseInfo.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new { message = responseInfo.Message });
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