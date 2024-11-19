using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PaymentService.Databases
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        // This method is called automatically by the EF Core CLI tools when running 'dotnet ef migrations add' or 'dotnet ef database update'
        // If DataContext only has a constructor that requires DbContextOptions<DataContext, it is unnecessary to implement this method
        // EF finds the implementation of IDesignTimeDbContextFactory in Assembly and uses it to create an instance of DataContext
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);

            return new DataContext(optionsBuilder.Options, null);
        }
    }
}