using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcExtras.Attributes
{
    /// <summary>
    /// Extends on the existing HandleErrorAttribute by ensuring JSON is returned to XMLHttpRequests,
    /// and provides a log delegate that can be set to handle logging or notification of errors 
    /// </summary>
    public class HandleErrorPlusAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            // Ignore if exception is already handled, or if Custom Errors are not enabled
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }

            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message,
                        stackTrace = Settings.ShowStackTrack ? filterContext.Exception.StackTrace : string.Empty
                    }
                };
            }
            else
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            // Log delegate exists?
            if (Settings.HandleErrorPlugLogDelegate != null)
            {
                // wrap in empty try-catch... don't want it to get double-exception and break MVC
                try
                {
                    Settings.HandleErrorPlugLogDelegate(filterContext.Exception);
                }
                catch {}
            }

            // Tell IIS the error is handled
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
