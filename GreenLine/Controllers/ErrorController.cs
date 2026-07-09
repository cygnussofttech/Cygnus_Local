using System;
using System.Web.Mvc;

namespace GreenLine.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {
        // GET: /Error/General
        public ActionResult General()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        // GET: /Error/NotFound
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        // GET: /Error/ServerError
        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        // GET: /Error/Unauthorized
        public ActionResult Unauthorized(string returnUrl)
        {
            Response.StatusCode = 401;
            Response.TrySkipIisCustomErrors = true;
            
            // Prevent Forms Authentication from redirecting 401 to Login page
            Response.SuppressFormsAuthenticationRedirect = true;

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
