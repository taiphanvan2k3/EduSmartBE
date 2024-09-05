using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AuthService.Services.MailSender
{
    public class RazorViewService(IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider,
        ILogger<RazorViewService> logger)
    {
        private readonly IRazorViewEngine _viewEngine = viewEngine
            ?? throw new ArgumentNullException(nameof(viewEngine));

        private readonly ITempDataProvider _tempDataProvider = tempDataProvider
            ?? throw new ArgumentNullException(nameof(tempDataProvider));

        private readonly IServiceProvider _serviceProvider = serviceProvider
            ?? throw new ArgumentNullException(nameof(serviceProvider));

        private readonly ILogger<RazorViewService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> RenderViewToStringAsync<TModel>(string viewPath, TModel model)
        {
            try
            {
                var actionContext = new ActionContext(
                new DefaultHttpContext { RequestServices = _serviceProvider },
                new RouteData(),
                new ActionDescriptor());

                using var writer = new StringWriter();
                var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath, isMainPage: false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewPath} does not match any available view");
                }

                var viewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.ToString();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when rendering view to string with error: {0}", e.Message);
                throw;
            }
        }
    }
}