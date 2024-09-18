using AutoMapper;
using UserService.Databases.Schemas;
using UserService.Dtos;
using UserService.Services.Users.Schemas;

namespace UserService.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserPublishedDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
                .ForMember(dest => dest.LastLogout, opt => opt.Ignore())
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => DateTime.ParseExact(src.CreateAt, "yyyy-MM-dd HH:mm:ss", null)));
            CreateMap<User, UserDto>();
            CreateMap<UserPublishedDto, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => DateTime.ParseExact(src.CreateAt, "yyyy-MM-dd HH:mm:ss", null)))
                .ForMember(dest => dest.LastLogout, opt => opt.Ignore())
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        }
    }
}