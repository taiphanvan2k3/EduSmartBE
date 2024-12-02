using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase;
using DiscussionSearchCondition = CourseManagementService.Services.DiscussionManagement.Discussions.Schemas.SearchCondition;
using MyDiscussionsSearchCondition = CourseManagementService.Services.DiscussionManagement.Discussions.Schemas.DiscussionSearchCondition;

namespace CourseManagementService.Services.DiscussionManagement.Discussions
{
    public interface IListOfDiscussionsService
    {
        /// <summary>
        /// Get list of discussions in a lesson
        /// <para>Created at: 2024/11/29</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<PaginatedList<DiscussionInfo>> GetDiscussionsInLesson(Guid lessonId, DiscussionSearchCondition searchCondition);

        /// <summary>
        /// Get my discussions in a course or all discussions in a course (for teacher)
        /// <para>Created at: 2024/12/02</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetMyDiscussions(MyDiscussionsSearchCondition searchCondition);
    }

    public class ListOfDiscussionsService(IServiceProvider serviceProvider, ILogger<IListOfDiscussionsService> logger)
        : BaseService(serviceProvider, logger), IListOfDiscussionsService
    {
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ICacheService)));

        public async Task<PaginatedList<DiscussionInfo>> GetDiscussionsInLesson(Guid lessonId, DiscussionSearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();
                if (!await _lessonBaseDetailService.CanAccessLessonMaterial(lessonId, currentUser.UserId))
                {
                    throw new UnauthorizedAccessException("You are not allowed to access this resource");
                }

                // BUG: Cần sử dụng () để đảm bảo thứ tự thực hiện của các toán tử logic
                // nếu không thì nó sẽ (x.LessonId == lessonId && searchCondition.TargetUserType == TargetUserType.Me) ? ... : ...
                var discussions = await _context.Discussions
                    .Where(x => x.LessonId == lessonId && (searchCondition.TargetUserType == TargetUserType.Me
                        ? x.CreatedBy == currentUser.UserId : x.CreatedBy != currentUser.UserId))
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new DiscussionInfo()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        IsAnswered = x.IsAnswered,
                        Type = new LookupDto()
                        {
                            Id = x.Type.Id.ToString(),
                            Name = x.Type.Name
                        },
                        CreatedAt = x.CreatedAt
                    })
                    .ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                return discussions;
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

        public async Task<ResponseInfo> GetMyDiscussions(MyDiscussionsSearchCondition searchCondition)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var coursePermission = await _lessonBaseDetailService.CheckCoursePermission(searchCondition.CourseId, currentUser.UserId);
                if (!coursePermission.IsSuccess)
                {
                    return coursePermission;
                }

                var isTeacher = coursePermission.Data["isTeacher"] as bool? ?? false;
                var cacheKey = CacheManager.DiscussionInCourse.Key(searchCondition.CourseId, currentUser.UserId,
                    searchCondition.Filter.ToString(), searchCondition.CurrentPage, searchCondition.PageSize);
                var cacheData = _cacheService.GetData<PaginatedList<LessonWithDiscussion>>(cacheKey);
                if (cacheData != null)
                {
                    return CreateResponseInfo("discussions", cacheData);
                }

                var query = _context.Discussions.AsQueryable();
                if (searchCondition.Filter == DiscussionFilter.CreatedByMe || searchCondition.Filter == DiscussionFilter.ReplyByMe)
                {
                    query = query.Where(d => d.CourseId == searchCondition.CourseId &&
                        ((searchCondition.Filter == DiscussionFilter.CreatedByMe && d.CreatedBy == currentUser.UserId)
                            || (searchCondition.Filter == DiscussionFilter.ReplyByMe && d.CreatedBy != currentUser.UserId && d.Comments.Any(c => c.CreatedBy == currentUser.UserId))));
                }
                else if (searchCondition.Filter == DiscussionFilter.ForTeacher)
                {
                    if (!isTeacher)
                    {
                        return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You are not allowed to access this resource");
                    }

                    query = query.Where(d => d.CourseId == searchCondition.CourseId);
                }

                var discussions = await query
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new LessonWithDiscussion()
                    {
                        Lesson = new LookupDto()
                        {
                            Id = d.LessonId.ToString(),
                            Name = d.Lesson.Title
                        },
                        Chapter = new SimpleChapterInfo()
                        {
                            Order = d.Lesson.Chapter.Order,
                            Name = d.Lesson.Chapter.Name
                        },
                        Discussion = new DiscussionInfo()
                        {
                            Id = d.Id,
                            Title = d.Title,
                            IsAnswered = d.IsAnswered,
                            Type = new LookupDto()
                            {
                                Id = d.Type.Id.ToString(),
                                Name = d.Type.Name
                            },
                            CreatedAt = d.CreatedAt
                        }
                    })
                    .ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                var expireTime = isTeacher ? CacheManager.DiscussionInCourse.ExpireTimeInMinutesForTeacher
                    : CacheManager.DiscussionInCourse.ExpireTimeInMinutesForStudent;
                _cacheService.SetData(cacheKey, discussions, DateTimeOffset.Now.AddMinutes(expireTime));

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("discussions", discussions);
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
    }
}