using AutoMapper;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Database.Schemas.NotificationEntities;
using CourseManagementService.Enumerations;
using CourseManagementService.Hubs;
using CourseManagementService.Services.NotificationManagement.Schemas;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TblUserNotification = CourseManagementService.Database.Schemas.NotificationEntities.UserNotification;

namespace CourseManagementService.Services.NotificationManagement
{
    public interface INotificationDetailService
    {
        /// <summary>
        /// Handle the creation of a notification based on the notification type
        /// <para>Created at: 2024/12/26</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="notificationCreateDto">Notification data</param>
        /// <returns></returns>
        public Task CreateNotificationAsync(NotificationCreateDto notificationCreateDto);

        /// <summary>
        /// Save the notification to the database and send it to the receiver
        /// </summary>
        /// <returns></returns>
        public Task NotifyUsersInCourseByBatch(NotificationByBatchData notificationByBatchData);
    }

    public class NotificationDetailService(IServiceProvider serviceProvider, ILogger<NotificationDetailService> logger)
        : BaseService(serviceProvider, logger), INotificationDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));
        private readonly IHubContext<NotificationHub> _hubContext = serviceProvider.GetRequiredService<IHubContext<NotificationHub>>()
            ?? throw new ArgumentNullException(ServiceInjectionError(nameof(IHubContext<NotificationHub>)));
        private readonly INotificationQueue<NotificationByBatchData> _notificationQueue = serviceProvider.GetRequiredService<INotificationQueue<NotificationByBatchData>>()
            ?? throw new ArgumentNullException(ServiceInjectionError(nameof(INotificationQueue<NotificationByBatchData>)));

        public async Task CreateNotificationAsync(NotificationCreateDto notificationCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                switch (notificationCreateDto.Type)
                {
                    case NotificationType.Mention:
                        await CreateMentionNotification(notificationCreateDto);
                        break;
                    case NotificationType.CommentInDiscussion:
                        await CreateCommentInDiscussionNotification(notificationCreateDto);
                        break;
                    case NotificationType.Reaction:
                        await CreateReactionNotification(notificationCreateDto);
                        break;
                    case NotificationType.NewLesson:
                        await CreateNewLessonNotification(notificationCreateDto);
                        break;
                }

                await _context.SaveChangesAsync();
                LogInfo("End", methodName);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        private async Task CreateCommentInDiscussionNotification(NotificationCreateDto notificationCreateDto)
        {
            await CreateMentionNotification(notificationCreateDto);
        }

        public async Task NotifyUsersInCourseByBatch(NotificationByBatchData notificationByBatchData)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                _logger.LogInformation("Batch Id: {BatchId} is sending notifications to {UserCount} users",
                    notificationByBatchData.BatchId, notificationByBatchData.UserIdsInBatch.Count);

                var metaData = JsonConvert.SerializeObject(new
                {
                    LessonName = notificationByBatchData.NotificationCreateDto.MetaData["LessonName"]
                });

                var userNotifications = notificationByBatchData.UserIdsInBatch.Select(userId =>
                {
                    var userNotification = _mapper.Map<TblUserNotification>(notificationByBatchData.NotificationCreateDto);
                    userNotification.ReceiverId = userId;
                    userNotification.MetaData = metaData;

                    return userNotification;
                });

                await _context.UserNotifications.AddRangeAsync(userNotifications);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.Users(notificationByBatchData.UserIdsInBatch.Select(id => id.ToString()).ToList())
                    .SendAsync("ReceiveNotification", userNotifications);
                LogInfo("Sent notification to users", methodName);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        private async Task CreateMentionNotification(NotificationCreateDto notificationCreateDto)
        {
            var userNotification = _mapper.Map<TblUserNotification>(notificationCreateDto);
            var discussionId = notificationCreateDto.MetaData["DiscussionId"];
            var lessonId = notificationCreateDto.MetaData["LessonId"];

            userNotification.MetaData = JsonConvert.SerializeObject(new
            {
                DiscussionId = discussionId,
                LessonId = lessonId
            });

            await _context.UserNotifications.AddAsync(userNotification);
            await _hubContext.Clients.User(notificationCreateDto.ReceiverId.ToString())
                .SendAsync("ReceiveNotification", userNotification);
        }

        private async Task CreateReactionNotification(NotificationCreateDto notificationCreateDto)
        {
            var userNotification = _mapper.Map<TblUserNotification>(notificationCreateDto);

            var reactionCount = await _context.Reactions
                .Where(r => r.CommentId == Guid.Parse(notificationCreateDto.RelatedEntityId) && r.UserId != notificationCreateDto.SenderInfo.Id)
                .CountAsync();

            var discussionId = notificationCreateDto.MetaData["DiscussionId"];
            var lessonId = notificationCreateDto.MetaData["LessonId"];

            userNotification.MetaData = JsonConvert.SerializeObject(new
            {
                DiscussionId = discussionId,
                LessonId = lessonId,
                OtherReactionsCount = reactionCount
            });

            await _context.UserNotifications.AddAsync(userNotification);
            await _hubContext.Clients.User(notificationCreateDto.ReceiverId.ToString())
                .SendAsync("ReceiveNotification", userNotification);
        }

        private async Task CreateNewLessonNotification(NotificationCreateDto notificationCreateDto)
        {
            var courseInfo = await _context.Courses
                .Where(c => c.Id == notificationCreateDto.CourseId)
                .Select(c => new
                {
                    c.Name,
                    c.PreviewVideoURL,
                    UserIdsInCourse = c.Enrollments
                        .Where(ce => !ce.LeaveDate.HasValue)
                        .Select(ce => ce.StudentId)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (courseInfo == null)
            {
                LogError(GetActualAsyncMethodName(), "Course not found");
                return;
            }

            notificationCreateDto.MetaData.Add("CourseName", courseInfo.Name);
            notificationCreateDto.SenderInfo = new SenderInfo()
            {
                Id = 0,
                FullName = courseInfo.Name,
                AvatarURL = courseInfo.PreviewVideoURL,
                IsSystem = true
            };

            // Chia nhỏ thành các batch để gửi notification
            var batchSize = 5;
            var batchCount = (int)Math.Ceiling(courseInfo.UserIdsInCourse.Count / (double)batchSize);
            var batchId = Guid.NewGuid().ToString();

            // TODO: Optimize bằng cách sử dụng Parallel.ForEach
            for (int i = 0; i < batchCount; i++)
            {
                var batch = courseInfo.UserIdsInCourse.Skip(i * batchSize).Take(batchSize).ToList();
                var notificationByBatchData = new NotificationByBatchData
                {
                    BatchId = $"{batchId}_{i + 1}",
                    UserIdsInBatch = batch,
                    NotificationCreateDto = notificationCreateDto
                };

                await _notificationQueue.EnqueueAsync(notificationByBatchData);
            }
        }
    }
}