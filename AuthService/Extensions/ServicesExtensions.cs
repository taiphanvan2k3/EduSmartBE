using System.Data.Common;
using System.Reflection;
using System.Text;
using AuthService.Commons;
using AuthService.Databases;
using AuthService.Databases.Schemas;
using AuthService.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace AuthService.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                // Dùng AddDbContextPool để tạo ra một pool các DbContext, giúp tăng hiệu suất trong việc sử dụng các DbContext instances
                services.AddDbContextPool<DataContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING)
                        .EnableSensitiveDataLogging() // cho phép log dữ liệu nhạy cảm
                        .EnableDetailedErrors());

                services.AddScoped<DbConnection>(provider =>
                {
                    return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection") ?? Constants.CONNECTION_STRING);
                });

                // Thêm IDbContextFactory để cho phép tạo ra các instance của DbContext 
                services.AddDbContextFactory<DataContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return services;
        }

        public static IServiceCollection AddMyIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<int>>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false; // không yêu cầu chữ hoa
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true; // yêu cầu xác nhận email trước khi đăng nhập
            });

            // TODO: Cấu hình thời gian sống cho từng loại token thay vì dùng toàn bộ 10 phút
            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromMinutes(10));

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
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService", Version = "v1", Description = "API for AuthService" });

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
    }
}