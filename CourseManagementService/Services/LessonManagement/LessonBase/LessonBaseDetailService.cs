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
    }
}