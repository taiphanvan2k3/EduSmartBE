using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.LessonManagement
{
    [Route("course-service/api/common-lessons", Order = 6)]
    [ApiController]
    public class CommonLessonController(ILessonBaseDetailService lessonBaseDetailService) : ControllerBase
    {
        private readonly ILessonBaseDetailService _lessonBaseDetailService = lessonBaseDetailService
            ?? throw new ArgumentNullException(nameof(lessonBaseDetailService));

        /// <summary>
        /// Get the continue lesson of current user
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="courseId">Id of course</param>
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
    }
}