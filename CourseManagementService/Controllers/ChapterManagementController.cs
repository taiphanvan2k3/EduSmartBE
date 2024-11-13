using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.ChapterManagement.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course-service/api/chapters", Order = 2)]
    [ApiController]
    public class ChapterManagementController(IListOfChaptersService listOfChaptersService, IChapterDetailService chapterDetailService) : BaseController
    {
        private readonly IListOfChaptersService _listOfChaptersService = listOfChaptersService
            ?? throw new ArgumentNullException(nameof(listOfChaptersService));
        private readonly IChapterDetailService _chapterDetailService = chapterDetailService
            ?? throw new ArgumentNullException(nameof(chapterDetailService));

        /// <summary>
        /// Get chapter by id
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// <para>Modified at: 2024/11/06</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of chapter</param>
        /// <returns></returns>
        /// <response code="200">Return chapter</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChapterDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChapterById(Guid id)
        {
            try
            {
                var chapter = await _chapterDetailService.GetChapterById(id);
                if (chapter == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, ErrorResponseHelper.GetContentOfNotFoundResponse("Chapter not found"));
                }

                return Ok(chapter);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Create a new chapter
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// <para>Modified at: 2024/11/07</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        /// <param name="chapterDetail"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateChapter([FromBody] ChapterDetailCreate chapterDetail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return GetInvalidModelStateResponse();
                }

                var responseInfo = await _chapterDetailService.CreateChapter(chapterDetail);
                return HandleResponseInfo(responseInfo, resourceName: "Chapter");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Update chapter
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// <para>Modified at: 2024/11/07</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of chapter</param>
        /// <param name="chapterDetail"></param>
        /// <returns></returns>
        /// <response code="200">Return updated chapter</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateChapter([FromRoute] Guid id, [FromBody] ChapterDetailUpdate chapterDetail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return GetInvalidModelStateResponse();
                }

                chapterDetail.Id = id;
                var responseInfo = await _chapterDetailService.UpdateChapter(chapterDetail);
                return HandleResponseInfo(responseInfo, resourceName: "Chapter");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Delete one or multiple chapters
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// <para>Modified at: 2024/11/07</para>
        /// <para>Modified by: TaiPV</para>
        /// </summary>
        /// <param name="chapterIds"></param>
        /// <returns></returns>
        /// <response code="200">Return deleted chapters</response>
        /// <response code="500">Internal server error</response>
        /// <response code="404">Not found</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpDelete]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteChapters([FromBody] List<Guid> chapterIds)
        {
            try
            {
                var response = await _chapterDetailService.DeleteChapters(chapterIds);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Delete one or multiple chapters
        /// <para>Created at: 2024/11/07</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of chapter</param>
        /// <param name="isPublished">Is published or not</param>
        /// <returns></returns>
        /// <response code="200">Return deleted chapters</response>
        /// <response code="500">Internal server error</response>
        /// <response code="404">Not found</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpPut("{id}/public")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> PublishChapter([FromRoute] Guid id, [FromBody] bool isPublished)
        {
            try
            {
                var responseInfo = await _chapterDetailService.UpdateChapterPublishedStatus(id, isPublished);
                return HandleResponseInfo(responseInfo, resourceName: "Chapter");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}