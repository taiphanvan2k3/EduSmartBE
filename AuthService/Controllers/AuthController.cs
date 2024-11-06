using AuthService.Commons;
using AuthService.Commons.Helpers;
using AuthService.Enumerations;
using AuthService.Services.Auth;
using AuthService.Services.Auth.Schemas;
using AuthService.Services.Otp.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("auth-service/api/auth")]
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
        ///             id: 2003,
        ///             "userName": "userName",
        ///             "email": "email",
        ///             "firstName": "firstName",
        ///             "lastName": "lastName",
        ///             "avatarUrl": avatarUrl,
        ///             "createdAt": "2024-10-02T19:38:40.2021319+07:00",
        ///             "isActive": false,
        ///             "roles": [ "Student", "Assistant" ]
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
        ///         statusCode: 400,
        ///         error: "Bad Request",
        ///         message: "Only one of UserName or Email is required | UserName or Email is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 401,
        ///         error: "Unauthorized",
        ///         message: "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 403,
        ///         error: "Forbidden",
        ///         message: "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 404,
        ///         error: "InvalidAccount",
        ///         message: "Username or email not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 500,
        ///         error: "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("Only one of UserName or Email is required"));
            }

            if (string.IsNullOrEmpty(request.Username) && string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("UserName or Email is required"));
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

                return StatusCode(responseInfo.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    responseInfo.StatusCode, responseInfo.Error, responseInfo.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
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
        ///     DO NOT PASS the Code parameter!!!
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
        ///             "roles": [ "Student", "Assistant" ]
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
        ///         statusCode: 400,
        ///         error: "Bad Request",
        ///         message: "IdToken is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 401,
        ///         error: "Unauthorized",
        ///         message: "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 403,
        ///         error: "Forbidden",
        ///         message: "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 404,
        ///         error: "....",
        ///         message: "User not found"
        ///     }
        /// </response>
        /// <response code="409">
        /// User not signed up yet
        /// 
        ///     {
        ///         statusCode: 409,
        ///         error: "UserNotSignedUp",
        ///         message: "The user has not signed up yet. Additional information is required to complete the registration."
        ///     }
        ///     
        /// Email is already used by another method
        /// 
        ///     {
        ///         statusCode: 409,
        ///         error: "EmailInUse",
        ///         message: "Email has been used by another method."
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 500,
        ///         error: "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("login-google-by-token")]
        public async Task<IActionResult> LoginGoogleByToken([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("IdToken is required"));
            }

            if (!string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("Passing Code is not allowed"));
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

                if (responseInfo.StatusCode == StatusCodes.Status409Conflict)
                {
                    var userInfo = responseInfo.Data.TryGetValue("userInfo", out dynamic value)
                        ? value : new { error = "Cannot get user info from your JWT" };

                    return StatusCode(responseInfo.StatusCode, new
                    {
                        statusCode = responseInfo.StatusCode,
                        error = responseInfo.Error,
                        message = responseInfo.Message,
                        userInfo
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
        /// Login google with authorization code. It will be used to get IdToken
        /// <para>Created at: 2024/09/22</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Note:
        /// 
        ///     DO NOT PASS the IdToken parameter!!!
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
        ///             "roles": [ "Student", "Assistant" ]
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
        ///         statusCode: 400,
        ///         error: "Bad Request",
        ///         message: "Code is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 401,
        ///         error: "Unauthorized",
        ///         message: "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 403,
        ///         error: "Forbidden",
        ///         message: "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 404,
        ///         error: "....",
        ///         message: "User not found"
        ///     }
        /// </response>
        /// <response code="409">
        /// User not signed up yet
        /// 
        ///     {
        ///         statusCode: 409,
        ///         error: "UserNotSignedUp",
        ///         message: "The user has not signed up yet. Additional information is required to complete the registration."
        ///     }
        ///     
        /// Email is already used by another method
        /// 
        ///     {
        ///         statusCode: 409,
        ///         error: "EmailInUse",
        ///         message: "Email has been used by another method."
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 500,
        ///         error: "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("login-google-by-code")]
        public async Task<IActionResult> LoginGoogleByCode([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("Code is required"));
            }

            if (!string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("Passing IdToken is not allowed"));
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

                if (responseInfo.StatusCode == StatusCodes.Status409Conflict)
                {
                    var userInfo = responseInfo.Data.TryGetValue("userInfo", out dynamic value)
                        ? value : new { error = "Cannot get user info from your authorization code" };

                    return StatusCode(responseInfo.StatusCode, new
                    {
                        statusCode = responseInfo.StatusCode,
                        error = responseInfo.Error,
                        message = responseInfo.Message,
                        userInfo
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
        /// Login google with user info from Google (The request data structure is below)
        /// <para>Created at: 2024/09/24</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Provider
        /// 
        ///     Email
        ///     Google
        ///     Facebook
        ///     GitHub
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
        ///             "roles": [ "Student", "Assistant" ]
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
        ///         statusCode: 400,
        ///         error: "Bad Request",
        ///         message: "Provider must be one of the following ..."
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 401,
        ///         error: "Unauthorized",
        ///         message: "Invalid login"
        ///     }
        /// </response>
        /// <response code="403">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 403,
        ///         error: "Forbidden",
        ///         message: "Account is locked out | Account is not allowed | Requires two factor"
        ///     }
        /// </response>
        /// <response code="404">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 404,
        ///         error: "....",
        ///         message: "User not found"
        ///     }
        /// </response>
        /// <response code="409">
        /// User not signed up yet
        /// 
        ///     {
        ///         statusCode: 409,
        ///         error: "UserNotSignedUp",
        ///         message: "The user has not signed up yet. Additional information is required to complete the registration."
        ///     }
        ///     
        /// Email is already used by another method
        /// 
        ///     {
        ///         statusCode: 409,
        ///         error: "EmailInUse",
        ///         message: "Email has been used by another method."
        ///     }
        /// </response>
        /// <response code="500">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 500,
        ///         error: "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(
                    ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()));
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

                if (responseInfo.StatusCode == StatusCodes.Status409Conflict)
                {
                    var userInfo = responseInfo.Data.TryGetValue("userInfo", out dynamic value)
                        ? value : new { error = "Cannot get user info from your JWT" };

                    return StatusCode(responseInfo.StatusCode, new
                    {
                        statusCode = responseInfo.StatusCode,
                        error = responseInfo.Error,
                        message = responseInfo.Message,
                        data = new { userInfo }
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
        ///         statusCode: 400,
        ///         error: "Bad Request",
        ///         message: "RefreshToken is required"
        ///     }
        /// </response>
        /// <response code="401">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 401,
        ///         error: "Unauthorized",
        ///         message: "Refresh token is invalid"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         statusCode: 500,
        ///         error: "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("RefreshToken is required"));
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
                        expires = new
                        {
                            accessToken = responseInfo.Data["AccessTokenExpireIn"],
                            refreshToken = responseInfo.Data["RefreshTokenExpireIn"]
                        }
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
        /// Register a new user account
        /// <para>Created at: 2024/09/05</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="request">Contains user registration information</param>
        /// <returns></returns>
        /// <remarks>
        /// Note:
        /// 
        ///     You don't need to pass the password when signing up with social providers.
        ///     The password is required when signing up with email.
        /// Role
        /// 
        ///     1 - Admin. But you cannot pass this role
        ///     2 - Teacher
        ///     3 - Student
        /// Code
        /// 
        ///     200 - User created successfully
        ///     400 - Error during signup
        /// Provider
        /// 
        ///     Email
        ///     Google
        ///     Facebook
        ///     GitHub
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         message: "Signup successfully"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         statusCode: 400,
        ///         message: "Error message explaining why signup failed"
        ///     }
        /// </response>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid || request.Role == Role.Admin)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                if (request.Role == Role.Admin)
                {
                    modelStateErrors.Add("Create account with Admin role is not allowed");
                }

                return StatusCode(400, ErrorResponseHelper.GetContentOfBadRequestResponse(modelStateErrors));
            }

            try
            {
                var response = await _authService.SignUp(request);
                if (response.StatusCode == StatusCodes.Status201Created)
                {
                    return Ok(new SuccessResponse(response.Message));
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
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
        ///         message: "Account confirmed successfully"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "statusCode": 400,
        ///         "error": "Bad Request",
        ///         message: "Token and Email are required | Error message explaining why confirmation failed"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         "statusCode": 500,
        ///         "error": "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpGet("confirm-account", Name = "ConfirmAccount")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse("Token and Email are required"));
            }

            try
            {
                ResponseInfo response = await _authService.ConfirmAccount(token, userId);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new SuccessResponse(response.Message));
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Initiate the password recovery process for a user.
        /// <para>Created at: 2024/10/08</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="forgotPasswordContent">The content for forgot password</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     200 - Reset password link sent successfully
        ///     400 - Validation error | User not found
        ///     500 - Server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         message: "Reset password link has been sent to your email"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "statusCode": 400,
        ///         "error": "Bad Request",
        ///         message: "Error messages explaining the validation failure"
        ///     }
        /// </response>
        /// <response code="404">
        /// User not found
        ///     
        ///     {
        ///         "statusCode": 404,
        ///         "error": "Not Found",
        ///         message: "User not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         "statusCode": 500,
        ///         "error": "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordContent forgotPasswordContent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(
                    ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()));
            }

            try
            {
                var response = await _authService.ForgotPassword(forgotPasswordContent);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new SuccessResponse(response.Message));
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
                throw;
            }
        }

        /// <summary>
        /// Initiate the password recovery process for a user.
        /// <para>Created at: 2024/10/08</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="otpContentBase">Contains the email and OTP code</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     200 - Reset password link sent successfully
        ///     400 - Validation error | User not found
        ///     500 - Server error
        ///  OtpType
        ///     
        ///     1 - Forgot password
        ///     2 - Confirm delete account
        ///     
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         message: "OTP code is valid"
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "statusCode": 400,
        ///         "error": "Bad Request",
        ///         message: "Error messages explaining the validation failure"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         "statusCode": 500,
        ///         "error": "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpPost("validate-otp")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        public IActionResult ValidateOtp([FromBody] OtpContentBase otpContentBase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(
                    ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()));
            }

            try
            {
                var response = _authService.ValidateOtpCode(otpContentBase.OtpType, otpContentBase.Email, otpContentBase.OtpCode);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new SuccessResponse(response.Message));
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Reset the user's password using the provided token and new password.
        /// <para>Created at: 2024/10/08</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="resetPasswordContent">The content for resetting the password</param>
        /// <returns></returns>
        /// <remarks>
        /// Code
        /// 
        ///     200 - Password reset successfully
        ///     400 - Validation error
        ///     404 - User not found
        ///     500 - Server error
        /// </remarks>
        /// <response code="200">
        /// Success
        /// 
        ///     {
        ///         message: "Password has been reset successfully."
        ///     }
        /// </response>
        /// <response code="400">
        /// Validate error
        /// 
        ///     {
        ///         "statusCode": 400,
        ///         "error": "Bad Request",
        ///         message: "Error messages explaining the validation failure or user not found"
        ///     }
        /// </response>
        /// <response code="404">
        /// User not found
        ///     
        ///     {
        ///         "statusCode": 404,
        ///         "error": "Not Found",
        ///         message: "User not found"
        ///     }
        /// </response>
        /// <response code="500">
        /// Server error
        /// 
        ///     {
        ///         "statusCode": 500,
        ///         "error": "Internal Server Error",
        ///         message: "Server error message ..."
        ///     }
        /// </response>
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordContent resetPasswordContent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(
                    ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()));
            }

            try
            {
                var response = await _authService.ResetPassword(resetPasswordContent);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new SuccessResponse(response.Message));
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper.GetContentOfAnyError(
                    response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
                throw;
            }
        }
    }
}