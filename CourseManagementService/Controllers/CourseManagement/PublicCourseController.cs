using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.CourseManagement.Public;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.CourseManagement
{
    [Route("course-service/api/public-courses")]
    [ApiController]
    public class PublicCourseController(IListOfPublicCourseService listOfPublicCourseService) : ControllerBase
    {
        private readonly IListOfPublicCourseService _listOfPublicCourseService = listOfPublicCourseService
            ?? throw new ArgumentNullException(nameof(listOfPublicCourseService));

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
            try
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
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
    }
}