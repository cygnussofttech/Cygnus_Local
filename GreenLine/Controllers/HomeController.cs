using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GreenLine.Filters;
using GreenLine.Classes;
using GreenLineDataService;
using Newtonsoft.Json;
using System.Data;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper;
using GreenLineDataService.Models;

namespace GreenLine.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly GeneralFunctions GF = new GeneralFunctions();
        //private readonly OperationService OS = new OperationService();
        public readonly IMasterService MS;

        public HomeController()
        {
            MS = new MasterService();
        }
        public ActionResult URLRedirect()
        {
            //ViewBag.LocationCode = BaseLocationCode;
            //ViewBag.LocationCode = BaseLocationCode;
            return View();
        }
        public string UserForDashBoard
        {
            get { return Convert.ToString(ConfigurationManager.AppSettings["UserForDashBoard"]); }
        }
        [OutputCache(Duration = 60)]
        [InitializeSimpleMembership]
        public ActionResult Index()
        {
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            ViewBag.IPAddress = addr[1].ToString();
            string[] UserDashboardEmployee = UserForDashBoard == null ? new string[0] : UserForDashBoard.Trim().Split(',').Select(n => n.Trim()).ToArray();
            if (UserDashboardEmployee.Any(x => x == BaseUserName))
            {
                return RedirectToAction("UserDashboard", "Home");
            }
            //if (BaseUserName == "EM0001")
            //{
            //    return RedirectToAction("UserDashboard", "Home");
            //}
            if (BaseEmpType == "4")
            {
                return RedirectToAction("Index", "CustomerPortal");
            }
            else
            {
                ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
                return View();
            }
        }
        public ActionResult About()
        {

            ViewBag.Message = "Your app description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        public string GetData(string Method, IDictionary<string, string> parameters)
        {
            DataSet DS = GF.GetdatasetFromParams(Method, parameters);
            if (DS.Tables.Count == 1)
            {
                return JsonConvert.SerializeObject(DS.Tables[0]);
            }
            else
            {
                return JsonConvert.SerializeObject(DS);
            }
        }
        public JsonResult GetPasswordFromUserID(string Id)
        {
            string password = "";
            try
            {
                IMasterService MS = new MasterService();
                CYGNUS_Master_Users objUser = new CYGNUS_Master_Users();
                objUser = MS.GetUserDetails().Where(c => c.UserId.ToUpper() == Id.ToUpper()).FirstOrDefault();
                string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
                password = GF.Decrypt(objUser.UserPwd, psSult);
            }
            catch (Exception ex)
            {
                password = ex.Message.ToString();
            }
            return Json(password, JsonRequestBehavior.AllowGet);
        }
    }
}