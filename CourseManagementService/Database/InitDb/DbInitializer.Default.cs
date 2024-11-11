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
                await SeedCurrencyData();

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
                List<IconInfo> webIcons =
                [
                    new() { Icon = "FaCode", Color = "#E83E8C" },
                    new() { Icon = "FaMusic", Color = "#FF6347" },
                    new() { Icon = "FaLanguage", Color = "#1E90FF" },
                    new() { Icon = "FaRuler", Color = "#FFD700" },
                    new() { Icon = "FaChartLine", Color = "#32CD32" },
                ];

                List<IconInfo> mobileIcons =
                [
                    new() { Icon = "0xf653", Color = "" },
                    new() { Icon = "0xf1fb", Color = "" },
                    new() { Icon = "0xf45e", Color = "" },
                    new() { Icon = "0xefaf", Color = "" },
                    new() { Icon = "0xef0a", Color = "" },
                ];

                var categories = new List<string> { "Programming", "Music", "Language", "Design", "Marketing" };
                var categoryList = categories.Select((category, index) => new Category()
                {
                    Name = category,
                    CreatedBy = 1,
                    IsCreatedByAdmin = true,
                    WebIconInfo = webIcons[index],
                    MobileIconInfo = mobileIcons[index]
                });

                await _context.Categories.AddRangeAsync(categoryList);
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

        private async Task SeedCurrencyData()
        {
            if (!await _context.Currencies.AnyAsync())
            {
                await _context.Currencies.AddRangeAsync(
                    new Currency()
                    {
                        Name = "Viet Nam Dong",
                        Code = "VND"
                    },
                    new Currency()
                    {
                        Name = "US Dollar",
                        Code = "USD"
                    });
            }
        }
    }
}