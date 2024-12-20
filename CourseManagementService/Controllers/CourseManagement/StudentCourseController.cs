using CourseManagementService.Common.Helpers;
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
        IStudentCourseDetailService studentCourseDetailService) : BaseController
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
        [ProducesResponseType(typeof(List<EnrolledCourseInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEnrolledCourses()
        {
            var responseInfo = await _listOfStudentCoursesService.GetEnrolledCourses();
            return HandleResponseInfo(responseInfo, resourceName: "courses");
        }

        /// <summary>
        /// View course progress of a student
        /// <para>Created at: 2024/11/24</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpGet("{userId}/courses")]
        [ProducesResponseType(typeof(ListOfEnrolledCourses), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ErrorResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCoursesByUserId([FromRoute] int userId)
        {
            var responseInfo = await _listOfStudentCoursesService.GetCourseProgressOfOtherUser(userId);
            return HandleResponseInfo(responseInfo, resourceName: "courseProgress");
        }

        /// <summary>
        /// Update the visibility status of a course (private/friends/public)
        /// <para>Created at: 2024/11/23</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <response code="200">Return list of courses</response>
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [HttpPut("courses/{courseId}/public-status")]
        public async Task<IActionResult> UpdateCourseStatus([FromRoute] Guid courseId, [FromForm] SingleUpdateVisibilityStatus request)
        {
            var responseInfo = await _studentCourseDetailService.UpdateCourseStatus(courseId, request);
            return HandleResponseInfo(responseInfo, resourceName: "currentStatus");
        }

        /// <summary>
        /// Update the visibility status of all courses (private/friends/public)
        /// <para>Created at: 2024/11/23</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        [HttpPut("courses/public-status")]
        public async Task<IActionResult> UpdateCoursesStatus([FromForm] UpdateAllVisibilityStatus request)
        {
            var responseInfo = await _listOfStudentCoursesService.UpdateAllCourseVisibilityStatus(request);
            return HandleResponseInfo(responseInfo, resourceName: "currentStatus");
        }
    }
}