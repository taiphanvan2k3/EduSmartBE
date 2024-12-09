using PaymentService.EventData;
using PaymentService.Services.StorageInfos;

namespace PaymentService.EventProcessing
{
    public partial class EventProcessor : IEventProcessor
    {
        private async Task CreateStorageInfo(StorageInfoCreatedEventData storageInfo)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var storageInfoDetailService = scope.ServiceProvider.GetRequiredService<IStorageInfoDetailService>();
                var responseInfo = await storageInfoDetailService.CreateDefaultStorageInfo(storageInfo.UserId);
                _logger.LogInformation("--> StorageInfo created with result: {message}", responseInfo.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "--> Could not add user to database: {message}", e.Message);
            }
        }
    }
}