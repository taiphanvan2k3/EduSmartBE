using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
                    // Tự động đăng ký các Model nằm trong thư mục Models
                    var assembly = Assembly.GetExecutingAssembly();
                    containerBuilder.RegisterAssemblyTypes(assembly)
                        .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains("Services"))
                        .AsImplementedInterfaces()
                        .InstancePerLifetimeScope(); // lifetime scope
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