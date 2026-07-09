using System;
using System.Collections.Generic;
using System.Web.Mvc;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper.Service;
using GreenLineDataService.Models;

namespace GreenLine.Controllers
{
    [Authorize]
    public class TruckController : BaseController
    {
        private readonly IPgTruckService _truckService;

        public TruckController()
        {
            _truckService = new PgTruckService();
        }

        public ActionResult Index()
        {
            try
            {
                List<PG_Truck> trucks = _truckService.GetAllTrucks();
                return View(trucks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load trucks: " + ex.Message;
                return View(new List<PG_Truck>());
            }
        }

        [HttpPost]
        public ActionResult GetTruckListJson()
        {
            try
            {
                List<PG_Truck> trucks = _truckService.GetAllTrucks();
                return Json(new { data = trucks });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<PG_Truck>(), error = ex.Message });
            }
        }
    }
}
