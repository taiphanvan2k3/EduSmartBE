using System.Runtime.CompilerServices;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Database;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.Cache;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TblCourseEnrollment = CourseManagementService.Database.Schemas.CourseEnrollment;

namespace CourseManagementService.GrpcServices
{
    public class GrpcCourseService(DataContext context, ILogger<GrpcCourseService> logger, ICacheService cacheService,
        CommonProducer commonProducer)
        : Course.CourseBase
    {
        private readonly DataContext _context = context
            ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<GrpcCourseService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        private readonly ICacheService _cacheService = cacheService
            ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly CommonProducer _commonProducer = commonProducer
            ?? throw new ArgumentNullException(nameof(commonProducer));

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
                        EnrollmentDate = request.EnrollmentDate.ToDateTimeOffset().ToUniversalTime(),
                        VisibilityStatus = CourseProgressVisibility.Private
                    };

                    await _context.CourseEnrollments.AddAsync(courseEnrollment);
                    await _context.SaveChangesAsync();

                    responseInfo.IsSuccess = true;
                    responseInfo.Message = "Enroll course successfully";

                    // Clear cache for student's owned courses
                    _cacheService.RemoveData(CacheManager.EnrolledCourses.Key(request.StudentId));
                    _cacheService.RemoveData(CacheManager.CourseDetail.Key(courseId, request.StudentId));
                    _cacheService.RemoveData(CacheManager.PopularCourses.Key(request.StudentId));
                    _cacheService.RemoveData(CacheManager.RecommendedCourses.Key(request.StudentId));

                    await UnlockFirstLesson(courseId, request.StudentId);
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

        public override async Task<GetInfoCourseByIdsResponse> GetInfoCourseByIds(GetInfoCourseByIdsRequest request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcCourseService] [{Method}] Start", methodName);
                var responseInfo = new GetInfoCourseByIdsResponse();

                var courseIds = request.RevenueCourses.Select(x => x.RelatedInfo.CourseId).Distinct().ToList();

                var courses = await _context.Courses
                    .Where(c => courseIds.Contains(c.Id.ToString()))
                    .Select(c => new
                    {
                        c.Id,
                        c.Name
                    })
                    .ToListAsync();

                foreach (var earning in request.RevenueCourses)
                {
                    var course = courses.FirstOrDefault(c => c.Id.ToString() == earning.RelatedInfo.CourseId);
                    if (course != null)
                    {
                        earning.CourseName = course.Name;
                    }
                }
                responseInfo.RevenueCourses.AddRange(request.RevenueCourses);
                responseInfo.IsSuccess = true;
                responseInfo.Message = "Enroll course successfully";
                _logger.LogInformation("[GrpcCourseService] [{Method}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcCourseService] [{Method}] Error", methodName);
                throw;
            }
        }

        public override async Task<MonthlyDataCourseResponse> GetMonthlyDiscussions(Empty request, ServerCallContext context)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation("[GrpcCourseService] [{Method}] Start", methodName);
                var responseInfo = new MonthlyDataCourseResponse();

                var monthlyDiscussions = await _context.Discussions
                    .Where(d => d.CreatedAt.Year == DateTime.Now.Year)
                    .GroupBy(d => d.CreatedAt.Month)
                    .Select(d => new MonthlyDataCourse
                    {
                        Month = d.Key,
                        Amount = d.Count()
                    })
                    .ToListAsync();

                responseInfo.MonthlyDataCourse.AddRange(monthlyDiscussions);
                responseInfo.IsSuccess = true;
                responseInfo.Message = "Get monthly discussions successfully";

                _logger.LogInformation("[GrpcCourseService] [{Method}] End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcCourseService] [{Method}] Error", methodName);
                throw;
            }
        }

        private async Task UnlockFirstLesson(Guid courseId, int userId)
        {
            try
            {
                _logger.LogInformation("[GrpcCourseService][UnlockFirstLesson] Start");
                await _commonProducer.EnqueueDataAsync(new BackgroundJobData
                {
                    JobType = BackgroundJobType.UnlockFirstLesson,
                    Data = new Dictionary<string, dynamic>
                    {
                        { "courseId", courseId },
                        { "userId",userId },
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[GrpcCourseService][UnlockFirstLesson] Error");
                throw;
            }
        }
    }
}