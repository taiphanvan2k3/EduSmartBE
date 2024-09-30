namespace UserService.Databases.InitDb
{
    public interface IDbInitializer
    {
        Task Initialize();
    }

    public partial class DbInitializer(DataContext context) : IDbInitializer
    {
        private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task Initialize()
        {
            _context.Database.EnsureCreated();
            await SeedDataDefault();
        }
    }
}