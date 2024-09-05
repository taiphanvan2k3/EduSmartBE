using AuthService.Commons;
using AuthService.Services.Auth;
using AuthService.Services.Auth.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Only one of UserName or Email is required"
                });
            }

            if (string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "UserName or Email is required"
                });
            }

            var responseInfo = await _authService.CheckLogin(request);
            if (responseInfo.StatusCode == HttpStatusCode.OK)
            {
                // Set cookie token
                Response.Cookies.Append("access_token", responseInfo.Data["meta"].accessToken.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(responseInfo.Data["AccessTokenExpireIn"])
                });

                return Ok(new
                {
                    userInfo = responseInfo.Data["userInfo"],
                    meta = responseInfo.Data["meta"],
                });
            }

            return Unauthorized(new
            {
                status = false,
                message = responseInfo.Message
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            string message = await _authService.SignUp(request);
            if (string.IsNullOrEmpty(message))
            {
                return Ok(new
                {
                    status = true
                });
            }
            return BadRequest(new
            {
                status = false,
                message
            });
        }

        [HttpGet("confirm-account", Name = "ConfirmAccount")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Token and Email are required"
                });
            }

            try
            {
                ResponseInfo response = await _authService.ConfirmAccount(token, userId);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(new
                    {
                        status = true,
                        message = response.Message
                    });
                }

                return BadRequest(new
                {
                    status = false,
                    message = response.Message
                });
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.INTERNAL_SERVER_ERROR, new
                {
                    status = false,
                    message = e.Message
                });
            }
        }
    }
}