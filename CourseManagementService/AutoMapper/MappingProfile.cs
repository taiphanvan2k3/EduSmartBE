using AutoMapper;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using TblCourse = CourseManagementService.Database.Schemas.Course;

namespace CourseManagementService.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CourseCreateDto, TblCourse>();
            CreateMap<TblCourse, CourseDto>();
        }
    }
}