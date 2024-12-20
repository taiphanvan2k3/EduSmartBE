using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Commons.Schemas;
using PaymentService.Services.TeacherEarnings;
using PaymentService.Services.TeacherEarnings.Schemas;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/teacher-earnings")]
    [ApiController]
    public class TeacherEarningController(ITeacherEarningService teacherEarningService) : BaseController
    {
        private readonly ITeacherEarningService _teacherEarningService = teacherEarningService
            ?? throw new ArgumentNullException(nameof(teacherEarningService));

        /// <summary>
        /// Get revenue of all courses of teacher
        /// <para>Created at: 2024/12/02</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     1 - VND
        ///     2 - USD
        ///
        /// </remarks>
        /// <response code="200">Revenue of all courses of teacher</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("me/revenue")]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRevenueAllCoursesOfTeacher(int currencyType)
        {
            try
            {
                var response = await _teacherEarningService.GetRevenueAllCoursesOfTeacherAsync(currencyType);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }


        /// <summary>
        /// Get list of revenue of all courses of teacher
        /// <para>Created at: 2024/12/02</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     1 - VND
        ///     2 - USD
        ///
        /// </remarks>
        /// <response code="200">Revenue of all courses of teacher</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("me/revenue-list")]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(ResponseInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListOfRevenueAllCoursesOfTeacher(int currencyType)
        {
            try
            {
                var response = await _teacherEarningService.GetListOfRevenueAllCoursesOfTeacherAsync(currencyType);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get total withdrawal amount of teacher
        /// <para>Created at: 2024/12/02</para>
        /// <para>Created by ManhTD</para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Note: 
        /// 
        ///     This API return total withdrawal amount of teacher in VND
        /// </remarks>
        /// <response code="200">Total withdrawal amount of teacher</response>
        /// <response code="500">Internal server error</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("me/total-withdrawal")]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalWithdrawal()
        {
            try
            {
                var response = await _teacherEarningService.GetTotalWithdrawalAmountOfTeacherAsync();
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get list of revenue of all courses of teacher in a year
        /// <para>Created at: 2024/12/08</para>
        /// <para>Created by: QuyMTX</para>
        /// </summary>
        /// <param name="year"></param>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     2024
        ///     1 - VND
        ///     2 - USD
        ///
        /// </remarks>
        /// <response code="200">Revenue of all courses of teacher in a year</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("me/revenue-in-year")]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(List<RevenueInMonth>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRevenueInYear(int year, int currencyType)
        {
            try
            {
                var response = await _teacherEarningService.GetRevenueInYearAsync(year, currencyType);
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponseHelper.GetContentOfInternalServerResponse(e));
            }
        }

        /// <summary>
        /// Get total withdrawal amount of teacher
        /// <para>Created at: 2024/12/20</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <returns></returns>
        [HttpGet("me/course-payment-history")]
        [Filters.Auth(Roles = "Teacher")]
        [ProducesResponseType(typeof(PaginatedList<CoursePaymentHistory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursePaymentHistory([FromQuery] ParamsSearch paramsSearch)
        {
            var responseInfo = await _teacherEarningService.GetCoursePaymentHistoryAsync(paramsSearch);
            if (responseInfo.IsSuccess)
            {
                var enrollmentHistories = responseInfo.Data["enrollmentHistories"] as PaginatedList<CoursePaymentHistory>;
                return Ok(enrollmentHistories);
            }
            else
            {
                return StatusCode(responseInfo.StatusCode,
                    ErrorResponseHelper.GetContentOfAnyError(responseInfo.StatusCode,
                        responseInfo.Error, responseInfo.Message));
            }
        }
    }
}