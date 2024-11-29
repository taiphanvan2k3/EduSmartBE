using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.Common;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using CourseManagementService.Services.Grpc.UserService;
using CourseManagementService.Services.LessonManagement.LessonBase;
using Microsoft.EntityFrameworkCore;
using TblDiscussion = CourseManagementService.Database.Schemas.DiscussionEntities.Discussion;

namespace CourseManagementService.Services.DiscussionManagement.Discussions
{
    public interface IDiscussionDetailService
    {
        /// <summary>
        /// Get discussion by id
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        public Task<ResponseInfo> GetDiscussion(Guid id);

        /// <summary>
        /// Create a new discussion
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="discussionCreate">Information of discussion</param>
        public Task<ResponseInfo> CreateDiscussion(DiscussionCreateDto discussionCreate);

        /// <summary>
        /// Update a discussion
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <param name="discussionUpdate">Information of discussion</param>
        public Task<ResponseInfo> UpdateDiscussion(Guid id, DiscussionUpdateDto discussionUpdate);
    }

    public class DiscussionDetailService(IServiceProvider serviceProvider, ILogger<DiscussionDetailService> logger)
        : BaseService(serviceProvider, logger), IDiscussionDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IGrpcUserService)));

        public async Task<ResponseInfo> GetDiscussion(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var discussionDto = await _context.Discussions
                    .Where(d => d.Id == id)
                    .ProjectTo<DiscussionDetail>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (discussionDto == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Discussion not found");
                }

                if (discussionDto.User.Id != currentUser.UserId)
                {
                    if (!await _lessonBaseDetailService.CanAccessLessonMaterial(discussionDto.LessonId, currentUser.UserId))
                    {
                        return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this lesson");
                    }
                }

                var grpcResponse = await _grpcUserService.GetListOfUsers([discussionDto.User.Id]);
                var userData = grpcResponse.Users.FirstOrDefault();
                discussionDto.User.Username = userData?.UserName;
                discussionDto.User.FullName = userData?.FullName;
                discussionDto.User.AvatarURL = userData?.AvatarURL;
                discussionDto.User.Email = userData?.Email;

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("discussion", discussionDto);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateDiscussion(DiscussionCreateDto discussionCreate)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                if (!await _lessonBaseDetailService.CanAccessLessonMaterial(discussionCreate.LessonId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this lesson");
                }

                var discussionEntity = _mapper.Map<TblDiscussion>(discussionCreate);
                discussionEntity.CreatedBy = currentUser.UserId;

                var courseInfo = await _context.Lessons
                    .Where(l => l.Id == discussionCreate.LessonId)
                    .Select(l => new
                    {
                        l.Chapter.CourseId,
                        l.Chapter.Course.TeacherId
                    })
                    .FirstOrDefaultAsync();

                discussionEntity.CourseId = courseInfo.CourseId;
                discussionEntity.RoleOfUser = currentUser.UserId == courseInfo.TeacherId
                    ? "Teacher"
                    : "Student";

                await _context.Discussions.AddAsync(discussionEntity);
                await _context.SaveChangesAsync();

                var discussionDto = _mapper.Map<DiscussionCreateDto>(discussionEntity);
                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("discussion", discussionDto);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateDiscussion(Guid id, DiscussionUpdateDto discussionUpdate)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var discussionEntity = await _context.Discussions
                    .Where(d => d.Id == id)
                    .FirstOrDefaultAsync();
                if (discussionEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Discussion not found");
                }

                if (!await _lessonBaseDetailService.CanAccessLessonMaterial(discussionEntity.LessonId, currentUser.UserId)
                    || discussionEntity.CreatedBy != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this lesson");
                }

                discussionEntity.Title = discussionUpdate.Title;
                discussionEntity.Content = discussionUpdate.Content;
                await _context.SaveChangesAsync();

                var discussionDto = _mapper.Map<DiscussionCreateDto>(discussionEntity);
                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("discussion", discussionDto);

                LogInfo("End", methodName);
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