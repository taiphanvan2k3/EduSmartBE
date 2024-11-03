using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.LessonManagement
{
    public interface ILessonBaseDetailService
    {
        public Task<bool> CanAccessCourseMaterial(Guid courseId, int userId);
        public Task<bool> CanModifyCourseMaterial(Guid courseId, int userId);
    }

    public class LessonBaseDetailService(IServiceProvider serviceProvider, ILogger<LessonBaseDetailService> logger)
        : BaseService(serviceProvider, logger), ILessonBaseDetailService
    {
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
    }
}