using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.DiscussionManagement.Discussions;
using CourseManagementService.Services.DiscussionManagement.Discussions.Schemas;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiscussionSearchCondition = CourseManagementService.Services.DiscussionManagement.Discussions.Schemas.SearchCondition;

namespace CourseManagementService.Controllers.LessonManagement
{
    [Route("course-service/api/common-lessons", Order = 6)]
    [ApiController]
    public class CommonLessonController(ILessonBaseDetailService lessonBaseDetailService, IListOfDiscussionsService listOfDiscussionsService) : ControllerBase
    {
        private readonly ILessonBaseDetailService _lessonBaseDetailService = lessonBaseDetailService
            ?? throw new ArgumentNullException(nameof(lessonBaseDetailService));
        private readonly IListOfDiscussionsService _listOfDiscussionsService = listOfDiscussionsService
            ?? throw new ArgumentNullException(nameof(listOfDiscussionsService));

        /// <summary>
        /// Get the continue lesson of current user
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="courseId">Id of course</param>
        /// <remarks>
        /// NOTE: 
        /// 
        ///     Text = 1,
        ///     Video = 2,
        ///     Quiz = 3,
        ///     ProgrammingExercise = 4 
        /// </remarks> 
        [Filters.Auth]
        [HttpGet("next-lesson")]
        [ProducesResponseType(typeof(ContinueLessonInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetContinueLesson(Guid courseId)
        {
            try
            {
                var nextLesson = await _lessonBaseDetailService.GetContinueLessonInfo(courseId);
                return Ok(new
                {
                    nextLesson
                });
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ErrorResponseHelper.GetContentOfUnauthorizedResponse(e.Message));
            }
        }

        [Authorize]
        [HttpGet("{lessonId}/discussions")]
        [ProducesResponseType(typeof(PaginatedList<DiscussionInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDiscussions([FromRoute] Guid lessonId, [FromQuery] DiscussionSearchCondition searchCondition)
        {
            try
            {
                var discussions = await _listOfDiscussionsService.GetDiscussions(lessonId, searchCondition);
                return Ok(discussions);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ErrorResponseHelper.GetContentOfUnauthorizedResponse(e.Message));
            }
        }
    }
}