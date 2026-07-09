using System;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using GreenLine.Models;

namespace GreenLine.Security
{
    public class CustomPrincipal : ICustomPrincipal
    {
        private CustomPrincipal() { }

        private CustomPrincipal(ICustomIdentity identity)
        {
            this.Identity = identity;
        }

        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("Role is null");
            }
            return ((ICustomIdentity)Identity).IsInRole(role);
        }


        public static void Logout()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
            }
            HttpContext.Current.User =
                new GenericPrincipal(new GenericIdentity(""), new string[] { });
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <param name="rememberMe">True, if authentication should persist between browser sessions
        /// </param>
        /// <returns>True if login succeeds</returns>
        public static bool Login(LoginModel loginModel, System.Data.DataTable userDetails)
        {
            var identity = CustomIdentity.GetCustomIdentity(loginModel, "", "", userDetails);
            int timeout = 1;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TimeOut"].ToString()))
                timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"].ToString());


            if (identity.IsAuthenticated)
            {
                HttpContext.Current.User = new CustomPrincipal(identity);

                FormsAuthenticationTicket ticket =
                       new FormsAuthenticationTicket(
                           1, identity.Name, DateTime.Now, DateTime.Now.AddHours(timeout), loginModel.RememberMe,
                           identity.ToJson(), FormsAuthentication.FormsCookiePath);
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
                cookie.HttpOnly = true;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                if (loginModel.RememberMe)
                {
                    cookie.Expires = DateTime.Now.AddYears(1);// good for one year
                }

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            return identity.IsAuthenticated;
        }


        public static void SetChangeLocation(string Username, string loccode, string finyear, string compnycode)
        {
            var identity = CustomIdentity.GetCustomIdentity(Username, loccode, finyear, compnycode, "");

            HttpContext.Current.User = new CustomPrincipal(identity);
            bool rememberMe = true;
            FormsAuthenticationTicket ticket =
                   new FormsAuthenticationTicket(
                       1, identity.Name, DateTime.Now, DateTime.Now.AddMinutes(60), rememberMe,
                       identity.ToJson(), FormsAuthentication.FormsCookiePath);
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (rememberMe)
            {
                cookie.Expires = DateTime.Now.AddYears(1);// good for one year
            }

            HttpContext.Current.Response.Cookies.Add(cookie);

        }

        public static bool Login(string cookieString)
        {
            if (cookieString != "")
            {
                ICustomIdentity identity = CustomIdentity.FromJson(cookieString);
                if (identity.IsAuthenticated)
                {
                    HttpContext.Current.User = new CustomPrincipal(identity);
                }
                return identity.IsAuthenticated;
            }
            else
                return false;
        }
    }
}