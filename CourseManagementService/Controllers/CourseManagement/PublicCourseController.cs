using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.ChapterManagement.Schemas;
using CourseManagementService.Services.CourseManagement.Public;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.TagManagement;
using CourseManagementService.Services.TagManagement.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.CourseManagement
{
    [Route("course-service/api/public-courses", Order = 3)]
    [ApiController]
    public class PublicCourseController(IListOfPublicCourseService listOfPublicCourseService,
        IListOfChaptersService listOfChaptersService,
        IPublicCourseDetailService publicCourseDetailService,
        IListOfTagService listOfTagService) : ControllerBase
    {
        private readonly IListOfPublicCourseService _listOfPublicCourseService = listOfPublicCourseService
            ?? throw new ArgumentNullException(nameof(listOfPublicCourseService));
        private readonly IListOfChaptersService _listOfChaptersService = listOfChaptersService
            ?? throw new ArgumentNullException(nameof(listOfChaptersService));
        private readonly IPublicCourseDetailService _publicCourseDetailService = publicCourseDetailService
            ?? throw new ArgumentNullException(nameof(publicCourseDetailService));

        private readonly IListOfTagService _listOfTagService = listOfTagService
            ?? throw new ArgumentNullException(nameof(listOfTagService));

        /// <summary>
        /// [Public API] Get list of courses by keyword
        /// <para>Created at: 2024/10/22</para>
        /// <para>Created by: TaiPV</para>  
        /// </summary>
        /// <remarks>
        /// NOTE:
        /// 
        ///     - This API is used to search courses by keyword
        ///     - The keyword can be course name or teacher name
        ///     - The length of keyword must be at least 3 characters
        /// </remarks>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ListOfSearchItems), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursesByKeyword([FromQuery] SearchCondition condition)
        {
            if (ModelState.IsValid)
            {
                var courses = await _listOfPublicCourseService.GetCoursesByKeyword(condition);
                return Ok(courses);
            }
            else
            {
                return BadRequest(ErrorResponseHelper.GetContentOfBadRequestResponse(
                    ModelState.Values.SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()));
            }
        }

        /// <summary>
        /// [Public API] Get popular courses
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <remarks>
        /// !!! IMPORTANT:
        /// 
        ///     - This API is public but use authentication to do some actions
        ///     So, PASS the TOKEN in the header if user is logged-in
        /// </remarks>
        [Authorize] // Adding this attribute to the method to send Bearer token when using Swagger 
        [AllowAnonymous]
        [HttpGet("popular")]
        [ProducesResponseType(typeof(PaginatedList<CourseDetailWithTeacherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopularCourses()
        {
            var courses = await _listOfPublicCourseService.GetPopularCourses();
            return Ok(courses);
        }

        /// <summary>
        /// [Public API] Get recommended courses
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <remarks>
        /// !!! IMPORTANT:
        /// 
        ///     - This API is public but use authentication to do some actions
        ///     So, PASS the TOKEN in the header if user is logged-in
        /// </remarks>
        [Authorize] // Adding this attribute to the method to send Bearer token when using Swagger 
        [AllowAnonymous]
        [HttpGet("recommendation")]
        [ProducesResponseType(typeof(PaginatedList<CourseDetailWithTeacherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendedCourses()
        {
            var courses = await _listOfPublicCourseService.GetRecommendedCourses();
            return Ok(courses);
        }

        /// <summary>
        /// [Public API] Get course detail by course id
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Course id</param>
        /// <returns></returns>
        [Authorize]
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid id)
        {
            var courses = await _publicCourseDetailService.GetCourseDetail(id);
            if (courses == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ErrorResponseHelper.GetContentOfNotFoundResponse("Course not found"));
            }
            return Ok(courses);
        }

        /// <summary>
        /// Get list of chapters by course id
        /// <para>Created at: 2024/10/24</para>
        /// <para>Created by: ManhTD</para>
        /// <para>Created at: 2024/11/07</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="id">Id of course</param>
        /// <returns></returns>
        /// <response code="200">Return list of chapters</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [Filters.Auth(Roles = "Teacher")]
        [HttpGet("{id}/chapters")]
        [ProducesResponseType(typeof(List<ChapterDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChaptersByCourseId(Guid id)
        {
            var isCanAccessCourse = await _publicCourseDetailService.CanAccessCourseMaterial(id);
            if (!isCanAccessCourse)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    ErrorResponseHelper.GetContentOfNotFoundResponse("Cannot access course material"));
            }

            var chapters = await _listOfChaptersService.GetListOfChaptersByCourseId(id, isTeacher: true);
            return Ok(chapters);
        }

        /// <summary>
        /// Get list of tags
        /// <para>Created at: 2024/11/14</para>
        /// <para>Created by: ManhTD</para>
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return list of tags</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("tags")]
        [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _listOfTagService.GetTags();
            return Ok(tags);
        }
    }
}