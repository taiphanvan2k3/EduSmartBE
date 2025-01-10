using AuthService.Databases.Schemas;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Databases.InitDb
{
    public interface IDbInitializer
    {
        Task Initialize();
    }

    public partial class DbInitializer(DataContext context,
        UserManager<ApplicationUser> userManager) : IDbInitializer
    {
        private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task Initialize()
        {
            await _context.Database.EnsureCreatedAsync();
            await SeedDataDefault(_context, userManager);
        }
    }
}