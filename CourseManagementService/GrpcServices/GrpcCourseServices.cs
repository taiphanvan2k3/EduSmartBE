using System.Runtime.CompilerServices;
using CourseManagementService.Database;
using CourseManagementService.Services.Cache;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TblCourseEnrollment = CourseManagementService.Database.Schemas.CourseEnrollment;

namespace CourseManagementService.GrpcServices
{
    public class GrpcCourseService(DataContext context, ILogger<GrpcCourseService> logger, ICacheService cacheService)
        : Course.CourseBase
    {
        private readonly DataContext _context = context
            ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<GrpcCourseService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        private readonly ICacheService _cacheService = cacheService
            ?? throw new ArgumentNullException(nameof(cacheService));

        protected static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public override async Task<CourseEnrollmentResponse> EnrollCourse(CourseEnrollmentRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcCourseService] [{Method}] Start", methodName);
                var responseInfo = new CourseEnrollmentResponse();

                var courseId = Guid.Parse(request.CourseId);
                var isEnrolled = await _context.CourseEnrollments
                    .AnyAsync(ce => ce.CourseId == courseId && ce.StudentId == request.StudentId);

                if (isEnrolled)
                {
                    responseInfo.IsSuccess = false;
                    responseInfo.Message = "Student already enrolled in this course";
                }
                else
                {
                    var courseEnrollment = new TblCourseEnrollment
                    {
                        CourseId = courseId,
                        StudentId = request.StudentId,
                        EnrollmentDate = request.EnrollmentDate.ToDateTimeOffset().ToUniversalTime()
                    };

                    await _context.CourseEnrollments.AddAsync(courseEnrollment);
                    await _context.SaveChangesAsync();

                    responseInfo.IsSuccess = true;
                    responseInfo.Message = "Enroll course successfully";

                    // Clear cache for student's owned courses
                    _cacheService.RemoveData(CacheManager.EnrolledCourses.Key(request.StudentId));
                }

                _logger.LogInformation("[GrpcCourseService] [{Method}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcCourseService] [{Method}] Error", methodName);
                throw;
            }
        }
    }
}