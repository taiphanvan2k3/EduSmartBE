using CourseManagementService.Common;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.LessonManagement.LessonBase;
using Microsoft.EntityFrameworkCore;
using TblBookmark = CourseManagementService.Database.Schemas.Bookmark;

namespace CourseManagementService.Services.BookmarkManagement
{
    public interface IBookmarkService
    {
        /// <summary>
        /// Toggle bookmark for a lesson
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="lessonId">Id of lesson</param>
        /// <returns></returns>
        public Task<ResponseInfo> ToggleBookmark(Guid lessonId);

        /// <summary>
        /// Get list of bookmarked lesson ids
        /// <para>Created at: 2024/12/04</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<List<Guid>> GetBookmarkedLessonIds(Guid courseId);
    }

    public class BookmarkService(IServiceProvider serviceProvider, ILogger<BookmarkService> logger)
        : BaseService(serviceProvider, logger), IBookmarkService
    {
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetRequiredService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ICacheService)));

        public async Task<List<Guid>> GetBookmarkedLessonIds(Guid courseId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var currentUser = GetCurrentUser();

                var cachedKey = CacheManager.Bookmark.Key(courseId, currentUser.UserId);
                var cachedData = _cacheService.GetData<List<Guid>>(cachedKey);
                if (cachedData != null)
                {
                    return cachedData;
                }

                var lessonIds = await _context.Bookmarks
                    .Where(b => b.UserId == currentUser.UserId && b.CourseId == courseId)
                    .Select(b => b.LessonId)
                    .ToListAsync();

                _cacheService.SetData(cachedKey, lessonIds, DateTimeOffset.Now.AddMinutes(CacheManager.Bookmark.ExpireTimeInMinutes));
                LogInfo("End", method);
                return lessonIds;
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

        public async Task<ResponseInfo> ToggleBookmark(Guid lessonId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var currentUser = GetCurrentUser();

                var lessonInfo = await _context.Lessons
                    .Where(l => l.Id == lessonId)
                    .Select(l => new
                    {
                        l.Chapter.CourseId
                    })
                    .FirstOrDefaultAsync();

                if (lessonInfo == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Lesson not found");
                }

                var lessonPermissionResponse = await _lessonBaseDetailService.CheckLessonPermission(lessonId, currentUser.UserId);
                if (!lessonPermissionResponse.IsSuccess)
                {
                    return lessonPermissionResponse;
                }

                var bookmark = await _context.Bookmarks
                    .Where(b => b.LessonId == lessonId && b.UserId == currentUser.UserId)
                    .FirstOrDefaultAsync();

                var isTurningOn = bookmark != null;

                if (bookmark == null)
                {
                    bookmark = new TblBookmark
                    {
                        LessonId = lessonId,
                        UserId = currentUser.UserId,
                        CourseId = lessonInfo.CourseId
                    };

                    await _context.Bookmarks.AddAsync(bookmark);
                }
                else
                {
                    _context.Bookmarks.Remove(bookmark);
                }

                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("bookmarkStatus", new
                {
                    isTurningOn = !isTurningOn,
                    lessonId
                });

                _cacheService.RemoveData(CacheManager.Bookmark.Key(lessonInfo.CourseId, currentUser.UserId));

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
    }
}