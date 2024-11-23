using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using PaymentService.Commons;
using PaymentService.GrpcServices;
using PaymentService.Services.Grpc.CourseService.Schemas;
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
    }
}