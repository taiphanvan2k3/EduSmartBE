using AuthService.Commons;

namespace AuthService.Services.Account
{
    public interface IAccountDetailService
    {
        Task<ResponseInfo> ActivateUser(int userId, bool isActive);
    }

    public class AccountDetailService(IServiceProvider serviceProvider,
        ILogger<AccountDetailService> logger)
        : BaseService(serviceProvider, logger), IAccountDetailService
    {
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

                LogInfo("End", method);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}