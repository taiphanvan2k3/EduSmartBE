using CourseManagementService.Common;
using CourseManagementService.Services.ChapterManagement.Schemas;
using Microsoft.EntityFrameworkCore;
using TblChapter = CourseManagementService.Database.Schemas.Chapter;

namespace CourseManagementService.Services.ChapterManagement
{
    public interface IChapterDetailService
    {
        public Task<ResponseInfo> CreateChapter(ChapterDetailCreate chapterDetail);
        public Task<ResponseInfo> UpdateChapter(ChapterDetail chapterDetail);
        public Task<ResponseInfo> DeleteChapters(List<string> chapterIds);
    }

    public class ChapterDetailService(IServiceProvider serviceProvider, ILogger<ChapterDetailService> logger) : BaseService(serviceProvider, logger), IChapterDetailService
    {
        private readonly string _serviceName = nameof(ChapterDetailService);

        public async Task<ResponseInfo> CreateChapter(ChapterDetailCreate chapterDetail)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var response = new ResponseInfo();
                var chapter = new TblChapter
                {
                    Id = Guid.NewGuid(),
                    Name = chapterDetail.Name,
                    Order = chapterDetail.Order,
                    CourseId = Guid.Parse(chapterDetail.CourseId)
                };

                _context.Chapters.Add(chapter);
                await _context.SaveChangesAsync();

                response.StatusCode = StatusCodes.Status201Created;
                response.Message = "Chapter created successfully";
                response.Data.Add("ChapterCreated", chapter);

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateChapter(ChapterDetail chapterDetail)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var response = new ResponseInfo();
                var chapter = await _context.Chapters
                    .Where(c => c.Id == Guid.Parse(chapterDetail.Id))
                    .FirstOrDefaultAsync();

                if (chapter == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Chapter not found";
                    _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                    return response;
                }

                chapter.Name = chapterDetail.Name;
                chapter.Order = chapterDetail.Order;
                chapter.CourseId = Guid.Parse(chapterDetail.CourseId);

                await _context.SaveChangesAsync();

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chapter updated successfully";
                response.Data.Add("ChapterUpdated", chapter);

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteChapters(List<string> chapterIds)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var response = new ResponseInfo();
                var chapters = await _context.Chapters
                    .Where(c => chapterIds.Contains(c.Id.ToString()))
                    .ToListAsync();

                if (chapters.Count == 0)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Chapters not found";
                    _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                    return response;
                }

                _context.Chapters.RemoveRange(chapters);
                await _context.SaveChangesAsync();

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chapters deleted successfully";
                response.Data.Add("ChaptersDeleted", chapters);

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }
    }
}