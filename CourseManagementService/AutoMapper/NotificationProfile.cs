using AutoMapper;
using CourseManagementService.Services.NotificationManagement.Schemas;
using TblUserNotification = CourseManagementService.Database.Schemas.NotificationEntities.UserNotification;

namespace CourseManagementService.AutoMapper
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationCreateDto, TblUserNotification>()
                .ForMember(d => d.MetaData, opt => opt.Ignore());
        }
    }
}