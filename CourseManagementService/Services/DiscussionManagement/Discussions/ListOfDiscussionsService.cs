using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase;
using DiscussionSearchCondition = CourseManagementService.Services.DiscussionManagement.Discussions.Schemas.SearchCondition;

namespace CourseManagementService.Services.DiscussionManagement.Discussions
{
    public interface IListOfDiscussionsService
    {
        public Task<PaginatedList<DiscussionInfo>> GetDiscussions(Guid lessonId, DiscussionSearchCondition searchCondition);
    }

    public class ListOfDiscussionsService(IServiceProvider serviceProvider, ILogger<IListOfDiscussionsService> logger)
        : BaseService(serviceProvider, logger), IListOfDiscussionsService
    {
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ILessonBaseDetailService)));

        public async Task<PaginatedList<DiscussionInfo>> GetDiscussions(Guid lessonId, DiscussionSearchCondition searchCondition)
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

                var discussions = await _context.Discussions
                    .Where(x => x.LessonId == lessonId && searchCondition.TargetUserType == TargetUserType.Me
                        ? x.CreatedBy == currentUser.UserId : x.CreatedBy != currentUser.UserId)
                    .Select(x => new DiscussionInfo()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        IsAnswered = x.IsAnswered,
                        Type = new LookupDto()
                        {
                            Id = x.Type.Id.ToString(),
                            Name = x.Type.Name
                        }
                    })
                    .ToPaginatedListAsync(searchCondition.CurrentPage, searchCondition.PageSize);

                LogInfo("End", methodName);
                return discussions;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}