using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases.InitDb
{
    public interface IDbInitializer
    {
        void Migrate();
        void Initialize();
    }

    public partial class DbInitializer : IDbInitializer
    {
        private readonly DataContext _context;

        public DbInitializer(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();
            SeedDataDefault(_context);
        }

        public void Migrate()
        {
            _context.Database.Migrate();
        }
    }
}