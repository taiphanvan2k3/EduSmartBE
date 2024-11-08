using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.LessonManagement.VideoLesson;
using CourseManagementService.Services.LessonManagement.VideoLesson.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.LessonManagement
{
    [Route("course-service/api/video-lessons")]
    [ApiController]
    public class VideoLessonController(IVideoLessonDetailService videoLessonDetailService) : BaseController
    {
        private readonly IVideoLessonDetailService _videoLessonDetailService = videoLessonDetailService
            ?? throw new ArgumentNullException(nameof(videoLessonDetailService));

        /// <summary>
        /// Get video lesson by id
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// !!! IMPORTANT:
        ///     
        ///     - Using VideoURLWithSAS to access video content
        /// </remarks>
        [HttpGet("{id}")]
        [Filters.Auth]
        [ProducesResponseType(typeof(VideoLessonDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVideoLesson(Guid id)
        {
            try
            {
                ResponseInfo responseInfo = await _videoLessonDetailService.GetVideoLessonById(id);
                return HandleResponseInfo(responseInfo, resourceName: "lesson");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Create video lesson
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="lessonInfo"></param>
        /// <remarks>
        /// !!! IMPORTANT:
        ///     
        ///     - Maximum file size is 100MB
        /// </remarks>
        [HttpPost]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(VideoLessonDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVideoLesson([FromForm] VideoLessonCreateUpdateDto lessonInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return GetInvalidModelStateResponse();
                }

                var responseInfo = await _videoLessonDetailService.CreateVideoLesson(lessonInfo);
                return HandleResponseInfo(responseInfo, resourceName: "Lesson");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Update video lesson
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of Lesson</param>
        /// <param name="lessonInfo">New information of lesson</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(VideoLessonDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVideoLesson([FromRoute] Guid id, [FromForm] VideoLessonCreateUpdateDto lessonInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return GetInvalidModelStateResponse();
                }

                var responseInfo = await _videoLessonDetailService.UpdateVideoLesson(id, lessonInfo);
                return HandleResponseInfo(responseInfo, resourceName: "Lesson");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Delete video lesson
        /// <para>Created at: 2024/11/03</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of Lesson</param>
        /// <returns></returns>
        [Filters.Auth(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideoLesson(Guid id)
        {
            try
            {
                ResponseInfo responseInfo = await _videoLessonDetailService.DeleteVideoLesson(id);
                return HandleResponseInfo(responseInfo, resourceId: id.ToString());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}