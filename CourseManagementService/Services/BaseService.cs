using System.Runtime.CompilerServices;
using CourseManagementService.Database;
using CourseManagementService.Services.AppState;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services
{
    public class BaseService
    {
        protected readonly DataContext _context;
        protected ILogger _logger;
        protected IHttpContextAccessor _httpContextAccessor;
        protected readonly IDbContextFactory<DataContext> _dbContextFactory;
        protected readonly AppStateService _appStateService;
        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public BaseService() { }

        public BaseService(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetService<DataContext>()
                ?? throw new InvalidOperationException("DataContext is null");
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                ?? throw new InvalidOperationException("HttpContextAccessor is null");
            _dbContextFactory = serviceProvider.GetService<IDbContextFactory<DataContext>>()
                ?? throw new InvalidOperationException("DbContextFactory is null");
            _appStateService = serviceProvider.GetService<AppStateService>()
                ?? throw new InvalidOperationException("AppStateService is null");
        }

        public BaseService(IServiceProvider serviceProvider, ILogger logger)
        {
            _context = serviceProvider.GetService<DataContext>()
                ?? throw new InvalidOperationException("DataContext is null");
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                ?? throw new InvalidOperationException("HttpContextAccessor is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContextFactory = serviceProvider.GetService<IDbContextFactory<DataContext>>()
                ?? throw new InvalidOperationException("DbContextFactory is null");
            _appStateService = serviceProvider.GetService<AppStateService>()
                ?? throw new InvalidOperationException("AppStateService is null");
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
    }
}