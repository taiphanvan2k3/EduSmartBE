using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using CourseManagementService.Common;
using Microsoft.IdentityModel.Tokens;
using CourseManagementService.Database;
using CourseManagementService.Filters;
using System.Reflection;

namespace CourseManagementService.Extensions
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
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "CourseService", Version = "v1" });

                opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                });

                opt.OrderActionsBy(api => api.ActionDescriptor.AttributeRouteInfo.Order.ToString());

                // Chỉ hiển thị lock icon cho các API cần xác thực
                opt.OperationFilter<AuthenticationRequirementOperationFilter>();

                // Configure để hiển thị Enum dưới dạng 1-Active, 2-Inactive
                opt.SchemaFilter<EnumSchemaFilter>();

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
                        builder.WithOrigins("http://localhost:3000", "http://localhost:3030")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); // Cho phép client gửi cookie qua cross-origin
                    });
            });

            return services;
        }
    }
}