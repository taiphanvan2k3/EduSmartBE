using AuthService.AsyncDataServices;
using AuthService.BackgroundServices;
using AuthService.Commons;
using AuthService.Commons.Helpers;
using AuthService.Databases.Schemas;
using AuthService.Enumerations;
using AuthService.Extensions;
using AuthService.Services.Account.Schemas;
using AuthService.Services.Auth.Schemas;
using AuthService.Services.Cache;
using AuthService.Services.MailSender.Schemas;
using AuthService.Services.Otp.Schemas;
using AuthService.Services.Otp.Schemas.Wrappers;
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
        /// <param name="signUpRequest"></param>
        /// <returns></returns>
        public Task<ResponseInfo> SignUp(SignUpRequest signUpRequest);

        /// <summary>
        /// Handle confirm account
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 30/08/2024</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> ConfirmAccount(string token, string userId);

        /// <summary>
        /// Handle forgot password
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 08/10/2024</para>
        /// </summary>
        /// <param name="forgotPasswordContent">Data for forgot password</param>
        /// <returns></returns>
        public Task<ResponseInfo> ForgotPassword(ForgotPasswordContent forgotPasswordContent);

        /// <summary>
        /// Check OTP code
        /// </summary>
        /// <param name="otpType">Type of OTP</param>
        /// <param name="email">Email of user</param>
        /// <param name="otpCode">OTP code</param>
        /// <returns></returns>
        public ResponseInfo ValidateOtpCode(OtpType otpType, string email, string otpCode);

        /// <summary>
        /// Handle reset password after forgot password
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 08/10/2024</para>
        /// </summary>
        /// <param name="resetPasswordContent">Data for forgot password</param>
        /// <returns></returns>
        public Task<ResponseInfo> ResetPassword(ResetPasswordContent resetPasswordContent);
    }

    public class AuthService(IServiceProvider serviceProvider,
        ILogger<AuthService> logger)
        : BaseService(serviceProvider, logger), IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>()
            ?? throw new InvalidOperationException(ServiceInjectionError("UserManager"));
        private readonly SignInManager<ApplicationUser> _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>()
            ?? throw new InvalidOperationException(ServiceInjectionError("SignInManager"));
        private readonly MailProducer _mailProducer = serviceProvider.GetRequiredService<MailProducer>()
            ?? throw new InvalidOperationException(ServiceInjectionError("MailProducer"));
        private readonly CommonProducer _commonProducer = serviceProvider.GetRequiredService<CommonProducer>()
            ?? throw new InvalidOperationException(ServiceInjectionError("CommonProducer"));
        private readonly ServerSetting _serverSetting = serviceProvider.GetRequiredService<IOptions<ServerSetting>>().Value
            ?? throw new InvalidOperationException(ServiceInjectionError("ServerSetting"));
        private readonly ITokenService _tokenService = serviceProvider.GetRequiredService<ITokenService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("TokenService"));
        private readonly IGoogleAuthService _googleAuthService = serviceProvider.GetRequiredService<IGoogleAuthService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("GoogleAuthService"));
        private readonly IUrlHelper urlHelper = serviceProvider.GetRequiredService<IUrlHelper>()
            ?? throw new InvalidOperationException(ServiceInjectionError("UrlHelper"));
        private readonly IMessagePublisher _messageBusPublisher = serviceProvider.GetRequiredService<IMessagePublisher>()
            ?? throw new InvalidOperationException(ServiceInjectionError("MessageBusPublisher"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("CacheService"));

        public async Task<ResponseInfo> CheckLogin(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("[AuthService][CheckLogin] Start");
                var responseInfo = new ResponseInfo();

                loginRequest.Email = loginRequest.Email.Trim().ToLower();
                var user = await _userManager.FindByEmailAsync(loginRequest.Email)
                    ?? await _userManager.FindByNameAsync(loginRequest.Username);

                if (user == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Error = "InvalidAccount";
                    responseInfo.Message = "Username or email not found";

                    _logger.LogInformation("[AuthService][CheckLogin] End");
                    return responseInfo;
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginRequest.Password, isPersistent: false,
                    lockoutOnFailure: false);
                if (!result.Succeeded || !user.IsActive)
                {
                    responseInfo.Error = "InvalidAccount";
                    if (result.IsLockedOut || !user.IsActive)
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
                        responseInfo.Message = "Password is incorrect";
                    }

                    _logger.LogInformation("[AuthService][CheckLogin] End");
                    return responseInfo;
                }

                var userInfo = await ConvertAppUserToUserInfo(user);

                await GenerateTokens(userInfo, responseInfo);
                _messageBusPublisher.PublishMessage(EventTypes.UserLastLoginUpdated, new
                {
                    UserId = userInfo.Id,
                    LastLogin = DateTimeOffset.Now.ToUniversalTime()
                });

                _logger.LogInformation("[AuthService][CheckLogin] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][CheckLogin][{Error}]", e.InnerException?.Message ?? e.Message);
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

                if (payLoad == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status401Unauthorized;
                    responseInfo.Error = "InvalidToken";
                    responseInfo.Message = "Invalid token";
                    return responseInfo;
                }

                var user = await _userManager.FindByEmailAsync(payLoad.Email);
                if (user == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status409Conflict;
                    responseInfo.Error = "UserNotSignedUp";
                    responseInfo.Message = "The user has not signed up yet. Additional information is required to complete the registration.";
                    responseInfo.Data.Add("userInfo", new ExternalUserInfo()
                    {
                        Email = payLoad.Email,
                        FirstName = payLoad.GivenName,
                        LastName = payLoad.FamilyName,
                        Picture = payLoad.Picture
                    });

                    _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                    return responseInfo;
                }
                else
                {
                    // Kiểm tra xem email đã được liên kết với tài khoản google chưa
                    var isAuthenticatedByGoogle = await _userManager.FindByLoginAsync("Google", payLoad.Subject) != null;
                    if (!isAuthenticatedByGoogle)
                    {
                        // Người dùng đã đăng ký bằng email, không phải google
                        responseInfo.StatusCode = StatusCodes.Status409Conflict;
                        responseInfo.Error = "EmailInUse";
                        responseInfo.Message = "Email has been used by another method";
                        return responseInfo;
                    }
                }

                user = await _userManager.FindByEmailAsync(payLoad.Email);
                var userInfo = await ConvertAppUserToUserInfo(user);

                await GenerateTokens(userInfo, responseInfo);
                _messageBusPublisher.PublishMessage(EventTypes.UserCreated, userInfo);
                _messageBusPublisher.PublishMessage(EventTypes.UserLastLoginUpdated, new
                {
                    UserId = userInfo.Id,
                    LastLogin = DateTimeOffset.Now
                });

                _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{MethodName}][{Error}]", methodName, e.InnerException?.Message ?? e.Message);
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
                _logger.LogError(e, "[AuthService][{MethodName}][{Error}]", methodName, e.InnerException?.Message ?? e.Message);
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
                    responseInfo.StatusCode = StatusCodes.Status409Conflict;
                    responseInfo.Error = "UserNotSignedUp";
                    responseInfo.Message = "The user has not signed up yet. Additional information is required to complete the registration.";
                    responseInfo.Data.Add("userInfo", new ExternalUserInfo()
                    {
                        Email = externalLoginRequest.UserInfo.Email,
                        FirstName = externalLoginRequest.UserInfo.FirstName,
                        LastName = externalLoginRequest.UserInfo.LastName,
                        Picture = externalLoginRequest.UserInfo.Picture
                    });

                    _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                    return responseInfo;
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
                var userInfo = await ConvertAppUserToUserInfo(user);

                await GenerateTokens(userInfo, responseInfo);
                _messageBusPublisher.PublishMessage(EventTypes.UserCreated, userInfo);
                _messageBusPublisher.PublishMessage(EventTypes.UserLastLoginUpdated, new
                {
                    UserId = userInfo.Id,
                    LastLogin = DateTimeOffset.Now
                });

                _logger.LogInformation("[AuthService][{MethodName}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{MethodName}][{Error}]", methodName, e.InnerException?.Message ?? e.Message);
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
                    Email = signUpRequest.Email.ToLower(),
                    UserName = !string.IsNullOrEmpty(signUpRequest.Username)
                        ? signUpRequest.Username
                        : signUpRequest.Email.Split('@')[0],
                    FirstName = signUpRequest.FirstName,
                    LastName = signUpRequest.LastName,
                    AvatarURL = Utils.GetDefaultAvatarUrl(signUpRequest.AvatarURL, $"{signUpRequest.LastName} {signUpRequest.FirstName}", signUpRequest.Username),
                };

                var password = signUpRequest.Password;
                if (signUpRequest.Provider != ProviderType.Email)
                {
                    password = Utils.GenerateRandomPassword();
                }

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    responseInfo.StatusCode = StatusCodes.Status401Unauthorized;
                    responseInfo.Message = result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}\n{b}");
                    return responseInfo;
                }

                string role = signUpRequest.Role.GetDisplayName();
                await _userManager.AddToRoleAsync(user, role ?? "Student");

                if (!string.IsNullOrEmpty(signUpRequest.Provider))
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo(signUpRequest.Provider, signUpRequest.Email, signUpRequest.Provider));
                }

                if (signUpRequest.Provider == ProviderType.Email)
                {
                    string callbackUrl = await GenerateEmailConfirmationTokenAsync(user);
                    await SendMailConfirmAccount(user.Email, user.UserName, callbackUrl);
                }

                responseInfo.StatusCode = StatusCodes.Status201Created;
                responseInfo.Message = "Sign up successfully";
                responseInfo.Data.Add("userRegister", user);

                await PublishUserCreated(user.Id);
                _logger.LogInformation("[AuthService][SignUp] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][SignUp][{Error}]", e.InnerException?.Message ?? e.Message);
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
                    return responseInfo;
                }

                var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
                if (!confirmResult.Succeeded)
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = confirmResult.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}\n{b}");
                    return responseInfo;
                }

                _messageBusPublisher.PublishMessage(EventTypes.ActiveStatusUpdated, new
                {
                    UserId = user.Id,
                    IsActive = true
                });

                user.IsActive = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("[AuthService][ConfirmAccount] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][ConfirmAccount][{Error}]", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> ForgotPassword(ForgotPasswordContent forgotPasswordContent)
        {
            try
            {
                _logger.LogInformation("[AuthService][ForgotPassword] Start");
                var responseInfo = new ResponseInfo();

                forgotPasswordContent.Email = forgotPasswordContent.Email.Trim().ToLower();
                var user = await _userManager.FindByEmailAsync(forgotPasswordContent.Email);
                if (user == null)
                {
                    responseInfo.Error = "UserNotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";
                    return responseInfo;
                }

                string otp = Utils.GenerateOtp();
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var otpData = new ResetPasswordWrapper
                {
                    OtpCode = otp,
                    ResetPasswordToken = token,
                    OtpExpiry = DateTimeOffset.Now.AddMinutes(2)
                };

                // Lưu mã OTP vào cache
                var cacheKey = CacheKeyManager.GetResetPasswordKey(user.Email);
                var isOtpSaved = _cacheService.SetData(cacheKey, otpData, otpData.OtpExpiry);
                if (!isOtpSaved)
                {
                    responseInfo.Error = "SaveOtpFailed";
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                    responseInfo.Message = "Save OTP failed";
                    return responseInfo;
                }

                var mailBody = new OtpVerificationMailBody()
                {
                    Subject = "Reset password",
                    ToEmail = user.Email,
                    ToUserName = user.UserName,
                    OtpCode = otp,
                    ActionName = OtpVerificationType.ResetPassword
                };

                await _mailProducer.EnqueueMailAsync(mailBody);
                responseInfo.Message = "Reset password link has been sent to your email";

                _logger.LogInformation("[AuthService][ForgotPassword] End");
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][ForgotPassword][{Error}]", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }

        public ResponseInfo ValidateOtpCode(OtpType otpType, string email, string otpCode)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[AuthService][{Method}] Start", method);
                var responseInfo = new ResponseInfo();

                email = email?.Trim().ToLower();
                (string cacheKey, Type otpWrapperType) = Utils.GetOtpCacheKey(email, otpType);

                var otpData = otpWrapperType switch
                {
                    Type t when t == typeof(OtpWrapperBase) => _cacheService.GetData<OtpWrapperBase>(cacheKey),
                    Type t when t == typeof(ResetPasswordWrapper) => _cacheService.GetData<ResetPasswordWrapper>(cacheKey),
                    _ => null
                };

                if (otpData == null || otpData.OtpCode != otpCode)
                {
                    responseInfo.Error = otpData == null ? "OtpNotFound" : "InvalidOtp";
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = otpData == null ? "OTP not found or expired" : "Invalid OTP";
                    return responseInfo;
                }

                // Gia hạn thời gian sống của mã OTP
                if (otpData.OtpExpiry - DateTimeOffset.Now < TimeSpan.FromMinutes(1))
                {
                    otpData.OtpExpiry = otpData.OtpExpiry.AddMinutes(1);
                }

                if (otpData is ResetPasswordWrapper resetPasswordOtpData)
                {
                    _cacheService.SetData(cacheKey, resetPasswordOtpData, otpData.OtpExpiry);
                }
                else
                {
                    _cacheService.SetData(cacheKey, otpData, otpData.OtpExpiry);
                }

                responseInfo.Message = "OTP is valid";
                _logger.LogInformation("[AuthService][{Method}] End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][CheckOtp][{Error}]", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }

        public async Task<ResponseInfo> ResetPassword(ResetPasswordContent resetPasswordContent)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[AuthService][{Method}] Start", method);
                var responseInfo = new ResponseInfo();

                resetPasswordContent.Email = resetPasswordContent.Email.Trim().ToLower();
                var user = await _userManager.FindByEmailAsync(resetPasswordContent.Email);
                if (user == null)
                {
                    responseInfo.Error = "UserNotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";
                    return responseInfo;
                }

                var cacheKey = CacheKeyManager.GetResetPasswordKey(user.Email);
                var otpData = _cacheService.GetData<ResetPasswordWrapper>(cacheKey);

                if (otpData == null || otpData.OtpCode != resetPasswordContent.OtpCode)
                {
                    responseInfo.Error = otpData == null ? "OtpNotFound" : "InvalidOtp";
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = otpData == null ? "OTP not found or expired" : "Invalid OTP";
                    return responseInfo;
                }

                var result = await _userManager.ResetPasswordAsync(user, otpData.ResetPasswordToken, resetPasswordContent.NewPassword);

                if (!result.Succeeded)
                {
                    responseInfo.Error = "ResetPasswordFailed";
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                    responseInfo.Message = result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}\n{b}");
                    return responseInfo;
                }

                _cacheService.RemoveData(cacheKey);
                responseInfo.Message = "Reset password successfully";
                _logger.LogInformation("[AuthService][{Method}] End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{Method}][{Error}]", method, e.InnerException?.Message ?? e.Message);
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
                Subject = "Verify your email",
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

        private async Task<UserInfo> ConvertAppUserToUserInfo(ApplicationUser user)
        {
            return new UserInfo()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarURL = user.AvatarURL,
                CreatedAt = DateTimeOffset.Now,
                Roles = [.. (await _userManager.GetRolesAsync(user))],
                IsActive = user.IsActive,
                Phone = user.Phone,
                Gender = user.Gender
            };
        }

        private async Task PublishUserCreated(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId: userId.ToString());
            var userCreatedEventData = await ConvertAppUserToUserInfo(user);

            _messageBusPublisher.PublishMessage(EventTypes.UserCreated, userCreatedEventData);
        }
    }
}