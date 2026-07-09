using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GreenLine.Classes;
using GreenLine.Security;
using Newtonsoft.Json;
using GreenLine.Models;
using GreenLineDataService;
using GreenLineDataService.Classes;
using GreenLineDataService.Helper;
using GreenLineDataService.Helper.Interface;

namespace GreenLine.Controllers
{
    //[NoCache]
    public class BaseController : Controller
    {
        // readonly MasterService MS = new MasterService();
        public readonly IMasterService MS;

        public BaseController()
        {
            MS = new MasterService();
        }

        private UIThemeConfig GetUITheme()
        {
            const string cacheKey = "UIThemeConfig";
            var cached = System.Web.HttpRuntime.Cache[cacheKey] as UIThemeConfig;
            if (cached != null) return cached;

            try
            {
                var configPath = Server.MapPath("~/Content/UITheme.json");
                if (System.IO.File.Exists(configPath))
                {
                    var json = System.IO.File.ReadAllText(configPath);
                    var theme = JsonConvert.DeserializeObject<UIThemeConfig>(json);
                    if (theme != null && theme.Buttons != null)
                    {
                        var dep = new System.Web.Caching.CacheDependency(configPath);
                        System.Web.HttpRuntime.Cache.Insert(cacheKey, theme, dep);
                        return theme;
                    }
                }
            }
            catch (Exception) { }

            // Return a safe default object to prevent null reference exceptions
            return new UIThemeConfig
            {
                Buttons = new UIThemeConfig.ButtonsConfig
                {
                    Add = new UIThemeConfig.ButtonConfig { CssClass = "btn btn-primary", Icon = "ri-add-line", Label = "Add" },
                    Edit = new UIThemeConfig.ButtonConfig { CssClass = "btn btn-info", Icon = "ri-pencil-line", Label = "Edit" },
                    Delete = new UIThemeConfig.ButtonConfig { CssClass = "btn btn-danger", Icon = "ri-delete-bin-line", Label = "Delete" },
                    Cancel = new UIThemeConfig.ButtonConfig { CssClass = "btn btn-light", Icon = "ri-close-line", Label = "Cancel" },
                    Submit = new UIThemeConfig.ButtonConfig { CssClass = "btn btn-success", Icon = "ri-save-line", Label = "Save" },
                    Clear = new UIThemeConfig.ButtonConfig { CssClass = "btn btn-info", Icon = "ri-refresh-line", Label = "Clear" }
                }
            };
        }
        string _BaseCurrency;
        public string BaseCurrency
        {
            get { return _BaseCurrency; }
        }
        string _CurrencyName;
        public string CurrencyName
        {
            get { return _CurrencyName; }
        }

        string _BaseUserName = "";
        public string BaseUserName
        {
            get { return _BaseUserName; }
        }

        string _BaseUserType = "";
        public string BaseUserType
        {
            get { return _BaseUserType; }
        }

        string _BaseCompanyCode = "";
        public string BaseCompanyCode
        {
            get { return _BaseCompanyCode; }
        }

        string _FinYearStartDate = "";
        public string FinYearStartDate
        {
            get { return _FinYearStartDate; }
        }

        string _FinYearEndDate = "";
        public string FinYearEndDate
        {
            get { return _FinYearEndDate; }
        }

        string _BaseCompanyName = "";
        public string BaseCompanyName
        {
            get { return _BaseCompanyName; }
        }

        string _BaseLocationCode = "";
        public string BaseLocationCode
        {
            get { return _BaseLocationCode; }
        }

        bool _IsGstApplied = true;
        public bool IsGstApplied
        {
            get { return _IsGstApplied; }
        }

        string _MainLocCode = "";
        public string MainLocCode
        {
            get { return _MainLocCode; }
        }

        string _BaseLocationName = "";
        public string BaseLocationName
        {
            get { return _BaseLocationName; }
        }

        Nullable<decimal> _BaseLocationLevel;
        public Nullable<decimal> BaseLocationLevel
        {
            get { return _BaseLocationLevel; }
        }

        string _BaseFinYear = "";
        public string BaseFinYear
        {
            get { return _BaseFinYear; }
        }

        string _BaseYearVal = "";
        public string BaseYearVal
        {
            get { return _BaseYearVal; }
        }

        string _BaseYearValFirst = "";
        public string BaseYearValFirst
        {
            get { return _BaseYearValFirst; }
        }

        string _HeadOfficeCode = "";
        public string HeadOfficeCode
        {
            get { return _HeadOfficeCode; }
        }

        string _DomainName = "";
        public string DomainName
        {
            get { return _DomainName; }
        }

        string _User_Image = "";
        public string User_Image
        {
            get { return _User_Image; }
        }

        string _CurrFinYear = "";
        public string CurrFinYear
        {
            get { return _CurrFinYear; }
        }

        string _CurrYearVal = "";
        public string CurrYearVal
        {
            get { return _CurrYearVal; }
        }

        string _BaseUserReadWrite = "";
        public string BaseUserReadWrite
        {
            get { return _BaseUserReadWrite; }
        }

        string _BaseEmpType = "";
        public string BaseEmpType
        {
            get { return _BaseEmpType; }
        }
        string _IsFinyear = "";
        public string IsFinyear
        {
            get { return _IsFinyear; }
        }

        //GetListObjects GLO = new GetListObjects();

        #region E Invoice Token
        string _EINVToken = "";
        public string EINVToken
        {
            get { return _EINVToken; }
        }
        DateTime _EINVExpiryTime = DateTime.Now;
        public DateTime INVExpiryTime
        {
            get { return _EINVExpiryTime; }
        }
        #endregion
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var newTicket = FormsAuthentication.RenewTicketIfOld(ticket);
                if (newTicket.Expiration != ticket.Expiration)
                {
                    string encryptedTicket = FormsAuthentication.Encrypt(newTicket);

                    cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    cookie.Path = FormsAuthentication.FormsCookiePath;
                    Response.Cookies.Add(cookie);
                }
                //if (ticket.UserData != "")
                CustomPrincipal.Login(ticket.UserData);
            }
            // Cache GetIsFinYear to avoid DB call on every request (5-min cache)
            const string finYearCacheKey = "App_IsFinYear";
            var IsFinYear = System.Web.HttpRuntime.Cache[finYearCacheKey] as string;
            if (IsFinYear == null)
            {
                DataTable dt = MS.GetIsFinYear();
                IsFinYear = dt.Rows[0]["IsFinyear"].ToString();
                System.Web.HttpRuntime.Cache.Insert(finYearCacheKey, IsFinYear, null,
                    DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            //Define Default 
            // Set Global Defaults (Available even for non-authenticated pages like Error/Login)
            ViewBag.UI = GetUITheme();
            ViewBag.DomainName = _DomainName = ConfigurationManager.AppSettings["DomainName"]?.ToString() ?? "";
            string IsDomainNameRequired = ConfigurationManager.AppSettings["IsDomainNameRequired"]?.ToString() ?? "false";
            if (IsDomainNameRequired == "false")
            {
                _DomainName = "";
                ViewBag.DomainName = "";
            }

            ViewBag.THCCalledAs = "THC";
            ViewBag.DKTCalledAs = "CNote";
            ViewBag.DefaultDateFormat = "{0:dd/MM/yyyy}";
            ViewBag.DefaultJqueryDateFormat = "dd/MM/yyyy";
            ViewBag.DefaultDateTimeFormat = "{0:dd/MM/yyyy hh:mm tt}";

            if (User.Identity.IsAuthenticated == true)
            {
                ViewBag.BaseCurrency = _BaseCurrency = (((CustomIdentity)User.Identity).Currency);
                ViewBag.CurrencyName = _CurrencyName = (((CustomIdentity)User.Identity).CurrencyName);
                ViewBag.BaseUserName = _BaseUserName = (((CustomIdentity)User.Identity).Name.ToUpper());
                ViewBag.BaseUserType = _BaseUserType = (((CustomIdentity)User.Identity).UserType);
                ViewBag.BaseLocationLevel = _BaseLocationLevel = (((CustomIdentity)User.Identity).LocationLevel);
                ViewBag.BaseLocationCode = _BaseLocationCode = (((CustomIdentity)User.Identity).LocationCode);
                ViewBag.BaseLocationName = _BaseLocationName = (((CustomIdentity)User.Identity).LocationName);
                ViewBag.MainLocCode = _MainLocCode = (((CustomIdentity)User.Identity).MainLocCode);
                ViewBag.HeadOfficeCode = _HeadOfficeCode = (((CustomIdentity)User.Identity).HeadOfficeCode);
                ViewBag.BaseYearVal = _BaseYearVal = (((CustomIdentity)User.Identity).YearVal);
                ViewBag.BaseFinYear = _BaseFinYear = (((CustomIdentity)User.Identity).FinYear);
                ViewBag.BaseCompanyCode = _BaseCompanyCode = (((CustomIdentity)User.Identity).CompanyCode);
                ViewBag.BaseCompanyName = _BaseCompanyName = (((CustomIdentity)User.Identity).CompanyName);
                ViewBag.User_Image = _User_Image = (((CustomIdentity)User.Identity).User_Image);
                ViewBag.CurrYearVal = _CurrYearVal = (((CustomIdentity)User.Identity).CurrYearVal);
                ViewBag.CurrFinYear = _CurrFinYear = (((CustomIdentity)User.Identity).CurrFinYear);
                ViewBag.BaseUserReadWrite = _BaseUserReadWrite = (((CustomIdentity)User.Identity).UserReadWrite);
                ViewBag.BaseEmpType = _BaseEmpType = (((CustomIdentity)User.Identity).EmpType);
                ViewBag.IsGstApplied = _IsGstApplied = true;

                string[] FinYerArray = _BaseFinYear.Split('-');
                ViewBag.FinYearStart = FinYerArray[0];
                ViewBag.FinYearEnd = FinYerArray[1];
                //_BaseYearValFirst = FinYerArray[0];
                ViewBag._BaseYearValFirst = _BaseYearValFirst = FinYerArray[0];
                ViewBag.IsFinyear = IsFinYear;
                if (IsFinYear == "1")
                {
                    ViewBag.FinYearStartDate = "01 Apr " + FinYerArray[0];
                    ViewBag.FinYearEndDate = "31 Mar " + FinYerArray[1];
                    ViewBag.FinYearStartDate_1 = _FinYearStartDate = Convert.ToDateTime("01 Apr " + FinYerArray[0] + " 00:00:00").ToString();
                    ViewBag.FinYearEndDate_1 = _FinYearEndDate = Convert.ToDateTime("31 Mar " + FinYerArray[1] + " 23:59:59").ToString();
                }
                else
                {
                    ViewBag.FinYearStartDate = "01 Jan " + FinYerArray[0];
                    ViewBag.FinYearEndDate = "31 Dec " + FinYerArray[0];
                    ViewBag.FinYearStartDate_1 = _FinYearStartDate = Convert.ToDateTime("01 Jan " + FinYerArray[0] + " 00:00:00").ToString();
                    ViewBag.FinYearEndDate_1 = _FinYearEndDate = Convert.ToDateTime("31 Dec " + FinYerArray[0] + " 23:59:59").ToString();
                }

                ViewBag.GSTStartDate = ConfigurationManager.AppSettings["GSTStartDate"].ToString();
                ViewBag.StaxEndDate = ConfigurationManager.AppSettings["StaxEndDate"].ToString();

                ViewBag.StandardReportPath = ConfigurationManager.AppSettings["MvcReportViewer.ReportPathPrefix"].ToString();

                GeneralFunctions GF = new GeneralFunctions();
                var fromLogin = TempData["login"] as string;
                if (!string.IsNullOrEmpty(fromLogin))
                {
                    MS.GetMenuListWithRights(BaseUserName, true, "0", BaseFinYear);
                    MS.GetMenusList(true);
                    GF.DeleteUserMenuTextFiles(BaseUserName);
                }
                try
                {
                    if (string.IsNullOrEmpty(_EINVToken) || _EINVExpiryTime <= DateTime.Now)
                    {
                        ViewBag.EINVToken = _EINVToken;
                        ViewBag.EINVExpiryTime = _EINVExpiryTime;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [ChildActionOnly]
        [OutputCache(Duration = 120, VaryByCustom = "Username")]
        public ActionResult RenderDynamicSidebar()
        {
            try
            {
                if (!User.Identity.IsAuthenticated) return Content("");

                string userName = User.Identity.Name.ToUpper();
                string finYear = BaseFinYear ?? (((CustomIdentity)User.Identity).FinYear);

                // 1. Get flat data from existing service
                var flatMenu = MS.GetMenuListWithRights(userName, false, "0", finYear)?
                                 .Where(m => m.HasAccess && m.IsNewPortal).ToList() ?? new List<GreenLineDataService.Models.VW_GetUserMenuRights>();

                // 2. Build Hierarchy with Dynamic Icons
                var hierarchicalMenu = new List<MenuViewModel>();
                var lookup = new Dictionary<int, MenuViewModel>();

                // Define icon mapping for top-level categories based on user requirements
                var iconMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Dashboard", "ri-dashboard-2-line" },
                    { "Admin", "ri-settings-3-line" },
                    { "Operations", "ri-stack-line" },
                    { "Fleet", "ri-truck-line" },
                    { "Income", "ri-money-dollar-box-line" },
                    { "Payments", "ri-bank-card-2-line" },
                    { "Accounts", "ri-money-dollar-circle-line" },
                    { "Exceptions", "ri-error-warning-line" },
                    { "View & Track", "ri-road-map-line" },
                    { "Reports", "ri-file-chart-line" },
                    { "Master", "ri-database-2-line" },
                    { "Settings", "ri-settings-4-line" }
                };

                foreach (var item in flatMenu)
                {
                    lookup[item.MenuId] = new MenuViewModel
                    {
                        MenuId = item.MenuId,
                        DisplayName = item.DisplayName?.Trim() ?? "",
                        NavigationURL = string.IsNullOrEmpty(item.NavigationURL) ? "#" : item.NavigationURL,
                        ParentID = item.ParentID,
                        Icon = "" // Initialize empty
                    };
                }

                foreach (var item in lookup.Values)
                {
                    // Treat 0 or invalid ParentID as root
                    if (item.ParentID == 0 || !lookup.ContainsKey(item.ParentID))
                    {
                        // Assign mapped icon or default "apps" icon for root level items
                        item.Icon = iconMapping.ContainsKey(item.DisplayName) ? iconMapping[item.DisplayName] : "ri-apps-2-line";
                        hierarchicalMenu.Add(item);
                    }
                    else
                    {
                        lookup[item.ParentID].Children.Add(item);
                    }
                }

                return PartialView("_DynamicSidebar", hierarchicalMenu.OrderBy(m => m.MenuId));
            }
            catch (Exception ex)
            {
                return Content("<!-- Menu Load Error: " + ex.Message + " -->");
            }
        }

        #region Save Error

        public void Error_Logs(string Controller, string ModuleName, string View_Partial, string Transaction, string Error)
        {
            //try
            //{
            //    var Errors = CL.ErrorEntry(Controller, ModuleName, View_Partial, Transaction, Error, User.Identity.Name.ToString());
            //}
            //catch (Exception ex)
            //{
            //    var Errors = CL.ErrorEntry(Controller, "Error_Logs" + ModuleName, "Error Save Methode", "Insert", "Error in Error Save", User.Identity.Name.ToString());
            //}
        }

        #endregion
    }
}
