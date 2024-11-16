using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
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
        /// <param name="isTeacher">Check if user is teacher</param>
        public Task<List<ChapterDetail>> GetListOfChaptersByCourseId(Guid courseId, bool isTeacher = false);
    }

    public class ListOfChaptersService(IServiceProvider serviceProvider, ILogger<ListOfChaptersService> logger) : BaseService(serviceProvider, logger), IListOfChaptersService
    {
        public async Task<List<ChapterDetail>> GetListOfChaptersByCourseId(Guid courseId, bool isTeacher = false)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var lessonTypesType = typeof(LessonType);

                if (!isTeacher)
                {
                    var currentUser = GetCurrentUser();
                    var isCanAccessCourse = await _context.Courses.AnyAsync(course => course.Id == courseId
                        && course.TeacherId == currentUser.UserId);
                    if (!isCanAccessCourse)
                    {
                        LogError("Cannot access course", method);
                        throw new UnauthorizedAccessException("User is not teacher");
                    }
                }

                var chapters = await _context.Chapters
                    .OrderBy(c => c.Order)
                    .Where(c => c.CourseId == courseId && (isTeacher || c.IsPublished))
                    .Select(c => new ChapterDetail()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Order = c.Order,
                        CourseId = c.CourseId,
                        Lessons = c.Lessons
                            .Where(l => isTeacher || l.IsPublished)
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
                                LessonOrder = l.Order
                            })
                            .ToList(),
                        IsPublished = c.IsPublished
                    })
                    .ToListAsync();

                foreach (var chapter in chapters)
                {
                    chapter.Duration = Utils.ConvertSecondsToDuration(chapter.Lessons.Sum(l => l.DurationInSeconds));
                }

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