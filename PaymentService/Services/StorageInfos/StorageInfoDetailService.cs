using PaymentService.Commons;
using TblStorageInfo = PaymentService.Databases.Schemas.StorageInfo;

namespace PaymentService.Services.StorageInfos
{
    public interface IStorageInfoDetailService
    {
        /// <summary>
        /// Create default storage info for user when user register
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/12/09</para>
        /// </summary>
        public Task<ResponseInfo> CreateDefaultStorageInfo(int userId);
    }

    public class StorageInfoDetailService(IServiceProvider serviceProvider, ILogger<StorageInfoDetailService> logger)
        : BaseService(serviceProvider, logger), IStorageInfoDetailService
    {
        public async Task<ResponseInfo> CreateDefaultStorageInfo(int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var storageInfoEntity = new TblStorageInfo()
                {
                    UserId = userId,
                    MaximumStorage = Constants.FREE_MAXIMUM_STORAGE_AMOUNT
                };

                await _context.StorageInfos.AddAsync(storageInfoEntity);
                await _context.SaveChangesAsync();

                responseInfo.StatusCode = StatusCodes.Status201Created;
                responseInfo.Message = "Create default storage info successfully";

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}