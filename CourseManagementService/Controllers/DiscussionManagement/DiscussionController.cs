using CourseManagementService.Common;
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
    public class DiscussionController(
        IListOfDiscussionsService listOfDiscussionsService,
        IDiscussionDetailService discussionDetailService,
        IListOfCommentsService listOfCommentsService,
        ICommentDetailService commentDetailService) : BaseController
    {
        private readonly IListOfDiscussionsService _listOfDiscussionsService = listOfDiscussionsService
            ?? throw new ArgumentNullException(nameof(listOfDiscussionsService));
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
        /// <param name="id">Id of discussion</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiscussionDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDiscussion(Guid id)
        {
            var responseInfo = await _discussionDetailService.GetDiscussion(id);
            return HandleResponseInfo(responseInfo, resourceName: "discussion");
        }

        /// <summary>
        /// Get list of my discussions or discussions (for teacher) in a course
        /// <para>Created at: 2024/12/02</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId">Id of course</param>
        /// <param name="searchCondition">Search condition</param>
        // Endpoint này sử dụng đường dẫn tuyệt đối và do vậy cần thêm Order = 10 để xác định thứ tự của controller
        [HttpGet("/course-service/api/courses/{courseId}/discussions", Order = 10)]
        [ProducesResponseType(typeof(PaginatedList<LessonWithDiscussion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDiscussions([FromRoute] Guid courseId,
            [FromQuery] DiscussionSearchCondition searchCondition)
        {
            if (!ModelState.IsValid)
            {
                return GetInvalidModelStateResponse();
            }

            searchCondition.CourseId = courseId;
            var responseInfo = await _listOfDiscussionsService.GetMyDiscussions(searchCondition);
            return HandleResponseInfo(responseInfo, resourceName: "discussions", isWrapperInObject: false);
        }

        /// <summary>
        /// Get list of parent comments of a discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <param name="paramsSearch">Pagination</param>
        /// <returns></returns>
        [HttpGet("{id}/comments")]
        [ProducesResponseType(typeof(PaginatedList<CommentDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComments([FromRoute] Guid id, [FromQuery] ParamsSearch paramsSearch)
        {
            var responseInfo = await _listOfCommentsService.GetListOfComments(id, paramsSearch);
            return HandleResponseInfo(responseInfo, resourceName: "comments", isWrapperInObject: false);
        }

        /// <summary>
        /// Get discussion types which are used to create a discussion
        /// <para>Created at: 2024/11/30</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpGet("types")]
        [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(DiscussionCreateDto), StatusCodes.Status201Created)]
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
        [ProducesResponseType(typeof(CommentDetail), StatusCodes.Status201Created)]
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
        /// <param name="id">Id of discussion</param>
        /// <param name="discussionUpdate">Content of discussion</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DiscussionCreateDto), StatusCodes.Status200OK)]
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
        /// Mark a comment as best answer in a discussion
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <param name="markBestAnswerRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}/best-answer")]
        public async Task<IActionResult> MarkBestAnswer(Guid id, [FromBody] MarkBestAnswerRequest markBestAnswerRequest)
        {
            markBestAnswerRequest.DiscussionId = id;
            var responseInfo = await _discussionDetailService.MarkBestComment(markBestAnswerRequest);
            return HandleResponseInfo(responseInfo, resourceName: "bestAnswerInfo");
        }

        /// <summary>
        /// Restore a delete discussion
        /// <para>Created at: 2024/12/01</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of discussion</param>
        /// <returns></returns>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreDiscussion(Guid id)
        {
            var responseInfo = await _discussionDetailService.RestoreDiscussion(id);
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