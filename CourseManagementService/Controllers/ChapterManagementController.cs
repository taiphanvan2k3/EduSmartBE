using CourseManagementService.Common;
using CourseManagementService.Filters;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.ChapterManagement.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course/api/chapters")]
    [ApiController]
    public class ChapterManagementController(IListOfChaptersService listOfChaptersService, IChapterDetailService chapterDetailService) : ControllerBase
    {
        private readonly IListOfChaptersService _listOfChaptersService = listOfChaptersService
            ?? throw new ArgumentNullException(nameof(listOfChaptersService));
        private readonly IChapterDetailService _chapterDetailService = chapterDetailService
            ?? throw new ArgumentNullException(nameof(chapterDetailService));
        

        /// <summary>
        /// Get all chapters
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return list of chapters</response>
        /// <response code="201">Created</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Auth(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ChapterDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChapters()
        {
            try
            {
                var chapters = await _listOfChaptersService.GetListOfChapters();
                return Ok(chapters);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Get chapters by course id
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        /// <response code="200">Return list of chapters</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("course/{courseId}")]
        [ProducesResponseType(typeof(List<ChapterDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChaptersByCourseId(string courseId)
        {
            try
            {
                var chapters = await _listOfChaptersService.GetListOfChaptersByCourseId(courseId);
                return Ok(chapters);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Get chapter by id
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        /// <response code="200">Return chapter</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{chapterId}")]
        [ProducesResponseType(typeof(ChapterDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChapterById(string chapterId)
        {
            try
            {
                var chapter = await _listOfChaptersService.GetChapterById(chapterId);
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
        /// </summary>
        /// <param name="chapterDetail"></param>
        /// <returns></returns>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateChapter([FromBody] ChapterDetailCreate chapterDetail)
        {
            try
            {
                var response = await _chapterDetailService.CreateChapter(chapterDetail);
                return StatusCode(response.StatusCode, response);
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
        /// </summary>
        /// <param name="chapterDetail"></param>
        /// <returns></returns>
        /// <response code="200">Return updated chapter</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateChapter([FromBody] ChapterDetail chapterDetail)
        {
            try
            {
                var response = await _chapterDetailService.UpdateChapter(chapterDetail);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Delete chapters
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <param name="chapterIds"></param>
        /// <returns></returns>
        /// <response code="200">Return deleted chapters</response>
        /// <response code="500">Internal server error</response>
        /// <response code="404">Not found</response>
        [HttpDelete("{chapterIds}")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteChapter(List<string> chapterIds)
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

    }
}