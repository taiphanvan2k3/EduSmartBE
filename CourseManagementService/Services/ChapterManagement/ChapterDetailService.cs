using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase;
using Microsoft.EntityFrameworkCore;
using TblChapter = CourseManagementService.Database.Schemas.Chapter;

namespace CourseManagementService.Services.ChapterManagement
{
    public interface IChapterDetailService
    {
        /// <summary>
        /// Check if chapter is existing
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="chapterId">Id of chapter</param>
        /// <returns></returns>
        public Task<bool> IsExistingChapter(Guid chapterId);

        /// <summary>
        /// Get course id that the chapter belongs to
        /// <para>Created at: 2024/11/12</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="chapterId">Id of chapter</param>
        public Task<Guid> GetCourseIdBelongToChapter(Guid chapterId);

        /// <summary>
        /// Create chapter
        /// <para>Created at: 2024/10/24 - ManhTD</para>
        /// <para>Modified at: 2024/11/07 - TaiPV</para>
        /// </summary>
        /// <param name="chapterDetailCreate"></param>
        /// <returns></returns>
        public Task<ResponseInfo> CreateChapter(ChapterDetailCreate chapterDetailCreate);

        /// <summary>
        /// Update chapter
        /// <para>Created at: 2024/10/24 - ManhTD</para>
        /// <para>Modified at: 2024/11/07 - TaiPV</para>
        /// </summary>
        /// <param name="chapterDetailUpdate"></param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateChapter(ChapterDetailUpdate chapterDetailUpdate);

        /// <summary>
        /// Delete chapters
        /// <para>Created at: 2024/11/07</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="chapterIds">List of chapter ids</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteChapters(List<Guid> chapterIds);

        /// <summary>
        /// Update chapter published status
        /// <para>Created at: 2024/11/07</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="chapterId">Id of chapter</param>
        /// <param name="isPublished">Published status</param>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateChapterPublishedStatus(Guid chapterId, bool isPublished);
    }

    public class ChapterDetailService(IServiceProvider serviceProvider, ILogger<ChapterDetailService> logger)
        : BaseService(serviceProvider, logger), IChapterDetailService
    {
        private readonly string _serviceName = nameof(ChapterDetailService);
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ICacheService)));
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetRequiredService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));

        public async Task<bool> IsExistingChapter(Guid chapterId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var isExist = await _context.Chapters
                    .AnyAsync(c => c.Id == chapterId);

                LogInfo("End", method);
                return isExist;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<Guid> GetCourseIdBelongToChapter(Guid chapterId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var courseId = await _context.Chapters
                    .Where(c => c.Id == chapterId)
                    .Select(c => c.CourseId)
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

        public async Task<ResponseInfo> CreateChapter(ChapterDetailCreate chapterDetailCreate)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var response = new ResponseInfo();

                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanModifyCourseMaterial(chapterDetailCreate.CourseId, currentUser.UserId))
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You don't have permission to create chapter";
                    _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                    return response;
                }

                var chapter = new TblChapter
                {
                    Id = Guid.NewGuid(),
                    Name = chapterDetailCreate.Name,
                    Order = chapterDetailCreate.Order,
                    CourseId = chapterDetailCreate.CourseId,
                    IsPublished = chapterDetailCreate.IsPublished,
                };

                _context.Chapters.Add(chapter);
                await _context.SaveChangesAsync();

                try
                {
                    await _cacheService.RemoveDataByPattern(CacheManager.CourseDetail.PrefixKey);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "[{ServiceName}] {MethodName} Error when remove cache", _serviceName, method);
                }

                var chapterDetail = _mapper.Map<ChapterDetail>(chapter);

                response.StatusCode = StatusCodes.Status201Created;
                response.Data.Add("Chapter", chapterDetail);

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateChapter(ChapterDetailUpdate chapterDetailUpdate)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var response = new ResponseInfo();
                var chapter = await _context.Chapters
                    .Where(c => c.Id == chapterDetailUpdate.Id)
                    .FirstOrDefaultAsync();

                if (chapter == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Chapter not found";
                    _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                    return response;
                }

                chapter.Name = chapterDetailUpdate.Name;
                chapter.Order = chapterDetailUpdate.Order;
                chapter.IsPublished = chapterDetailUpdate.IsPublished;

                await _context.SaveChangesAsync();

                response.StatusCode = StatusCodes.Status200OK;

                var chapterDetail = _mapper.Map<ChapterDetail>(chapter);
                response.Data.Add("Chapter", chapterDetail);

                try
                {
                    await _cacheService.RemoveDataByPattern(CacheManager.CourseDetail.PrefixKey);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "[{ServiceName}] {MethodName} Error when remove cache", _serviceName, method);
                }

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteChapters(List<Guid> chapterIds)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var response = new ResponseInfo();

                var invalidChapterIds = await _context.Chapters
                    .Where(c => chapterIds.Contains(c.Id) && c.Lessons.Count != 0)
                    .Select(c => c.Id)
                    .ToListAsync();

                if (invalidChapterIds.Count > 0)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Some chapters have lessons";
                    response.Data.Add("ChaptersHaveLesson", invalidChapterIds);
                    _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                    return response;
                }

                int deletedChapterCount = await _context.Chapters
                    .Where(c => chapterIds.Contains(c.Id))
                    .ExecuteDeleteAsync();

                if (deletedChapterCount < chapterIds.Count)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Some chapters not found";
                    return response;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Data.Add("chapterIds", chapterIds);

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateChapterPublishedStatus(Guid chapterId, bool isPublished)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var response = new ResponseInfo();

                var chapter = await _context.Chapters
                    .FindAsync(chapterId);

                if (chapter == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Chapter not found";
                    LogInfo("End", method);
                    return response;
                }

                chapter.IsPublished = isPublished;
                await _context.SaveChangesAsync();

                response.StatusCode = StatusCodes.Status200OK;
                response.Data.Add("Chapter", new
                {
                    chapter.Id,
                    chapter.IsPublished
                });

                try
                {
                    await _cacheService.RemoveDataByPattern(CacheManager.CourseDetail.PrefixKey);
                }
                catch (Exception e)
                {
                    LogError(e, method);
                }

                LogInfo("End", method);
                return response;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}