using System.Security.Claims;
using CourseManagementService.Services.AppState;
using CourseManagementService.Services.AppState.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseManagementService.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var appStateService = context.HttpContext.RequestServices.GetService<AppStateService>();
            appStateService.UserInfo = new UserInfoState
            {
                UserId = int.Parse(user.FindFirst("userId")?.Value ?? "0"),
                UserName = user.FindFirst("username")?.Value,
                Email = user.FindFirst(ClaimTypes.Email)?.Value,
                Roles = user.FindFirst(ClaimTypes.Role)?.Value.Split(',').ToList()
            };
        }
    }
}