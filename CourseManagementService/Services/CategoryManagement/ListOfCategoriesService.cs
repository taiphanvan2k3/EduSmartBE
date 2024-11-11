using CourseManagementService.Services.CategoryManagement.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.CategoryManagement
{
    public interface IListOfCategoriesService
    {
        public Task<List<CategoryDto>> GetListOfCategories();
    }

    public class ListOfCategoriesService(IServiceProvider serviceProvider, ILogger<ListOfCategoriesService> logger)
        : BaseService(serviceProvider, logger), IListOfCategoriesService
    {
        public async Task<List<CategoryDto>> GetListOfCategories()
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var categories = await _context.Categories
                    .Where(c => c.IsCreatedByAdmin)
                    .Select(c => new CategoryDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        WebIconInfo = new IconInfoDto()
                        {
                            Icon = c.WebIconInfo.Icon,
                            Color = c.WebIconInfo.Color
                        },
                        MobileIconInfo = new IconInfoDto()
                        {
                            Icon = c.MobileIconInfo.Icon,
                            Color = c.MobileIconInfo.Color
                        }
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