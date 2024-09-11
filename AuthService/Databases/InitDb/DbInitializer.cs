using AuthService.Databases.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases.InitDb
{
    public interface IDbInitializer
    {
        Task Migrate();

        Task Initialize();
    }

    public partial class DbInitializer(DataContext context,
        UserManager<ApplicationUser> userManager) : IDbInitializer
    {
        private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task Migrate()
        {
            await _context.Database.MigrateAsync();
        }

        public async Task Initialize()
        {
            _context.Database.EnsureCreated();
            await SeedDataDefault(_context, userManager);
        }
    }
}