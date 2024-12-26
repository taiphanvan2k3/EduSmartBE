using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManagementService.Common;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
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

        /// <summary>
        /// Delete a discussion
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteDiscussion(Guid id);

        /// <summary>
        /// Restore a discussion
        /// <para>Created at: 2024/12/26</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <returns></returns>
        public Task<ResponseInfo> RestoreDiscussion(Guid id);

        /// <summary>
        /// Get discussion types
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<List<LookupDto>> GetDiscussionTypes();

        /// <summary>
        /// Check if user can access discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<bool> CanAccessDiscussion(Guid discussionId, int userId);

        /// <summary>
        /// Get teacher id of course by discussion id
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<int> GetTeacherIdOfCourse(Guid discussionId);

        /// <summary>
        /// Mark a comment as best answer
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="markBestAnswerRequest"></param>
        /// <returns></returns>
        public Task<ResponseInfo> MarkBestComment(MarkBestAnswerRequest markBestAnswerRequest);
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
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ICacheService)));

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

                var teacherIdInCourse = await GetTeacherIdOfCourse(id);
                var userInfo = await _grpcUserService.GetUserInfoWithRole(discussionDto.User.Id);
                discussionDto.User = userInfo;
                discussionDto.User.RoleInCourse = discussionDto.User.Id == teacherIdInCourse
                    ? "Teacher"
                    : "Student";

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

        public async Task<List<LookupDto>> GetDiscussionTypes()
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var discussionTypes = await _context.DiscussionTypes
                    .Select(d => new LookupDto()
                    {
                        Id = d.Id.ToString(),
                        Name = d.Name
                    })
                    .ToListAsync();

                LogInfo("End", methodName);
                return discussionTypes;
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

                await ClearCacheData(courseInfo.CourseId, currentUser.UserId, courseInfo.TeacherId);

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
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no longer permission to update this discussion");
                }

                discussionEntity.Title = discussionUpdate.Title;
                discussionEntity.Content = discussionUpdate.Content;
                await _context.SaveChangesAsync();

                var discussionDto = _mapper.Map<DiscussionCreateDto>(discussionEntity);
                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("discussion", discussionDto);

                var teacherIdInCourse = await GetTeacherIdOfCourse(id);
                await ClearCacheData(discussionEntity.CourseId, currentUser.UserId, teacherIdInCourse);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteDiscussion(Guid id)
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

                if (discussionEntity.CreatedBy != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have permission to delete this discussion");
                }

                if (!await _lessonBaseDetailService.CanAccessLessonMaterial(discussionEntity.LessonId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no longer permission to delete this discussion");
                }

                // Check có comments nào của discussion này không
                var hasComments = await _context.Comments
                    .AnyAsync(dc => dc.DiscussionId == id);
                if (hasComments)
                {
                    discussionEntity.IsDelFlag = true;
                }
                else
                {
                    _context.Discussions.Remove(discussionEntity);
                }

                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("discussionId", id);

                var teacherIdInCourse = await GetTeacherIdOfCourse(id);
                await ClearCacheData(discussionEntity.CourseId, currentUser.UserId, teacherIdInCourse);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> RestoreDiscussion(Guid id)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var discussionEntity = await _context.Discussions
                    .Include(d => d.Type)
                    .Where(d => d.Id == id)
                    .FirstOrDefaultAsync();

                if (discussionEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Discussion not found");
                }

                if (discussionEntity.CreatedBy != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have permission to delete this discussion");
                }

                if (!await _lessonBaseDetailService.CanAccessLessonMaterial(discussionEntity.LessonId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no longer permission to delete this discussion");
                }

                discussionEntity.IsDelFlag = false;
                await _context.SaveChangesAsync();

                var responseInfo = new ResponseInfo();
                var discussionDto = _mapper.Map<DiscussionInfo>(discussionEntity);

                responseInfo.Data.Add("discussion", discussionDto);

                var teacherIdInCourse = await GetTeacherIdOfCourse(id);
                await ClearCacheData(discussionEntity.CourseId, currentUser.UserId, teacherIdInCourse);

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

        public async Task<bool> CanAccessDiscussion(Guid discussionId, int userId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                var canAccess = await _context.Discussions
                    .Where(d => d.Id == discussionId)
                    .Select(d => new
                    {
                        d.Course.TeacherId,
                        d.CreatedBy,
                        IsEnrolled = d.Course.Enrollments.Any(e => e.StudentId == userId && e.LeaveDate == null)
                    })
                    .FirstOrDefaultAsync();

                if (canAccess == null) return false;
                return canAccess.TeacherId == userId || canAccess.IsEnrolled;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<int> GetTeacherIdOfCourse(Guid discussionId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var cachedTeacherId = _cacheService.GetData<int>(CacheManager.TeacherIdOfCourse.KeyFromDiscussionId(discussionId));
                if (cachedTeacherId != 0)
                {
                    return cachedTeacherId;
                }

                var teacherId = await _context.Discussions
                    .Where(d => d.Id == discussionId)
                    .Select(d => d.Course.TeacherId)
                    .FirstOrDefaultAsync();

                _cacheService.SetData(CacheManager.TeacherIdOfCourse.KeyFromDiscussionId(discussionId), teacherId,
                    DateTimeOffset.Now.AddMinutes(CacheManager.TeacherIdOfCourse.ExpireTimeInMinutes));

                LogInfo("End", methodName);
                return teacherId;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> MarkBestComment(MarkBestAnswerRequest markBestAnswerRequest)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var discussionEntity = await _context.Discussions.FindAsync(markBestAnswerRequest.DiscussionId);
                if (discussionEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Discussion not found");
                }

                if (discussionEntity.CreatedBy != currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You have no permission to mark best answer");
                }

                if (discussionEntity.IsAnswered && markBestAnswerRequest.IsTurnOn)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Discussion has already had best answer");
                }

                var commentEntity = await _context.Comments.FindAsync(markBestAnswerRequest.CommentId);
                if (commentEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Comment not found");
                }

                if (commentEntity.DiscussionId != markBestAnswerRequest.DiscussionId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Comment does not belong to this discussion");
                }

                commentEntity.IsApproved = markBestAnswerRequest.IsTurnOn;
                discussionEntity.IsAnswered = markBestAnswerRequest.IsTurnOn;

                await _context.SaveChangesAsync();
                var responseInfo = new ResponseInfo();

                responseInfo.Data.Add("bestAnswerInfo", new
                {
                    DiscussionId = discussionEntity.Id,
                    CommentId = commentEntity.Id,
                    IsBestAnswer = markBestAnswerRequest.IsTurnOn,
                    ParentCommentId = commentEntity.ParentId
                });

                var teacherIdInCourse = await GetTeacherIdOfCourse(markBestAnswerRequest.DiscussionId);
                await ClearCacheData(discussionEntity.CourseId, currentUser.UserId, teacherIdInCourse);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        private async Task ClearCacheData(Guid courseId, int userId, int teacherId)
        {
            List<string> discussionCacheKeys = [
                CacheManager.DiscussionInCourse.PrefixKey(courseId, userId, "CreatedByMe"),
                CacheManager.DiscussionInCourse.PrefixKey(courseId, teacherId, "ForTeacher")
            ];

            await _cacheService.RemoveDataByPatterns(discussionCacheKeys);
        }
    }
}