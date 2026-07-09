using System;
using System.Collections.Generic;
using System.Web.Mvc;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper.Service;
using GreenLineDataService.Models;

namespace GreenLine.Controllers
{
    [Authorize]
    public class GeofenceController : BaseController
    {
        private readonly IPgGeofenceService _geofenceService;

        public GeofenceController()
        {
            _geofenceService = new PgGeofenceService();
        }

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load geofence page: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetGeofenceListJson()
        {
            try
            {
                List<PG_Geofence> geofences = _geofenceService.GetAllGeofences();
                var jsonResult = Json(new { data = geofences }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<PG_Geofence>(), error = ex.Message });
            }
        }
    }
}
