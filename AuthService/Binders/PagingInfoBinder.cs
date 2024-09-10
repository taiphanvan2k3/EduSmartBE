using Microsoft.AspNetCore.Mvc.ModelBinding;
using AuthService.Services.Permission.Schemas.Function;

namespace AuthService.Binders
{
    public class PagingInfoModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var query = bindingContext.HttpContext.Request.Query;

            var pageIndexValue = query["page"];
            var pageSizeValue = query["size"];

            var pagingInfo = new PagingInfo();

            if (int.TryParse(pageIndexValue, out int pageIndex))
            {
                pagingInfo.PageIndex = pageIndex;
            }

            if (int.TryParse(pageSizeValue, out int pageSize))
            {
                pagingInfo.PageSize = pageSize;
            }

            bindingContext.Result = ModelBindingResult.Success(pagingInfo);
            return Task.CompletedTask;
        }
    }

}