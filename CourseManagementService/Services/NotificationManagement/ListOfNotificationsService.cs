using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.NotificationManagement.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.NotificationManagement
{
    public interface IListOfNotificationsService
    {
        /// <summary>
        /// Get my notifications with paging
        /// <para>Created at: 2024/12/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public Task<PaginatedList<NotificationDto>> GetMyNotifications(ParamsSearch searchCondition);

        /// <summary>
        /// Mark all notifications as read
        /// <para>Created at: 2025/01/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> MarkAllAsRead();
    }

    public class ListOfNotificationsService(IServiceProvider serviceProvider, ILogger<ListOfNotificationsService> logger)
        : BaseService(serviceProvider, logger), IListOfNotificationsService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(nameof(IMapper));
        private readonly CommonProducer _commonProducer = serviceProvider.GetRequiredService<CommonProducer>()
            ?? throw new InvalidDataException(nameof(CommonProducer));

        public async Task<PaginatedList<NotificationDto>> GetMyNotifications(ParamsSearch searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var notifications = await _context.UserNotifications
                    .Where(x => x.ReceiverId == currentUser.UserId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                LogInfo("End", methodName);
                return notifications;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> MarkAllAsRead()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                int updatedRecords = await _context.UserNotifications
                    .Where(x => x.ReceiverId == currentUser.UserId && !x.IsRead)
                    .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsRead, true));

                LogInfo("End", methodName);
                return CreateResponseInfo("updatedNotifications", updatedRecords);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}