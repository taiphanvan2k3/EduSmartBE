using System.Runtime.CompilerServices;
using AuthService.Databases;
using Grpc.Core;

namespace AuthService.GrpcServices
{
    public class GrpcAuthServices(DataContext context, ILogger<GrpcAuthServices> logger) : Auth.AuthBase
    {
        private readonly DataContext _context = context
            ?? throw new ArgumentNullException(nameof(context));

        private readonly ILogger<GrpcAuthServices> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public override async Task<SaveUserProfileResponse> SaveUserProfile(SaveUserProfileRequest request, ServerCallContext serverContext)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcAuthServices] [{Method}] Start", methodName);
                var user = await _context.Users.FindAsync(request.Id);
                if (user is null)
                {
                    return new SaveUserProfileResponse
                    {
                        Success = false
                    };
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.AvatarURL = request.AvatarURL;
                user.Phone = request.Phone;
                user.Gender = request.Gender;

                await _context.SaveChangesAsync();

                _logger.LogInformation("[GrpcAuthServices] [{Method}] End", methodName);
                return new SaveUserProfileResponse
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcAuthServices] [{Method}] Error", methodName);
                throw;
            }
        }
    }
}