using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcExtras.Attributes
{
    /// <summary>
    /// Redirects to an "offline" page if the IsOffline setting is true.
    /// </summary>
    public class HandleOfflineAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Settings.IsOffline)
                filterContext.Result = new RedirectResult(Settings.OfflineUrl ?? "/down.html", false);
        }
    }
}
