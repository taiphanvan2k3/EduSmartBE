using AutoMapper;
using CourseManagementService.Services.RatingManagement.Schemas;
using TblLessonRating = CourseManagementService.Database.Schemas.LessonRating;
using TblCourseRating = CourseManagementService.Database.Schemas.CourseRating;
using CourseManagementService.Common.Schemas;

namespace CourseManagementService.AutoMapper
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMapForCourseRating();
            CreateMapForLessonRating();
        }

        private void CreateMapForCourseRating()
        {
            CreateMap<CourseRatingCreateDto, TblCourseRating>();
            CreateMap<TblCourseRating, CourseRatingDetail>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDetail()
                {
                    Id = (int)src.UserId
                }));
        }

        private void CreateMapForLessonRating()
        {
            CreateMap<LessonRatingCreateDto, TblLessonRating>();
        }
    }
}