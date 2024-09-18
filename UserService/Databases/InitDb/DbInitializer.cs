using Microsoft.EntityFrameworkCore;

namespace UserService.Databases.InitDb
{
    public interface IDbInitializer
    {
        Task Migrate();

        Task Initialize();
    }

    public partial class DbInitializer(DataContext context) : IDbInitializer
    {
        private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task Migrate()
        {
            await _context.Database.MigrateAsync();
        }

        public async Task Initialize()
        {
            _context.Database.EnsureCreated();
            await SeedDataDefault(_context);
        }
    }
}