using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.CategoryManagement;
using CourseManagementService.Services.CategoryManagement.Schemas;
using CourseManagementService.Services.CourseManagement.Public;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("course-service/api/categories", Order = 1)]
    [ApiController]
    public class CategoryManagementController(IListOfCategoriesService listOfCategoriesService,
        IListOfPublicCourseService listOfPublicCourseService) : ControllerBase
    {
        private readonly IListOfCategoriesService _listOfCategoriesService = listOfCategoriesService
            ?? throw new ArgumentNullException(nameof(listOfCategoriesService));
        private readonly IListOfPublicCourseService _listOfPublicCourseService = listOfPublicCourseService
            ?? throw new ArgumentNullException(nameof(listOfPublicCourseService));

        /// <summary>
        /// Get all categories
        /// <para>Created at: 2024/10/23</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _listOfCategoriesService.GetListOfCategories();
                return Ok(categories);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// [Public API] Get list of courses by category
        /// <para>Created at: 2024/10/23</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <remarks>
        /// !!! IMPORTANT:
        ///     
        ///     - This API is public but use authentication to do some actions
        ///     So, PASS the TOKEN in the header if user is logged-in
        /// 
        /// Available sortable fields:
        ///     
        ///     - Price
        ///     - Popularity (number of enrollments)
        ///     - Newest (last updated date)
        /// 
        /// Direction: 
        /// 
        ///     - ASC (default) -> Ascending
        ///     - DESC -> Descending
        /// 
        /// `Keyword` search is applied to course title and description
        /// 
        /// </remarks>
        /// <param name="id">Category id</param>
        /// <param name="condition">Search condition</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Authorize]
        [HttpGet("{id}/courses")]
        [ProducesResponseType(typeof(PaginatedList<CourseDetailWithTeacherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursesByCategory([FromRoute] int id, [FromQuery] PublicCourseSearchCondition condition)
        {
            try
            {
                var courses = await _listOfPublicCourseService.GetCoursesByCategory(id, condition);
                return Ok(courses);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }
    }
}