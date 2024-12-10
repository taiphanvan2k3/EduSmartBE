using Microsoft.EntityFrameworkCore;
using PaymentService.Commons.Helpers;
using PaymentService.Databases.Schemas;
using PaymentService.Services.Banks.Schemas;

namespace PaymentService.Databases.InitDb
{
    public partial class DbInitializer
    {
        public async Task SeedDataDefault()
        {
            await SeedDataForBanks();
            await SeedDataForAchievementTemplates();
            await _context.SaveChangesAsync();
        }

        private async Task SeedDataForBanks()
        {
            try
            {
                if (!await _context.Banks.AnyAsync())
                {
                    // Đọc dữ liệu từ file banks.json
                    var currentDirectory = Directory.GetCurrentDirectory();
                    var banksJson = File.ReadAllText(Path.Combine(currentDirectory, "Databases/InitDb/Data", "banks.json"));

                    var banksData = JsonSerializerUtils.Deserialize<BanksData>(banksJson);
                    var bankEntities = banksData.Banks.Select(bank => new Bank
                    {
                        Id = bank.Id,
                        Bin = bank.Bin,
                        Name = bank.Name,
                        LogoURL = bank.LogoURL,
                        ShortName = bank.ShortName
                    });

                    await _context.Banks.AddRangeAsync(bankEntities);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while seeding banks data");
                throw;
            }
        }

        private async Task SeedDataForAchievementTemplates()
        {
            try
            {
                if (!await _context.AchievementTemplates.AnyAsync())
                {
                    await _context.AchievementTemplates.AddRangeAsync(new List<AchievementTemplate>
                    {
                        new() {
                            Name = "Template_01",
                            ThumbnailURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759920/1_pvlh7h.png",
                            TemplateURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759920/1_pvlh7h.png"
                        },
                        new() {
                            Name = "Template_02",
                            ThumbnailURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759920/2_py3f5x.png",
                            TemplateURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759920/2_py3f5x.png"
                        },
                        new() {
                            Name = "Template_03",
                            ThumbnailURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759920/3_pp5kno.png",
                            TemplateURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759920/3_pp5kno.png"
                        },
                        new() {
                            Name = "Template_04",
                            ThumbnailURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759919/4_plshrm.png",
                            TemplateURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759919/4_plshrm.png"
                        },
                        new() {
                            Name = "Template_05",
                            ThumbnailURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759920/5_t1xvlk.png",
                            TemplateURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759920/5_t1xvlk.png"
                        },
                        new() {
                            Name = "Template_06",
                            ThumbnailURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759920/6_t1eehy.png",
                            TemplateURL = "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759920/6_t1eehy.png"
                        }
                    });
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while seeding achievement templates data");
                throw;
            }
        }
    }
}