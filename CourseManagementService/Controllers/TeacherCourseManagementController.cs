using Microsoft.AspNetCore.Mvc;
using CourseManagementService.Commons.Helpers;
using CourseManagementService.Services.CourseManagement.Teacher;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;

namespace CourseManagementService.Controllers
{
    [Route("api/teacher-course-management")]
    [ApiController]
    [Filters.Auth(Roles = "Teacher")]
    public class TeacherCourseManagementController(ITeacherCourseDetailService teacherCourseDetailService,
        IListOfTeacherCoursesService listOfTeacherCoursesService) : ControllerBase
    {
        private readonly ITeacherCourseDetailService _teacherCourseDetailService = teacherCourseDetailService
            ?? throw new ArgumentNullException(nameof(teacherCourseDetailService));
        private readonly IListOfTeacherCoursesService _listOfTeacherCoursesService = listOfTeacherCoursesService
            ?? throw new ArgumentNullException(nameof(listOfTeacherCoursesService));

        /// <summary>
        /// Get all courses that teacher created
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/07</para>
        /// </summary>
        /// <remarks>
        /// CourseType
        ///     
        ///     1: Tutorial
        ///     2: Direct - A course that is live and interactive
        ///
        /// CurrencyType
        ///     1: VNĐ
        ///     2: USD
        /// </remarks>
        /// <response code="200">Return list of courses</response>
        [HttpGet("courses")]
        [ProducesResponseType(typeof(List<CourseCreateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCreatedCourses()
        {
            try
            {
                var course = await _listOfTeacherCoursesService.GetOwnCourses();
                return Ok(new
                {
                    courses = course
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
        }

        /// <summary>
        /// Create a new course
        /// <para>Created by: TaiPV</para> 
        /// <para>Created at: 2024/10/02</para>
        /// </summary>
        /// <param name="courseCreateDto">Course information is need for create</param>
        /// <returns></returns>
        /// <remarks>
        /// CourseType
        ///     
        ///     1: Tutorial
        ///     2: Direct - A course that is live and interactive
        ///
        /// CurrencyType
        ///     1: VNĐ
        ///     2: USD
        /// </remarks>
        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto courseCreateDto)
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
                return BadRequest(e.InnerException?.Message ?? e.Message);
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
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] CourseUpdateDto courseUpdateDto)
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
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
        }

        /// <summary>
        /// Delete a course
        /// <para>Created by: TaiPV</para>
        /// <para>Created at: 2024/10/06</para>
        /// </summary>
        /// <param name="id">Id of course is need for delete</param>
        /// <returns></returns>
        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {
            try
            {
                var response = await _teacherCourseDetailService.DeleteCourse(id);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(new
                    {
                        message = response.Message
                    });
                }

                return StatusCode(response.StatusCode, ErrorResponseHelper
                    .GetContentOfAnyError(response.StatusCode, response.Error, response.Message));
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}