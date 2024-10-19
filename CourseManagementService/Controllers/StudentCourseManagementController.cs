using CourseManagementService.Services.CourseManagement.Student;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    [Route("api/student-course-management")]
    [ApiController]
    [Filters.Auth(Roles = "Student")]
    public class StudentCourseManagementController(IListOfStudentCoursesService listOfStudentCoursesService) : ControllerBase
    {
        private readonly IListOfStudentCoursesService _listOfStudentCoursesService = listOfStudentCoursesService
            ?? throw new ArgumentNullException(nameof(listOfStudentCoursesService));

        /// <summary>
        /// Get all courses that teacher created
        /// <para>Author: TaiPV</para>
        /// <para>Created at: 2024/10/07</para>
        /// </summary>
        /// <response code="200">Return list of courses</response>
        [HttpGet("courses")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEnrolledCourses()
        {
            try
            {
                var course = await _listOfStudentCoursesService.GetEnrolledCourses();
                return Ok(course);
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}