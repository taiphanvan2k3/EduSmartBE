using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using TblChapter = CourseManagementService.Database.Schemas.Chapter;

namespace CourseManagementService.AutoMapper
{
    public class ChapterProfile : Profile
    {
        public ChapterProfile()
        {
            CreateMap<TblChapter, ChapterDetail>()
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => 
                    src.Lessons == null
                        ? new List<LessonDetail>()
                        : src.Lessons.Select(lesson => new LessonDetail
                        {
                            Id = lesson.Id,
                            Title = lesson.Title,
                            Description = lesson.Description,
                            ChapterId = lesson.ChapterId,
                            LessonType = new LookupDto
                            {
                                Id = lesson.LessonType.ToString(),
                                Name = lesson.LessonType.ToString()
                            }
                        })
                        .ToList()));
        }
    }
}