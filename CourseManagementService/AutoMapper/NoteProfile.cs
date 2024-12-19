using AutoMapper;
using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.NoteManagement.Schemas;
using TblNote = CourseManagementService.Database.Schemas.Note;

namespace CourseManagementService.AutoMapper
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<NoteCreateDto, TblNote>();
            CreateMap<TblNote, NoteDetail>()
                .ForMember(d => d.LessonName, opt => opt.MapFrom(s => s.Lesson.Title))
                .ForMember(d => d.ChapterInfo, opt => opt.MapFrom(s => new SimpleChapterInfo()
                {
                    Order = s.Chapter.Order,
                    Name = s.Chapter.Name
                }));
        }
    }
}