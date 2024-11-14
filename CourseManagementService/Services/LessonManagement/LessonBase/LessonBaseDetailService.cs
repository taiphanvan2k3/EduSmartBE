using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.LessonManagement.LessonBase
{
    public interface ILessonBaseDetailService
    {
        /// <summary>
        /// Check the existence of the lesson
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsExistLesson(Guid lessonId);

        /// <summary>
        /// Check if the user can access the course material
        /// <para>Created at: 2024/11/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> CanAccessCourseMaterial(Guid courseId, int userId);

        /// <summary>
        /// Check if the user can access the lesson material
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> CanAccessLessonMaterial(Guid lessonId, int userId);

        /// <summary>
        /// Check if the user can modify the course material (create, update, delete)
        /// Only the teacher of the course can modify the course material
        /// <para>Created at: 2024/11/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<bool> CanModifyCourseMaterial(Guid courseId, int userId);

        /// <summary>
        /// Check if the user can modify the lesson material (create, update, delete)
        /// Only the teacher of the course or the student who has enrolled in the course can modify the lesson material
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> CanModifyLessonMaterial(Guid lessonId, int userId);

        /// <summary>
        /// Get the list of lesson ids that the user has learned in the course
        /// <para>Created at: 2024/11/06</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public Task<List<LessonTrackingDetail>> GetLearnedLessons(Guid courseId, int userId);

        /// <summary>
        /// Get the course id that the lesson belongs to
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonId">Lesson id</param>
        public Task<Guid> GetCourseIdBelongToLesson(Guid lessonId);

        /// <summary>
        /// Get the first lesson id of the course
        /// <para>Created at: 2024/11/14</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Task<Guid> GetFirstLessonId(Guid courseId);

        /// <summary>
        /// Get the previous and next lesson id of the lesson having the lessonId
        /// <para>Created at: 2024/11/14</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<(Guid?, Guid?)> GetPreviousAndNextLessonId(int currentChapterOrder, int currentLessonOrder);

        /// <summary>
        /// Get the lesson id that the user should continue learning
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ContinueLessonInfo> GetContinueLessonInfo(Guid courseId);
    }

    public class LessonBaseDetailService(IServiceProvider serviceProvider, ILogger<LessonBaseDetailService> logger)
        : BaseService(serviceProvider, logger), ILessonBaseDetailService
    {
        public async Task<bool> IsExistLesson(Guid lessonId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var isExist = await _context.Lessons.AnyAsync(l => l.Id == lessonId);

                LogInfo("End", method);
                return isExist;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<bool> CanAccessCourseMaterial(Guid courseId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var course = await _context.Courses
                    .Where(c => c.Id == courseId)
                    .Select(c => new
                    {
                        c.TeacherId,
                        IsEnrolled = c.Enrollments.Any(e => e.StudentId == userId && !e.LeaveDate.HasValue)
                    })
                    .FirstOrDefaultAsync();

                LogInfo("End", method);
                return course != null && (course.TeacherId == userId || course.IsEnrolled);
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<bool> CanAccessLessonMaterial(Guid lessonId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var lesson = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => new
                    {
                        l.Chapter.Course.TeacherId,
                        IsEnrolled = l.Chapter.Course
                            .Enrollments.Any(e => e.StudentId == userId && !e.LeaveDate.HasValue)
                    })
                    .FirstOrDefaultAsync();

                LogInfo("End", method);
                return lesson != null && (lesson.TeacherId == userId || lesson.IsEnrolled);
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<bool> CanModifyCourseMaterial(Guid courseId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var isCanModify = await _context.Courses
                    .AnyAsync(c => c.Id == courseId && c.TeacherId == userId);

                LogInfo("End", method);
                return isCanModify;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<bool> CanModifyLessonMaterial(Guid lessonId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var isCanModify = await _context.Lessons
                    .AnyAsync(l => l.Id == lessonId && l.Chapter.Course.TeacherId == userId);

                LogInfo("End", method);
                return isCanModify;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<Guid> GetCourseIdBelongToLesson(Guid lessonId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var courseId = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => l.Chapter.CourseId)
                    .FirstOrDefaultAsync();

                LogInfo("End", method);
                return courseId;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<List<LessonTrackingDetail>> GetLearnedLessons(Guid courseId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var lessonTrackingRecords = await _context.LessonTrackings
                    .Where(lt => lt.Lesson.Chapter.CourseId == courseId && lt.StudentId == userId)
                    .OrderBy(lt => lt.CreatedAt)
                    .Select(lt => new LessonTrackingDetail()
                    {
                        LessonId = lt.LessonId,
                        TimeSpent = lt.TimeSpent,
                        LastAccessed = lt.UpdatedAt
                    })
                    .ToListAsync();

                LogInfo("End", method);
                return lessonTrackingRecords;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<Guid> GetFirstLessonId(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var lessonId = await _context.Lessons
                    .Where(l => l.Chapter.CourseId == courseId && l.Chapter.Order <= 1)
                    .OrderBy(l => l.Order)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync();

                LogInfo("End", methodName);
                return lessonId;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<(Guid?, Guid?)> GetPreviousAndNextLessonId(int currentChapterOrder, int currentLessonOrder)
        {
            var methodName = GetActualAsyncMethodName();

            try
            {
                LogInfo("Start", methodName);

                int currentCompositeOrder = currentChapterOrder * 1000 + currentLessonOrder;
                var previousLesson = await _context.Lessons
                    .Where(l => l.Chapter.Order * 1000 + l.Order < currentCompositeOrder)
                    .OrderByDescending(l => l.Chapter.Order * 1000 + l.Order)
                    .FirstOrDefaultAsync();

                var nextLesson = await _context.Lessons
                    .Where(l => l.Chapter.Order * 1000 + l.Order > currentCompositeOrder)
                    .OrderBy(l => l.Chapter.Order * 1000 + l.Order)
                    .FirstOrDefaultAsync();

                return (previousLesson?.Id, nextLesson?.Id);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ContinueLessonInfo> GetContinueLessonInfo(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                var currentUser = GetCurrentUser()
                    ?? throw new UnauthorizedAccessException("You are not allowed to access this resource");

                var isCanAccess = await CanAccessCourseMaterial(courseId, currentUser.UserId);
                if (!isCanAccess)
                {
                    throw new UnauthorizedAccessException("You are not allowed to access this resource");
                }

                var lastLearnedLesson = await _context.LessonTrackings
                    .Where(lt => lt.CourseId == courseId && lt.StudentId == currentUser.UserId)
                    .OrderByDescending(lt => lt.CreatedAt)
                    .Select(lt => new
                    {
                        lt.LessonId,
                        lt.Lesson.LessonType,
                        lt.TimeSpent,
                        LessonOrder = lt.Lesson.Order,
                        ChapterOrder = lt.Lesson.Chapter.Order
                    })
                    .FirstOrDefaultAsync();

                if (lastLearnedLesson == null)
                {
                    // Điều này có thể xảy ra khi người dùng chưa học bài nào trong khoá học
                    return null;
                }

                // Lấy bài học tiếp theo có thể học
                var currentCompositeOrder = lastLearnedLesson.ChapterOrder * 1000 + lastLearnedLesson.LessonOrder;
                var nextLesson = await _context.Lessons
                    .Where(l => l.Chapter.CourseId == courseId && l.Chapter.Order * 1000 + l.Order > currentCompositeOrder)
                    .OrderBy(l => l.Chapter.Order * 1000 + l.Order)
                    .Select(l => new
                    {
                        l.Id,
                        l.LessonType
                    })
                    .FirstOrDefaultAsync();

                if (nextLesson == null)
                {
                    return new ContinueLessonInfo()
                    {
                        LessonId = lastLearnedLesson.LessonId,
                        LessonType = lastLearnedLesson.LessonType,
                        TimeSpent = lastLearnedLesson.TimeSpent
                    };
                }

                return new ContinueLessonInfo()
                {
                    LessonId = nextLesson.Id,
                    LessonType = nextLesson.LessonType,
                    TimeSpent = 0
                };
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}