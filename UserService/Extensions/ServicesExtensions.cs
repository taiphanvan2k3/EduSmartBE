using System.Data.Common;
using System.Text;
using UserService.Commons;
using UserService.Databases;
using UserService.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using UserService.Settings;
using System.Reflection;

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
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1", Description = "API for UserService" });

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

                // Configure để hiển thị chú thích
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);
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
                        builder.WithOrigins("http://localhost:3000", "http://localhost:3030",
                                "https://edu-smart-dut.vercel.app", "https://edu-smart-admin.vercel.app",
                                "https://edu-smart-web-client.vercel.app")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); // Cho phép client gửi cookie qua cross-origin
                    });
            });

            return services;
        }

        public static IServiceCollection AddCloudinarySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            return services;
        }

        public static IServiceCollection AddMinioSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MinioSetting>(options =>
            {
                var section = configuration.GetSection("MinioSetting");
                options.Host = configuration["MINIO_HOST"] ?? section["Host"];
                options.AccessKey = configuration["MINIO_ACCESS_KEY"] ?? section["AccessKey"];
                options.SecretKey = configuration["MINIO_SECRET_KEY"] ?? section["SecretKey"];
                options.Bucket = configuration["MINIO_BUCKET"] ?? section["Bucket"];
                var sslVal = configuration["MINIO_SSL"];
                options.SSL = !string.IsNullOrEmpty(sslVal) ? bool.Parse(sslVal) : (bool.TryParse(section["SSL"], out var result) ? result : true);
            });

            return services;
        }
    }
}