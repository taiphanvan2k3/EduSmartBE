using Microsoft.AspNetCore.Mvc;
using PaymentService.Commons;
using PaymentService.Commons.Helpers;
using PaymentService.Enumerations;
using PaymentService.Services.TeacherEarnings;

namespace PaymentService.Controllers
{
    [Route("payment-service/api/teacher-earnings")]
    [ApiController]
    public class TeacherEarningController(ITeacherEarningService teacherEarningService) : ControllerBase
    {
        private readonly ITeacherEarningService _teacherEarningService = teacherEarningService
            ?? throw new ArgumentNullException(nameof(teacherEarningService));

        /// <summary>
        /// Get revenue of all courses of teacher
        /// <para>Created at: 2/12/2024</para>
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
        [HttpGet("/me/revenue")]
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
        /// <para>Created at: 2/12/2024</para>
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
        [HttpGet("/me/revenue-list")]
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
        /// <para>Created at: 2/12/2024</para>
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
        [HttpGet("/me/total-withdrawal")]
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
    }
}