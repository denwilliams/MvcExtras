using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

//namespace MvcExtras.Attributes
//{
//    /// <summary>
//    /// Attribute that is used to restrict access by to an action method
//    /// to logged on users with non-Expired accounts.
//    /// NOTE: use AllowAnonymousAttribute & AllowExpireAccountAttribute to bypass.
//    /// </summary>
//    public class AuthorizePlusAttribute : AuthorizeAttribute
//    {
//        public override void OnAuthorization(AuthorizationContext filterContext)
//        {
//            base.OnAuthorization(filterContext);

//            if (filterContext.Result is HttpUnauthorizedResult)
//            {
//                // USER IS NOT LOGGED ON & . 
//                // YOU PROBABLY WANT TO LEAVE THE DEFAULT ACTION HERE.

//                // DEFAULT ACTION IS TO REDIRECT TO LOGON PAGE...
//            }
//            else if (!filterContext.HttpContext.Request.IsAuthenticated)
//            {
//                // USER IS NOT LOGGED ON AND AllowAnonymous IS DEFINED.
//                // YOU PROBABLY WANT TO LEAVE THE DEFAULT ACTION HERE.

//                // DEFAULT ACTION LEAVE AS.
//            }
//            else if (SecurityContext.Current.LoggedOnUser.AccountLocked
//                        && !filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
//                        && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
//                        && !filterContext.ActionDescriptor.IsDefined(typeof(AllowExpiredAccountAttribute), true)
//                        && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowExpiredAccountAttribute), true)
//                    )
//            {
//                // USER IS LOGGED ON, BUT ACCOUNT HAS EXPIRED DUE TO LACK OF PAYMENT.
//                // ... PAGE DOES NOT HAVE AllowExpiredAccountAttribute DEFINED
//                // SEND USER TO ACCOUNT EXPIRED PAGE

//                filterContext.Result = new RedirectToRouteResult(
//                    new RouteValueDictionary(new
//                    {
//                        controller = "account",
//                        action = "expired"
//                    })
//                );
//            }


//            // USER IS LOGGED ON & PASSED ALL TESTS!
//        }
//    }
//}
