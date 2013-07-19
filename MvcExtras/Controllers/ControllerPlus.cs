using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcExtras.Utils;

namespace MvcExtras.Controllers
{
    /// <summary>
    /// Adds some additional functionality to an MVC controller
    /// </summary>
    class ControllerPlus : Controller
    {
        /// <summary>
        /// Creates a Json response with gzip compression force-enabled.
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
