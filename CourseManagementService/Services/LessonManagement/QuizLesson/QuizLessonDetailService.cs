using CourseManagementService.Common;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.LessonManagement.QuizLesson.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase;
using TblLesson = CourseManagementService.Database.Schemas.Lesson;
using TblQuizAnswer = CourseManagementService.Database.Schemas.QuizAnswer;
using TblQuizLesson = CourseManagementService.Database.Schemas.QuizLesson;
using CourseManagementService.Enumerations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using CourseManagementService.Services.NotificationManagement.Schemas;

namespace CourseManagementService.Services.LessonManagement.QuizLesson
{
    public interface IQuizLessonDetailService
    {
        /// <summary>
        /// Get quiz lesson detail
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/12</para> 
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetQuizLessonDetail(Guid lessonId);

        /// <summary>
        /// Create quiz lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/12</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CreateQuizLesson(QuizLessonCreateDto quizLessonCreateDto);

        /// <summary>
        /// Update quiz lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/12</para>
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <param name="quizLessonUpdateDto">Data to update</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateQuizLesson(Guid lessonId, QuizLessonUpdateDto quizLessonUpdateDto);

        /// <summary>
        /// Delete quiz lesson
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/11/12</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteQuizLesson(Guid lessonId);
    }

    public class QuizLessonDetailService(IServiceProvider serviceProvider, ILogger<QuizLessonDetailService> logger)
        : BaseService(serviceProvider, logger), IQuizLessonDetailService
    {
        private readonly IChapterDetailService _chapterDetailService = serviceProvider.GetService<IChapterDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IChapterDetailService)));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly IMapper _mapper = serviceProvider.GetService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));

        public async Task<ResponseInfo> GetQuizLessonDetail(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var currentUser = GetCurrentUser();
                var courseId = await _lessonBaseDetailService.GetCourseIdBelongToLesson(lessonId);
                if (!await _lessonBaseDetailService.CanAccessCourseMaterial(courseId, currentUser.UserId))
                {
                    responseInfo.Error = "Forbidden";
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You do not have permission to access this course";
                    return responseInfo;
                }

                var lessonDto = await _context.Lessons
                    .AsNoTracking()
                    .Where(l => l.Id == lessonId)
                    .ProjectTo<QuizLessonDetail>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (lessonDto == null)
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                (LessonInfoBase previousLesson, LessonInfoBase nextLesson) = await _lessonBaseDetailService.GetPreviousAndNextLessonId(courseId,
                    lessonDto.ChapterOrder, lessonDto.LessonOrder);

                lessonDto.PreviousLesson = previousLesson;
                lessonDto.NextLesson = nextLesson;

                responseInfo.Data.Add("lesson", lessonDto);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateQuizLesson(QuizLessonCreateDto quizLessonCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var isExistChapterResponse = await _chapterDetailService.IsExistingChapter(quizLessonCreateDto.ChapterId);
                if (!isExistChapterResponse.IsSuccess)
                {
                    return isExistChapterResponse;
                }

                var currentUser = GetCurrentUser();
                Guid courseId = await _chapterDetailService.GetCourseIdBelongToChapter(quizLessonCreateDto.ChapterId);
                if (!await _lessonBaseDetailService.CanModifyCourseMaterial(courseId, currentUser.UserId))
                {
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You do not have permission to create lesson in this course";
                    return responseInfo;
                }

                var currentLessonOrder = await _context.Lessons
                    .Where(l => l.ChapterId == quizLessonCreateDto.ChapterId)
                    .MaxAsync(l => (int?)l.Order) ?? 0;

                var lessonEntity = new TblLesson()
                {
                    Title = quizLessonCreateDto.Title,
                    Description = null,
                    ChapterId = quizLessonCreateDto.ChapterId,
                    IsPublished = quizLessonCreateDto.IsPublished,
                    IsCommentAllowed = quizLessonCreateDto.IsCommentAllowed,
                    IsRatingAllowed = quizLessonCreateDto.IsRatingAllowed,
                    Order = currentLessonOrder + 1,
                    LessonType = LessonType.Quiz,
                    CreatedBy = currentUser.UserId,
                    PublishedAt = quizLessonCreateDto.IsPublished ? DateTimeOffset.UtcNow : null,
                    QuizLesson = new TblQuizLesson()
                    {
                        Question = quizLessonCreateDto.Question,
                        IsMultipleChoice = quizLessonCreateDto.IsMultipleChoice,
                        Answers = quizLessonCreateDto.Answers.Select(a => new TblQuizAnswer()
                        {
                            Answer = a.Answer,
                            IsCorrect = a.IsCorrect,
                            Explanation = a.Explanation
                        }).ToList()
                    },
                    DifficultyLevel = quizLessonCreateDto.DifficultyLevel
                };

                await _context.Lessons.AddAsync(lessonEntity);
                await _context.SaveChangesAsync();

                var lessonDto = _mapper.Map<QuizLessonDetail>(lessonEntity);
                responseInfo.Data.Add("lesson", lessonDto);

                await _lessonBaseDetailService.ClearCourseDetailCache(courseId: isExistChapterResponse.Data["courseId"]);
                await _lessonBaseDetailService.NotifyWhenLessonAdded(new LessonAddedNotificationData()
                {
                    CourseId = isExistChapterResponse.Data["courseId"],
                    LessonId = lessonEntity.Id,
                    LessonName = lessonEntity.Title
                });

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateQuizLesson(Guid lessonId, QuizLessonUpdateDto quizLessonUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                if (!await _lessonBaseDetailService.IsExistLesson(lessonId))
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                var isExistChapterResponse = await _chapterDetailService.IsExistingChapter(quizLessonUpdateDto.ChapterId);
                if (!isExistChapterResponse.IsSuccess)
                {
                    return isExistChapterResponse;
                }

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanModifyLessonMaterial(lessonId, currentUser.UserId))
                {
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You do not have permission to update this lesson";
                    return responseInfo;
                }

                var lessonEntity = await _context.Lessons
                    .Include(l => l.QuizLesson)
                        .ThenInclude(ql => ql.Answers)
                    .FirstOrDefaultAsync(l => l.Id == lessonId);

                lessonEntity.Title = quizLessonUpdateDto.Title;
                lessonEntity.ChapterId = quizLessonUpdateDto.ChapterId;

                lessonEntity.QuizLesson.Question = quizLessonUpdateDto.Question;
                lessonEntity.QuizLesson.IsMultipleChoice = quizLessonUpdateDto.IsMultipleChoice;

                if (quizLessonUpdateDto.IsPublished && lessonEntity.PublishedAt == null)
                {
                    lessonEntity.PublishedAt = DateTimeOffset.UtcNow;
                }
                else if (!quizLessonUpdateDto.IsPublished)
                {
                    lessonEntity.PublishedAt = null;
                }

                // Check xem answer có hợp lệ không
                var oldAnswerIdsFromInput = quizLessonUpdateDto.Answers
                    .Where(a => a.Id != 0)
                    .Select(a => a.Id).ToList();
                var oldAnswerIdsFromDB = lessonEntity.QuizLesson.Answers.Select(a => a.Id).ToList();

                if (oldAnswerIdsFromInput.Except(oldAnswerIdsFromDB).Any())
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = "Some answer ids are invalid";
                    return responseInfo;
                }

                var answerEntitiesFromInput = quizLessonUpdateDto.Answers.Select(a => new TblQuizAnswer()
                {
                    Id = a.Id,
                    Answer = a.Answer,
                    IsCorrect = a.IsCorrect,
                    Explanation = a.Explanation,
                    QuizLessonId = lessonEntity.QuizLesson.Id
                }).ToList();

                var answerEntitiesFromDB = lessonEntity.QuizLesson.Answers;
                var deletedAnswers = answerEntitiesFromDB
                    .Except(answerEntitiesFromInput, new CommonComparer<TblQuizAnswer, long>(answer => answer.Id))
                    .ToList();

                var insertedAnswers = answerEntitiesFromInput
                    .Except(answerEntitiesFromDB, new CommonComparer<TblQuizAnswer, long>(answer => answer.Id))
                    .ToList();

                var updatedAnswers = answerEntitiesFromInput
                    .Intersect(answerEntitiesFromDB, new CommonComparer<TblQuizAnswer, long>(answer => answer.Id))
                    .ToList();

                var answerDict = answerEntitiesFromDB.ToDictionary(a => a.Id);
                await _context.QuizAnswers.AddRangeAsync(insertedAnswers);
                _context.QuizAnswers.RemoveRange(deletedAnswers);

                foreach (var updatedAnswer in updatedAnswers)
                {
                    if (answerDict.TryGetValue(updatedAnswer.Id, out var answerEntity))
                    {
                        answerEntity.Answer = updatedAnswer.Answer;
                        answerEntity.IsCorrect = updatedAnswer.IsCorrect;
                        answerEntity.Explanation = updatedAnswer.Explanation;
                    }
                }

                await _context.SaveChangesAsync();
                LogInfo("End", methodName);

                responseInfo.Data.Add("lesson", _mapper.Map<QuizLessonDetail>(lessonEntity));
                await _lessonBaseDetailService.ClearCourseDetailCache(courseId: isExistChapterResponse.Data["courseId"]);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteQuizLesson(Guid lessonId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                if (!await _lessonBaseDetailService.IsExistLesson(lessonId))
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanModifyLessonMaterial(lessonId, currentUser.UserId))
                {
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    responseInfo.Message = "You do not have permission to delete this lesson";
                    return responseInfo;
                }

                int deletedRecordCount = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .ExecuteDeleteAsync();

                if (deletedRecordCount == 0)
                {
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    responseInfo.Message = "Lesson not found";
                    return responseInfo;
                }

                LogInfo("End", methodName);
                responseInfo.Data.Add("id", lessonId);
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