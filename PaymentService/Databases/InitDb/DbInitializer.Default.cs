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
    }
}