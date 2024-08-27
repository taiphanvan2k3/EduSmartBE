using System.Runtime.CompilerServices;
using AuthService.Databases;

namespace AuthService.Models
{
    public class BaseModel
    {
        protected readonly DataContext _context;
        protected ILogger _logger;

        public BaseModel() { }
        
        public BaseModel(IServiceProvider serviceProvider)
        {
            DataContext context = serviceProvider.GetService<DataContext>();
            _context = serviceProvider.GetService<DataContext>();
        }
        protected virtual void LogInfo(string message, [CallerMemberName] string? method = null)
        {
            _logger.LogInformation($"[{GetType().Name}] [{method}] {message}");
        }

        protected virtual void LogError(string message, [CallerMemberName] string? method = null)
        {
            _logger.LogError($"[{GetType().Name}] [{method}] Exception: {message}");
        }
    }
}