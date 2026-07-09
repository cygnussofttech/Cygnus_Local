using GreenLine.Classes;
using GreenLineDataService.Helper;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Serialization;

namespace GreenLine.Controllers
{
    [Authorize]
    public class FleetController : BaseController
    {
        GeneralFunctions GF = new GeneralFunctions();

        private readonly IFleetServices _FS;
        private readonly IMasterService _MS;

        public FleetController() : this(new FleetServices(), new MasterService())
        {
        }
        public FleetController(IFleetServices FS, IMasterService MS)
        {
            _FS = FS;
            _MS = MS;
        }
        public string Connstr
        {
            get { return GF.GetConnstr(); }
        }
        public ActionResult Index()
        {
            return View();
        }

        #region Pickup Request
        public ActionResult Pickup_Request()
        {
            CYGNUS_PickUp_Request CL = new CYGNUS_PickUp_Request();
            CL.EnrtExpList = new List<CYGNUS_FLEET_ENROUTE_EXP>();
            ViewBag.CustomerGroupList = _MS.GetCustomerGroupMasterObject().Where(c => c.ActiveFlag == "Y").ToList();
            ViewBag.LaneList = _MS.GetLaneDetails().ToList();
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
            ViewBag.ListpackegType = _MS.GetGeneralMasterWithParam("PROD");
            ViewBag.ListRateType = _MS.GetGeneralMasterWithParam("RATETYP");
            ViewBag.ExpenseMasterList = _FS.GetTripExpenseMasterList();
            ViewBag.ServiceTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "PTL", Value = "1" },
                new SelectListItem { Text = "FTL", Value = "2" }
            };
            return View(CL);
        }

        public ActionResult AddEditPickup_Request(string PRNo)
        {
            CYGNUS_PickUp_Request CL = new CYGNUS_PickUp_Request();
            try
            {
                if (!string.IsNullOrEmpty(PRNo))
                {
                    CL = _FS.GetPickUpRequestDetails().FirstOrDefault(c => c.PRNo == PRNo);
                    if (CL != null)
                    {
                        CL.EnrtExpList = _FS.Get_TripExp_List(PRNo);
                    }
                }

                if (CL == null)
                {
                    CL = new CYGNUS_PickUp_Request();
                }
                if (CL.EnrtExpList == null)
                {
                    CL.EnrtExpList = new List<CYGNUS_FLEET_ENROUTE_EXP>();
                }
                //ViewBag.StateList = _masterService.GetStateMaster().ToList();
                //ViewBag.ZoneList = _masterService.GetGeneralMaster().Where(c => c.CodeType.ToUpper() == "ZONE" && c.StatusCode == "Y").ToList();
                ViewBag.CustomerGroupList = _MS.GetCustomerGroupMasterObject().Where(c => c.ActiveFlag == "Y").ToList();
                ViewBag.CustomerList = _MS.GetCustomer(CL.Customer_Group).ToList();
                ViewBag.LaneList = _MS.GetLaneDetails().ToList();
                ViewBag.ContractList = _MS.GetCustomerContractList(CL.Customer_Code).ToList();
                ViewBag.ExpenseMasterList = _FS.GetTripExpenseMasterList();
                ViewBag.ServiceTypeList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "PTL", Value = "1" },
                    new SelectListItem { Text = "FTL", Value = "2" }
                };
            }
            catch (Exception)
            {
                return RedirectToAction("Pickup_Request");
            }
            return PartialView("_AddEditPickup_Request", CL);
        }

        [HttpPost]
        public ActionResult AddEditPickup_Request(CYGNUS_PickUp_Request CPR)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(CPR.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, CPR);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);

                    DataTable Dt;
                    if (!string.IsNullOrEmpty(CPR.PRNo))
                    {
                        Dt = _FS.AddPickUpRequest(xmlDoc.InnerXml, "U", BaseCompanyCode, BaseUserName);
                    }
                    else
                    {
                        Dt = _FS.AddPickUpRequest(xmlDoc.InnerXml, "I", BaseCompanyCode, BaseUserName);
                    }
                    string PRNo = "", Status = "";
                    if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["Message"].ToString() == "Done")
                    {
                        PRNo = Dt.Rows[0]["PRNo"].ToString();
                        Status = Dt.Rows[0]["Status"].ToString();
                    }
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Status = true, Message = "Done", PRNo = PRNo });
                    }
                    return RedirectToAction("Pickup_RequestDone", new { PRNo = PRNo, status = Status });
                }
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.ToString().Replace('\n', '_');
                return View("Error");
            }
        }

        public ActionResult Pickup_RequestDone(string PRNo, string Status)
        {
            ViewBag.PRNo = PRNo;
            ViewBag.Status = Status;
            return View();
        }

        public ActionResult AddTripExpenses(int SRNO)
        {
            CYGNUS_FLEET_ENROUTE_EXP model = new CYGNUS_FLEET_ENROUTE_EXP();
            model.SRNO = SRNO;
            ViewBag.ExpenseMasterList = _FS.GetTripExpenseMasterList();
            return PartialView("_TripExpenses", model);
        }
        #endregion

        #region Vehicle Allocation

        public ActionResult Vehicle_AllocationList()
        {
            CYGNUS_PickUpRequestViewModel CLVM = new CYGNUS_PickUpRequestViewModel();
            try
            {
                CLVM.listCPR = new List<CYGNUS_PickUp_Request>();
                CLVM.listCPR = _FS.GetPickUpRequestDetails().Where(c => c.IsApprove == "Y").ToList();
                CLVM.CPR = new CYGNUS_PickUp_Request();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(CLVM);
        }

        public ActionResult VehicleAllocation()
        {
            CYGNUS_Vehicle_Allocation CVA = new CYGNUS_Vehicle_Allocation();
            ViewBag.VehicleTypeList = _MS.GetGeneralMasterWithParam("FTLTYP");
            //ViewBag.ListVehicle = _FS.GetUnallocatedVehicles();
            ViewBag.ListVehicle = _MS.GetVehicleList("",BaseUserName, "Vehicle_Allocation");

            ViewBag.AllocationTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "PTL", Value = "PTL" },
                new SelectListItem { Text = "FTL", Value = "FTL" }
            };
            CVA.AllocationType = "FTL";
            CVA.PlacementDate = DateTime.Now;

            // Added for Allocation List in Add mode
            //if (string.IsNullOrEmpty(PRNo) || PRNo == "0" || PRNo != null) // usually add mode
            //{
            //ViewBag.AllocationList = _FS.GetPickUpRequestDetails("PTL").ToList();
            ViewBag.AllocationList = _FS.GetPickUpRequestDetails().Where(c => c.IsApprove == "Y").ToList();
            //}
            //ViewBag.DriverList = _MS.GetDriverMstDetails().Where(c => c.ActiveFlag == "Y").ToList();
            ViewBag.DriverList = "";

            return View(CVA);
        }

        [HttpPost]
        public ActionResult AddEditVehicleAllocation(CYGNUS_Vehicle_Allocation CPR)
        {
            try
            {
                string MstDetails = "<CYGNUS_Vehicle_Allocation>";
                MstDetails = MstDetails + "<PRNo>" + CPR.PRNo + "</PRNo>";
                //MstDetails = MstDetails + "<VehicleType>" + CPR.VehicleType + "</VehicleType>";
                MstDetails = MstDetails + "<VehicleId>" + CPR.VehicleId + "</VehicleId>";
                MstDetails = MstDetails + "<PlacementDate>" + (CPR.PlacementDate.Year > 1900 ? CPR.PlacementDate.ToString("yyyy-MM-dd HH:mm:ss tt") : "") + "</PlacementDate>";
                MstDetails = MstDetails + "<FirstDriverId>" + CPR.FirstDriverId + "</FirstDriverId>";
                MstDetails = MstDetails + "<SecondDriverId>" + CPR.SecondDriverId + "</SecondDriverId>";
                MstDetails = MstDetails + "<TrailerNo>" + CPR.TrailerNo + "</TrailerNo>";
                MstDetails = MstDetails + "<AllocationType>" + CPR.AllocationType + "</AllocationType>";
                MstDetails = MstDetails + "<CompanyCode>" + BaseCompanyCode + "</CompanyCode>";
                MstDetails = MstDetails + "<EntryBy>" + BaseUserName.ToUpper() + "</EntryBy>";
                MstDetails = MstDetails + "</CYGNUS_Vehicle_Allocation>";
                DataTable Dt = _FS.AddVehicle_Allocation(MstDetails);

                string Status = "";
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["Message"].ToString() == "Done")
                {
                    Status = Dt.Rows[0]["Status"].ToString();
                }

                if (Request.IsAjaxRequest())
                {
                    return Json(new { Status = true, Message = "Done" });
                }
                return RedirectToAction("VehicleAllocationDone", new { status = Status, VehicleNo = CPR.VehicleId, TrailerNo = CPR.TrailerNo });
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }
        public ActionResult VehicleAllocationDone(string Status, string VehicleNo, string TripNo)
        {
            ViewBag.Status = Status;
            ViewBag.VehicleNo = VehicleNo;
            ViewBag.TripNo = TripNo;
            return View();
        }

        [HttpPost]
        public ActionResult CheckDriverAvailability(int DriverId, string VehicleId)
        {
            try
            {
                // Logic to check if driver is already mapped to ANOTHER vehicle
                var mapping = _MS.Get_VehicleDriverMapping_Details(0,BaseUserName).FirstOrDefault(m =>
                                                (m.First_Driver == DriverId || m.Second_Driver == DriverId) && m.VehicleId != VehicleId);

                if (mapping != null)
                {
                    return Json(new
                    {
                        IsAttached = true,
                        Message = $"Warning: This driver is already attached to Vehicle {mapping.VehicleId}. Do you want to proceed?"
                    });
                }
                return Json(new { IsAttached = false });
            }
            catch (Exception ex)
            {
                return Json(new { IsAttached = false, Error = ex.Message });
            }
        }

        public ActionResult Trip_InvoiceUpload(string PRNo, string VehicleNo, string TripNo, string EventId)
        {
            CYGNUS_LR_ENTRY CMD = new CYGNUS_LR_ENTRY();
            CMD.PRNo = PRNo;
            CMD.VehicleNo = VehicleNo;
            CMD.TripNo = TripNo;
            CMD.EventId = EventId;
            return View(CMD);
        }

        [HttpGet]
        public JsonResult GetContractAdditionalFields(string contractId)
        {
            try
            {
                if (string.IsNullOrEmpty(contractId))
                {
                    return Json(new { success = false, fields = new List<CYGNUS_Additonal_Field_Configurator>() }, JsonRequestBehavior.AllowGet);
                }
                string sqlstr = "Execute USP_GetCustomerContractDetails '" + contractId + "','AdditionalField'";
                DataTable dt = GF.GetDataTableFromSP(sqlstr);
                List<CYGNUS_Additonal_Field_Configurator> list = DataRowToObject.CreateListFromTable<CYGNUS_Additonal_Field_Configurator>(dt);
                var configuredFields = list.Where(f => !string.IsNullOrEmpty(f.Label) && !string.IsNullOrEmpty(f.DataType)).ToList();
                return Json(new { success = true, fields = configuredFields }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Trip 

        public ActionResult TripList()
        {
            CYGNUS_TripViewModel CTVM = new CYGNUS_TripViewModel();
            try
            {
                CTVM.listCTM = new List<CYGNUS_Trip_Master>();
                CTVM.listCTM = _FS.GetTripList(BaseUserName).ToList();
                CTVM.CTM = new CYGNUS_Trip_Master();

                CTVM.listEvents = new List<Cygnus_Master_Event>();
                CTVM.listEvents = _FS.GetEventList().ToList();
                CTVM.listVehicleEvents = _FS.GetVehicleEventList().ToList();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(CTVM);
        }

        public ActionResult TripOperation(string TripNo, string PRNo, string VehicleNo, string tab = "overview")
        {
            CYGNUS_TripViewModel CTVM = new CYGNUS_TripViewModel();
            try
            {
                CTVM.listCTM = _FS.GetTripList(BaseUserName).ToList();
                CTVM.CTM = CTVM.listCTM.FirstOrDefault(t => t.TripNo == TripNo)
                           ?? new CYGNUS_Trip_Master { TripNo = TripNo, PRNo = PRNo, VehicleNo = VehicleNo };
                CTVM.listEvents = _FS.GetEventList().ToList();
                CTVM.listVehicleEvents = _FS.GetVehicleEventList().ToList();

                // If Event_id is less than 6 (stages 0 to 5), default/force the active tab to "tripevents"
                bool isStageReached = CTVM.CTM != null && CTVM.CTM.Event_id < 6;
                bool isFromSaveRedirect = TempData.ContainsKey("SuccessMessage") || TempData.ContainsKey("ErrorMessage");
                string defaultTab = isStageReached ? "tripevents" : "overview";
                ViewBag.ActiveTab = isStageReached ? "tripevents" : (isFromSaveRedirect ? tab : defaultTab);
                ViewBag.ExpenseMasterList = _FS.GetTripExpenseMasterList();
                ViewBag.LrExpenseMasterList = _FS.GetLrExpenseMasterList();
                ViewBag.PaymentTypeList = new List<SelectListItem>
                {
                    new SelectListItem{ Text = "Online" ,Value = "O"},
                    new SelectListItem{ Text = "Cash" ,Value = "C"},
                    new SelectListItem{ Text = "To Be Paid" ,Value = "P"},
                };

                CTVM.TripNote = new CYGNUS_TripNotes { TripNo = TripNo, VehicleId = VehicleNo };

                System.Data.DataSet ds = _FS.GetTripOperationCount(PRNo, TripNo, VehicleNo);
                CTVM.CTM.TripNotesCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TripNotesCount"]);
                CTVM.CTM.TimeLineCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TimeLineCount"]);
                CTVM.CTM.VehicleEventCount = Convert.ToInt32(ds.Tables[2].Rows[0]["VehicleEventCount"]);
                CTVM.CTM.DispatchLRCount = Convert.ToInt32(ds.Tables[3].Rows[0]["DispatchLRCount"]);

                string activePRNo = !string.IsNullOrEmpty(PRNo) ? PRNo : (CTVM.CTM != null ? CTVM.CTM.PRNo : "");
                CTVM.listStageActivities = _FS.GetTripStageByPRNo(activePRNo);

                CYGNUS_Vehicle_Event Event = new CYGNUS_Vehicle_Event();
                Event.TripNo = TripNo;
                Event.PRNo = PRNo;
                Event.VehicleId = VehicleNo;
                CTVM.VehicleEvent = Event;
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(CTVM);
        }

        [HttpPost]
        public ActionResult UpdateEventStatus(string prno, string VehicleNo, int eventId, string TripNo, string remarks)
        {
            try
            {
                DataTable Dt = _FS.UpdateTripEventStatus(prno, VehicleNo, eventId, BaseUserName.ToUpper(), TripNo, remarks);
                string Message = "";
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Message = "Saved Successfully";
                }
                else
                {
                    Message = Dt.Rows[0]["Message"].ToString();
                }
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult UpdateTripEventLastStatus(string prno, string VehicleNo, int eventId, string TripNo, int stopId, string remarks)
        {
            try
            {
                DataTable Dt = _FS.UpdateTripEventLastStatus(prno, VehicleNo, eventId, BaseUserName.ToUpper(), TripNo, stopId, remarks);
                bool status = false;
                string message = "";
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    status = Dt.Rows[0]["TranXaction"].ToString() == "Done";
                    message = status ? "Saved Successfully" : "Failed to save event";
                }
                return Json(new { Status = status, Message = message });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetTripChecklistPartial(string PRNo)
        {
            CYGNUS_TripViewModel CTVM = new CYGNUS_TripViewModel();
            try
            {
                CTVM.listStageActivities = _FS.GetTripStageByPRNo(PRNo);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
            }
            return PartialView("_Trip_Events_Checklist", CTVM);
        }

        [HttpGet]
        public ActionResult GetTripDocketsPartial(string TripNo, string PRNo, string StopId, int? stageId = null)
        {
            CYGNUS_TripViewModel CTVM = new CYGNUS_TripViewModel();
            try
            {
                CTVM.listCTM = _FS.GetTripList(BaseUserName).ToList();
                CTVM.CTM = CTVM.listCTM.FirstOrDefault(t => t.TripNo == TripNo) ?? new CYGNUS_Trip_Master { TripNo = TripNo, PRNo = PRNo };

                string cleanStopId = "";
                if (!string.IsNullOrEmpty(StopId) && int.TryParse(StopId, out int parsedStopId))
                {
                    cleanStopId = parsedStopId.ToString();
                }

                CTVM.listDockets = _FS.GetTripLRList(TripNo, PRNo, cleanStopId);
                ViewBag.StageId = stageId;
                ViewBag.StopId = cleanStopId;
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
            }
            return PartialView("_Trip_Events_Dockets", CTVM);
        }

        #endregion

        #region Trip Stage Approval Master
        public ActionResult TripStageApprovalMaster()
        {
            return View();
        }

        public class TripStageUpdateModel
        {
            public string TripNo { get; set; }
            public bool IsMannualStageMark { get; set; }
        }

        [HttpPost]
        public ActionResult SaveTripStageApproval(List<TripStageUpdateModel> TripList)
        {
            try
            {
                if (TripList == null || !TripList.Any())
                {
                    return Json(new { Status = false, Message = "Please select at least one trip to mark." });
                }

                var list = TripList.Where(m => m.IsMannualStageMark == true);

                var sb = new StringBuilder();
                sb.Append("<Trips>");
                foreach (var item in list)
                {
                    sb.Append("<Trip>");
                    sb.Append("<TripNo>" + HttpUtility.HtmlEncode(item.TripNo) + "</TripNo>");
                    sb.Append("<IsMannualStageMark>" + (item.IsMannualStageMark ? "1" : "0") + "</IsMannualStageMark>");
                    sb.Append("<IsMannualStageMarkBy>" + BaseUserName + "</IsMannualStageMarkBy>");
                    sb.Append("</Trip>");
                }
                sb.Append("</Trips>");

                string xmlData = sb.ToString();
                string queryStr = "EXEC USP_UpdateTripStageManualMark '" + xmlData.Replace("'", "''") + "'";
                GF.ExecuteNonQuery(queryStr);

                return Json(new { Status = true, Message = "Trip stage approval updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }
        #endregion

        #region LR Entry
        public ActionResult LREntry()
        {
            CYGNUS_LR_ENTRY CMD = new CYGNUS_LR_ENTRY();
            return View(CMD);
        }

        [HttpPost]
        public ActionResult SubmitLREntry(CYGNUS_LR_ENTRY CLE, HttpPostedFileBase UploadFile, HttpPostedFileBase LRUpload, HttpPostedFileBase Other_Doc)
        {
            try
            {
                string invoiceFilePath = "";
                string lrFilePath = "";
                string otherDocFilePath = "";

                string baseFolder = Server.MapPath("~/Uploads/LR/");
                if (!Directory.Exists(baseFolder))
                    Directory.CreateDirectory(baseFolder);

                if (UploadFile != null && UploadFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(UploadFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string fullPath = Path.Combine(baseFolder, fileName);
                    UploadFile.SaveAs(fullPath);
                    invoiceFilePath = "~/Uploads/LR/" + fileName;
                }

                if (LRUpload != null && LRUpload.ContentLength > 0)
                {
                    string ext = Path.GetExtension(LRUpload.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string fullPath = Path.Combine(baseFolder, fileName);
                    LRUpload.SaveAs(fullPath);
                    lrFilePath = "~/Uploads/LR/" + fileName;
                }

                if (Other_Doc != null && Other_Doc.ContentLength > 0)
                {
                    string ext = Path.GetExtension(Other_Doc.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string fullPath = Path.Combine(baseFolder, fileName);
                    Other_Doc.SaveAs(fullPath);
                    otherDocFilePath = "~/Uploads/LR/" + fileName;
                }

                // Re-enable disabled eway bill fields
                if (CLE.eway_bill_no == "NA") CLE.eway_bill_no = "";

                // Store file paths in the model properties
                CLE.InvoiceUpload = invoiceFilePath;
                CLE.LRUpload = lrFilePath;
                CLE.Other_Doc = otherDocFilePath;

                // Ensure all null string properties are populated with string.Empty, null DateTime? properties
                // are populated with the default DateTime value (0001-01-01T00:00:00), and null standard charge
                // properties (SCHGxx) are set to 0 so that all nodes are generated in the serialized XML payload.
                foreach (var prop in CLE.GetType().GetProperties())
                {
                    if (!prop.CanWrite)
                        continue;

                    var value = prop.GetValue(CLE);

                    if (prop.PropertyType == typeof(string) && value == null)
                    {
                        prop.SetValue(CLE, string.Empty);
                    }
                    else if (prop.PropertyType == typeof(DateTime?) && value == null)
                    {
                        prop.SetValue(CLE, new DateTime(1900, 1, 1));
                    }
                    else if (prop.PropertyType == typeof(decimal?) && value == null && prop.Name.StartsWith("SCHG"))
                    {
                        prop.SetValue(CLE, 0m);
                    }
                }

                // Map dynamic standard charges from form fields to matching SCHGxx properties on CLE (model side)
                foreach (string key in Request.Form.AllKeys)
                {
                    if (key != null && key.StartsWith("charge_"))
                    {
                        var chargeCode = key.Substring(7); // e.g. "SCHG01", "SCHG02"
                        var chargeValueStr = Request.Form[key];

                        if (decimal.TryParse(chargeValueStr, out decimal parsedVal))
                        {
                            var prop = CLE.GetType().GetProperty(chargeCode);
                            if (prop != null && prop.CanWrite)
                            {
                                var t = prop.PropertyType;
                                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    prop.SetValue(CLE, Convert.ChangeType(parsedVal, Nullable.GetUnderlyingType(t)));
                                }
                                else
                                {
                                    prop.SetValue(CLE, Convert.ChangeType(parsedVal, t));
                                }
                            }
                        }
                    }
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(CLE.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, CLE);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                }

                // Inject dynamic charges into the serialized XML document
                var root = xmlDoc.DocumentElement;
                if (root != null)
                {
                    var chargesElem = xmlDoc.CreateElement("Charges");
                    foreach (string key in Request.Form.AllKeys)
                    {
                        if (key != null && key.StartsWith("charge_"))
                        {
                            var chargeCode = key.Substring(7);
                            var chargeValue = Request.Form[key];

                            // 1. Direct node format (e.g. <charge_DKT>value</charge_DKT>)
                            var directElem = xmlDoc.CreateElement("charge_" + chargeCode);
                            directElem.InnerText = chargeValue;
                            root.AppendChild(directElem);

                            // 2. Array item format (e.g. <Charge><chargecode>DKT</chargecode><chargeamount>value</chargeamount></Charge>)
                            var chargeElem = xmlDoc.CreateElement("Charge");

                            var codeElem = xmlDoc.CreateElement("chargecode");
                            codeElem.InnerText = chargeCode;
                            chargeElem.AppendChild(codeElem);

                            var amtElem = xmlDoc.CreateElement("chargeamount");
                            amtElem.InnerText = chargeValue;
                            chargeElem.AppendChild(amtElem);

                            chargesElem.AppendChild(chargeElem);
                        }
                    }
                    root.AppendChild(chargesElem);
                }

                SqlConnection con = new SqlConnection(Connstr);
                con.Open();
                SqlTransaction trn = con.BeginTransaction();
                try
                {
                    DataSet result = _FS.SaveLREntry(xmlDoc.InnerXml, BaseCompanyCode, BaseUserName, BaseFinYear);

                    if (result != null && result.Tables.Count > 0 && result.Tables[1].Rows[0]["Message"].ToString() == "Done")
                    {
                        TempData["SuccessMessage"] = "LR entry saved successfully.";
                        var Dockno = result.Tables[1].Rows[0]["DockNo"].ToString();
                        var PRNo = result.Tables[1].Rows[0]["PRNo"].ToString();
                        var VehicleNo = result.Tables[1].Rows[0]["VehicleNo"].ToString();
                        var TripNo = result.Tables[1].Rows[0]["TripNo"].ToString();
                        DataTable dtEvent = _FS.UpdateTripEventStatus(PRNo, VehicleNo, 7, BaseUserName.ToUpper(), TripNo, "");
                        if (dtEvent != null && dtEvent.Rows.Count > 0 && dtEvent.Rows[0]["TranXaction"].ToString() == "Done")
                        {
                            string PRNO = dtEvent.Rows[0]["PRNo"].ToString();
                        }

                        //if (!string.IsNullOrWhiteSpace(CLE.Contract_Id))
                        //{
                        //    string QueryString = "EXEC USP_CreatseFTLChallan_From_Docket  '" + Dockno + "'";
                        //    DataTable dt1 = GF.GetDataTableFromSP(QueryString);

                        //    if (dt1 != null && dt1.Rows.Count > 0 && dt1.Rows[0]["TranXaction"].ToString() == "Successfully Generated")
                        //    {
                        //        string THC = dt1.Rows[0]["DOCNO"].ToString();
                        //    }
                        //}
                        trn.Commit();
                    }
                    else
                    {
                        string errMsg = (result != null && result.Tables.Count > 0) ? result.Tables[1].Rows[0][0].ToString() : "LR entry could not be saved.";
                        TempData["ErrorMessage"] = errMsg;
                        trn.Rollback();
                    }

                    con.Close();
                    return RedirectToAction("TripOperation", new
                    {
                        TripNo = CLE.TripNo,
                        PRNo = CLE.PRNo,
                        VehicleNo = CLE.VehicleNo,
                        tab = "dispatch"
                    });
                }
                catch (Exception ex)
                {
                    if (trn != null && trn.Connection != null)
                    {
                        try
                        {
                            trn.Rollback();
                        }
                        catch { }
                    }
                    con.Close();
                    ViewBag.StrError = ex.Message;
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("TripOperation", new
                {
                    TripNo = CLE.TripNo,
                    PRNo = CLE.PRNo,
                    VehicleNo = CLE.VehicleNo,
                    tab = "dispatch"
                });
            }
        }

        [HttpPost]
        public ActionResult ProcessInvoiceOCR(HttpPostedFileBase UploadFile, CYGNUS_LR_ENTRY model)
        {
            try
            {
                if (UploadFile != null && UploadFile.ContentLength > 0)
                {
                    using (var client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://ai.greenlogistics.tech/invoice_parser_2");
                        request.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huZG9lIiwiZXhwIjoxOTU3ODk4NTgwfQ.7yU_0RF_MmQy3XrP9g-UMtejbq6MU-qlxwGisf0F24U");

                        var content = new MultipartFormDataContent();
                        byte[] fileBytes;
                        using (var ms = new MemoryStream())
                        {
                            UploadFile.InputStream.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        content.Add(new ByteArrayContent(fileBytes), "file1", UploadFile.FileName);
                        request.Content = content;

                        var response = client.SendAsync(request).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = response.Content.ReadAsStringAsync().Result;

                            string jsonResponse = "{\"success\":true,\"data\":" + responseString + "}";
                            return Content(jsonResponse, "application/json");
                        }
                        else
                        {
                            return Json(new { success = false, message = "API Error: " + response.ReasonPhrase });
                        }
                    }
                }
                return Json(new { success = false, message = "No file uploaded" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Process Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> FetchEWayBillData(string ewayBillNo)
        {
            if (string.IsNullOrEmpty(ewayBillNo))
            {
                return Json(new { success = false, message = "E-Way Bill Number is required" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // 1. Get Auth Token
                    var authRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.webcorevision.com:3000/EwayBill/auth");
                    var authData = new
                    {
                        gstin = "24AAICG8668B1ZI",
                        user_name = "GLPLTRANSP_API_gms",
                        eInvPwd = "Gre3nl!ne034#",
                        company_name = "GMSL",
                        api = "TEST"
                    };
                    string authJson = JsonConvert.SerializeObject(authData);
                    authRequest.Content = new StringContent(authJson, Encoding.UTF8, "application/json");

                    var authResponse = await client.SendAsync(authRequest);
                    if (!authResponse.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = $"Auth API Error: {authResponse.ReasonPhrase} ({authResponse.StatusCode})" }, JsonRequestBehavior.AllowGet);
                    }

                    string authResponseString = await authResponse.Content.ReadAsStringAsync();

                    dynamic authResult = JsonConvert.DeserializeObject<dynamic>(authResponseString);
                    string authtoken = "";
                    if (authResult != null)
                    {
                        if (authResult.authtoken != null)
                        {
                            authtoken = authResult.authtoken.ToString();
                        }
                        else if (authResult.data != null && authResult.data.authtoken != null)
                        {
                            authtoken = authResult.data.authtoken.ToString();
                        }
                        else if (authResult.token != null)
                        {
                            authtoken = authResult.token.ToString();
                        }
                        else if (authResult.data != null && authResult.data.token != null)
                        {
                            authtoken = authResult.data.token.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(authtoken))
                    {
                        return Json(new { success = false, message = "Failed to retrieve authtoken from Auth API response: " + authResponseString }, JsonRequestBehavior.AllowGet);
                    }

                    // 2. Fetch EWayBill Data using the token
                    var ewbRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.webcorevision.com:3000/EwayBill/GetEwayBill");
                    var ewbData = new
                    {
                        gstin = "24AAICG8668B1ZI",
                        authtoken = authtoken,
                        ewbNo = ewayBillNo,
                        company_name = "Greenline"
                    };
                    string ewbJson = JsonConvert.SerializeObject(ewbData);
                    ewbRequest.Content = new StringContent(ewbJson, Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(ewbRequest);

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Json(new { success = false, message = "No content found for this E-Way Bill Number" }, JsonRequestBehavior.AllowGet);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return Content(responseString, "application/json");
                    }
                    else
                    {
                        return Json(new { success = false, message = $"EWayBill API Error: {response.ReasonPhrase} ({response.StatusCode})" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return Json(new { success = false, message = "Request timed out while fetching E-Way Bill data" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Network Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Spot PR Approve/Reject
        public ActionResult ApproveRejectSpotPR()
        {
            CYGNUS_PickUp_Request CL = new CYGNUS_PickUp_Request();
            CL.listCPR = _FS.GetSpotPR_ForApproval().ToList();
            return View(CL);
        }

        [HttpGet]
        public ActionResult GetPickupRequestDetails(string PRNo)
        {
            try
            {
                var pr = _FS.GetPickUpRequestDetails().FirstOrDefault(c => c.PRNo == PRNo);
                if (pr != null)
                {
                    pr.EnrtExpList = _FS.Get_TripExp_List(PRNo);
                    return Json(new { Status = true, Data = pr }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Status = false, Message = "Pickup Request not found." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Payment Link Management

        [HttpGet]
        public async Task<JsonResult> GetPaymentLinks(string PRNo)
        {
            try
            {
                if (string.IsNullOrEmpty(PRNo))
                {
                    return Json(new { Status = false, Message = "PR Number is required." }, JsonRequestBehavior.AllowGet);
                }

                List<CYGNUS_Payment_Links> links = _FS.GetPaymentLinks(PRNo);

                foreach (var link in links)
                {
                    if (link.Status == "CREATED")
                    {
                        DateTime expiryDate;
                        if (DateTime.TryParse(link.ExpiryDate, out expiryDate))
                        {
                            if (DateTime.Now >= expiryDate)
                            {
                                _FS.UpdatePaymentLinkStatus(link.MerchantOrderId, "EXPIRED", updatedBy: BaseUserName);
                                link.Status = "EXPIRED";
                            }
                            else
                            {
                                var details = await PhonePeHelper.GetPaymentStatusDetailsAsync(link.MerchantOrderId);
                                if (details != null && !string.IsNullOrEmpty(details.status) && details.status != "CREATED")
                                {
                                    string paymentTime = (details.status == "SUCCESS" || details.status == "PAID") ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : null;
                                    _FS.UpdatePaymentLinkStatus(link.MerchantOrderId, details.status, details.transactionId, details.phonePeTransactionId ?? details.paymentMode, paymentTime, BaseUserName);
                                    link.Status = details.status;
                                }
                            }
                        }
                    }
                }

                links = _FS.GetPaymentLinks(PRNo);
                return Json(new { Status = true, Data = links }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreatePaymentLink(string PRNo, decimal Amount)
        {
            try
            {
                if (string.IsNullOrEmpty(PRNo) || Amount <= 0)
                {
                    return Json(new { Status = false, Message = "Invalid PR Number or Amount." });
                }

                var existingLink = _FS.CheckActivePaymentLink(PRNo, Amount);
                if (existingLink != null)
                {
                    return Json(new { Status = true, Message = "Active payment link already exists for this amount.", Data = existingLink });
                }

                string customerName = "";
                string customerPhone = "";
                string customerEmail = "";
                var customer = _FS.GetCustomerDetailsByPRNo(PRNo);
                if (customer != null)
                {
                    customerName = customer.CUSTNM ?? "";
                    customerPhone = customer.MOBILENO ?? "";
                    customerEmail = customer.EMAILIDS ?? "";
                }

                string encryptedPR = GF.Encrypt(PRNo, "CygnuXSecretKey").Replace("+", "-").Replace("/", "_").Replace("=", "");
                string merchantOrderId = encryptedPR + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                var payLinkData = await PhonePeHelper.CreatePayLinkAsync(Amount, merchantOrderId, customerName, customerPhone, customerEmail);

                if (payLinkData != null)
                {
                    string paymentUrl = payLinkData.GetUrl();
                    string token = payLinkData.token ?? payLinkData.payLinkId ?? "";
                    string expiryIso = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");

                    _FS.SavePaymentLink(PRNo, merchantOrderId, Amount, paymentUrl, token, "CREATED", expiryIso, BaseUserName);

                    var newLink = new CYGNUS_Payment_Links
                    {
                        Amount = Amount,
                        PaymentLink = paymentUrl,
                        PaymentToken = token,
                        Status = "CREATED",
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ExpiryDate = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                        MerchantOrderId = merchantOrderId,
                        CreatedBy = BaseUserName
                    };

                    return Json(new { Status = true, Message = "Payment Link created successfully.", Data = newLink });
                }

                return Json(new { Status = false, Message = "Failed to generate payment link." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        #endregion

        #region TripExpense

        [HttpGet]
        public ActionResult GetTripExpenses(string TripNo)
        {
            try
            {
                var list = _FS.GetTripExpenses(TripNo);
                return Json(new { Status = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveTripExpense(string TripNo, string PRNo, string VehicleNo,
            string ExpenseCode, decimal Amount, string PaymentType, bool IsBilled,
            HttpPostedFileBase ExpFile)
        {
            try
            {
                string docPath = "";
                if (ExpFile != null && ExpFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/UploadedDocuments/TripExpence/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                    string ext = Path.GetExtension(ExpFile.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileNameWithoutExtension(ExpFile.FileName) + ext;
                    ExpFile.SaveAs(Path.Combine(folderPath, fileName));
                    docPath = "/UploadedDocuments/TripExpence/" + fileName;
                }
                DataTable dt = _FS.SaveTripExpense(TripNo, PRNo, VehicleNo, ExpenseCode, Amount, PaymentType, IsBilled, docPath, BaseUserName);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["Message"].ToString() == "Done")
                    return Json(new { Status = true, Message = "Expense saved successfully.", DocPath = docPath });
                return Json(new { Status = false, Message = dt?.Rows[0]["Message"]?.ToString() ?? "Failed to save expense." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveDocketExpense(int Id, string DockNo, string ChargeCode, string PaymentType,
            bool IsBilled, decimal Amount, string Operator, string ExistingAttachment, HttpPostedFileBase ExpFile)
        {
            try
            {
                string docPath = string.IsNullOrEmpty(ExistingAttachment) ? "" : ExistingAttachment;
                if (ExpFile != null && ExpFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/UploadedDocuments/TripExpence/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                    string ext = Path.GetExtension(ExpFile.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileNameWithoutExtension(ExpFile.FileName) + ext;
                    ExpFile.SaveAs(Path.Combine(folderPath, fileName));
                    docPath = "/UploadedDocuments/TripExpence/" + fileName;
                }

                string xmlData = "<Cygnus_Master_Docket_Expense>";
                xmlData += "<Id>" + Id + "</Id>";
                xmlData += "<DockNo>" + DockNo + "</DockNo>";
                xmlData += "<ChargeCode>" + ChargeCode + "</ChargeCode>";
                xmlData += "<PaymentType>" + PaymentType + "</PaymentType>";
                xmlData += "<IsBillable>" + (IsBilled ? "Y" : "N") + "</IsBillable>";
                xmlData += "<Amount>" + Amount + "</Amount>";
                xmlData += "<Attachment>" + (docPath ?? "") + "</Attachment>";
                xmlData += "<Operator>" + Operator + "</Operator>";
                xmlData += "</Cygnus_Master_Docket_Expense>";

                DataTable dt = _FS.SaveDocketExpense(xmlData, BaseCompanyCode, BaseUserName);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string status = dt.Rows[0]["Status"]?.ToString();
                    string message = dt.Rows[0]["Message"]?.ToString();
                    string newId = dt.Rows[0]["Id"]?.ToString();
                    if (status == "1")
                    {
                        return Json(new { Status = true, Message = message, Id = newId, DocPath = docPath });
                    }
                    return Json(new { Status = false, Message = message ?? "Failed to save docket expense." });
                }
                return Json(new { Status = false, Message = "Failed to save docket expense." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SubmitDoPodUpload(CYGNUS_DO_POD_Upload model, HttpPostedFileBase evtFile, HttpPostedFileBase evtFileBack, string PRNo, int? EventId, int? StopId, bool? isFromList)
        {
            try
            {
                if (model == null)
                {
                    model = new CYGNUS_DO_POD_Upload();
                }

                string fileName = "";
                if (evtFile != null && evtFile.ContentLength > 0)
                {
                    string folderName = "TripEvents";
                    string filePrefix = "File";

                    if (model.ActivityType == "POD")
                    {
                        folderName = "POD";
                        filePrefix = model.DocketNo;
                    }
                    else if (model.ActivityType == "DO")
                    {
                        folderName = "DO";
                        filePrefix = model.TripNo;
                    }

                    string folderPath = Server.MapPath("~/UploadedDocuments/" + folderName + "/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string ext = Path.GetExtension(evtFile.FileName);
                    string uniqueFileName = filePrefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                    evtFile.SaveAs(Path.Combine(folderPath, uniqueFileName));
                    fileName = "/UploadedDocuments/" + folderName + "/" + uniqueFileName;
                }

                string fileNameBack = "";
                if (evtFileBack != null && evtFileBack.ContentLength > 0)
                {
                    string folderName = "TripEvents";
                    string filePrefix = "File_Back";

                    if (model.ActivityType == "POD")
                    {
                        folderName = "POD";
                        filePrefix = model.DocketNo + "_Back";
                    }
                    else if (model.ActivityType == "DO")
                    {
                        folderName = "DO";
                        filePrefix = model.TripNo + "_Back";
                    }

                    string folderPath = Server.MapPath("~/UploadedDocuments/" + folderName + "/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string ext = Path.GetExtension(evtFileBack.FileName);
                    string uniqueFileName = filePrefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                    evtFileBack.SaveAs(Path.Combine(folderPath, uniqueFileName));
                    fileNameBack = "/UploadedDocuments/" + folderName + "/" + uniqueFileName;
                }

                string xmlData = "<CYGNUS_DO_POD_Upload>";
                xmlData += "<Id>" + model.Id + "</Id>";
                xmlData += "<TripNo>" + model.TripNo + "</TripNo>";
                xmlData += "<DocketNo>" + (model.DocketNo ?? "") + "</DocketNo>";
                xmlData += "<ActivityType>" + model.ActivityType + "</ActivityType>";
                xmlData += "<Remarks>" + (model.Remarks ?? "") + "</Remarks>";
                xmlData += "<FilePath>" + fileName + "</FilePath>";
                xmlData += "<FilePathBack>" + fileNameBack + "</FilePathBack>";
                if (model.Pieces.HasValue)
                {
                    xmlData += "<Pieces>" + model.Pieces.Value.ToString() + "</Pieces>";
                }
                if (model.Qty.HasValue)
                {
                    xmlData += "<Qty>" + model.Qty.Value.ToString() + "</Qty>";
                }
                xmlData += "</CYGNUS_DO_POD_Upload>";

                DataTable dt = _FS.SaveDoPodUpload(xmlData, BaseCompanyCode, BaseUserName);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bool status = Convert.ToInt32(dt.Rows[0]["Status"]) == 1;
                    string message = dt.Rows[0]["Message"].ToString();

                    if (isFromList != true)
                    {
                        if (status && model.ActivityType == "POD" && StopId.HasValue)
                        {
                            try
                            {
                                DataTable countDt = _FS.GetTripPODCounts(model.TripNo, StopId.Value);
                                if (countDt != null && countDt.Rows.Count > 0)
                                {
                                    int totalDockets = Convert.ToInt32(countDt.Rows[0]["TotalDockets"]);
                                    int uploadedPODs = Convert.ToInt32(countDt.Rows[0]["UploadedPODs"]);
                                    string activePR = countDt.Rows[0]["PRNo"].ToString();
                                    string activeVeh = countDt.Rows[0]["VehicleNo"].ToString();
                                    int stopIdVal = countDt.Rows[0]["StopId"] != DBNull.Value ? Convert.ToInt32(countDt.Rows[0]["StopId"]) : StopId.Value;

                                    if (totalDockets > 0 && uploadedPODs == totalDockets)
                                    {
                                        int activeEventId = (EventId.HasValue && EventId.Value > 0) ? EventId.Value : 11;
                                        _FS.UpdateTripEventLastStatus(activePR, activeVeh, activeEventId, BaseUserName.ToUpper(), model.TripNo, stopIdVal, "All PODs uploaded successfully");
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // Silent fallback so it does not block successful save
                            }
                        }

                        if (status && model.ActivityType == "DO")
                        {
                            try
                            {
                                var tripList = _FS.GetTripList(BaseUserName);
                                var trip = tripList.FirstOrDefault(t => t.TripNo == model.TripNo);
                                if (trip != null)
                                {
                                    int activeEventId = (EventId.HasValue && EventId.Value > 0) ? EventId.Value : 6;
                                    _FS.UpdateTripEventStatus(PRNo, trip.VehicleId, activeEventId, BaseUserName.ToUpper(), model.TripNo, "DO uploaded successfully");
                                }
                            }
                            catch (Exception)
                            {
                                // Silent fallback so it does not block successful save
                            }
                        }
                    }

                    return Json(new { Status = status, Message = message });
                }
                return Json(new { Status = false, Message = "Failed to save data." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }
        #endregion

        #region Trip Notes
        [HttpPost]
        public ActionResult AddTripNotes(CYGNUS_TripViewModel tripData, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/UploadedDocuments/TripNotes/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string ext = Path.GetExtension(fileUpload.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    fileUpload.SaveAs(Path.Combine(folderPath, fileName));
                    tripData.TripNote.Document = fileName;
                }
                else
                {
                    tripData.TripNote.Document = tripData.TripNote.Document ?? "";
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(tripData.TripNote.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, tripData.TripNote);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                }

                DataTable dt = _FS.SaveTripNotes(xmlDoc.InnerXml, BaseUserName.ToUpper(), BaseCompanyCode.ToUpper());
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetTripNotes(string tripNo)
        {
            try
            {
                List<CYGNUS_TripNotes> list = _FS.GetTripNotesList(tripNo);
                return Json(new { Status = true, Data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNotesSubCategoryByCategory(string category)
        {
            try
            {
                var mapping = _MS.GetNotesCategoryMappingList().FirstOrDefault(m => m.Category == category && m.Active == "Y");

                if (mapping != null && !string.IsNullOrEmpty(mapping.NotesSubCategory))
                {
                    var subCatIds = mapping.NotesSubCategory.Split(',').Select(s => s.Trim()).ToList();

                    var allSubCats = _MS.GetGeneralMasterWithParam("NOTESSUBCAT").ToList();

                    var filtered = allSubCats.Where(c => subCatIds.Contains(c.CodeId)).Select(c => new
                    {
                        id = c.CodeId,
                        text = c.CodeDesc
                    }).ToList();

                    return Json(new { Status = true, Data = filtered }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Status = true, Data = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Vehicle Event
        [HttpPost]
        public ActionResult SaveVehicleEvent(CYGNUS_Vehicle_Event VehicleEvent, HttpPostedFileBase UploadFile)
        {
            try
            {
                if (UploadFile != null && UploadFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/UploadedDocuments/VehicleEvent/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string ext = Path.GetExtension(UploadFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    UploadFile.SaveAs(Path.Combine(folderPath, fileName));
                    VehicleEvent.Attachment = fileName;
                }
                else
                {
                    VehicleEvent.Attachment = VehicleEvent.Attachment ?? "";
                }

                // Populate null string properties to empty strings for XML serialization safety
                foreach (var prop in VehicleEvent.GetType().GetProperties())
                {
                    if (!prop.CanWrite)
                        continue;

                    var value = prop.GetValue(VehicleEvent);

                    if (prop.PropertyType == typeof(string) && value == null)
                    {
                        prop.SetValue(VehicleEvent, string.Empty);
                    }
                }

                if (!string.IsNullOrEmpty(VehicleEvent.FromDate))
                {
                    if (DateTime.TryParse(VehicleEvent.FromDate, out DateTime fromDate))
                    {
                        VehicleEvent.FromDate = fromDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                }

                XmlDocument xmlBasicInfo = new XmlDocument();
                XmlSerializer xmlBasicInfoSerializer = new XmlSerializer(VehicleEvent.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlBasicInfoSerializer.Serialize(xmlStream, VehicleEvent);
                    xmlStream.Position = 0;
                    xmlBasicInfo.Load(xmlStream);
                }
                DataTable dt = _FS.SaveVehicleEvent(xmlBasicInfo.InnerXml.ReplaceSpecialCharacters(), BaseUserName, BaseCompanyCode);

                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Please contact your support team." });
            }
        }
        #endregion

        #region POD Verify
        public ActionResult PODList()
        {
            return View();
        }

        public JsonResult GetPODListJson()
        {
            List<CYGNUS_DO_POD_Upload> POD = new List<CYGNUS_DO_POD_Upload>();
            try
            {
                POD = _FS.GetAllPODList();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
            }
            return Json(POD, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePODForVerify(int id)
        {
            try
            {
                var data = _FS.EditPODForVerify(id, BaseUserName);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditPOD(int Id)
        {
            try
            {
                var model = _FS.GetPODById(Id);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Approve and Edit Consignor, Consignee, Lane 
       
        public ActionResult Consignor_Consignee_LaneList()
        {
            CCLMModel CLVM = new CCLMModel();
            try
            {
                CLVM.listCCCLM = new List<Cygnus_Consignor_Consignee_Lane_Mapping>();
                CLVM.listCCCLM = _FS.GetConsignor_Consignee_LaneList().ToList();
                CLVM.CCCLM = new Cygnus_Consignor_Consignee_Lane_Mapping();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(CLVM);
        }

        public ActionResult AddEditConsignorConsigneeLaneMapping(int id)
        {
            Cygnus_Consignor_Consignee_Lane_Mapping CL = new Cygnus_Consignor_Consignee_Lane_Mapping();
            try
            {
                if (id > 0)
                {
                    CL = _FS.GetConsignorConsigneeById(id);
                }
                //ViewBag.StateList = _masterService.GetStateMaster().ToList();
                //ViewBag.ZoneList = _masterService.GetGeneralMaster().Where(c => c.CodeType.ToUpper() == "ZONE" && c.StatusCode == "Y").ToList();
            }
            catch (Exception)
            {
                return RedirectToAction("Consignor_Consignee_LaneList");
            }
            return PartialView("_Consignor_Consignee_LaneModal", CL);
        }
        [HttpPost]
        public JsonResult AddEditConsignorConsigneeLaneMapping(Cygnus_Consignor_Consignee_Lane_Mapping CCCLM)
        {
            bool Status = false;
            string Message = "";
            try
            {
                string Query = "EXEC USP_EditConsignorConsigneeLaneMapping '" + CCCLM.ConsignorCode + "','" + CCCLM.ConsigneeCode + "','" + CCCLM.LaneId + "','" + CCCLM.Id +"','" + BaseUserName + "'";
                DataTable dt = GF.GetDataTableFromSP(Query);
                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() == "Done")
                {
                    Status = true;
                }
                else
                {
                    Message = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Json(new { Status = Status, Message = Message });
        }
        public JsonResult GetConsignorListJson(string searchTerm)
        {
            try
            {
                searchTerm = searchTerm ?? "";
                var list = _MS.GetConsignnorCustomerListJson()
                              .Where(c => string.IsNullOrEmpty(searchTerm) ||
                                          (c.CUSTCD != null && c.CUSTCD.ToUpper().Contains(searchTerm.ToUpper())) ||
                                          (c.CUSTNM != null && c.CUSTNM.ToUpper().Contains(searchTerm.ToUpper())))
                              .Take(50)
                              .Select(c => new { id = c.CUSTCD, text = c.CUSTNM + " (" + c.CUSTCD + ")" })
                              .ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetConsigneeListJson(string searchTerm)
        {
            try
            {
                searchTerm = searchTerm ?? "";
                var list = _MS.GetConsigneeCustomerListJson()
                              .Where(c => string.IsNullOrEmpty(searchTerm) ||
                                          (c.CUSTCD != null && c.CUSTCD.ToUpper().Contains(searchTerm.ToUpper())) ||
                                          (c.CUSTNM != null && c.CUSTNM.ToUpper().Contains(searchTerm.ToUpper())))
                              .Take(50)
                              .Select(c => new { id = c.CUSTCD, text = c.CUSTNM + " (" + c.CUSTCD + ")" })
                              .ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ApproveConsignorConsigneeMapping(string ConsignorCode, string ConsigneeCode,int LaneId, int id)
        {
            bool status = false;
            try
            {
                status = _FS.ApproveConsignorConsigneeMapping(ConsignorCode, ConsigneeCode, LaneId, id, BaseUserName);
            }
            catch (Exception)
            {
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        
        #endregion
    }
}