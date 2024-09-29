using AuthService.BackgroundServices;
using AuthService.Commons;
using AuthService.Databases.Schemas;
using AuthService.Extensions;
using AuthService.Services.Auth.Schemas;
using AuthService.Services.MailSender.Schemas;
using AuthService.Services.User.Schemas;
using AuthService.Settings;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.Services.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// Check login by username/email and password
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 29/08/2024</para>
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        public Task<ResponseInfo> CheckLogin(LoginRequest loginRequest);

        /// <summary>
        /// Login with Google by token (IdToken)
        /// <para>Author: TaiPV</para>
        /// <para> Created at: 11/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> LoginGoogleByToken(GoogleLoginRequest googleLoginRequest, string refreshToken = null);

        /// <summary>
        /// Login with Google by code (Authorization code)
        /// <para>Author: TaiPV</para>
        /// <para> Created at: 12/09/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> LoginGoogleByCode(GoogleLoginRequest googleLoginRequest);

        /// <summary>
        /// Login with external provider (Google, Facebook, etc.)
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 29/09/2024</para>
        /// </summary>
        /// <param name="externalLoginRequest"></param>
        /// <returns></returns>
        public Task<ResponseInfo> ExternalLogin(ExternalLoginRequest externalLoginRequest);

        /// <summary>
        /// Check login by username/email and password
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 29/08/2024</para>
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        public Task<ResponseInfo> SignUp(SignUpRequest signUpRequest);

        /// <summary>
        /// Handle confirm account
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 30/08/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> ConfirmAccount(string token, string userId);
    }

    public class AuthService(IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUrlHelper urlHelper,
        IOptions<ServerSetting> serverSetting,
        MailProducer mailProducer,
        CommonProducer commonProducer,
        ITokenService tokenService,
        IGoogleAuthService googleAuthService,
        ILogger<AuthService> logger)
        : BaseService(serviceProvider, logger), IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager
            ?? throw new ArgumentNullException(nameof(userManager));
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager
            ?? throw new ArgumentNullException(nameof(signInManager));
        private readonly MailProducer _mailProducer = mailProducer
            ?? throw new ArgumentNullException(nameof(mailProducer));
        private readonly CommonProducer _commonProducer = commonProducer
            ?? throw new ArgumentNullException(nameof(commonProducer));
        private readonly ServerSetting _serverSetting = serverSetting?.Value
            ?? throw new ArgumentNullException(nameof(serverSetting));
        private readonly ITokenService _tokenService = tokenService
            ?? throw new ArgumentNullException(nameof(tokenService));
        private readonly IGoogleAuthService _googleAuthService = googleAuthService
            ?? throw new ArgumentNullException(nameof(googleAuthService));

        public async Task<ResponseInfo> CheckLogin(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("[AuthService][CheckLogin] Start");
                var responseInfo = new ResponseInfo();
                var userName = loginRequest.UserName;
                var user = await _userManager.FindByEmailAsync(loginRequest.Email)
                    ?? await _userManager.FindByNameAsync(loginRequest.UserName);

                if (user == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";

                    _logger.LogInformation("[AuthService][CheckLogin] End");
                    return responseInfo;
                }
                userName = user.UserName;

                var result = await _signInManager.PasswordSignInAsync(userName, loginRequest.Password, isPersistent: false,
                    lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                        responseInfo.Message = "Account is locked out";
                    }
                    else if (result.IsNotAllowed)
                    {
                        responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                        responseInfo.Message = "Account is not allowed";
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                        responseInfo.Message = "Requires two factor";
                    }
                    else
                    {
                        responseInfo.StatusCode = StatusCodes.Status401Unauthorized;
                        responseInfo.Message = "Invalid login";
                    }

                    _logger.LogInformation("[AuthService][CheckLogin] End");
                    return responseInfo;
                }

                var userInfo = new UserInfo()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = [.. (await _userManager.GetRolesAsync(user))] // convert to list
                };

                await GenerateTokens(userInfo, responseInfo);
                _logger.LogInformation("[AuthService][CheckLogin] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CheckLogin");
                throw;
            }
        }

        public async Task<ResponseInfo> LoginGoogleByToken(GoogleLoginRequest googleLoginRequest, string refreshToken = null)
        {
            string methodName = GetActualAsyncMethodName();
            try
            {
                var responseInfo = new ResponseInfo();
                _logger.LogInformation("[AuthService][{MethodName}] Start", methodName);
                var payLoad = await GoogleJsonWebSignature.ValidateAsync(googleLoginRequest.IdToken);

                var user = await _userManager.FindByEmailAsync(payLoad.Email);
                if (user == null)
                {
                    await CreateUserFromGooglePayload(payLoad, googleLoginRequest.Role.GetDisplayName(), refreshToken);
                }
                else
                {
                    // Kiểm tra xem email đã được liên kết với tài khoản google chưa
                    var isAuthenticatedByGoogle = await _userManager.FindByLoginAsync("Google", payLoad.Subject) != null;
                    if (!isAuthenticatedByGoogle)
                    {
                        // Người dùng đã đăng ký bằng email, không phải google
                        responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                        responseInfo.Message = "Email has been used by another method";
                        return responseInfo;
                    }
                }

                user ??= await _userManager.FindByEmailAsync(payLoad.Email);
                var userInfo = new UserInfo()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = [.. (await _userManager.GetRolesAsync(user))]
                };
                await GenerateTokens(userInfo, responseInfo);

                _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{MethodName}][{Error}]", methodName, e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> LoginGoogleByCode(GoogleLoginRequest googleLoginRequest)
        {
            var methodName = GetActualAsyncMethodName();
            _logger.LogInformation("[AuthService][{MethodName}] Start", methodName);
            try
            {
                GoogleTokenResponse googleTokenResponse = await _googleAuthService.GetAccessToken(googleLoginRequest.Code);
                googleLoginRequest.IdToken = googleTokenResponse.IdToken;

                var responseInfo = await LoginGoogleByToken(googleLoginRequest, googleTokenResponse.RefreshToken);
                _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{MethodName}][{Error}]", methodName, e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> ExternalLogin(ExternalLoginRequest externalLoginRequest)
        {
            var methodName = GetActualAsyncMethodName();
            _logger.LogInformation("[AuthService][{MethodName}] Start", methodName);

            try
            {
                var responseInfo = new ResponseInfo();
                var user = await _userManager.FindByEmailAsync(externalLoginRequest.UserInfo.Email);
                if (user == null)
                {
                    await CreateUserFromExternalPayload(externalLoginRequest.UserInfo, externalLoginRequest.Provider,
                        externalLoginRequest.UserInfo.Role.GetDisplayName());
                }
                else
                {
                    var loginInfo = await _userManager.FindByLoginAsync(externalLoginRequest.Provider,
                        externalLoginRequest.UserInfo.Email);

                    if (loginInfo == null)
                    {
                        // Người dùng đã đăng ký bằng email, không phải provider mà người dùng đang đăng nhập
                        responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                        responseInfo.Message = "Email has been used by another method";
                    }
                }

                user ??= await _userManager.FindByEmailAsync(externalLoginRequest.UserInfo.Email);
                var userInfo = new UserInfo()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = [.. (await _userManager.GetRolesAsync(user))]
                };

                await GenerateTokens(userInfo, responseInfo);
                _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{MethodName}][{Error}]", methodName, e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> SignUp(SignUpRequest signUpRequest)
        {
            try
            {
                _logger.LogInformation("[AuthService][SignUp] Start");
                var responseInfo = new ResponseInfo();
                var user = new ApplicationUser()
                {
                    Email = signUpRequest.Email,
                    UserName = !string.IsNullOrEmpty(signUpRequest.Username)
                        ? signUpRequest.Username
                        : signUpRequest.Email.Split('@')[0],
                    FirstName = signUpRequest.FirstName,
                    LastName = signUpRequest.LastName,
                };

                var result = await _userManager.CreateAsync(user, signUpRequest.Password);
                if (!result.Succeeded)
                {
                    responseInfo.StatusCode = StatusCodes.Status401Unauthorized;
                    responseInfo.Message = result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}\n{b}");
                    return responseInfo;
                }

                string role = signUpRequest.Role.GetDisplayName();
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                string callbackUrl = await GenerateEmailConfirmationTokenAsync(user);
                await SendMailConfirmAccount(user.Email, user.UserName, callbackUrl);

                responseInfo.StatusCode = StatusCodes.Status201Created;
                responseInfo.Message = "Login success!";
                responseInfo.Data.Add("userRegister", user);

                _logger.LogInformation("[AuthService][SignUp] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][SignUp][{Error}]", e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> ConfirmAccount(string token, string userId)
        {
            try
            {
                _logger.LogInformation("[AuthService][ConfirmAccount] Start");
                var responseInfo = new ResponseInfo();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";
                }

                var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
                if (!confirmResult.Succeeded)
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = confirmResult.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}\n{b}");
                }

                _logger.LogInformation("[AuthService][ConfirmAccount] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][ConfirmAccount][{Error}]", e.Message);
                throw;
            }
        }

        private async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return $"{_serverSetting.BaseUrl}{urlHelper.RouteUrl("ConfirmAccount", new { userId = user.Id, token })}";
        }

        private async Task SendMailConfirmAccount(string email, string userName, string callbackUrl)
        {
            var mailBody = new ConfirmMailBody()
            {
                Subject = "Xác thực tài khoản",
                ToEmail = email,
                ToUserName = userName,
                ConfirmLink = callbackUrl
            };
            await _mailProducer.EnqueueMailAsync(mailBody);
        }

        private async Task GenerateTokens(UserInfo userInfo, ResponseInfo responseInfo)
        {
            var authTokens = _tokenService.GenerateTokens(userInfo);

            responseInfo.Data.Add("AccessTokenExpireIn", authTokens.AccessTokenExpireIn);
            responseInfo.Data.Add("meta", new
            {
                accessToken = authTokens.AccessToken,
                refreshToken = authTokens.RefreshToken
            });

            // Lưu refresh token vào db
            await _commonProducer.EnqueueDataAsync(new BackgroundJobData()
            {
                JobType = BackgroundJobType.SAVE_REFRESH_TOKEN,
                Data = new Dictionary<string, dynamic>()
                {
                    { "refreshToken", authTokens.RefreshToken },
                    { "userId", userInfo.Id },
                    { "ipAddress", _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString() ?? "::1" }
                }
            });

            responseInfo.Data.Add("userInfo", userInfo);
        }

        private async Task CreateUserFromGooglePayload(GoogleJsonWebSignature.Payload payLoad,
            string role = null, string refreshToken = null)
        {
            var user = new ApplicationUser()
            {
                Email = payLoad.Email,
                UserName = payLoad.Email.Split('@')[0],
                FirstName = payLoad.GivenName,
                LastName = payLoad.FamilyName,
                AvatarURL = payLoad.Picture
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(e => e.Description)
                    .Aggregate((a, b) => $"{a}\n{b}"));
            }

            await _userManager.AddToRoleAsync(user, role ?? "Student");
            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payLoad.Subject, "Google"));

            if (refreshToken != null)
            {
                await _userManager.SetAuthenticationTokenAsync(user, "Google", "refresh_token", refreshToken);
            }
        }

        private async Task CreateUserFromExternalPayload(ExternalUserInfo externalUserInfo, string provider,
            string role = null)
        {
            var user = new ApplicationUser()
            {
                Email = externalUserInfo.Email,
                UserName = externalUserInfo.Email.Split('@')[0],
                FirstName = externalUserInfo.FirstName,
                LastName = externalUserInfo.LastName,
                AvatarURL = externalUserInfo.Picture
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(e => e.Description)
                    .Aggregate((a, b) => $"{a}\n{b}"));
            }

            await _userManager.AddToRoleAsync(user, role ?? "Student");
            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, externalUserInfo.Email, provider));
        }
    }
}