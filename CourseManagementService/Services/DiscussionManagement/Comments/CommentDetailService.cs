using AutoMapper;
using CourseManagementService.BackgroundServices;
using CourseManagementService.Common;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Database.Schemas.NotificationEntities;
using CourseManagementService.Enumerations;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions;
using CourseManagementService.Services.Gemini;
using CourseManagementService.Services.Grpc.UserService;
using CourseManagementService.Services.NotificationManagement.Schemas;
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
        /// Get detail of a comment
        /// <para>Created at: 2024/12/25</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="commentId">Id of the comment</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetCommentDetail(Guid commentId);

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
        public Task<ResponseInfo> UpdateReactionOfComment(Guid commentId, ReactionRequestDto reactionRequestDto);

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
        private readonly IUserContentValidationService _userContentValidationService = serviceProvider.GetService<IUserContentValidationService>()
            ?? throw new ArgumentNullException(nameof(IUserContentValidationService));
        private readonly CommonProducer _commonProducer = serviceProvider.GetRequiredService<CommonProducer>()
            ?? throw new InvalidOperationException(ServiceInjectionError("CommonProducer"));

        public async Task<ResponseInfo> GetCommentDetail(Guid commentId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var currentUser = GetCurrentUser();

                var canAccessCommentResponse = await CanAccessComment(commentId, currentUser.UserId);
                if (!canAccessCommentResponse.IsSuccess || canAccessCommentResponse.Data["comment"] == null)
                {
                    return canAccessCommentResponse;
                }

                var teacherIdOfCourse = canAccessCommentResponse.Data["comment"].TeacherId as int?;

                var commentDto = await _context.Comments
                    .Where(c => c.Id == commentId)
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
                        UpdatedAt = c.UpdatedAt,
                    })
                    .FirstOrDefaultAsync();

                var userIds = new List<int> { commentDto.CreatedBy.Id };
                if (commentDto.MentionedUser != null)
                {
                    userIds.Add(commentDto.MentionedUser.Id);
                }

                var userInfos = await _grpcUserService.GetListOfUsers(userIds);
                if (userInfos.Count == 0)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status500InternalServerError, "Cannot get user info");
                }

                string roleInCourseOfCreator = commentDto.CreatedBy.RoleInCourse;
                commentDto.CreatedBy = userInfos.FirstOrDefault(x => x.Id == commentDto.CreatedBy.Id);
                commentDto.CreatedBy.RoleInCourse = roleInCourseOfCreator;

                if (commentDto.MentionedUser != null)
                {
                    commentDto.MentionedUser = userInfos.FirstOrDefault(x => x.Id == commentDto.MentionedUser.Id);
                    commentDto.MentionedUser.RoleInCourse = commentDto.MentionedUser.Id == teacherIdOfCourse ? "Teacher" : "Student";
                }

                return new ResponseInfo(resource: "comment", commentDto);
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
                var responseInfo = new ResponseInfo();
                var currentUser = GetCurrentUser();

                var discussionInfo = await _context.Discussions
                    .Where(x => x.Id == commentCreateDto.DiscussionId)
                    .Select(x => new
                    {
                        x.CourseId,
                        x.LessonId,
                        OwnerId = x.CreatedBy,
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

                await ValidateCommentContent(commentCreateDto.Content, responseInfo);
                if (!responseInfo.IsSuccess)
                {
                    return responseInfo;
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

                responseInfo.Data.Add("comment", commentDto);

                await _cacheService.RemoveDataByPattern($"DiscussionInCourse:{discussionInfo.CourseId}:{currentUser.UserId}_ReplyByMe");

                var receiverId = commentCreateDto.MentionedUserId ?? discussionInfo.OwnerId;
                if (receiverId != currentUser.UserId)
                {
                    await _commonProducer.EnqueueDataAsync(new BackgroundJobData()
                    {
                        JobType = BackgroundJobType.CreateNotification,
                        Data = new Dictionary<string, dynamic>
                        {
                            {"notificationCreateDto", new NotificationCreateDto()
                            {
                                Type = commentCreateDto.MentionedUserId.HasValue ? NotificationType.Mention : NotificationType.CommentInDiscussion,
                                ReceiverId = commentCreateDto.MentionedUserId ?? discussionInfo.OwnerId,
                                SenderInfo = new SenderInfo()
                                {
                                    Id = currentUser.UserId,
                                    Username = currentUser.UserName,
                                    FullName = currentUser.FullName,
                                    IsSystem = false,
                                    IsTeacher = commentEntity.RoleOfUser == "Teacher"
                                },
                                RelatedEntityId = commentEntity.Id.ToString(),
                                RelatedEntityType = RelatedEntityType.Comment,
                                CourseId = discussionInfo.CourseId,
                                MetaData = new Dictionary<string, string>
                                {
                                    {"DiscussionId", commentCreateDto.DiscussionId.ToString()},
                                    {"LessonId", discussionInfo.LessonId.ToString()}
                                }
                            }}
                        }
                    });
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

        public async Task<ResponseInfo> UpdateComment(CommentUpdateDto commentUpdateDto)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var responseInfo = new ResponseInfo();
                var currentUser = GetCurrentUser();

                var canModifyCommentResponse = await CanModifyComment(commentUpdateDto.Id, currentUser.UserId);
                if (!canModifyCommentResponse.IsSuccess)
                {
                    return canModifyCommentResponse;
                }

                var commentEntity = canModifyCommentResponse.Data["comment"] as TblComment;

                await ValidateCommentContent(commentUpdateDto.Content, responseInfo);
                if (!responseInfo.IsSuccess)
                {
                    return responseInfo;
                }

                commentEntity.Content = commentUpdateDto.Content;
                await _context.SaveChangesAsync();

                var commentDto = _mapper.Map<CommentDetail>(commentEntity);
                var createdBy = await _grpcUserService.GetUserInfoWithRole(commentEntity.CreatedBy);

                if (createdBy != null)
                {
                    commentDto.CreatedBy = createdBy;
                }

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

        public async Task<ResponseInfo> UpdateReactionOfComment(Guid commentId, ReactionRequestDto reactionRequestDto)
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
                        x.ParentId,
                        x.DiscussionId,
                        x.CreatedBy,
                        x.Discussion.CourseId,
                        x.Discussion.LessonId
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
                        await _commonProducer.EnqueueDataAsync(new BackgroundJobData()
                        {
                            JobType = BackgroundJobType.CreateNotification,
                            Data = new Dictionary<string, dynamic>
                            {
                                {"notificationCreateDto", new NotificationCreateDto()
                                {
                                    Type = NotificationType.Reaction,
                                    ReceiverId = commentEntity.CreatedBy,
                                    SenderInfo = new SenderInfo()
                                    {
                                        Id = currentUser.UserId,
                                        Username = currentUser.UserName,
                                        FullName = currentUser.FullName,
                                        IsSystem = false,
                                        IsTeacher = currentUser.IsTeacher
                                    },
                                    RelatedEntityId = commentId.ToString(),
                                    RelatedEntityType = RelatedEntityType.Comment,
                                    CourseId = commentEntity.CourseId,
                                    MetaData = new Dictionary<string, string>
                                    {
                                        {"DiscussionId", commentEntity.DiscussionId.ToString()},
                                        {"LessonId", commentEntity.LessonId.ToString()}
                                    }
                                }}
                            }
                        });
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

        private async Task<ResponseInfo> CanAccessComment(Guid commentId, int userId)
        {
            var commentInfo = await _context.Comments
                .Where(x => x.Id == commentId)
                .Select(x => new
                {
                    x.Discussion.Course.TeacherId,
                    x.CreatedBy,
                    IsEnrolled = x.Discussion.Course.Enrollments.Any(e => e.StudentId == userId && !e.LeaveDate.HasValue),
                    RoleInCourse = x.Discussion.Course.TeacherId == userId ? "Teacher" : "Student"
                })
                .FirstOrDefaultAsync();

            if (commentInfo == null)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Comment not found");
            }

            if (commentInfo.CreatedBy != userId && commentInfo.TeacherId != userId && !commentInfo.IsEnrolled)
            {
                return CreateEarlyResponseInfo(StatusCodes.Status403Forbidden, "You do not have permission to access this comment");
            }

            return new ResponseInfo(resource: "comment", commentInfo);
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

        private async Task ValidateCommentContent(string content, ResponseInfo responseInfo)
        {
            var geminiResponse = await _userContentValidationService.ValidateUserContentAsync(content);
            if (!geminiResponse.IsSuccess)
            {
                responseInfo.StatusCode = geminiResponse.StatusCode;
                responseInfo.Message = geminiResponse.Message;
            }
        }
    }
}