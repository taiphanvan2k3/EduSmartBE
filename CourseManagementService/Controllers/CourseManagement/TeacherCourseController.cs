using Microsoft.AspNetCore.Mvc;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Services.CourseManagement.Teacher;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Common;

namespace CourseManagementService.Controllers.CourseManagement
{
    [Route("course-service/api/teacher-course-management")]
    [ApiController]
    [Filters.Auth(Roles = "Teacher")]
    public class TeacherCourseController(ITeacherCourseDetailService teacherCourseDetailService,
        IListOfTeacherCoursesService listOfTeacherCoursesService) : ControllerBase
    {
        private readonly ITeacherCourseDetailService _teacherCourseDetailService = teacherCourseDetailService
            ?? throw new ArgumentNullException(nameof(teacherCourseDetailService));
        private readonly IListOfTeacherCoursesService _listOfTeacherCoursesService = listOfTeacherCoursesService
            ?? throw new ArgumentNullException(nameof(listOfTeacherCoursesService));

        /// <summary>
        /// Get all courses that teacher created
        /// <para>Created at: 2024/10/07</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <response code="200">Return list of courses</response>
        [HttpGet("courses")]
        [ProducesResponseType(typeof(List<PaginatedList<CourseCreateDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCreatedCourses([FromQuery] CourseSearchCondition searchCondition)
        {
            try
            {
                var courses = await _listOfTeacherCoursesService.GetOwnCourses(searchCondition);
                return Ok(courses);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException?.Message ?? e.Message);
            }
        }

        /// <summary>
        /// Create a new course
        /// <para>Created by: TaiPV</para> 
        /// <para>Created at: 2024/10/02</para>
        /// </summary>
        /// <param name="courseCreateDto">Course information is need for create</param>
        /// <remarks>
        /// CourseType
        ///     
        ///     1: Tutorial
        ///     2: Direct - A course that is live and interactive
        ///
        /// CurrencyType
        /// 
        ///     1: VNĐ
        ///     2: USD
        /// </remarks>
        [HttpPost("courses")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCourse([FromForm] CourseCreateDto courseCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                    return StatusCode(400, ErrorResponseHelper.GetContentOfBadRequestResponse(modelStateErrors));
                }

                var response = await _teacherCourseDetailService.CreateCourse(courseCreateDto);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    var data = response.Data.TryGetValue("course", out var course) ? course : null;
                    return Ok(new
                    {
                        course = data
                    });
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper
                    .GetContentOfAnyError(response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException?.Message ?? e.Message);
            }
        }

        /// <summary>
        /// Login with UserName or Email and Password
        /// <para>Created by: TaiPV</para> 
        /// <para>Created at: 2024/10/06</para>
        /// </summary>
        /// <param name="id">Course Id is need for update</param>
        /// <param name="courseUpdateDto">Course information is need for update</param>
        /// <returns></returns>
        /// <remarks>
        /// CourseType
        ///     
        ///     1: Tutorial
        ///     2: Direct - A course that is live and interactive
        ///
        /// CurrencyType
        ///     
        ///     1: VNĐ
        ///     2: USD
        /// </remarks>
        [HttpPut("courses/{id}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromForm] CourseUpdateDto courseUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                    return StatusCode(400, ErrorResponseHelper.GetContentOfBadRequestResponse(modelStateErrors));
                }

                var response = await _teacherCourseDetailService.UpdateCourse(id, courseUpdateDto);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    var data = response.Data.TryGetValue("course", out var course) ? course : null;
                    return Ok(new
                    {
                        course = data
                    });
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper
                    .GetContentOfAnyError(response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException?.Message ?? e.Message);
            }
        }

        /// <summary>
        /// Delete a course
        /// <para>Created by: TaiPV</para>
        /// <para>Created at: 2024/10/06</para>
        /// </summary>
        /// <param name="id">Id of course is need for delete</param>
        [HttpDelete("courses/{id}")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {
            try
            {
                var response = await _teacherCourseDetailService.DeleteCourse(id);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new SuccessResponse(response.Message));
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper
                    .GetContentOfAnyError(response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.InnerException?.Message ?? e.Message);
            }
        }
    }
}