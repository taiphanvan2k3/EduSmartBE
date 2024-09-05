using System.Runtime.CompilerServices;
using AuthService.Databases;

namespace AuthService.Services
{
    public class BaseService
    {
        protected readonly DataContext _context;
        protected ILogger _logger;

        public BaseService() { }

        public BaseService(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetService<DataContext>()
                ?? throw new InvalidOperationException("DataContext is null");
        }

        public BaseService(IServiceProvider serviceProvider, ILogger logger)
        {
            _context = serviceProvider.GetService<DataContext>()
                ?? throw new InvalidOperationException("DataContext is null");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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