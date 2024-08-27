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
                builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();

                var logger = new LoggerConfiguration()
                    .MinimumLevel.Warning()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.WithProperty("ApplicationContext", serviceName)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine("Logs", $"{serviceName}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14, rollOnFileSizeLimit: true)
                    .CreateLogger();
                builder.Logging.AddSerilog(logger);
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
                builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
                {
                    // builder.RegisterType<UserModel>().As<IUserModel>();
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