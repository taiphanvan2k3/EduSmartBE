using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.CategoryManagement.Schemas;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using TblCourse = CourseManagementService.Database.Schemas.Course;

namespace CourseManagementService.AutoMapper
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseCreateDto, TblCourse>();
            CreateMap<TblCourse, CourseDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => new LookupDto
                {
                    Id = ((int)src.Type).ToString(),
                    Name = src.Type.ToString()
                }))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new CategoryDto
                {
                    Id = src.CategoryId,
                }))
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
        }
    }
}