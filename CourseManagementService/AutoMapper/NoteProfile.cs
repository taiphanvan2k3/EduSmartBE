using AutoMapper;
using CourseManagementService.Services.NoteManagement.Schemas;
using TblNote = CourseManagementService.Database.Schemas.Note;

namespace CourseManagementService.AutoMapper
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<NoteCreateDto, TblNote>();
            CreateMap<TblNote, NoteDetail>();
        }
    }
}