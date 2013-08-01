using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcExtras.ActionResults;
using MvcExtras.Utils;

namespace MvcExtras.Controllers
{
    /// <summary>
    /// Adds some additional functionality to an MVC controller
    /// </summary>
    class ControllerPlus : Controller
    {
        /// <summary>
        /// Returns a HTTP 403 forbidden ActionResult.
        /// </summary>
        /// <param name="message">The message to include.</param>
        /// <returns></returns>
        protected HttpForbiddenResult Forbidden(string message)
        {
            return new HttpForbiddenResult(message);
        }

        /// <summary>
        /// Returns a HTTP 404 not found ActionResult.
        /// </summary>
        /// <param name="message">The message to include.</param>
        /// <returns></returns>
        protected ActionResults.HttpNotFoundResult NotFound(string message)
        {
            return new ActionResults.HttpNotFoundResult(message);
        }

        /// <summary>
        /// Creates a Json response with gzip compression force-enabled.
        /// Especially useful on Azure, where no other way to output gzipped responses is provided.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        protected JsonResult JsonCompressed(object data, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            // force compression (azure doesn't compress json without this)
            Compression.Force(Request, Response);

            return Json(data, behavior);
        }

        /// <summary>
        /// Gets the client (browser) ip address.
        /// </summary>
        /// <returns></returns>
        protected string GetClientIpAddress()
        {
            return Request.GetClientIpAddress();
        }
    }
}
