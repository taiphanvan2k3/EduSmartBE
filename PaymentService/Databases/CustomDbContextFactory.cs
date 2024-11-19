using Microsoft.EntityFrameworkCore;

namespace PaymentService.Databases
{
    /// <summary>
    /// Because DataContext has 2 constructors, we need to create a factory to specify which constructor to use
    /// </summary>
    /// <param name="options"></param>
    /// <param name="httpContextAccessor"></param>
    public class CustomDbContextFactory(DbContextOptions<DataContext> options, IHttpContextAccessor httpContextAccessor) : IDbContextFactory<DataContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly DbContextOptions<DataContext> _options = options;

        public DataContext CreateDbContext()
        {
            return new DataContext(_options, _httpContextAccessor);
        }
    }
}