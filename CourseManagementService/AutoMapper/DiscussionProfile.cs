using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using TblDiscussion = CourseManagementService.Database.Schemas.DiscussionEntities.Discussion;

namespace CourseManagementService.AutoMapper
{
    public class DiscussionProfile : Profile
    {
        public DiscussionProfile()
        {
            CreateMap<DiscussionCreateDto, TblDiscussion>();
            CreateMap<TblDiscussion, DiscussionCreateDto>();
            CreateMap<TblDiscussion, DiscussionDetail>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => new LookupDto
                {
                    Id = src.Type.Id.ToString(),
                    Name = src.Type.Name
                }))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDetail
                {
                    Id = src.CreatedBy,
                    Role = src.RoleOfUser
                }));
        }
    }
}