using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GreenLine.Filters
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            // Skip if [AllowAnonymous] is present OR user is already authenticated
            bool isAnonymousAction = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                                     filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            
            bool isAuthenticated = filterContext.HttpContext.Request.IsAuthenticated;

            // Also check if we are already on the login page to prevent infinite loops
            string currentUrl = ctx.Request.RawUrl.ToLower();
            string loginUrl = FormsAuthentication.LoginUrl.ToLower().Replace("~/", "/");
            bool isLoginPage = currentUrl.Contains(loginUrl) || currentUrl.Contains("account/login");

            if (!isAnonymousAction && !isLoginPage && !isAuthenticated &&
                (filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), true) ||
                 filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AuthorizeAttribute), true)))
            {
                // Check if session exists or is new but a cookie is present (meaning it timed out)
                if (ctx.Session != null && ctx.Session.IsNewSession)
                {
                    string sessionCookie = ctx.Request.Headers["Cookie"];
                    if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            // Return 401 for AJAX so the client can handle redirect
                            filterContext.HttpContext.Response.StatusCode = 401;
                            filterContext.Result = new JsonResult
                            {
                                Data = new { Error = "SessionTimeout", LogOnUrl = FormsAuthentication.LoginUrl },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                        {
                            // Redirect to custom unauthorized page instead of directly to login
                            string unauthorizedUrl = "~/Error/Unauthorized?returnUrl=" + ctx.Server.UrlEncode(ctx.Request.RawUrl);
                            filterContext.Result = new RedirectResult(VirtualPathUtility.ToAbsolute(unauthorizedUrl));
                        }
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
