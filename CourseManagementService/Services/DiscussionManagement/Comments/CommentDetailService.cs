using AutoMapper;
using Azure;
using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions;
using CourseManagementService.Services.Grpc.UserService;
using Microsoft.EntityFrameworkCore;
using TblComment = CourseManagementService.Database.Schemas.DiscussionEntities.Comment;
using TblReaction = CourseManagementService.Database.Schemas.DiscussionEntities.Reaction;

namespace CourseManagementService.Services.DiscussionManagement.Comments
{
    public interface ICommentDetailService
    {
        /// <summary>
        /// Get reactions of a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> GetReactions(Guid commentId);

        /// <summary>
        /// Create a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> CreateComment(CommentCreateDto commentCreateDto);

        /// <summary>
        /// Update a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        public Task<ResponseInfo> UpdateComment(CommentUpdateDto commentUpdateDto);

        /// <summary>
        /// Update a comment reaction
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> UpdateCommentReaction(Guid commentId, ReactionRequestDto reactionRequestDto);

        /// <summary>
        /// Set a comment as deleted
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> DeleteComment(Guid commentId);

        /// <summary>
        /// Restore a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        public Task<ResponseInfo> RestoreComment(Guid commentId);
    }

    public class CommentDetailService(IServiceProvider serviceProvider, ILogger<CommentDetailService> logger)
        : BaseService(serviceProvider, logger), ICommentDetailService
    {
        private readonly IMapper _mapper = serviceProvider.GetService<IMapper>()
            ?? throw new ArgumentNullException(nameof(IMapper));
        private readonly IDiscussionDetailService _discussionDetailService = serviceProvider.GetService<IDiscussionDetailService>()
            ?? throw new ArgumentNullException(nameof(IDiscussionDetailService));
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(nameof(IGrpcUserService));
        private readonly ICacheService _cacheService = serviceProvider.GetService<ICacheService>()
            ?? throw new ArgumentNullException(nameof(ICacheService));

        public async Task<ResponseInfo> GetReactions(Guid commentId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var commentInfo = await _context.Comments
                    .Where(x => x.Id == commentId)
                    .Select(x => new
                    {
                        x.DiscussionId
                    })
                    .FirstOrDefaultAsync();

                if (commentInfo == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Comment not found");
                }

                var reactions = await _context.Reactions
                    .Where(x => x.CommentId == commentId)
                    .Select(x => new ReactionDetail()
                    {
                        User = new UserDetail()
                        {
                            Id = x.UserId
                        },
                        Type = x.Type
                    })
                    .ToListAsync();

                var userIds = reactions.Select(x => x.User.Id).Distinct().ToList();
                var userInfos = await _grpcUserService.GetListOfUsers(userIds);
                var teacherIdOfCourse = await _discussionDetailService.GetTeacherIdOfCourse(commentInfo.DiscussionId);

                foreach (var reaction in reactions)
                {
                    var userInfo = userInfos.FirstOrDefault(x => x.Id == reaction.User.Id);
                    if (userInfo != null)
                    {
                        reaction.User = userInfo;
                        reaction.User.RoleInCourse = reaction.User.Id == teacherIdOfCourse ? "Teacher" : "Student";
                    }
                }

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("reactions", reactions);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> CreateComment(CommentCreateDto commentCreateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var discussionInfo = await _context.Discussions
                    .Where(x => x.Id == commentCreateDto.DiscussionId)
                    .Select(x => new
                    {
                        x.CourseId
                    })
                    .FirstOrDefaultAsync();

                if (discussionInfo == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Discussion not found");
                }

                if (!await _discussionDetailService.CanAccessDiscussion(commentCreateDto.DiscussionId, currentUser.UserId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this discussion");
                }

                var parentComment = commentCreateDto.ParentId.HasValue
                    ? await _context.Comments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == commentCreateDto.ParentId)
                    : null;

                if (parentComment == null && commentCreateDto.ParentId.HasValue)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Parent comment not found");
                }

                var teacherIdOfCourse = await _discussionDetailService.GetTeacherIdOfCourse(commentCreateDto.DiscussionId);

                var commentEntity = new TblComment()
                {
                    Content = commentCreateDto.Content,
                    CreatedBy = currentUser.UserId,
                    DiscussionId = commentCreateDto.DiscussionId,
                    ParentId = commentCreateDto.ParentId,
                    MentionedUserId = commentCreateDto.MentionedUserId,
                    RoleOfUser = currentUser.UserId == teacherIdOfCourse ? "Teacher" : "Student"
                };

                await _context.Comments.AddAsync(commentEntity);
                await _context.SaveChangesAsync();

                var commentDto = _mapper.Map<CommentDetail>(commentEntity);

                var getCreatedUserTask = _grpcUserService.GetUserInfoWithRole(commentEntity.CreatedBy);
                var getMentionedUserTask = commentEntity.MentionedUserId.HasValue
                    ? _grpcUserService.GetUserInfoWithRole(commentEntity.MentionedUserId.Value)
                    : Task.FromResult(new UserDetail());

                await Task.WhenAll(getCreatedUserTask, getMentionedUserTask);

                if (getCreatedUserTask.Result != null)
                {
                    commentDto.CreatedBy = getCreatedUserTask.Result;
                    commentDto.CreatedBy.RoleInCourse = commentEntity.RoleOfUser;
                }

                if (commentEntity.MentionedUserId.HasValue)
                {
                    commentDto.MentionedUser = getMentionedUserTask.Result;
                    commentDto.MentionedUser.RoleInCourse = commentEntity.MentionedUserId == teacherIdOfCourse
                        ? "Teacher" : "Student";
                }

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("comment", commentDto);

                await _cacheService.RemoveDataByPattern($"DiscussionInCourse:{discussionInfo.CourseId}:{currentUser.UserId}_ReplyByMe");

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateComment(CommentUpdateDto commentUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canModifyCommentResponse = await CanModifyComment(commentUpdateDto.Id, currentUser.UserId);
                if (!canModifyCommentResponse.IsSuccess)
                {
                    return canModifyCommentResponse;
                }

                var commentEntity = canModifyCommentResponse.Data["comment"] as TblComment;
                commentEntity.Content = commentUpdateDto.Content;
                await _context.SaveChangesAsync();

                var commentDto = _mapper.Map<CommentDetail>(commentEntity);
                var createdBy = await _grpcUserService.GetUserInfoWithRole(commentEntity.CreatedBy);

                if (createdBy != null)
                {
                    commentDto.CreatedBy = createdBy;
                }

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("comment", commentDto);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> UpdateCommentReaction(Guid commentId, ReactionRequestDto reactionRequestDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var commentEntity = await _context.Comments
                    .Where(x => x.Id == commentId)
                    .Select(x => new
                    {
                        x.ParentId
                    })
                    .FirstOrDefaultAsync();

                if (commentEntity == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Comment not found");
                }

                var reactionEntity = await _context.Reactions
                    .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == currentUser.UserId);
                if (reactionRequestDto.IsTurnOn)
                {
                    if (reactionEntity == null)
                    {
                        var newReactionEntity = new TblReaction()
                        {
                            CommentId = commentId,
                            UserId = currentUser.UserId,
                            Type = reactionRequestDto.Type.ToString()
                        };

                        await _context.Reactions.AddAsync(newReactionEntity);
                    }
                    else
                    {
                        reactionEntity.Type = reactionRequestDto.Type.ToString();
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (reactionEntity != null)
                    {
                        _context.Reactions.Remove(reactionEntity);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Invalid reaction");
                    }
                }

                var responseInfo = new ResponseInfo();
                responseInfo.Data.Add("comment", new
                {
                    commentId,
                    Type = reactionRequestDto.Type.ToString(),
                    reactionRequestDto.IsTurnOn,
                    commentEntity.ParentId
                });

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> DeleteComment(Guid commentId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canModifyCommentResponse = await CanModifyComment(commentId, currentUser.UserId);
                if (!canModifyCommentResponse.IsSuccess)
                {
                    return canModifyCommentResponse;
                }

                var commentEntity = canModifyCommentResponse.Data["comment"] as TblComment;
                commentEntity.IsDelFlag = true;
                await _context.SaveChangesAsync();

                return new ResponseInfo(resource: "comment", new
                {
                    commentId,
                    ParentCommentId = commentEntity.ParentId,
                    commentEntity.DiscussionId,
                    IsDelFlag = true,
                });
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

        public async Task<ResponseInfo> RestoreComment(Guid commentId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                var currentUser = GetCurrentUser();

                var canModifyCommentResponse = await CanModifyComment(commentId, currentUser.UserId);
                if (!canModifyCommentResponse.IsSuccess)
                {
                    return canModifyCommentResponse;
                }

                var commentEntity = canModifyCommentResponse.Data["comment"] as TblComment;
                if (!commentEntity.IsDelFlag)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "Cannot restore a comment that is not deleted");
                }

                commentEntity.IsDelFlag = false;

                await _context.SaveChangesAsync();

                return new ResponseInfo(resource: "comment", new
                {
                    commentId,
                    ParentCommentId = commentEntity.ParentId,
                    commentEntity.DiscussionId,
                    IsDelFlag = false
                });
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

        private async Task<ResponseInfo> CanModifyComment(Guid commentId, int userId)
        {
            var commentEntity = await _context.Comments
                .Where(x => x.Id == commentId && x.CreatedBy == userId)
                .FirstOrDefaultAsync();

            if (commentEntity == null)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Comment not found");
            }

            if (!await _discussionDetailService.CanAccessDiscussion(commentEntity.DiscussionId, userId))
            {
                return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this discussion");
            }

            if (commentEntity.CreatedBy != userId)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to update this comment");
            }

            return new ResponseInfo(resource: "comment", commentEntity);
        }
    }
}