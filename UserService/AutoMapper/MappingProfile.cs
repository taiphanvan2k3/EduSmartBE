using AutoMapper;
using UserService.Databases.Schemas;
using UserService.EventData;
using UserService.Services.Users.Schemas;

namespace UserService.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<UserCreatedEventData, UserDto>();
        }
    }
}