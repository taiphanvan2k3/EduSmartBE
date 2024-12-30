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
using TblNotificationBellClickLog = CourseManagementService.Database.Schemas.NotificationEntities.NotificationBellClickLog;
using Newtonsoft.Json.Serialization;

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
        /// <para>Created at: 2024/12/26</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <returns></returns>
        public Task NotifyUsersInCourseByBatch(NotificationByBatchData notificationByBatchData);

        /// <summary>
        /// Mark a notification as read
        /// <para>Created at: 2024/12/28</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> MarkNotificationAsRead(Guid notificationId);

        /// <summary>
        /// Check whether a user has new notifications
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> HasNewNotification();

        /// <summary>
        /// Update the last clicked on bell time of the current user
        /// <para>Created at: 2024/12/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateLastClickedOnBellAsync();
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

        public async Task<ResponseInfo> MarkNotificationAsRead(Guid notificationId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var notificationEntity = await _context.UserNotifications.FindAsync(notificationId);
                var currentUser = GetCurrentUser();

                if (notificationEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Notification not found");
                }

                if (notificationEntity.ReceiverId != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You are not authorized to access this resource.");
                }

                notificationEntity.IsRead = true;
                await _context.SaveChangesAsync();

                LogInfo("End", methodName);
                return CreateResponseInfo("readingStatus", new
                {
                    notificationId,
                    isRead = true
                });
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }


        public async Task<bool> HasNewNotification()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();
                var lastNotificationDateTime = await _context.UserNotifications
                    .Where(n => n.ReceiverId == currentUser.UserId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => n.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastNotificationDateTime != default)
                {
                    var lastClickedOnBellDateTime = await _context.NotificationBellClickLogs
                        .Where(l => l.UserId == currentUser.UserId)
                        .OrderByDescending(l => l.LastClickedAt)
                        .Select(l => l.LastClickedAt)
                        .FirstOrDefaultAsync();

                    return lastNotificationDateTime > lastClickedOnBellDateTime;
                }

                LogInfo("Start", methodName);
                return false;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

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
                        await PrepareAndSendCourseNotification(notificationCreateDto);
                        break;
                    case NotificationType.CourseDetailModification:
                        await CreateNotificationForCourseModification(notificationCreateDto);
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
                    LessonName = notificationByBatchData.NotificationCreateDto.MetaData.TryGetValue("LessonName", out var lessonName) ? lessonName : null,
                    CourseName = notificationByBatchData.NotificationCreateDto.MetaData.TryGetValue("CourseName", out var courseName) ? courseName : null
                }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
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

                // Send notification to users
                var notificationSample = userNotifications.FirstOrDefault();
                var notificationData = new
                {
                    notificationSample.SenderInfo,
                    notificationType = notificationSample.Type.ToString(),
                    notificationSample.RelatedEntityId,
                    relatedType = notificationSample.RelatedEntityType.ToString(),
                    notificationSample.CourseId,
                    notificationSample.MetaData
                };

                await _hubContext.Clients.Users(notificationByBatchData.UserIdsInBatch.Select(id => id.ToString()).ToList())
                    .SendAsync("ReceiveNotification", notificationData);

                LogInfo("Sent notification to users", methodName);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateLastClickedOnBellAsync()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var existRecord = await _context.NotificationBellClickLogs.FirstOrDefaultAsync(l => l.UserId == currentUser.UserId);

                if (existRecord != null)
                {
                    existRecord.LastClickedAt = DateTimeOffset.UtcNow;
                }
                else
                {
                    var newRecord = new TblNotificationBellClickLog
                    {
                        UserId = currentUser.UserId,
                        LastClickedAt = DateTime.UtcNow
                    };

                    await _context.NotificationBellClickLogs.AddAsync(newRecord);
                }

                await _context.SaveChangesAsync();

                LogInfo("End", methodName);
                return CreateResponseInfo("bellClickMilestone", new
                {
                    LastClickedAt = DateTimeOffset.UtcNow
                });
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

            var discussionAndLessonInfo = await _context.Discussions
                .Where(d => d.Id == Guid.Parse(discussionId))
                .Select(d => new
                {
                    DiscussionTitle = d.Title,
                    LessonTitle = d.Lesson.Title
                })
                .FirstOrDefaultAsync();

            if (discussionAndLessonInfo == null)
            {
                LogError(GetActualAsyncMethodName(), "Discussion not found");
                return;
            }

            // Serialize metadata camelCase
            userNotification.MetaData = JsonConvert.SerializeObject(new
            {
                DiscussionId = discussionId,
                LessonId = lessonId,
                discussionAndLessonInfo.DiscussionTitle,
                discussionAndLessonInfo.LessonTitle
            }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            await _context.UserNotifications.AddAsync(userNotification);
            await _hubContext.Clients.User(notificationCreateDto.ReceiverId.ToString())
                .SendAsync("ReceiveNotification", ConvertEntityToSignalRData(userNotification));
        }

        private async Task CreateReactionNotification(NotificationCreateDto notificationCreateDto)
        {
            var userNotification = _mapper.Map<TblUserNotification>(notificationCreateDto);

            var reactionCount = await _context.Reactions
                .Where(r => r.CommentId == Guid.Parse(notificationCreateDto.RelatedEntityId) && r.UserId != notificationCreateDto.SenderInfo.Id)
                .CountAsync();

            var discussionAndLessonInfo = await _context.Discussions
                .Where(d => d.Id == Guid.Parse(notificationCreateDto.MetaData["DiscussionId"]))
                .Select(d => new
                {
                    DiscussionTitle = d.Title,
                    LessonTitle = d.Lesson.Title
                })
                .FirstOrDefaultAsync();

            if (discussionAndLessonInfo == null)
            {
                LogError(GetActualAsyncMethodName(), "Discussion not found");
                return;
            }

            var discussionId = notificationCreateDto.MetaData["DiscussionId"];
            var lessonId = notificationCreateDto.MetaData["LessonId"];

            userNotification.MetaData = JsonConvert.SerializeObject(new
            {
                DiscussionId = discussionId,
                LessonId = lessonId,
                discussionAndLessonInfo.DiscussionTitle,
                discussionAndLessonInfo.LessonTitle,
                OtherReactionsCount = reactionCount
            }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            await _context.UserNotifications.AddAsync(userNotification);
            await _hubContext.Clients.User(notificationCreateDto.ReceiverId.ToString())
                .SendAsync("ReceiveNotification", ConvertEntityToSignalRData(userNotification));
        }

        private async Task CreateNotificationForCourseModification(NotificationCreateDto notificationCreateDto)
        {
            await PrepareAndSendCourseNotification(notificationCreateDto);
        }

        private async Task PrepareAndSendCourseNotification(NotificationCreateDto notificationCreateDto)
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
                Username = "System",
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

        private static NotificationSignalRData ConvertEntityToSignalRData(TblUserNotification notification)
        {
            return new NotificationSignalRData
            {
                SenderInfo = notification.SenderInfo,
                NotificationType = notification.Type.ToString(),
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType.ToString(),
                CourseId = notification.CourseId,
                MetaData = notification.MetaData
            };
        }
    }
}