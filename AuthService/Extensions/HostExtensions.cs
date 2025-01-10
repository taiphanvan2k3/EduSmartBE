using System.Reflection;

using AuthService.Services.AppState;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Serilog;

namespace AuthService.Extensions
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
                    // Tự động đăng ký các Model nằm trong thư mục Models
                    var assembly = Assembly.GetExecutingAssembly();
                    containerBuilder.RegisterAssemblyTypes(assembly)
                        .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal) && t.Namespace.Contains("Services", StringComparison.Ordinal))
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope(); // lifetime scope

                    // Đăng ký AppStateService mà không cần interface
                    containerBuilder.RegisterType<AppStateService>().AsSelf().InstancePerLifetimeScope();
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