using CourseManagementService.Services.ChapterManagement.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.ChapterManagement
{
    public interface IListOfChaptersService
    {
        public Task<List<ChapterDetail>> GetListOfChapters();
        public Task<List<ChapterDetail>> GetListOfChaptersByCourseId(string courseId);
        public Task<ChapterDetail> GetChapterById(string chapterId);
    }

    public class ListOfChaptersService(IServiceProvider serviceProvider, ILogger<ListOfChaptersService> logger) : BaseService(serviceProvider, logger), IListOfChaptersService
    {
        private readonly string _serviceName = nameof(ListOfChaptersService);

        public async Task<List<ChapterDetail>> GetListOfChapters()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var chapters = await _context.Chapters
                    .Select(c => new ChapterDetail()
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name,
                        Order = c.Order,
                        CourseId = c.CourseId.ToString()
                    })
                    .ToListAsync();

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return chapters;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<ChapterDetail> GetChapterById(string chapterId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var chapter = await _context.Chapters
                    .Where(c => c.Id == Guid.Parse(chapterId))
                    .Select(c => new ChapterDetail()
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name,
                        Order = c.Order,
                        CourseId = c.CourseId.ToString()
                    })
                    .FirstOrDefaultAsync();

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return chapter;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }

        public async Task<List<ChapterDetail>> GetListOfChaptersByCourseId(string courseId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[{ServiceName}] {MethodName} Start", _serviceName, method);
                var chapters = await _context.Chapters
                    .Where(c => c.CourseId == Guid.Parse(courseId))
                    .Select(c => new ChapterDetail()
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name,
                        Order = c.Order,
                        CourseId = c.CourseId.ToString()
                    })
                    .ToListAsync();

                _logger.LogInformation("[{ServiceName}] {MethodName} End", _serviceName, method);
                return chapters;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{ServiceName}] {MethodName} Error", _serviceName, method);
                throw;
            }
        }
    }
}