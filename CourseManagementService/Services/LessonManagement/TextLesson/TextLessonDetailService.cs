using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.Common;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using Microsoft.EntityFrameworkCore;
using TblLesson = CourseManagementService.Database.Schemas.Lesson;

namespace CourseManagementService.Services.LessonManagement.TextLesson
{
    public interface ITextLessonDetailService
    {
        /// <summary>
        /// Get text lesson detail
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para> 
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetTextLessonDetail(Guid lessonId);

        /// <summary>
        /// Create text lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CreateTextLesson(TextLessonCreateUpdateDto TextLessonCreateDto);

        /// <summary>
        /// Update text lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para>
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <param name="textLessonUpdateDto">Data to update</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateTextLesson(Guid lessonId, TextLessonCreateUpdateDto textLessonUpdateDto);

        /// <summary>
        /// Delete text lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/24</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteTextLesson(Guid lessonId);
    }

    public class TextLessonDetailService(IServiceProvider serviceProvider, ILogger<TextLessonDetailService> logger)
        : BaseService(serviceProvider, logger), ITextLessonDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetRequiredService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly IChapterDetailService _chapterDetailService = serviceProvider.GetService<IChapterDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IChapterDetailService)));

        public async Task<ResponseInfo> GetTextLessonDetail(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var currentUser = GetCurrentUser();
                var courseId = await _lessonBaseDetailService.GetCourseIdBelongToLesson(lessonId);

                if (courseId == Guid.Empty)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
                }

                if (!await _lessonBaseDetailService.CanAccessCourseMaterial(courseId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden,
                        "You do not have permission to access this course");
                }

                var lessonDetail = await _context.Lessons
                    .Where(x => x.Id == lessonId)
                    .ProjectTo<LessonDetail>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (lessonDetail == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
                }

                (LessonInfoBase previousLesson, LessonInfoBase nextLesson) = await _lessonBaseDetailService.GetPreviousAndNextLessonId(courseId,
                    lessonDetail.ChapterOrder, lessonDetail.LessonOrder);

                lessonDetail.PreviousLesson = previousLesson;
                lessonDetail.NextLesson = nextLesson;

                responseInfo.Data.Add("lesson", lessonDetail);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateTextLesson(TextLessonCreateUpdateDto textLessonCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                if (!await _chapterDetailService.IsExistingChapter(textLessonCreateDto.ChapterId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Chapter not found");
                }

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanCreateLessonInChapter(textLessonCreateDto.ChapterId,
                    currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden,
                        "You do not have permission to create lesson in this chapter");
                }

                int videoLessonOrder = await _context.Lessons
                    .Where(l => l.ChapterId == textLessonCreateDto.ChapterId)
                    .MaxAsync(l => (int?)l.Order) ?? 0;

                var lessonEntity = _mapper.Map<TblLesson>(textLessonCreateDto);
                lessonEntity.CreatedBy = currentUser.UserId;
                lessonEntity.Order = videoLessonOrder + 1;
                lessonEntity.PublishedAt = textLessonCreateDto.IsPublished ? DateTimeOffset.UtcNow : null;

                await _context.Lessons.AddAsync(lessonEntity);
                await _context.SaveChangesAsync();

                var lessonDto = _mapper.Map<LessonDetail>(lessonEntity);
                var responseInfo = new ResponseInfo();

                responseInfo.Data.Add("lesson", lessonDto);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }


        public async Task<ResponseInfo> UpdateTextLesson(Guid lessonId, TextLessonCreateUpdateDto textLessonUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                if (!await _chapterDetailService.IsExistingChapter(textLessonUpdateDto.ChapterId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Chapter not found");
                }

                var currentUser = GetCurrentUser();
                var lessonEntity = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .FirstOrDefaultAsync();

                if (lessonEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
                }

                if (!await _lessonBaseDetailService.CanModifyLessonMaterial(lessonId,
                    currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden,
                        "You do not have permission to update this lesson");
                }

                lessonEntity.Title = textLessonUpdateDto.Title;
                lessonEntity.Description = textLessonUpdateDto.Description;
                lessonEntity.ChapterId = textLessonUpdateDto.ChapterId;
                lessonEntity.DifficultyLevel = textLessonUpdateDto.DifficultyLevel;

                if (!lessonEntity.IsPublished && textLessonUpdateDto.IsPublished)
                {
                    lessonEntity.PublishedAt = DateTimeOffset.UtcNow;
                }
                else if (!textLessonUpdateDto.IsPublished)
                {
                    lessonEntity.PublishedAt = null;
                }

                lessonEntity.IsPublished = textLessonUpdateDto.IsPublished;
                lessonEntity.IsRatingAllowed = textLessonUpdateDto.IsRatingAllowed;
                lessonEntity.IsCommentAllowed = textLessonUpdateDto.IsCommentAllowed;

                await _context.SaveChangesAsync();

                var lessonDto = _mapper.Map<LessonDetail>(lessonEntity);
                var responseInfo = new ResponseInfo();

                responseInfo.Data.Add("lesson", lessonDto);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteTextLesson(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanModifyLessonMaterial(lessonId,
                    currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden,
                        "You do not have permission to update this lesson");
                }

                int deletedRowCount = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .ExecuteDeleteAsync();

                if (deletedRowCount == 0)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
                }

                LogInfo("End", methodName);

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("lesson", lessonId);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}