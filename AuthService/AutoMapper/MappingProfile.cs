using AuthService.Databases.Schemas;
using AuthService.Services.Permission.Schemas;
using AuthService.Services.Permission.Schemas.Function;
using AuthService.Services.Permission.Schemas.Screen;
using AutoMapper;

namespace AuthService.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ScreenCreateDto, Screen>();
            CreateMap<ScreenUpdateDto, Screen>();
            CreateMap<FunctionCreateDto, Function>();
            CreateMap<FunctionUpdateDto, Function>();
            CreateMap<PermissionStatus, Permission>();
        }
    }
}