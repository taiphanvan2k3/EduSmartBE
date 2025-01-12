using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.NotificationManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course-service/api/notifications", Order = 14)]
    [ApiController]
    [Authorize]
    public class NotificationController(IListOfNotificationsService listOfNotificationsService,
        INotificationDetailService notificationDetailService) : BaseController
    {
        private readonly IListOfNotificationsService _listOfNotificationsService = listOfNotificationsService
            ?? throw new ArgumentNullException(nameof(listOfNotificationsService));
        private readonly INotificationDetailService _notificationDetailService = notificationDetailService
            ?? throw new ArgumentNullException(nameof(notificationDetailService));

        /// <summary>
        /// Get list of my notifications
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] ParamsSearch searchCondition)
        {
            var notifications = await _listOfNotificationsService.GetMyNotifications(searchCondition);
            return Ok(notifications);
        }

        /// <summary>
        /// Get the unread status of notifications of the current user
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpGet("unread-status")]
        public async Task<IActionResult> HasUnreadNotifications()
        {
            var hasUnread = await _notificationDetailService.HasNewNotification();
            return Ok(new
            {
                newNotification = hasUnread
            });
        }

        /// <summary>
        /// Mark a notification as read
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of notification</param>
        /// <returns></returns>
        [HttpPut("{id}/reading-status")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var responseInfo = await _notificationDetailService.MarkNotificationAsRead(id);
            return HandleResponseInfo(responseInfo, resourceName: "readingStatus");
        }

        /// <summary>
        /// Call this API for each click on the bell icon to update the last clicked time
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        [HttpPut("bell-click-milestone")]
        public async Task<IActionResult> UpdateBellClickMilestone()
        {
            var responseInfo = await _notificationDetailService.UpdateLastClickedOnBellAsync();
            return HandleResponseInfo(responseInfo, resourceName: "bellClickMilestone");
        }

        /// <summary>
        /// Mark all notifications as read
        /// <para>Created at: 2025/01/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var responseInfo = await _listOfNotificationsService.MarkAllAsRead();
            return HandleResponseInfo(responseInfo, resourceName: "updatedNotifications");
        }
    }
}