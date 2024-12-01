using AutoMapper;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
using TblComment = CourseManagementService.Database.Schemas.DiscussionEntities.Comment;

namespace CourseManagementService.AutoMapper
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            // Ignore để tránh lỗi do chúng không cùng type
            CreateMap<TblComment, CommentDetail>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => new UserDetail
                {
                    Id = src.CreatedBy,
                    RoleInCourse = src.RoleOfUser
                }))
                .ForMember(dest => dest.MentionedUser, opt => opt.MapFrom(src => !src.MentionedUserId.HasValue ? null : new UserDetail
                {
                    Id = src.MentionedUserId ?? 0,
                }))
                .ForMember(dest => dest.Reactions, opt => opt.Ignore())
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies == null ? 0 : src.Replies.Count))
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => new ReactionsInfo
                {
                    Count = src.Reactions == null ? 0 : src.Reactions.Count,
                    Types = src.Reactions == null ? new List<string>() : src.Reactions.Select(r => r.Type).Distinct().ToList()
                }));
        }
    }
}