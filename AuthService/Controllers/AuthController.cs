using AuthService.Commons;
using AuthService.Enumerations;
using AuthService.Services.Auth;
using AuthService.Services.Auth.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
    {
        private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        /// <summary>
        /// Login with UserName or Email and Password
        /// <para>Created at: 2024/09/05</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="request">Information is need for login</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     200 - Success
        ///     400 - Only one of UserName or Email is required | UserName or Email is required
        ///     401 - Invalid login
        ///     403 - Account is locked out | Account is not allowed | Requires two factor
        ///     404 - User not found
        ///     500 - Server error
        ///
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "userInfo": {
        ///             id": 2003,
        ///             "userName": "userName",
        ///             "email": "email",
        ///             "firstName": "firstName",
        ///             "lastName": "lastName",
        ///             "avatarUrl": avatarUrl,
        ///             "createdAt": "2024-10-02T19:38:40.2021319+07:00",
        ///             "isActive": false,
        ///             "roles": [ ... ]
        ///         },
        ///         "meta": {
        ///             "accessToken": "refreshToken",
        ///             "refreshToken": "refreshToken",
        ///         }
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Only one of UserName or Email is required | UserName or Email is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         "message": "User not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Server error message ..."
        ///     }
        /// </response>
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

        /// <summary>
        /// Login google with IdToken (JWT)
        /// <para>Created at: 2024/09/22</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="request">IdToken which is contains user info from Google. DON'T PASS Code!!!</param>
        /// <remarks>
        /// Note:
        /// 
        ///     DO NOT PASS the Code paramter!!!
        ///
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "userInfo": {
        ///             id": 2003,
        ///             "userName": "userName",
        ///             "email": "email",
        ///             "firstName": "firstName",
        ///             "lastName": "lastName",
        ///             "avatarUrl": avatarUrl,
        ///             "createdAt": "2024-10-02T19:38:40.2021319+07:00",
        ///             "isActive": false,
        ///             "roles": [ ... ]
        ///         },
        ///         "meta": {
        ///             "accessToken": "refreshToken",
        ///             "refreshToken": "refreshToken",
        ///         }
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "message": "IdToken is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         "message": "User not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("login-google-by-token")]
        public async Task<IActionResult> LoginGoogleByToken([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(new
                {
                    message = "IdToken is required"
                });
            }

            try
            {
                var responseInfo = await _authService.LoginGoogleByToken(request);
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

        /// <summary>
        /// Login google with authorization code. It will be used to get IdToken
        /// <para>Created at: 2024/09/22</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Note:
        /// 
        ///     DO NOT PASS the IdToken paramter!!!
        ///
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "userInfo": {
        ///             id": 2003,
        ///             "userName": "userName",
        ///             "email": "email",
        ///             "firstName": "firstName",
        ///             "lastName": "lastName",
        ///             "avatarUrl": avatarUrl,
        ///             "createdAt": "2024-10-02T19:38:40.2021319+07:00",
        ///             "isActive": false,
        ///             "roles": [ ... ]
        ///         },
        ///         "meta": {
        ///             "accessToken": "refreshToken",
        ///             "refreshToken": "refreshToken",
        ///         }
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Code is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         "message": "User not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("login-google-by-code")]
        public async Task<IActionResult> LoginGoogleByCode([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new
                {
                    message = "Code is required"
                });
            }

            try
            {
                var responseInfo = await _authService.LoginGoogleByCode(request);
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

        /// <summary>
        /// Login google with user info from Google (The request data structure is below)
        /// <para>Created at: 2024/09/24</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Role
        /// 
        ///     1 - Admin. But you cannot pass this role
        ///     2 - Teacher
        ///     3 - Student
        ///
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "userInfo": {
        ///             id": 2003,
        ///             "userName": "userName",
        ///             "email": "email",
        ///             "firstName": "firstName",
        ///             "lastName": "lastName",
        ///             "avatarUrl": avatarUrl,
        ///             "createdAt": "2024-10-02T19:38:40.2021319+07:00",
        ///             "isActive": false,
        ///             "roles": [ ... ]
        ///         },
        ///         "meta": {
        ///             "accessToken": "refreshToken",
        ///             "refreshToken": "refreshToken",
        ///         }
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Provider must be one of the following ..."
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         "message": "User not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()
                });
            }

            if (request.UserInfo.Role == Role.Admin)
            {
                return BadRequest(new
                {
                    message = "You cannot pass Admin role"
                });
            }

            try
            {
                var responseInfo = await _authService.ExternalLogin(request);
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

        /// <summary>
        /// Refresh the access token using a valid refresh token
        /// <para>Created at: 2024/09/15</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="request">Contains the refresh token to be validated</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     200 - Success
        ///     400 - RefreshToken is required
        ///     401 - Invalid refresh token
        ///     500 - Server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "meta": {
        ///             "accessToken": "accessToken",
        ///             "refreshToken": "refreshToken"
        ///         }
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "message": "RefreshToken is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Refresh token is invalid"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         "message": "Server error message ..."
        ///     }
        /// </response>
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

        /// <summary>
        /// Register a new user account
        /// <para>Created at: 2024/09/05</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="request">Contains user registration information</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     201 - User created successfully
        ///     400 - Error during signup
        /// </remarks>
        /// <response code="201">
        /// Success
        /// 
        ///     {
        ///         "status": true
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "status": false,
        ///         "message": "Error message explaining why signup failed"
        ///     }
        /// </response>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var response = await _authService.SignUp(request);
            if (response.StatusCode == StatusCodes.Status201Created)
            {
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

        /// <summary>
        /// Confirm a user's account using a token
        /// <para>Created at: 2024/09/05</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="token">The confirmation token sent to the user</param>
        /// <param name="userId">The user ID associated with the account</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     200 - Account confirmed successfully
        ///     400 - Token and Email are required | Error during confirmation
        ///     500 - Server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         "message": "Account confirmed successfully"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "message": "Token and Email are required | Error message explaining why confirmation failed"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         "message": "Server error message ..."
        ///     }
        /// </response>
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