using System.Runtime.CompilerServices;
using CourseManagementService.Database;

namespace CourseManagementService.GrpcServices
{
    public class GrpcCourseService(DataContext context, ILogger<GrpcCourseService> logger) : Course.CourseBase
    {
        private readonly DataContext _context = context
            ?? throw new ArgumentNullException(nameof(context));

        private readonly ILogger<GrpcCourseService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
    }
}