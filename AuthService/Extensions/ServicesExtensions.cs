using System.Data.Common;
using AuthService.Commons;
using AuthService.Databases;
using AuthService.Databases.Schemas;
using Microsoft.AspNetCore.Identity;
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
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                        .LogTo(Console.WriteLine, LogLevel.Information),
                    ServiceLifetime.Scoped);

                services.AddIdentity<User, IdentityRole<int>>()
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders();

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