using AutoMapper;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.NotificationManagement.Schemas;
using Newtonsoft.Json;
using TblUserNotification = CourseManagementService.Database.Schemas.NotificationEntities.UserNotification;

namespace CourseManagementService.Services.NotificationManagement
{
    public interface INotificationDetailService
    {
        public Task CreateNotificationAsync(NotificationCreateDto notificationCreateDto);
    }

    public class NotificationDetailService(IServiceProvider serviceProvider, ILogger<NotificationDetailService> logger)
        : BaseService(serviceProvider, logger), INotificationDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));

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
        }
    }
}