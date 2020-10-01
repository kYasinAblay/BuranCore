using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Buran.Core.Library.Utils;

namespace Buran.Core.MvcLibrary.Utils
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            if (viewName.IsEmpty())
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            controller.ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);
                if (viewResult.Success == false)
                    return $"{viewName} bulunamadı";
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View,
                    controller.ViewData, controller.TempData, writer, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }

        public static async Task<string> RenderViewAsync<TModel>(this ViewContext controller, string viewName, TModel model)
        {
            var viewData = new ViewDataDictionary<TModel>(metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
            {
                Model = model
            };
            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller, viewName, false);
                if (viewResult.Success == false)
                    return $"{viewName} bulunamadı";
                ViewContext viewContext = new ViewContext(controller.GetActionContext(), viewResult.View, viewData, controller.TempData, writer, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }

        public static ActionContext GetActionContext(this ViewContext controller)
        {
            return new ActionContext(controller.HttpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
