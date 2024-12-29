using AutoMapper;
using CourseManagementService.Services.NotificationManagement.Schemas;
using Newtonsoft.Json;
using TblUserNotification = CourseManagementService.Database.Schemas.NotificationEntities.UserNotification;

namespace CourseManagementService.AutoMapper
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationCreateDto, TblUserNotification>()
                .ForMember(d => d.MetaData, opt => opt.Ignore());

            var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            CreateMap<TblUserNotification, NotificationDto>()
                .ForMember(d => d.RelatedEntityId, opt => opt.MapFrom(src => Guid.Parse(src.RelatedEntityId)))
                .ForMember(d => d.MetaData, opt => opt.MapFrom(src => JsonConvert.DeserializeObject(src.MetaData, jsonSerializerSettings)));
        }
    }
}