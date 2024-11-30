using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.DiscussionManagement.Comments;
using CourseManagementService.Services.DiscussionManagement.Comments.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.DiscussionManagement
{
    [Route("course-service/api/discussions", Order = 10)]
    [ApiController]
    [Authorize]
    public class DiscussionController(IDiscussionDetailService discussionDetailService,
        IListOfCommentsService listOfCommentsService,
        ICommentDetailService commentDetailService) : BaseController
    {
        private readonly IDiscussionDetailService _discussionDetailService = discussionDetailService
            ?? throw new ArgumentNullException(nameof(discussionDetailService));
        private readonly IListOfCommentsService _listOfCommentsService = listOfCommentsService
            ?? throw new ArgumentNullException(nameof(listOfCommentsService));
        private readonly ICommentDetailService _commentDetailService = commentDetailService
            ?? throw new ArgumentNullException(nameof(commentDetailService));

        /// <summary>
        /// Get a discussion by id
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of quiz lesson</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscussion(Guid id)
        {
            var responseInfo = await _discussionDetailService.GetDiscussion(id);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments([FromRoute] Guid id, [FromQuery] ParamsSearch paramsSearch)
        {
            var responseInfo = await _listOfCommentsService.GetListOfComments(id, paramsSearch);
            return HandleResponseInfo(responseInfo, resourceName: "comments");
        }

        /// <summary>
        /// Get discussion types which are used to create a discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpGet("types")]
        public async Task<IActionResult> GetDiscussionTypes()
        {
            var discussionTypes = await _discussionDetailService.GetDiscussionTypes();
            return Ok(discussionTypes);
        }

        /// <summary>
        /// Create a new discussion in a lesson
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateDiscussion([FromBody] DiscussionCreateDto discussionCreate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _discussionDetailService.CreateDiscussion(discussionCreate);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }

        /// <summary>
        /// Add a comment to a discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <param name="commentCreate">Content of comment</param>
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> CreateComment(Guid id, [FromBody] CommentCreateDto commentCreate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            commentCreate.DiscussionId = id;
            var responseInfo = await _commentDetailService.CreateComment(commentCreate);
            return HandleResponseInfo(responseInfo, resourceName: "comment");
        }

        /// <summary>
        /// Update a discussion
        /// <para>Created at: 2024/11/28</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscussion(Guid id, [FromBody] DiscussionUpdateDto discussionUpdate)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            var responseInfo = await _discussionDetailService.UpdateDiscussion(id, discussionUpdate);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }

        /// <summary>
        /// Delete a discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscussion(Guid id)
        {
            var responseInfo = await _discussionDetailService.DeleteDiscussion(id);
            return HandleResponseInfo(responseInfo, resourceId: id.ToString());
        }
    }
}