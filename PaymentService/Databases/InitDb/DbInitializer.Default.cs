namespace PaymentService.Databases.InitDb
{
    public partial class DbInitializer
    {
        public async Task SeedDataDefault()
        {
            await SeedDataForBanks();
        }

        private async Task SeedDataForBanks()
        {
            try
            {
                // Đọc dữ liệu từ file banks.json
                var currentDirectory = Directory.GetCurrentDirectory();
                var banksJson = File.ReadAllText(Path.Combine(currentDirectory, "Databases/InitDb/Data", "banks.json"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while seeding banks data");
                throw;
            }
        }
    }
}