using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.DiscussionManagement.Comments;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.DiscussionManagement
{
    [Route("course-service/api/comments", Order = 11)]
    [ApiController]
    [Authorize]
    public class CommentController(
        IListOfCommentsService listOfCommentsService,
        ICommentDetailService commentDetailService) : BaseController
    {
        private readonly IListOfCommentsService _listOfCommentsService = listOfCommentsService
            ?? throw new ArgumentNullException(nameof(listOfCommentsService));
        private readonly ICommentDetailService _commentDetailService = commentDetailService
            ?? throw new ArgumentNullException(nameof(commentDetailService));

        /// <summary>
        /// Get list of reactions of a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of comment</param>
        [HttpGet("{id}/reactions")]
        [ProducesResponseType(typeof(List<ReactionDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentReactionsAsync(Guid id)
        {
            var responseInfo = await _commentDetailService.GetReactions(id);
            return HandleResponseInfo(responseInfo, resourceName: "reactions");
        }

        /// <summary>
        /// Get list of reply comments of a comment
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of comment</param>
        /// <param name="paramsSearch">Pagination</param>
        [HttpGet("{id}/reply-comments")]
        [ProducesResponseType(typeof(PaginatedList<CommentDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReplyCommentsAsync([FromRoute] Guid id, [FromQuery] ParamsSearch paramsSearch)
        {
            var responseInfo = await _listOfCommentsService.GetListOfReplies(id, paramsSearch);
            return HandleResponseInfo(responseInfo, resourceName: "replies", isWrapperInObject: false);
        }

        /// <summary>
        /// Update a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of comment</param>
        /// <param name="commentUpdate">Content of comment</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CommentDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCommentAsync(Guid id, [FromBody] CommentUpdateDto commentUpdate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            commentUpdate.Id = id;
            var responseInfo = await _commentDetailService.UpdateComment(commentUpdate);
            return HandleResponseInfo(responseInfo, resourceName: "comment");
        }

        /// <summary>
        /// Update reaction of a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of comment</param>
        /// <param name="reactionRequest">Reaction of comment</param>
        /// <returns></returns>
        [HttpPut("{id}/reactions")]
        public async Task<IActionResult> UpdateCommentReactionAsync(Guid id, [FromBody] ReactionRequestDto reactionRequest)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _commentDetailService.UpdateCommentReaction(id, reactionRequest);
            return HandleResponseInfo(responseInfo, resourceName: "comment");
        }

        /// <summary>
        /// Delete a comment
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of comment</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync(Guid id)
        {
            var responseInfo = await _commentDetailService.DeleteComment(id);
            return HandleResponseInfo(responseInfo, resourceId: id.ToString());
        }
    }
}