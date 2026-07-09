using GreenLine.Classes;
using GreenLineDataService.Helper;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenLine.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController() : this(new ReportService())
        {
        }

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEmployeeUserDriverVehicleList(string searchTerm, string SearchType)
        {
            var CMP = _reportService.GetCustomerListFromSearch(searchTerm, SearchType);

            var users = from user in CMP
                        select new
                        {
                            id = user.CUSTCD,
                            text = user.CUSTNM
                        };

            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}