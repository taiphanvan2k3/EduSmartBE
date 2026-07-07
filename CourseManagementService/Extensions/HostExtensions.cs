using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CourseManagementService.Services.AppState;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.Medias;
using Serilog;

namespace CourseManagementService.Extensions
{
    public static class HostExtensions
    {
        public static WebApplicationBuilder UseCustomLog(this WebApplicationBuilder builder, string serviceName)
        {
            try
            {
                builder.Host.UseSerilog((hostingContext, loggerConfig) =>
                {
                    loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return builder;
        }

        public static WebApplicationBuilder AddAutoFact(this WebApplicationBuilder builder)
        {
            try
            {
                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
                {
                    // Tự động đăng ký các Service nằm trong thư mục Services
                    var assembly = Assembly.GetExecutingAssembly();
                    containerBuilder.RegisterAssemblyTypes(assembly)
                        .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains("Services"))
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope(); // lifetime scope

                    // Đăng ký AppStateService mà không cần interface
                    containerBuilder.RegisterType<AppStateService>().AsSelf().InstancePerLifetimeScope();

                    // Đăng ký MinioVideoService làm IVideoService (thay cho Azure VideoService)
                    containerBuilder.RegisterType<MinioVideoService>().As<IVideoService>().InstancePerLifetimeScope();

                    // Đăng ký MinioPhotoService làm IPhotoService (thay cho Cloudinary PhotoService)
                    containerBuilder.RegisterType<MinioPhotoService>().As<IPhotoService>().InstancePerLifetimeScope();

                    // Chuyển lifetime scope của CacheService thành SingleInstance
                    containerBuilder.RegisterType<CacheService>()
                        .As<ICacheService>()
                        .SingleInstance();                    
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return builder;
        }
    }
}