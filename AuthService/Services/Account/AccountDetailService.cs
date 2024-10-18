using AuthService.AsyncDataServices;
using AuthService.BackgroundServices;
using AuthService.Commons;
using AuthService.Commons.Helpers;
using AuthService.Databases.Schemas;
using AuthService.Services.Account.Schemas;
using AuthService.Services.Cache;
using AuthService.Services.MailSender.Schemas;
using AuthService.Services.Otp.Schemas.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.Account
{
    public interface IAccountDetailService
    {
        /// <summary>
        /// Active/inactive an user
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 12/10/2024</para>
        /// </summary>
        /// <returns></returns>
        Task<ResponseInfo> ActivateUser(int userId, bool isActive);

        /// <summary>
        /// Change password
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 16/10/2024</para>
        /// </summary>
        /// <returns></returns>
        Task<ResponseInfo> ChangePassword(ChangePasswordContent changePasswordContent);

        /// <summary>
        /// Send email to delete account
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 16/10/2024</para>
        /// </summary>
        /// <returns></returns>
        Task<ResponseInfo> SendEmailDeleteAccount();

        /// <summary>
        /// Delete account
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 16/10/2024</para>
        /// </summary>
        /// <returns></returns>
        Task<ResponseInfo> DeleteAccount(OtpWrapperBase deleteAccountContent);
    }

    public class AccountDetailService(IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountDetailService> logger)
        : BaseService(serviceProvider, logger), IAccountDetailService
    {
        private readonly IMessagePublisher _messageBusPublisher = serviceProvider.GetRequiredService<IMessagePublisher>()
            ?? throw new InvalidDataException(ServiceInjectionError("IMessagePublisher"));
        private readonly UserManager<ApplicationUser> _userManager = userManager
            ?? throw new InvalidDataException(ServiceInjectionError("UserManager<ApplicationUser>"));
        private readonly MailProducer _mailProducer = serviceProvider.GetRequiredService<MailProducer>()
            ?? throw new InvalidCastException(ServiceInjectionError("MailProducer"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));

        public async Task<ResponseInfo> ActivateUser(int userId, bool isActive)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    responseInfo.Error = "NotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";

                    LogError(responseInfo.Message, method);
                    return responseInfo;
                }

                user.IsActive = isActive;
                await _context.SaveChangesAsync();

                _messageBusPublisher.PublishMessage(EventTypes.ActiveStatusUpdated, new
                {
                    UserId = user.Id,
                    IsActive = isActive
                });

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> ChangePassword(ChangePasswordContent changePasswordContent)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var user = await _userManager.FindByNameAsync(_appStateService.UserInfo.UserName);
                if (user == null)
                {
                    responseInfo.Error = "NotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";
                    return responseInfo;
                }

                var isChangePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordContent.CurrentPassword,
                    changePasswordContent.NewPassword);
                if (!isChangePasswordResult.Succeeded)
                {
                    responseInfo.Error = "ChangePasswordFailed";
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = isChangePasswordResult.Errors.FirstOrDefault()?.Description;
                    return responseInfo;
                }

                responseInfo.Message = "Change password successfully";

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> SendEmailDeleteAccount()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var user = await _userManager.FindByNameAsync(_appStateService.UserInfo.UserName);
                if (user == null)
                {
                    responseInfo.Error = "NotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";
                    return responseInfo;
                }

                var otpCode = Utils.GenerateOtp();
                var mailBody = new OtpVerificationMailBody()
                {
                    Subject = "Delete account verification",
                    ToUserName = user.UserName,
                    ToEmail = user.Email,
                    OtpCode = otpCode,
                    ActionName = OtpVerificationType.ConfirmDeleteAccount
                };

                await _mailProducer.EnqueueMailAsync(mailBody);

                var cacheKey = CacheKeyManager.GetConfirmDeleteAccountKey(user.Email);
                var otpData = new OtpWrapperBase()
                {
                    OtpCode = otpCode
                };

                var isOtpSaved = _cacheService.SetData(cacheKey, otpData, DateTimeOffset.Now.AddMinutes(2));
                if (!isOtpSaved)
                {
                    responseInfo.Error = "SaveOtpFailed";
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                    responseInfo.Message = "Save OTP failed";
                    return responseInfo;
                }

                responseInfo.Message = "Send email delete account successfully";
                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteAccount(OtpWrapperBase deleteAccountContent)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[AuthService][{Method}] Start", method);
                var responseInfo = new ResponseInfo();

                var user = await _userManager.FindByNameAsync(_appStateService.UserInfo.UserName);
                if (user == null)
                {
                    responseInfo.Error = "UserNotFound";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "User not found";
                    return responseInfo;
                }

                var cacheKey = CacheKeyManager.GetConfirmDeleteAccountKey(user.Email);
                var otpData = _cacheService.GetData<OtpWrapperBase>(cacheKey);

                if (otpData == null || otpData.OtpCode != deleteAccountContent.OtpCode)
                {
                    responseInfo.Error = otpData == null ? "OtpNotFound" : "InvalidOtp";
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = otpData == null ? "OTP not found or expired" : "Invalid OTP";
                    return responseInfo;
                }

                var isDeleteUserTask = _userManager.DeleteAsync(user);
                var deleteCoursePermissionTask = _context.CoursePermissions
                    .Where(cp => cp.AssistantId == user.Id).ExecuteDeleteAsync();
                await Task.WhenAll(isDeleteUserTask, deleteCoursePermissionTask);

                // TODO: Delete all user's data in other services

                if (!isDeleteUserTask.Result.Succeeded)
                {
                    responseInfo.Error = "DeleteUserFailed";
                    responseInfo.StatusCode = StatusCodes.Status500InternalServerError;
                    responseInfo.Message = isDeleteUserTask.Result.Errors.FirstOrDefault()?.Description;
                    return responseInfo;
                }

                responseInfo.Message = "Delete account successfully";
                _logger.LogInformation("[AuthService][{Method}] End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthService][{Method}][{Error}]", method, e.InnerException?.Message ?? e.Message);
                throw;
            }
        }
    }
}