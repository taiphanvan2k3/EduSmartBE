using AuthService.AsyncDataServices;
using AuthService.Commons;
using AuthService.Dtos;
using AuthService.Services.Auth;
using AuthService.Services.Auth.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService, ITokenService tokenService, IMessageBusClient messageBusClient) : ControllerBase
    {
        private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        private readonly IMessageBusClient _messageBusClient = messageBusClient ?? throw new ArgumentNullException(nameof(messageBusClient));

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new
                {
                    message = "Only one of UserName or Email is required"
                });
            }

            if (string.IsNullOrEmpty(request.UserName) && string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new
                {
                    message = "UserName or Email is required"
                });
            }

            try
            {
                var responseInfo = await _authService.CheckLogin(request);
                if (responseInfo.StatusCode == StatusCodes.Status200OK)
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
                    message = responseInfo.Message
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "RefreshToken is required"
                });
            }

            try
            {
                var responseInfo = await _tokenService.DoRefreshToken(request.RefreshToken);

                if (responseInfo.StatusCode == StatusCodes.Status200OK)
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
                        meta = responseInfo.Data["meta"],
                    });
                }

                return Unauthorized(new
                {
                    message = responseInfo.Message
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.Message
                });
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var response = await _authService.SignUp(request);
            if(response.StatusCode == StatusCodes.Status201Created)
            {
                var userRegister = response.Data["userRegister"];
                _messageBusClient.PublishUserCreated(new UserCreatedDto
                {
                    UserId = userRegister.Id.ToString(),
                    Email = userRegister.Email,
                    UserName = userRegister.UserName,
                    CreateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
                return Ok(new
                {
                    status = true
                });
            }
            return BadRequest(new
            {
                status = false,
                message = response.Message
            });
        }

        [HttpGet("confirm-account", Name = "ConfirmAccount")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(new
                {
                    message = "Token and Email are required"
                });
            }

            try
            {
                ResponseInfo response = await _authService.ConfirmAccount(token, userId);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new
                    {
                        message = response.Message
                    });
                }

                return BadRequest(new
                {
                    message = response.Message
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.Message
                });
            }
        }
    }
}