using CourseManagementService.Common;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.LessonManagement.TextLesson.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.ChapterManagement
{
    public interface IListOfChaptersService
    {
        /// <summary>
        /// Get list of chapters by course id
        /// <para>Created at: 2024/10/24 - ManhTD</para>
        /// <para>Modified at: 2024/11/06 - TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of course</param>
        public Task<List<ChapterDetail>> GetListOfChaptersByCourseId(Guid courseId);

        /// <summary>
        /// Get chapter by id
        /// <para>Created at: 2024/10/24 - ManhTD</para>
        /// <para>Modified at: 2024/11/06 - TaiPV</para>
        /// </summary>
        /// <param name="chapterId">Id of chapter</param>
        /// <returns></returns>
        public Task<ChapterDetail> GetChapterById(Guid chapterId);
    }

    public class ListOfChaptersService(IServiceProvider serviceProvider, ILogger<ListOfChaptersService> logger) : BaseService(serviceProvider, logger), IListOfChaptersService
    {
        public async Task<ChapterDetail> GetChapterById(Guid chapterId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var lessonTypesType = typeof(LessonType);

                var chapter = await _context.Chapters
                    .Where(c => c.Id == chapterId)
                    .Select(c => new ChapterDetail()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Order = c.Order,
                        CourseId = c.CourseId,
                        Lessons = c.Lessons
                            .Where(l => l.IsPublished)
                            .OrderBy(l => l.Order)
                            .ThenBy(l => l.IsPublished)
                            .Select(l => new LessonDetail()
                            {
                                Id = l.Id,
                                Title = l.Title,
                                ChapterId = l.ChapterId,
                                DurationInSeconds = l.DurationInSeconds,
                                LessonType = new LookupDto()
                                {
                                    Id = ((int)Enum.Parse(lessonTypesType, l.LessonType.ToString())).ToString(),
                                    Name = l.LessonType.ToString()
                                },
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                LogInfo("End", method);
                return chapter;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<List<ChapterDetail>> GetListOfChaptersByCourseId(Guid courseId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var lessonTypesType = typeof(LessonType);

                var chapters = await _context.Chapters
                    .OrderBy(c => c.Order)
                    .Where(c => c.CourseId == courseId && c.IsPublished)
                    .Select(c => new ChapterDetail()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Order = c.Order,
                        CourseId = c.CourseId,
                        Lessons = c.Lessons
                            .Where(l => l.IsPublished)
                            .OrderBy(l => l.Order)
                            .ThenBy(l => l.IsPublished)
                            .Select(l => new LessonDetail()
                            {
                                Id = l.Id,
                                Title = l.Title,
                                ChapterId = l.ChapterId,
                                DurationInSeconds = l.DurationInSeconds,
                                LessonType = new LookupDto()
                                {
                                    Id = ((int)Enum.Parse(lessonTypesType, l.LessonType.ToString())).ToString(),
                                    Name = l.LessonType.ToString()
                                },
                            })
                            .ToList()
                    })
                    .ToListAsync();

                LogInfo("End", method);
                return chapters;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}