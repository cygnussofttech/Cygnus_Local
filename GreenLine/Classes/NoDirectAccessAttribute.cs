using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GreenLine.Classes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public string URLACCESS
        {
            get { return Convert.ToString(ConfigurationManager.AppSettings["URLACCESS"]); }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //GeneralFunctions GF = new GeneralFunctions();
            //string SQRY = "select Count(*) as Cnt from CYGNUS_Master_General where codetype='URLACCESS' and StatusCode='Y' and CodeId='" + filterContext.HttpContext.User.Identity.Name + "'";
            //DataTable dataTable = GF.GetDateTableFromQuery(SQRY);         

            //if (dataTable.Rows.Count > 0)
            //{
            //    if (Convert.ToInt32(dataTable.Rows[0]["Cnt"].ToString()) == 0)
            //    {
            string[] URLACCESSArray = URLACCESS == null ? new string[0] : URLACCESS.Split(',').ToArray();

            if (!URLACCESSArray.Any(x => x == filterContext.HttpContext.User.Identity.Name))
            {
                    if ((filterContext.HttpContext.Request.UrlReferrer == null ||
                        filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host) &&
                        filterContext.HttpContext.Request.Url.AbsolutePath.Contains("BillSubmitViaLink") == false)
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                                       RouteValueDictionary(new { controller = "Home", action = "URLRedirect", area = "" }));
               
                    }
            }
            //}
        }
    }

    public class NoCache : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}