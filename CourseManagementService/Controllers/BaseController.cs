using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementService.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ObjectResult GetInvalidModelStateResponse()
        {
            var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            return StatusCode(StatusCodes.Status400BadRequest, ErrorResponseHelper.GetContentOfBadRequestResponse(modelStateErrors));
        }

        protected dynamic HandleResponseInfo(ResponseInfo responseInfo, string resourceId = "", string resourceName = "")
        {
            if (responseInfo.StatusCode == StatusCodes.Status200OK
                || responseInfo.StatusCode == StatusCodes.Status201Created)
            {
                var data = responseInfo.Data.TryGetValue(resourceName, out var resource) ? resource : null;
                if (data != null)
                {
                    var result = new Dictionary<string, object>
                    {
                        { resourceName, data }
                    };

                    return Ok(result);
                }

                return Ok(new
                {
                    ResourceId = resourceId
                });
            }

            return StatusCode(responseInfo.StatusCode, ErrorResponseHelper
                .GetContentOfAnyError(responseInfo.StatusCode, responseInfo.Error, responseInfo.Message));
        }

        protected dynamic HandleResponseInfoNoResource(ResponseInfo responseInfo)
        {
            if (responseInfo.StatusCode == StatusCodes.Status200OK
                || responseInfo.StatusCode == StatusCodes.Status201Created)
            {
                return Ok(new
                {
                    responseInfo.StatusCode,
                    responseInfo.Message
                });
            }

            return StatusCode(responseInfo.StatusCode, ErrorResponseHelper
                .GetContentOfAnyError(responseInfo.StatusCode, responseInfo.Error, responseInfo.Message));
        }
    }
}