using AutoMapper;
using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Extensions;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions;
using CourseManagementService.Services.Grpc.UserService;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Services.DiscussionManagement.Comments
{
    public interface IListOfCommentsService
    {
        /// <summary>
        /// Get list of comments of a discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        public Task<ResponseInfo> GetListOfComments(Guid discussionId, ParamsSearch paramsSearch);

        /// <summary>
        /// Get list of replies of a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> GetListOfReplies(Guid commentId, ParamsSearch paramsSearch);
    }

    public class ListOfCommentsService(IServiceProvider serviceProvider, ILogger<ListOfCommentsService> logger)
        : BaseService(serviceProvider, logger), IListOfCommentsService
    {
        private readonly IMapper _mapper = serviceProvider.GetRequiredService<IMapper>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IMapper)));
        private readonly IDiscussionDetailService _discussionDetailService = serviceProvider.GetRequiredService<IDiscussionDetailService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IDiscussionDetailService)));
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetRequiredService<IGrpcUserService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(IGrpcUserService)));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidDataException(ServiceInjectionError(nameof(ICacheService)));

        public async Task<ResponseInfo> GetListOfComments(Guid discussionId, ParamsSearch paramsSearch)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                if (!await _discussionDetailService.CanAccessDiscussion(discussionId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this discussion");
                }

                var comments = await _context.Comments
                    .Where(c => c.DiscussionId == discussionId
                        && c.ParentId == null
                        && (!c.IsDelFlag || c.CreatedBy == currentUser.UserId))
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDetail
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedBy = new UserDetail
                        {
                            Id = c.CreatedBy,
                            RoleInCourse = c.RoleOfUser
                        },
                        MentionedUser = c.MentionedUserId.HasValue ? new UserDetail
                        {
                            Id = c.MentionedUserId.Value
                        } : null,
                        Reactions = new ReactionsInfo
                        {
                            Count = c.Reactions.Count,
                            Types = c.Reactions.Select(r => r.Type).Distinct().ToList()
                        },
                        ReplyCount = c.Replies.Count,
                        HasReacted = c.Reactions.Any(r => r.UserId == currentUser.UserId),
                        ReactionType = c.Reactions.Where(r => r.UserId == currentUser.UserId).Select(r => r.Type).FirstOrDefault(),
                        IsApproved = c.IsApproved,
                        DiscussionId = c.DiscussionId,
                        ParentId = c.ParentId,
                        CreatedAt = c.CreatedAt,
                        IsDelFlag = c.IsDelFlag,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToPaginatedListAsync(paramsSearch.CurrentPage, paramsSearch.PageSize);

                var createdByIds = comments.Items.Select(c => c.CreatedBy.Id).Distinct().ToList();
                var userInfosData = await _grpcUserService.GetListOfUsers(createdByIds);
                foreach (var comment in comments.Items)
                {
                    var roleInCourse = comment.CreatedBy.RoleInCourse;
                    comment.CreatedBy = userInfosData.FirstOrDefault(u => u.Id == comment.CreatedBy.Id);
                    comment.CreatedBy.RoleInCourse = roleInCourse;
                }

                LogInfo("End", methodName);
                return CreateResponseInfo("comments", comments);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetListOfReplies(Guid commentId, ParamsSearch paramsSearch)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var commentInfo = await _context.Comments
                    .Where(c => c.Id == commentId)
                    .Select(c => new
                    {
                        c.DiscussionId
                    })
                    .FirstOrDefaultAsync();

                if (commentInfo == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Comment not found");
                }

                if (!await _discussionDetailService.CanAccessDiscussion(commentInfo.DiscussionId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this discussion");
                }

                PaginatedList<CommentDetail> comments = await _context.Comments
                    .Where(c => c.ParentId == commentId
                        && (!c.IsDelFlag || c.CreatedBy == currentUser.UserId))
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentDetail
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedBy = new UserDetail
                        {
                            Id = c.CreatedBy,
                            RoleInCourse = c.RoleOfUser
                        },
                        MentionedUser = c.MentionedUserId.HasValue ? new UserDetail
                        {
                            Id = c.MentionedUserId.Value
                        } : null,
                        Reactions = new ReactionsInfo
                        {
                            Count = c.Reactions.Count,
                            Types = c.Reactions.Select(r => r.Type).Distinct().ToList()
                        },
                        ReplyCount = c.Replies.Count,
                        HasReacted = c.Reactions.Any(r => r.UserId == currentUser.UserId),
                        ReactionType = c.Reactions.Where(r => r.UserId == currentUser.UserId).Select(r => r.Type).FirstOrDefault(),
                        IsApproved = c.IsApproved,
                        DiscussionId = c.DiscussionId,
                        ParentId = c.ParentId,
                        CreatedAt = c.CreatedAt,
                        IsDelFlag = c.IsDelFlag,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToPaginatedListAsync(paramsSearch.CurrentPage, paramsSearch.PageSize);

                var createdByIds = comments.Items.Select(c => c.CreatedBy.Id).Distinct().ToList();
                var mentionedUserIds = comments.Items.Where(c => c.MentionedUser != null)
                    .Select(c => c.MentionedUser.Id).Distinct().ToList();
                var userInfosData = await _grpcUserService.GetListOfUsers([.. createdByIds, .. mentionedUserIds]);

                foreach (var comment in comments.Items)
                {
                    var roleInCourse = comment.CreatedBy.RoleInCourse;
                    comment.CreatedBy = userInfosData.FirstOrDefault(u => u.Id == comment.CreatedBy.Id);
                    comment.CreatedBy.RoleInCourse = roleInCourse;

                    if (comment.MentionedUser != null)
                    {
                        comment.MentionedUser = userInfosData.FirstOrDefault(u => u.Id == comment.MentionedUser.Id);
                    }
                }

                LogInfo("End", methodName);
                return CreateResponseInfo("replies", comments);
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }
    }
}