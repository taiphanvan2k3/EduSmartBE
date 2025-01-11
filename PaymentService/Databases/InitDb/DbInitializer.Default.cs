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
            await SeedAdminBankAccount();
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

        private async Task SeedAdminBankAccount()
        {
            if (!await _context.BankAccounts.AnyAsync())
            {
                var adminBankAccount = new BankAccount
                {
                    UserId = 1,
                    AccountNumber = "SEPSMARTEDU2003",
                    BankId = 26, // OCB
                    AccountName = "Phan Văn Tài",
                    IsPrimary = true,
                    IsAdminAccount = true,
                };

                await _context.BankAccounts.AddAsync(adminBankAccount);
            }
        }

        private async Task SeedDataForAchievementTemplates()
        {
            try
            {
                var existingTemplates = await _context.AchievementTemplates.ToListAsync();
                var targetTemplates = new List<AchievementTemplate>
                {
                    new() {
                        Name = "Template_01",
                        ThumbnailURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443617/1_qqqxnv.png",
                        TemplateURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443617/1_qqqxnv.png"
                    },
                    new() {
                        Name = "Template_02",
                        ThumbnailURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443618/2_fvjcpy.png",
                        TemplateURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443618/2_fvjcpy.png"
                    },
                    new() {
                        Name = "Template_03",
                        ThumbnailURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443618/3_gw6zje.png",
                        TemplateURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443618/3_gw6zje.png"
                    },
                    new() {
                        Name = "Template_04",
                        ThumbnailURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443617/4_doqs2y.png",
                        TemplateURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443617/4_doqs2y.png"
                    },
                    new() {
                        Name = "Template_05",
                        ThumbnailURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443618/5_vmdpwp.png",
                        TemplateURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443618/5_vmdpwp.png"
                    },
                    new() {
                        Name = "Template_06",
                        ThumbnailURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443619/6_kfhzcq.png",
                        TemplateURL = "https://res.cloudinary.com/darzeepog/image/upload/v1783443619/6_kfhzcq.png"
                    }
                };

                foreach (var target in targetTemplates)
                {
                    var existing = existingTemplates.FirstOrDefault(x => x.Name == target.Name);
                    if (existing != null)
                    {
                        if (existing.ThumbnailURL != target.ThumbnailURL || existing.TemplateURL != target.TemplateURL)
                        {
                            existing.ThumbnailURL = target.ThumbnailURL;
                            existing.TemplateURL = target.TemplateURL;
                        }
                    }
                    else
                    {
                        await _context.AchievementTemplates.AddAsync(target);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while seeding achievement templates data");
                throw;
            }
        }
    }
}