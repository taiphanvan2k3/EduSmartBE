using CourseManagementService.Database.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database.InitDb
{
    public partial class DbInitializer
    {
        public async Task SeedDataDefault()
        {
            try
            {
                _logger.LogInformation("[DbInitializer] Seeding default data");

                await SeedCategoryData();
                await SeedTagData();

                await _context.SaveChangesAsync();
                _logger.LogInformation("[DbInitializer] Seeding default data completed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private async Task SeedCategoryData()
        {
            if (!await _context.Categories.AnyAsync())
            {
                await _context.Categories.AddRangeAsync(
                [
                    new Category()
                    {
                        Name= "IT",
                        IsCreatedByAdmin = true
                    },
                    new Category()
                    {
                        Name= "Toeic",
                        IsCreatedByAdmin = true
                    },
                ]);
            }
        }

        private async Task SeedTagData()
        {
            if (!await _context.Tags.AnyAsync())
            {
                List<string> tags = ["C/C++", "C#", "Java", "Python", "AI", "Cloud", "HTML, CSS",
                    "Basic Javascript", "Toeic", "Listening", "Reading"];

                await _context.Tags.AddRangeAsync(tags
                    .Select(tag => new Tag()
                    {
                        Name = tag,
                        IsCreatedByAdmin = true
                    }));
            }
        }
    }
}