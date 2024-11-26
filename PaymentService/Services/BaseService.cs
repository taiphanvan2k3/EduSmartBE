using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PaymentService.Commons;
using PaymentService.Databases;
using PaymentService.Services.AppState;
using PaymentService.Services.AppState.Schemas;

namespace PaymentService.Services
{
    public class BaseService
    {
        protected readonly DataContext _context;
        protected IHttpContextAccessor _httpContextAccessor;
        protected readonly IDbContextFactory<DataContext> _dbContextFactory;
        protected ILogger _logger;
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

        protected virtual void LogError(Exception exception, [CallerMemberName] string method = null)
        {
            _logger?.LogError(exception, "[{Type}] [{Method}] {Message}", GetType().Name, method,
                exception.InnerException?.Message ?? exception.Message);
        }

        protected UserInfoState GetCurrentUser()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (currentUser == null || currentUser.Identity == null || !currentUser.Identity.IsAuthenticated)
            {
                return null;
            }

            return new UserInfoState
            {
                UserId = int.Parse(currentUser.FindFirst("userId")?.Value ?? "0"),
                UserName = currentUser.FindFirst("username")?.Value,
                Email = currentUser.FindFirst(ClaimTypes.Email)?.Value,
                Roles = currentUser.FindFirst(ClaimTypes.Role)?.Value.Split(',').ToList()
            };
        }

        protected static ResponseInfo CreateEarlyResponseInfo(int statusCode, string error = "", string message = "")
        {
            if (string.IsNullOrEmpty(error))
            {
                error = statusCode switch
                {
                    StatusCodes.Status400BadRequest => "BadRequest",
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status403Forbidden => "Forbidden",
                    StatusCodes.Status404NotFound => "NotFound",
                    StatusCodes.Status500InternalServerError => "InternalServerError",
                    _ => "UnknownError"
                };
            }

            return new ResponseInfo
            {
                StatusCode = statusCode,
                Error = error,
                Message = message
            };
        }
    }
}