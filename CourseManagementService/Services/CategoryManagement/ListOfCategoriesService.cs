using CourseManagementService.Common;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CategoryManagement
{
    public interface IListOfCategoriesService
    {
        public Task<List<LookupDto>> GetListOfCategories();
    }

    public class ListOfCategoriesService(IServiceProvider serviceProvider, ILogger<ListOfCategoriesService> logger)
        : BaseService(serviceProvider, logger), IListOfCategoriesService
    {
        public async Task<List<LookupDto>> GetListOfCategories()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var categories = await _context.Categories
                    .Select(c => new LookupDto()
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name
                    })
                    .ToListAsync();

                LogInfo("End", method);
                return categories;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }
    }
}