using System.Runtime.CompilerServices;
using AuthService.Databases;
using AuthService.Services.AppState;

namespace AuthService.Services
{
    public class BaseService
    {
        protected readonly DataContext _context;
        protected ILogger _logger;
        protected IHttpContextAccessor _httpContextAccessor;
        protected readonly AppStateService _appStateService;
        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public BaseService() { }

        public BaseService(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetService<DataContext>()
                ?? throw new InvalidOperationException(ServiceInjectionError("DataContext"));
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                ?? throw new InvalidOperationException(ServiceInjectionError("IHttpContextAccessor"));
            _appStateService = serviceProvider.GetService<AppStateService>()
                ?? throw new InvalidOperationException(ServiceInjectionError("AppStateService"));
        }

        public BaseService(IServiceProvider serviceProvider, ILogger logger)
        {
            _context = serviceProvider.GetService<DataContext>()
                ?? throw new InvalidOperationException(ServiceInjectionError("DataContext"));
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                ?? throw new InvalidOperationException(ServiceInjectionError("IHttpContextAccessor"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appStateService = serviceProvider.GetService<AppStateService>()
                ?? throw new InvalidOperationException(ServiceInjectionError("AppStateService"));
        }

        protected static string ServiceInjectionError(string serviceName)
        {
            return $"Service injection error: {serviceName} is null";
        }
        
        protected virtual void LogInfo(string message, [CallerMemberName] string method = null)
        {
            _logger?.LogInformation("[{Type}] [{Method}] {Message}", GetType().Name, method, message);
        }

        protected virtual void LogError(string message, [CallerMemberName] string method = null)
        {
            _logger?.LogError("[{Type}] [{Method}] {Message}", GetType().Name, method, message);
        }

        protected virtual void LogError(Exception exception, [CallerMemberName] string method = null)
        {
            _logger?.LogError(exception, "[{Type}] [{Method}] {Message}", GetType().Name, method,
                exception.InnerException?.Message ?? exception.Message);
        }
    }
}