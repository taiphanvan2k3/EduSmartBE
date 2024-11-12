using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Student;
using CourseManagementService.Services.CourseManagement.Student.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers.CourseManagement
{
    [Route("course-service/api/student-course-management", Order = 5)]
    [ApiController]
    [Filters.Auth(Roles = "Student")]
    public class StudentCourseController(IListOfStudentCoursesService listOfStudentCoursesService,
        IStudentCourseDetailService studentCourseDetailService) : ControllerBase
    {
        private readonly IListOfStudentCoursesService _listOfStudentCoursesService = listOfStudentCoursesService
            ?? throw new ArgumentNullException(nameof(listOfStudentCoursesService));
        private readonly IStudentCourseDetailService _studentCourseDetailService = studentCourseDetailService
            ?? throw new ArgumentNullException(nameof(studentCourseDetailService));

        /// <summary>
        /// Get all courses that student enrolled
        /// <para>Created at: 2024/10/07</para>
        /// <para>Created by: TaiPV</para>
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

        [HttpPost("enrollment")]
        [Filters.Auth(Roles = "Student")]
        public async Task<IActionResult> CreateCourseOrder([FromBody] EnrollCourseRequest request)
        {
            try
            {
                // TODO: Move to another service (PaymentService)
                await _studentCourseDetailService.EnrollCourse(request);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}