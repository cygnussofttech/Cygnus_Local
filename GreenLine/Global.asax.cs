using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GreenLine
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();

        //    // Clear the error and the response so we can return our own custom view
        //    Response.Clear();
        //    Server.ClearError();

        //    // Perform Server-Side Logging
        //    try
        //    {
        //        System.Diagnostics.Debug.WriteLine("Application_Error: " + exception.Message);
        //    }
        //    catch { /* fallback if logging fails */ }

        //    HttpException httpException = exception as HttpException;
        //    RouteData routeData = new RouteData();
        //    routeData.Values.Add("controller", "Error");

        //    if (httpException != null)
        //    {
        //        int httpCode = httpException.GetHttpCode();
        //        Response.StatusCode = httpCode; // Sync response status

        //        switch (httpCode)
        //        {
        //            case 404:
        //                routeData.Values.Add("action", "NotFound");
        //                break;
        //            case 500:
        //                routeData.Values.Add("action", "ServerError");
        //                break;
        //            default:
        //                routeData.Values.Add("action", "General");
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        Response.StatusCode = 500;
        //        routeData.Values.Add("action", "General");
        //    }

        //    // Important: Tell IIS not to use its own custom errors
        //    Response.TrySkipIisCustomErrors = true;

        //    IController errorController = new GreenLine.Controllers.ErrorController();
        //    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        //}

        // Support OutputCache VaryByCustom="Username" for sidebar caching per user
        public override string GetVaryByCustomString(System.Web.HttpContext context, string custom)
        {
            if (string.Equals(custom, "Username", StringComparison.OrdinalIgnoreCase))
            {
                return context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name.ToUpper() : "Anonymous";
            }
            return base.GetVaryByCustomString(context, custom);
        }
    }
}
