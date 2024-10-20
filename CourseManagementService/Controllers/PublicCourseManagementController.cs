using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.CourseManagement.Public;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicCourseManagementController(IListOfPublicCourseService listOfPublicCourseService) : ControllerBase
    {
        private readonly IListOfPublicCourseService _listOfPublicCourseService = listOfPublicCourseService
            ?? throw new ArgumentNullException(nameof(listOfPublicCourseService));

        /// <summary>
        /// [Public API] Get popular courses
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        [Authorize] // Adding this attribute to the method to send Bearer token when using Swagger 
        [AllowAnonymous]
        [HttpGet("popular-courses")]
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
        /// <returns></returns>
        [Authorize] // Adding this attribute to the method to send Bearer token when using Swagger 
        [AllowAnonymous]
        [HttpGet("recommended-courses")]
        [ProducesResponseType(typeof(PaginatedList<CourseDetailWithTeacherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendedCourses()
        {
            var courses = await _listOfPublicCourseService.GetRecommendedCourses();
            return Ok(courses);
        }
    }
}