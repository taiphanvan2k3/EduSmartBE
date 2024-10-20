using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using TblCourse = CourseManagementService.Database.Schemas.Course;

namespace CourseManagementService.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CourseCreateDto, TblCourse>();
            CreateMap<TblCourse, CourseDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => new LookupDto
                {
                    Id = ((int)src.Type).ToString(),
                    Name = src.Type.ToString()
                }))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new LookupDto
                {
                    Id = src.CategoryId.ToString(),
                }))
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
        }
    }
}