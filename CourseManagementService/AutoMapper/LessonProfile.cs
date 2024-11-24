using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using CourseManagementService.Services.LessonManagement.VideoLesson.Schemas;
using TblLesson = CourseManagementService.Database.Schemas.Lesson;
using TblVideoLesson = CourseManagementService.Database.Schemas.VideoLesson;
using CourseManagementService.Services.LessonManagement.QuizLesson.Schemas;

namespace CourseManagementService.AutoMapper
{
    public class LessonProfile : Profile
    {
        public LessonProfile()
        {
            CreateMapForTextLesson();
            CreateMapForVideoLesson();
            CreateMapForQuizLesson();
        }

        private void CreateMapForTextLesson()
        {
            var lessonTypesType = typeof(LessonType);
            var difficultyLevelType = typeof(DifficultyLevel);

            CreateMap<TextLessonCreateUpdateDto, TblLesson>()
                .ForMember(dest => dest.LessonType, opt => opt.MapFrom(src => LessonType.Text))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel));

            CreateMap<TblLesson, LessonDetail>()
                .ForMember(dest => dest.LessonType, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(lessonTypesType, src.LessonType.ToString())).ToString(),
                    Name = src.LessonType.ToString()
                }))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(difficultyLevelType, src.DifficultyLevel.ToString())).ToString(),
                    Name = src.DifficultyLevel.ToString()
                }))
                .ForMember(dest => dest.LessonOrder, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.ChapterOrder, opt => opt.MapFrom(src => src.Chapter.Order));
        }

        private void CreateMapForVideoLesson()
        {
            var uploadStatusType = typeof(UploadStatus);
            var lessonTypesType = typeof(LessonType);
            var difficultyLevelType = typeof(DifficultyLevel);

            CreateMap<TblVideoLesson, VideoLessonDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LessonId))
                .ForMember(dest => dest.UploadStatus, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(uploadStatusType, src.UploadStatus.ToString())).ToString(),
                    Name = src.UploadStatus.ToString()
                }))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Lesson.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Lesson.Description))
                .ForMember(dest => dest.ChapterId, opt => opt.MapFrom(src => src.Lesson.ChapterId))
                .ForMember(dest => dest.LessonOrder, opt => opt.MapFrom(src => src.Lesson.Order))
                .ForMember(dest => dest.ChapterOrder, opt => opt.MapFrom(src => src.Lesson.Chapter.Order))
                .ForMember(dest => dest.LessonType, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(lessonTypesType, src.Lesson.LessonType.ToString())).ToString(),
                    Name = src.Lesson.LessonType.ToString()
                }))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(difficultyLevelType, src.Lesson.DifficultyLevel.ToString())).ToString(),
                    Name = src.Lesson.DifficultyLevel.ToString()
                }))
                .ForMember(dest => dest.DurationInSeconds, opt => opt.MapFrom(src => src.Lesson.DurationInSeconds))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.Lesson.IsPublished))
                .ForMember(dest => dest.IsCommentAllowed, opt => opt.MapFrom(src => src.Lesson.IsCommentAllowed))
                .ForMember(dest => dest.IsRatingAllowed, opt => opt.MapFrom(src => src.Lesson.IsRatingAllowed));
        }

        private void CreateMapForQuizLesson()
        {
            var lessonTypesType = typeof(LessonType);
            var difficultyLevelType = typeof(DifficultyLevel);

            CreateMap<TblLesson, QuizLessonDetail>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.QuizLesson.Question))
                .ForMember(dest => dest.IsMultipleChoice, opt => opt.MapFrom(src => src.QuizLesson.IsMultipleChoice))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.QuizLesson.Answers
                    .Select(answer => new QuizAnswerDetail()
                    {
                        Id = answer.Id,
                        Answer = answer.Answer,
                        IsCorrect = answer.IsCorrect,
                        Explanation = answer.Explanation
                    }))
                )
                .ForMember(dest => dest.LessonType, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(lessonTypesType, src.LessonType.ToString())).ToString(),
                    Name = src.LessonType.ToString()
                }))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => new LookupDto()
                {
                    Id = ((int)Enum.Parse(difficultyLevelType, src.DifficultyLevel.ToString())).ToString(),
                    Name = src.DifficultyLevel.ToString()
                }))
                .ForMember(dest => dest.LessonOrder, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.ChapterOrder, opt => opt.MapFrom(src => src.Chapter.Order));
        }
    }
}