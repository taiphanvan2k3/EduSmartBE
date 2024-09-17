using AuthService.BackgroundServices;
using AuthService.Commons;
using AuthService.Databases.Schemas;
using AuthService.Services.Auth.Schemas;
using AuthService.Services.MailSender.Schemas;
using AuthService.Services.User.Schemas;
using AuthService.Settings;
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
        /// Check login by username/email and password
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 29/08/2024</para>
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        public Task<string> SignUp(SignUpRequest signUpRequest);

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
                _logger.LogInformation("[AuthService][CheckLogin] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CheckLogin");
                throw;
            }
        }

        public async Task<string> SignUp(SignUpRequest signUpRequest)
        {
            try
            {
                _logger.LogInformation("[AuthService][SignUp] Start");
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
                    return result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}\n{b}");
                }

                if (!string.IsNullOrEmpty(signUpRequest.Role))
                {
                    await _userManager.AddToRoleAsync(user, signUpRequest.Role);
                }

                string callbackUrl = await GenerateEmailConfirmationTokenAsync(user);

                await SendMailConfirmAccount(user.Email, user.UserName, callbackUrl);

                _logger.LogInformation("[AuthService][SignUp] End");
                return "";
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
    }
}