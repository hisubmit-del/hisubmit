using HiSubmit.Application.Interfaces.RenderView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Web.Services;

public class RenderViewService(
    IRazorViewEngine razorViewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider,
    IHttpContextAccessor httpContextAccessor)
    : IRenderViewService
{
    public Task<string> RenderViewToStringAsync(string viewName, string folderName = null)
    {
        return RenderViewToStringAsync(viewName, string.Empty, folderName);
    }

    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, string folderName = null)
    {
        var viewPath = GetViewPath(viewName, folderName);
        var actionContext = GetActionContext();

        var viewEngineResult = razorViewEngine.FindView(actionContext, viewPath, isMainPage: false);
        if (!viewEngineResult.Success)
        {
            viewEngineResult = razorViewEngine.GetView("~/", viewPath, isMainPage: false);
            if (!viewEngineResult.Success)
            {
                throw new FileNotFoundException($"Couldn't find '{viewPath}'");
            }
        }

        var view = viewEngineResult.View;
        await using var output = new StringWriter();
        var viewDataDictionary =
            new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewDataDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            output,
            new HtmlHelperOptions());
        await view.RenderAsync(viewContext).ConfigureAwait(false);
        return output.ToString();
    }


    private ActionContext GetActionContext()
    {
        var httpContext = httpContextAccessor?.HttpContext;
        if (httpContext != null)
        {
            return new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
        }

        httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }

    private string GetViewPath(string viewName, string folderName)
    {
        var folder = folderName ?? "Emails";
        var path = $"/Views/{folder}/{viewName}.cshtml";
        return path;
    }
}