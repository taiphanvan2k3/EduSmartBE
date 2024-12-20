using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using PaymentService.Commons;
using PaymentService.Commons.Schemas;
using PaymentService.Enumerations;
using PaymentService.GrpcServices;
using PaymentService.Services.Grpc.CourseService.Schemas;
using PaymentService.Services.TeacherEarnings.Schemas;
using CourseEnrollmentRequestGrpc = PaymentService.GrpcServices.CourseEnrollmentRequest;

namespace PaymentService.Services.Grpc.CourseService
{
    public interface IGrpcCourseService
    {
        /// <summary>
        /// Call to the gRPC CourseService to enroll course
        /// <para>Created by TaiPV</para>
        /// <para>Created at: 2024/11/23</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> EnrollCourse(CourseEnrollmentDto courseEnrollmentDto);

        /// <summary>
        /// Get course info by ids (base on related info)
        /// <para>Created by ManhTD</para>
        /// <para>Created at: 2024/12/20</para>
        /// </summary>
        /// <param name="revenueCourses"></param>
        /// <returns></returns>
        public Task<ResponseInfo> GetInfoCourseByIds(List<RevenueCourse> revenueCourses);

        /// <summary>
        /// Get courses by ids
        /// <para>Created by TaiPV</para>
        /// <para>Created at: 2024/12/20</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetCoursesByIds(List<Guid> courseIds);
    }

    public class GrpcCourseService : BaseService, IGrpcCourseService
    {
        private readonly GrpcChannel _channel;

        public GrpcCourseService(IServiceProvider serviceProvider, ILogger<GrpcCourseService> logger)
            : base(serviceProvider, logger)
        {
            var configuration = serviceProvider.GetService<IConfiguration>()
                 ?? throw new InvalidOperationException(ServiceInjectionError("IConfiguration"));
            _channel = GrpcChannel.ForAddress(configuration["ExternalServices:CourseService:GrpcUrl"]);
        }

        public async Task<ResponseInfo> EnrollCourse(CourseEnrollmentDto courseEnrollmentDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var client = new Course.CourseClient(_channel);

                var request = new CourseEnrollmentRequestGrpc()
                {
                    CourseId = courseEnrollmentDto.CourseId,
                    StudentId = courseEnrollmentDto.StudentId,
                    EnrollmentDate = Timestamp.FromDateTimeOffset(courseEnrollmentDto.EnrollmentDate)
                };

                var courseEnrollmentResponse = await client.EnrollCourseAsync(request);
                if (courseEnrollmentResponse.IsSuccess)
                {
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                    responseInfo.Message = courseEnrollmentResponse.Message;
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = courseEnrollmentResponse.Message;
                }

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetCoursesByIds(List<Guid> courseIds)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);

                var client = new Course.CourseClient(_channel);
                var grpcResponse = await client.GetCoursesByIdsAsync(new GetCoursesByIdsRequest
                {
                    CourseIds = { courseIds.Select(x => x.ToString()) }
                });

                var responseInfo = new ResponseInfo();
                if (grpcResponse.IsSuccess)
                {
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                    responseInfo.Message = grpcResponse.Message;
                    responseInfo.Data.Add("courses", grpcResponse.Courses.Select(x => new CourseDetail
                    {
                        Id = Guid.Parse(x.Id),
                        Name = x.Name
                    })
                    .ToList());
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = grpcResponse.Message;
                }

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
            finally
            {
                LogInfo("End", methodName);
            }
        }

        public async Task<ResponseInfo> GetInfoCourseByIds(List<RevenueCourse> revenueCourses)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();

                var client = new Course.CourseClient(_channel);

                var request = new GetInfoCourseByIdsRequest();
                request.RevenueCourses.AddRange(revenueCourses.Select(x => new RevenueCourseGrpc
                {
                    Amount = (double)x.Amount,
                    Currency = x.Currency.ToString(),
                    RelatedInfo = new RelatedInfoGrpc
                    {
                        CourseId = x.RelatedInfo.CourseId.ToString(),
                        TeacherId = x.RelatedInfo.TeacherId
                    }
                }).ToList());

                var courseEnrollmentResponse = await client.GetInfoCourseByIdsAsync(request);
                if (courseEnrollmentResponse.IsSuccess)
                {
                    responseInfo.StatusCode = StatusCodes.Status200OK;
                    responseInfo.Message = courseEnrollmentResponse.Message;
                    responseInfo.Data.Add("listOfTotalEarning", courseEnrollmentResponse.RevenueCourses.Select(x => new RevenueCourse
                    {
                        Amount = (decimal)x.Amount,
                        Currency = CurrencyType.VND,
                        CourseName = x.CourseName,
                        RelatedInfo = new RelatedInfo
                        {
                            CourseId = Guid.Parse(x.RelatedInfo.CourseId),
                            TeacherId = x.RelatedInfo.TeacherId
                        }
                    }).ToList());
                }
                else
                {
                    responseInfo.StatusCode = StatusCodes.Status400BadRequest;
                    responseInfo.Message = courseEnrollmentResponse.Message;
                }

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}