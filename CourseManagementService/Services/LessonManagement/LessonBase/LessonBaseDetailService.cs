using CourseManagementService.Common;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.Cache;
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
        /// Check if the user can access the course material
        /// <para>Created at: 2024/11/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CheckCoursePermission(Guid courseId, int userId);

        /// <summary>
        /// Check if the user can access the lesson material
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> CanAccessLessonMaterial(Guid lessonId, int userId);

        /// <summary>
        /// Check if the user can access the lesson material
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CheckLessonPermission(Guid lessonId, int userId);

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
        /// Check if the user can create a lesson in the chapter
        /// <para>Created at: 2024/11/24</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<bool> CanCreateLessonInChapter(Guid chapterId, int userId);

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
        public Task<LessonInfoBase> GetFirstLessonInfo(Guid courseId);

        /// <summary>
        /// Get the previous and next lesson id of the lesson having the lessonId
        /// <para>Created at: 2024/11/14</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<(LessonInfoBase, LessonInfoBase)> GetPreviousAndNextLessonId(Guid courseId, int currentChapterOrder, int currentLessonOrder);

        /// <summary>
        /// Get the lesson id that the user should continue learning
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ContinueLessonInfo> GetContinueLessonInfo(Guid courseId);

        /// <summary>
        /// Clear the course detail cache when teacher updates the course
        /// <para>Created at: 2024/12/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task ClearCourseDetailCache(Guid courseId);
    }

    public class LessonBaseDetailService(IServiceProvider serviceProvider, ILogger<LessonBaseDetailService> logger)
        : BaseService(serviceProvider, logger), ILessonBaseDetailService
    {
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ICacheService)));

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

        public async Task<ResponseInfo> CheckCoursePermission(Guid courseId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var course = await _context.Courses
                    .Where(c => c.Id == courseId)
                    .Select(c => new
                    {
                        c.TeacherId,
                        IsEnrolled = c.Enrollments.Any(e => e.StudentId == userId && !e.LeaveDate.HasValue)
                    })
                    .FirstOrDefaultAsync();

                if (course == null)
                {
                    responseInfo.Error = "Course not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                if (course.TeacherId != userId && !course.IsEnrolled)
                {
                    responseInfo.Error = "You are not allowed to access this resource";
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    return responseInfo;
                }

                responseInfo.Data.Add("isTeacher", course.TeacherId == userId);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
            finally
            {
                LogInfo("End", method);
            }
        }

        public async Task<ResponseInfo> CheckLessonPermission(Guid lessonId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();

                var lesson = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => new
                    {
                        l.Chapter.Course.TeacherId,
                        IsEnrolled = l.Chapter.Course
                            .Enrollments.Any(e => e.StudentId == userId && !e.LeaveDate.HasValue)
                    })
                    .FirstOrDefaultAsync();

                if (lesson == null)
                {
                    responseInfo.Error = "Lesson not found";
                    responseInfo.StatusCode = StatusCodes.Status404NotFound;
                    return responseInfo;
                }

                if (lesson.TeacherId != userId && !lesson.IsEnrolled)
                {
                    responseInfo.Error = "You are not allowed to access this resource";
                    responseInfo.StatusCode = StatusCodes.Status403Forbidden;
                    return responseInfo;
                }

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
            finally
            {
                LogInfo("End", method);
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

        public async Task<bool> CanCreateLessonInChapter(Guid chapterId, int userId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);

                var isCanCreate = await _context.Chapters
                    .AnyAsync(c => c.Id == chapterId && c.Course.TeacherId == userId);

                LogInfo("End", method);
                return isCanCreate;
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
                        LastAccessed = lt.UpdatedAt ?? lt.CreatedAt
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

        public async Task<LessonInfoBase> GetFirstLessonInfo(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var lessonTypeEnum = typeof(LessonType);

                var firstPublishedChapter = await _context.Chapters
                    .Where(c => c.CourseId == courseId && c.IsPublished && c.Lessons.Any(l => l.IsPublished))
                    .OrderBy(c => c.Order)
                    .FirstOrDefaultAsync();

                if (firstPublishedChapter == null)
                {
                    return null;
                }

                var firstLesson = await _context.Lessons
                    .Where(l => l.ChapterId == firstPublishedChapter.Id && l.IsPublished)
                    .OrderBy(l => l.Order)
                    .Select(l => new LessonInfoBase()
                    {
                        Id = l.Id,
                        LessonType = new LookupDto()
                        {
                            Id = ((int)Enum.Parse(lessonTypeEnum, l.LessonType.ToString())).ToString(),
                            Name = l.LessonType.ToString()
                        }
                    })
                    .FirstOrDefaultAsync();

                LogInfo("End", methodName);
                return firstLesson;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<(LessonInfoBase, LessonInfoBase)> GetPreviousAndNextLessonId(Guid courseId, int currentChapterOrder, int currentLessonOrder)
        {
            var methodName = GetActualAsyncMethodName();

            try
            {
                LogInfo("Start", methodName);
                var lessonTypeEnum = typeof(LessonType);

                int currentCompositeOrder = currentChapterOrder * 1000 + currentLessonOrder;
                var previousLesson = await _context.Lessons
                    .Where(l => l.Chapter.CourseId == courseId && l.Chapter.IsPublished && l.IsPublished)
                    .Where(l => l.Chapter.Order * 1000 + l.Order < currentCompositeOrder)
                    .OrderByDescending(l => l.Chapter.Order * 1000 + l.Order)
                    .Select(l => new LessonInfoBase()
                    {
                        Id = l.Id,
                        LessonType = new LookupDto()
                        {
                            Id = ((int)Enum.Parse(lessonTypeEnum, l.LessonType.ToString())).ToString(),
                            Name = l.LessonType.ToString()
                        }
                    })
                    .FirstOrDefaultAsync();

                var nextLesson = await _context.Lessons
                    .Where(l => l.Chapter.CourseId == courseId && l.Chapter.IsPublished && l.IsPublished)
                    .Where(l => l.Chapter.Order * 1000 + l.Order > currentCompositeOrder)
                    .OrderBy(l => l.Chapter.Order * 1000 + l.Order)
                    .Select(l => new LessonInfoBase()
                    {
                        Id = l.Id,
                        LessonType = new LookupDto()
                        {
                            Id = ((int)Enum.Parse(lessonTypeEnum, l.LessonType.ToString())).ToString(),
                            Name = l.LessonType.ToString()
                        }
                    })
                    .FirstOrDefaultAsync();

                return (previousLesson, nextLesson);
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
                    .Where(l => l.Chapter.IsPublished && l.IsPublished)
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
                        LessonType = new LookupDto()
                        {
                            Id = ((int)lastLearnedLesson.LessonType).ToString(),
                            Name = lastLearnedLesson.LessonType.ToString()
                        },
                        TimeSpent = lastLearnedLesson.TimeSpent
                    };
                }

                return new ContinueLessonInfo()
                {
                    LessonId = nextLesson.Id,
                    LessonType = new LookupDto()
                    {
                        Id = ((int)nextLesson.LessonType).ToString(),
                        Name = nextLesson.LessonType.ToString()
                    },
                    TimeSpent = 0
                };
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task ClearCourseDetailCache(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                await _cacheService.RemoveDataByPatternUsingLuaScript(CacheManager.CourseDetail.GetPrefixKey(courseId));
                LogInfo("End", methodName);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}