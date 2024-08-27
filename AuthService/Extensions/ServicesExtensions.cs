using System.Data.Common;
using AuthService.Commons;
using AuthService.Databases;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthService.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING
                )
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors(), ServiceLifetime.Scoped);

                services.AddScoped<DbConnection>(provider =>
                {
                    return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING);
                });
            }
            catch { }

            return services;
        }
    }
}