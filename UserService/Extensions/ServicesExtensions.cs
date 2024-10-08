using System.Data.Common;
using System.Text;
using UserService.Commons;
using UserService.Databases;
using UserService.Databases.Schemas;
using UserService.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace UserService.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING)
                        .EnableSensitiveDataLogging() // cho phép log dữ liệu nhạy cảm
                        .EnableDetailedErrors(),
                    ServiceLifetime.Scoped);

                services.AddScoped<DbConnection>(provider =>
                {
                    return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true; // lưu token vào HttpContext.User
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWTSetting:SecretKey"])),

                    // Yêu cầu token phải có claim iss, aud và chính xác iss, aud đã chỉ định thì mới coi như token đó hợp lệ
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWTSetting:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWTSetting:Audience"],
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });

                opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                });

                // Chỉ hiển thị lock icon cho các API cần xác thực
                opt.OperationFilter<AuthenticationRequirementOperationFilter>();
            });

            return services;
        }

        public static IServiceCollection AddCustomCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); // Cho phép client gửi cookie qua cross-origin
                    });

                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            return services;
        }
    }
}