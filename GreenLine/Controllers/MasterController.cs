using GreenLine.Classes;
using GreenLine.Filters;
using GreenLineDataService.Helper;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using WebMatrix.WebData;

namespace GreenLine.Controllers
{
    [Authorize]
    public class MasterController : BaseController
    {
        GeneralFunctions GF = new GeneralFunctions();
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        private readonly IMasterService _masterService;
        public MasterController() : this(new MasterService())
        {
        }

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        public ActionResult DataTable()
        {
            return View();
        }

        #region State Master
        public ActionResult StateMaster(string search)
        {

            CYGNUS_StateViewModel WCVM = new CYGNUS_StateViewModel();
            try
            {
                WCVM.WST = new CYGNUS_State();
                WCVM.listWS = new List<CYGNUS_State>();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WCVM);


            //CYGNUS_StateViewModel WSVM = new CYGNUS_StateViewModel();
            //try
            //{
            //    WSVM.listWS = new List<CYGNUS_State>();
            //    WSVM.listWS = _masterService.GetStateMaster().ToList();

            //    if (!string.IsNullOrEmpty(search))
            //    {
            //        WSVM.listWS = WSVM.listWS.Where(x =>
            //            x.stnm.ToLower().Contains(search.ToLower()) ||
            //            x.stcd.ToLower().Contains(search.ToLower()) ||
            //            x.zonename.ToLower().Contains(search.ToLower()) ||
            //            x.CountryName.ToLower().Contains(search.ToLower())
            //        ).ToList();
            //    }

            //    WSVM.WST = new CYGNUS_State();
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
            //    return View("Error");
            //}
            //return View(WSVM);
        }

        public ActionResult AddEditStateMaster(CYGNUS_StateViewModel WSVM)
        {
            bool Status = false;
            try
            {
                if (WSVM.WST.activeflag2 == true)
                    WSVM.WST.activeflag = "Y";
                else
                    WSVM.WST.activeflag = "N";

                string MstDetails = "<State>";
                MstDetails = MstDetails + "<SrNo>" + WSVM.WST.srno + "</SrNo>";
                MstDetails = MstDetails + "<StateCode>" + WSVM.WST.stcd + "</StateCode>";
                MstDetails = MstDetails + "<Name>" + WSVM.WST.stnm + "</Name>";
                MstDetails = MstDetails + "<ActiveFlag>" + WSVM.WST.activeflag + "</ActiveFlag>";
                MstDetails = MstDetails + "<entryby>" + BaseUserName.ToUpper() + "</entryby>";
                MstDetails = MstDetails + "<UpdateBy>" + BaseUserName.ToUpper() + "</UpdateBy>";
                MstDetails = MstDetails + "<IsUnionTerritory>" + WSVM.WST.IsUnionTerritory + "</IsUnionTerritory>";
                MstDetails = MstDetails + "<statePrefix>" + WSVM.WST.statePrefix + "</statePrefix>";
                MstDetails = MstDetails + "<Zone>" + WSVM.WST.Zone + "</Zone>";
                MstDetails = MstDetails + "<Countrycd>" + WSVM.WST.countrycd + "</Countrycd>";
                MstDetails = MstDetails + "</State>";
                DataTable Dt = _masterService.AddEditStateMaster(MstDetails);
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStateDetailsById(int id)
        {
            try
            {
                var data = _masterService.GetStateMaster().FirstOrDefault(c => c.srno == id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckDuplicateState(string StateName, string StateCode, int srno = 0)
        {
            try
            {
                bool isDuplicate = _masterService.ExistsState(StateName, StateCode, srno);
                return Json(isDuplicate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ActiveInActiveState(int id)
        {
            try
            {
                var data = _masterService.ActiveInActiveState(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ActiveInActiveLocation(string LocCode)
        {
            try
            {
                var data = _masterService.ActiveInActiveLocation(LocCode);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public void ExportReport(string MethodName, string FileName)
        {
            DataTable dt = _masterService.GetExcelData(MethodName, BaseUserName, BaseCompanyCode, BaseLocationCode, BaseYearVal);
            ExportUtilities.ExportToCSV(dt, FileName);
        }
        #endregion

        #region Organization Master
        public ActionResult Organization()
        {
            CYGNUS_COMPANY_MASTER COM = new CYGNUS_COMPANY_MASTER();
            return View(COM);
        }

        public ActionResult OrganizationMaster(string CompanyCode)
        {
            var model = new CYGNUS_Organization_Master();
            if (CompanyCode != "0")
            {
                model = _masterService.GetComapanyDetails(CompanyCode);
                if (model.OrgDet != null)
                {
                    model.OrgDet.IsActive = (model.OrgDet.ACTIVEFLAG == "Y");
                }
            }
            else
            {
                if (model.OrgDet != null)
                {
                    model.OrgDet.IsActive = true;
                }
            }

            return View("_AddEditOrganizationMaster", model);
        }

        [HttpPost]
        public ActionResult AddEditOrganization(CYGNUS_Organization_Master iData, HttpPostedFileBase[] fileUpload, string BankJsonData)
        {
            string OrgCode = "", Message = "", Status = "0", OrgName = "";
            try
            {
                if (!string.IsNullOrEmpty(BankJsonData))
                {
                    var bankList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Organization_Bank_Details>>(BankJsonData);

                    iData.OrgBnkDetList = bankList;
                }

                string serverPath = "";

                // ✅ File Upload (Get only the first file, and save only the image name)
                if (fileUpload != null && fileUpload.Length > 0 && fileUpload[0] != null && fileUpload[0].ContentLength > 0)
                {
                    var file = fileUpload[0];

                    // Get the file extension
                    string extension = Path.GetExtension(file.FileName);

                    // Define the target file name
                    string userFileName = Guid.NewGuid() + extension;

                    // Define the target path
                    string relativePath = "~/assets/img/logos/" + userFileName;
                    serverPath = Server.MapPath(relativePath);

                    // Save the file to the target path
                    file.SaveAs(serverPath);

                    // 👉 Save only the image NAME (not full URL/path) in model
                    iData.OrgDet.CompanyLogoUrl = userFileName;
                }

                if (iData.OrgDet != null)
                {
                    iData.OrgDet.ACTIVEFLAG = iData.OrgDet.IsActive ? "Y" : "N";
                }

                // ✅ Convert Model → XML
                //string MstDetails = ConvertToXml(iData.OrgDet);

                string OrgDet = "<Organization_Details>";

                OrgDet = OrgDet + "<GST_Number>" + iData.OrgDet.GST_Number + "</GST_Number>";
                OrgDet = OrgDet + "<PANNo>" + iData.OrgDet.PANNo + "</PANNo>";
                OrgDet = OrgDet + "<BRCD>" + iData.OrgDet.BRCD + "</BRCD>";
                OrgDet = OrgDet + "<COMPANY_NAME>" + iData.OrgDet.COMPANY_NAME + "</COMPANY_NAME>";
                OrgDet = OrgDet + "<Biling_Type>" + iData.OrgDet.Biling_Type + "</Biling_Type>";
                OrgDet = OrgDet + "<Country_Code>" + iData.OrgDet.Country_Code + "</Country_Code>";
                OrgDet = OrgDet + "<State_Code>" + iData.OrgDet.State_Code + "</State_Code>";
                OrgDet = OrgDet + "<City_Code>" + iData.OrgDet.City_Code + "</City_Code>";
                OrgDet = OrgDet + "<Sap_Code>" + iData.OrgDet.Sap_Code + "</Sap_Code>";
                OrgDet = OrgDet + "<ADDRESS>" + iData.OrgDet.ADDRESS + "</ADDRESS>";
                OrgDet = OrgDet + "<Address1>" + iData.OrgDet.Address1 + "</Address1>";
                OrgDet = OrgDet + "<Address2>" + iData.OrgDet.Address2 + "</Address2>";
                OrgDet = OrgDet + "<PinCode>" + iData.OrgDet.PinCode + "</PinCode>";
                OrgDet = OrgDet + "<Location>" + iData.OrgDet.Location + "</Location>";
                OrgDet = OrgDet + "<CONTACTNO>" + iData.OrgDet.CONTACTNO + "</CONTACTNO>";
                OrgDet = OrgDet + "<Email>" + iData.OrgDet.Email + "</Email>";
                OrgDet = OrgDet + "<CINNumber>" + iData.OrgDet.CINNumber + "</CINNumber>";
                OrgDet = OrgDet + "<Employee_Code>" + iData.OrgDet.Employee_Code + "</Employee_Code>";
                OrgDet = OrgDet + "<Invoice_Bottom_Address>" + iData.OrgDet.Invoice_Bottom_Address + "</Invoice_Bottom_Address>";
                OrgDet = OrgDet + "<Trip_Series>" + iData.OrgDet.Trip_Series + "</Trip_Series>";
                OrgDet = OrgDet + "<Register_Office_Address>" + iData.OrgDet.Register_Office_Address + "</Register_Office_Address>";
                OrgDet = OrgDet + "<CONTACT_PERSON>" + iData.OrgDet.CONTACT_PERSON + "</CONTACT_PERSON>";
                OrgDet = OrgDet + "<TermsAndCondition>" + iData.OrgDet.TermsAndCondition + "</TermsAndCondition>";
                OrgDet = OrgDet + "<LR_Terms_And_Condition>" + iData.OrgDet.LR_Terms_And_Condition + "</LR_Terms_And_Condition>";
                OrgDet = OrgDet + "<Credit_Debit_Terms_And_Condition>" + iData.OrgDet.Credit_Debit_Terms_And_Condition + "</Credit_Debit_Terms_And_Condition>";
                OrgDet = OrgDet + "<CompanyLogoUrl>" + iData.OrgDet.CompanyLogoUrl + "</CompanyLogoUrl>";
                OrgDet = OrgDet + "<EInvoice_User_Name>" + iData.OrgDet.EInvoice_User_Name + "</EInvoice_User_Name>";
                OrgDet = OrgDet + "<EInvoice_Password>" + iData.OrgDet.EInvoice_Password + "</EInvoice_Password>";
                OrgDet = OrgDet + "<EInvoice_ASP_Id>" + iData.OrgDet.EInvoice_ASP_Id + "</EInvoice_ASP_Id>";
                OrgDet = OrgDet + "<EInvoice_Client_Id>" + iData.OrgDet.EInvoice_Client_Id + "</EInvoice_Client_Id>";
                OrgDet = OrgDet + "<ACTIVEFLAG>" + iData.OrgDet.ACTIVEFLAG + "</ACTIVEFLAG>";
                OrgDet = OrgDet + "</Organization_Details>";

                string OrgBnkDetList = "<Organization_Bank_Details_List>";
                foreach (var item in iData.OrgBnkDetList)
                {
                    OrgBnkDetList = OrgBnkDetList + "<Bank>";
                    OrgBnkDetList = OrgBnkDetList + "<SrNo>" + item.SrNo + "</SrNo>";
                    OrgBnkDetList = OrgBnkDetList + "<Beneficiary_Name>" + item.Beneficiary_Name + "</Beneficiary_Name>";
                    OrgBnkDetList = OrgBnkDetList + "<Bank_Name>" + item.Bank_Name + "</Bank_Name>";
                    OrgBnkDetList = OrgBnkDetList + "<Bank_Account_No>" + item.Bank_Account_No + "</Bank_Account_No>";
                    OrgBnkDetList = OrgBnkDetList + "<Branch_Name>" + item.Branch_Name + "</Branch_Name>";
                    OrgBnkDetList = OrgBnkDetList + "<IFSC_Code>" + item.IFSC_Code + "</IFSC_Code>";
                    OrgBnkDetList = OrgBnkDetList + "<MICR_Code>" + item.MICR_Code + "</MICR_Code>";
                    OrgBnkDetList = OrgBnkDetList + "<RTGS_NEFT_Code>" + item.RTGS_NEFT_Code + "</RTGS_NEFT_Code>";
                    OrgBnkDetList = OrgBnkDetList + "<Bank_Address>" + item.Bank_Address + "</Bank_Address>";
                    OrgBnkDetList = OrgBnkDetList + "<IsActive>" + item.IsActive + "</IsActive>";
                    OrgBnkDetList = OrgBnkDetList + "</Bank>";
                }
                OrgBnkDetList = OrgBnkDetList + "</Organization_Bank_Details_List>";

                DataTable Dt = _masterService.AddEditOrganization(iData.OrgDet.COMPANY_CODE, OrgDet, OrgBnkDetList, BaseUserName);

                if (Dt != null && Dt.Rows.Count > 0 && Convert.ToBoolean(Dt.Rows[0]["Status"]))
                {
                    OrgCode = Dt.Rows[0]["OrgCode"].ToString();
                    Message = Dt.Rows[0]["Message"].ToString();
                    Status = "1";
                    OrgName = Dt.Rows[0]["OrgName"].ToString().ToUpper();
                }
                else
                {
                    OrgCode = "";
                    Message = Dt.Rows[0]["Message"].ToString();
                    Status = "0";
                    OrgName = "";
                }
                return RedirectToAction("OrganizationDone", new { OrgCode = OrgCode, OrgName = OrgName, Status = Status });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the company details: " + ex.Message;
                return View("Error");
            }

        }

        public ActionResult OrganizationDone(string OrgCode, string OrgName, string Status)
        {
            if (Status == "1")
            {
                ViewBag.OrgCode = OrgCode.ToUpper() + " : " + OrgName.ToUpper();
            }
            ViewBag.Status = Status;
            return View();
        }

        public JsonResult ActiveInActiveOrganization(string CompanyCode)
        {
            try
            {
                var data = _masterService.ActiveInActiveOrganization(CompanyCode);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public static string ConvertToXml(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }



        public JsonResult GetStatesByCountry(string countryCode)
        {
            var states = _masterService.GetStateMaster()
                .Where(x => x.countrycd == countryCode)
                .Select(x => new
                {
                    Value = x.stcd,
                    Text = x.stnm
                }).ToList();

            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCitiesByState(string stateCode)
        {
            var cities = _masterService.GetCityMaster()
                .Where(x => x.state == stateCode)
                .Select(x => new
                {
                    Value = x.city_code,
                    Text = x.Location
                }).ToList();

            return Json(cities, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region General Master
        public ActionResult ListGeneralMaster()
        {
            return View(_masterService.GetCodetypesMasterList().Distinct().ToList());
        }
        public ActionResult GeneralMaster(string id)
        {
            CYGNUS_Master_GeneralViewModel WVM = new CYGNUS_Master_GeneralViewModel();
            try
            {
                List<CYGNUS_Master_General> GeneralMasterList = _masterService.GetGeneralMaster().Where(c => c.CodeType == id.ToUpper().Trim()).ToList();

                WVM.listWMG = GeneralMasterList;
                WVM.WMG = new CYGNUS_Master_General();
                WVM.WMCT = _masterService.GetCodetypesMasterList().FirstOrDefault(c => c.HeaderCode.ToUpper().Trim() == id.ToUpper().Trim());// change 18/7
                ViewBag.CodeDec = WVM.WMCT.HeaderDesc;
                WVM.WMG.CodeType = WVM.WMCT.HeaderCode;
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WVM);
        }
        public ActionResult GetGeneralMasterPartial(string id)
        {
            CYGNUS_Master_GeneralViewModel WVM = new CYGNUS_Master_GeneralViewModel();
            try
            {
                WVM.WMG = new CYGNUS_Master_General();
                WVM.WMCT = _masterService.GetCodetypesMasterList().FirstOrDefault(c => c.HeaderCode.ToUpper().Trim() == id.ToUpper().Trim());
                WVM.WMG.CodeType = WVM.WMCT.HeaderCode;
            }
            catch (Exception ex)
            {
                return Content("Error loading partial: " + ex.Message);
            }
            return PartialView("_GeneralMaster_Partial", WVM);
        }
        public ActionResult AddEditGeneralMaster(string CodeId, string CodeDesc, string CodeType, bool StatusCode, string codefor, decimal noofdigits)
        {
            bool Status = false;
            try
            {

                string MstDetails = "<root><MST>";
                MstDetails = MstDetails + "<CodeType>" + CodeType + "</CodeType>";
                MstDetails = MstDetails + "<CodeId>" + CodeId + "</CodeId>";
                MstDetails = MstDetails + "<CodeDesc>" + CodeDesc.Trim().ToUpper() + "</CodeDesc>";
                MstDetails = MstDetails + "<StatusCode>" + (StatusCode ? "Y" : "N") + "</StatusCode>";
                MstDetails = MstDetails + "<codefor>" + codefor + "</codefor>";
                MstDetails = MstDetails + "<noofdigits>" + noofdigits + "</noofdigits>";
                MstDetails = MstDetails + "<EntryBy>" + BaseUserName + "</EntryBy></MST></root>";

                DataTable Dt = _masterService.AddEditGeneralMaster(MstDetails.Replace("&", "&amp;"), "U", BaseFinYear.Split('-')[0].ToString());
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return new JsonResult()
                {
                    Data = new
                    {
                        Status,
                    }
                };
            }
            catch (Exception)
            {
                return Json(Status);
            }
        }
        public JsonResult CheckDuplicateGeneralMaster(string CodeType, string CodeDesc)
        {
            try
            {
                string Count = _masterService.CheckDuplicateGeneralMaster(CodeType, CodeDesc);

                return new JsonResult()
                {
                    Data = new
                    {
                        Count,
                    }
                };
            }
            catch (Exception)
            {
                return Json(0);
            }
        }
        public JsonResult GetGeneralMasterDetailsModules(string id, string str)
        {
            id = id.ToUpper();

            List<CYGNUS_Master_General> Modules = new List<CYGNUS_Master_General>();
            Modules = _masterService.GetGeneralMaster().Where(c => c.CodeType.ToUpper() == id && c.StatusCode.ToUpper() == "Y").ToList();

            var ModulesList = (from e in Modules
                               select new
                               {
                                   id = e.CodeId,
                                   text = e.CodeDesc,
                               }).Distinct().ToList();
            return Json(ModulesList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStatusCodeforGeneralMaster(string CodeId, string CodeType, bool StatusCode)
        {
            bool Status = false;
            try
            {
                string MstDetails = "<root><MST>";
                MstDetails = MstDetails + "<CodeType>" + CodeType + "</CodeType>";
                MstDetails = MstDetails + "<CodeId>" + CodeId + "</CodeId>";
                MstDetails += "<CodeDesc></CodeDesc>";
                MstDetails = MstDetails + "<StatusCode>" + (StatusCode ? "Y" : "N") + "</StatusCode>";
                MstDetails = MstDetails + "<EntryBy>" + BaseUserName + "</EntryBy></MST></root>";

                DataTable Dt = _masterService.AddEditGeneralMaster(MstDetails, "US", BaseFinYear.Split('-')[0].ToString());
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return new JsonResult()
                {
                    Data = new
                    {
                        Status,
                    }
                };
            }
            catch (Exception)
            {
                return Json(Status);
            }
        }
        #endregion

        #region Location Master

        public ActionResult LocationList()
        {
            List<CYGNUS_location> ListLocations = new List<CYGNUS_location>();
            try
            {
                ListLocations = _masterService.GetLocationDetails(1);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(ListLocations);
        }

        public JsonResult GetLocationListJson(string searchTerm)
        {
            List<CYGNUS_location> ListLocations = new List<CYGNUS_location>();

            ListLocations = _masterService.GetLocationDetails().Where(c => c.ActiveFlag.ToUpper() == "Y" && (c.LocName.ToUpper().Contains(searchTerm.ToUpper()) || c.LocCode.ToUpper().Contains(searchTerm.ToUpper()))).ToList().OrderBy(c => c.LocName).ToList();
            var SearchList = (from e in ListLocations
                              select new
                              {
                                  id = e.LocCode,
                                  text = e.LocName,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityByStateJson(string Id)
        {
            List<CYGNUS_citymaster> ListCity = _masterService.GetCityMaster();

            ListCity = ListCity.Where(c => c.state == Id).OrderBy(m => m.Location).ToList();
            var SearchList = (from e in ListCity
                              select new
                              {
                                  Value = e.city_code,
                                  Text = e.Location,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetReportingLocationByReportingTo(string Id)
        {
            List<CYGNUS_location> ListLocations = _masterService.GetLocationDetails();
            ListLocations = ListLocations.Where(c => c.Loc_Level == Convert.ToDecimal(Id) && c.ActiveFlag == "Y").OrderBy(m => m.LocName).ToList();
            var SearchList = (from e in ListLocations
                              select new
                              {
                                  Value = e.LocCode,
                                  Text = e.LocName,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }

        public bool CheckDuplicateLocation(string Id)
        {
            List<CYGNUS_location> ListLocations = _masterService.GetLocationDetails();
            ListLocations = ListLocations.Where(c => c.LocCode.ToUpper() == Id.ToUpper()).OrderBy(m => m.LocName).ToList();
            return ListLocations.Count > 0 ? false : true;
        }

        public ActionResult LocationMaster(string Id)
        {
            LocationViewModel WSVM = new LocationViewModel();
            try
            {
                WSVM.EditFlag = "false";
                List<CYGNUS_location> ListLocations = _masterService.GetLocationDetails(1);
                WSVM.WL = new CYGNUS_location();

                if (Id != null && Id != "")
                {
                    WSVM.EditFlag = "true";
                    WSVM.WL = ListLocations.Where(c => c.LocCode.ToUpper() == Id.ToUpper()).FirstOrDefault();
                }

                WSVM.ListGnMST = _masterService.GetGeneralMaster();
                WSVM.listWS = ListLocations;
                WSVM.ListLocState = _masterService.GetStateMaster();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WSVM);
        }


        [HttpPost]
        public string LocationMasterSubmit(LocationViewModel VM)
        {
            VM.WL.LocAddr = VM.WL.LocAddr.Replace("–", " ").Trim();
            VM.WL.loc_startdt_str = Convert.ToDateTime(VM.WL.loc_startdt).ToString("dd MMM yyyy");
            VM.WL.loc_enddt_str = Convert.ToDateTime(VM.WL.loc_enddt).ToString("dd MMM yyyy");

            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(VM.WL.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, VM.WL);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
            }

            bool Status = _masterService.InsertLocation(xmlDoc.InnerXml.ReplaceSpecialCharacters(), VM.EditFlag, BaseUserName.ToUpper(), "");

            return Status.ToString();
        }
        #endregion

        #region City Master

        // City Master List
        public ActionResult CityMaster()
        {
            CYGNUS_citymasterViewModel WCMVM = new CYGNUS_citymasterViewModel();
            try
            {
                WCMVM.listWCM = new List<CYGNUS_citymaster>();
                WCMVM.WCM = new CYGNUS_citymaster();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WCMVM);
        }
        public ActionResult GetCityByState(string Id)
        {
            List<CYGNUS_citymaster> listCity = new List<CYGNUS_citymaster>();
            try
            {
                listCity = _masterService.GetCityMaster();
                if (Id != null && Id.Length > 0)
                {
                    listCity = listCity.Where(c => c.state == Id).ToList();
                }
            }
            catch (Exception)
            {
            }
            return PartialView("_CityList", listCity);
        }
        public ActionResult GetCityDetails(int city_code)
        {
            CYGNUS_citymaster WCM = new CYGNUS_citymaster();
            try
            {
                if (city_code > 0)
                {
                    WCM = _masterService.GetCityMaster().FirstOrDefault(c => c.city_code == city_code);
                }
            }
            catch (Exception)
            {
            }

            return PartialView("_AddEditCity", WCM);
        }
        public ActionResult AddEditCity(CYGNUS_citymaster WCM)
        {
            bool Status = false;
            try
            {
                string MstDetails = "<root><MST>";
                MstDetails = MstDetails + "<city_code>" + WCM.city_code + "</city_code>";
                MstDetails = MstDetails + "<Location>" + WCM.Location.Trim() + "</Location>";
                MstDetails = MstDetails + "<state>" + WCM.state + "</state>";
                MstDetails = MstDetails + "<Region>" + WCM.Region + "</Region>";
                MstDetails = MstDetails + "<oda_yn>" + WCM.oda_yn + "</oda_yn>";
                MstDetails = MstDetails + "<ODAkm>" + WCM.ODAkm + "</ODAkm>";
                MstDetails = MstDetails + "<permit_yn>" + WCM.permit_yn + "</permit_yn>";
                MstDetails = MstDetails + "<activeflag>" + WCM.activeflag + "</activeflag>";
                MstDetails = MstDetails + "<Locations>" + WCM.Locations + "</Locations></MST></root>";

                DataTable Dt = _masterService.AddEditCityMaster(MstDetails);
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return new JsonResult()
                {
                    Data = new
                    {
                        Status,
                    }
                };
            }
            catch (Exception)
            {
                return Json(Status);
            }
        }
        public JsonResult CheckDuplicateCity_New(string City, string state)
        {
            try
            {
                int Count = _masterService.GetCityCount(City, state);

                return new JsonResult()
                {
                    Data = new
                    {
                        Count,
                    }
                };
            }
            catch (Exception)
            {
                return Json(new { Count = 0 });
            }
        }
        public JsonResult ActiveInActive_City(int id)
        {
            try
            {
                var data = _masterService.ActiveInActive_City(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public void ExportCityMasterReport()
        {
            DataTable dt = _masterService.GetCityMasterReportDetails();
            string FileName = "CityMasterReport";
            ExportUtilities.ExportToCSV(dt, FileName);
        }
        #endregion

        #region Customer Group Master

        public ActionResult CustomerGroup()
        {
            CYGNUS_GRPMSTViewModel WGVM = new CYGNUS_GRPMSTViewModel();
            try
            {
                WGVM.listWGM = new List<CYGNUS_GRPMST>();
                WGVM.WGM = new CYGNUS_GRPMST();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WGVM);
        }

        public ActionResult CustomerGroupMasters(string GroupCode)
        {
            CYGNUS_GRPMSTViewModel WGVM = new CYGNUS_GRPMSTViewModel
            {
                WGM = new CYGNUS_GRPMST()
            };
            try
            {
                if (GroupCode != "0")
                {
                    WGVM.WGM = _masterService.GetCustomerGroupMasterObject().FirstOrDefault(c => c.GRPCD == GroupCode);
                }
            }
            catch (Exception) { }
            return PartialView("_AddEditCustomerGroup", WGVM.WGM);
        }

        public ActionResult CustomerGroupMastersList()
        {
            CYGNUS_GRPMSTViewModel WGVM = new CYGNUS_GRPMSTViewModel();
            try
            {
                WGVM.listWGM = _masterService.GetCustomerGroupMasterObject();
            }
            catch (Exception) { }
            return PartialView("_CustomerGroupList", WGVM.listWGM);
        }

        public ActionResult AddEditCustomerGroup(CYGNUS_GRPMST WGM)
        {
            bool Status = false;
            try
            {
                if (WGM.ActiveFlag.ToString() == "true")
                    WGM.ActiveFlag = "Y";
                else
                    WGM.ActiveFlag = "N";
                string MstDetails = "<CustomerGroupMaster>";
                MstDetails = MstDetails + "<GRPCD>" + WGM.GRPCD + "</GRPCD>";
                MstDetails = MstDetails + "<GRPNM>" + WGM.GRPNM + "</GRPNM>";
                MstDetails = MstDetails + "<GRP_Pwd>" + WGM.GRP_Pwd + "</GRP_Pwd>";
                MstDetails = MstDetails + "<locregion>" + WGM.locregion + "</locregion>";
                MstDetails = MstDetails + "<ActiveFlag>" + WGM.ActiveFlag + "</ActiveFlag>";
                MstDetails = MstDetails + "<OLD_GRPCD>" + WGM.OLD_GRPCD + "</OLD_GRPCD>";
                MstDetails = MstDetails + "<UPDTBY>" + BaseUserName + "</UPDTBY>";
                MstDetails += "<isSysGenerated>Y</isSysGenerated>";
                MstDetails += "</CustomerGroupMaster>";
                DataTable Dt = _masterService.AddEditCustomerGroupMaster(MstDetails);
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCustomerList(string Type, string searchTerm)
        {
            List<CYGNUS_CUSTHDR> CUSTDHR = new List<CYGNUS_CUSTHDR>();
            //CUSTDHR = _masterService.GetCustomerMasterObject().Where(c => (c.CUSTNM.ToUpper().Contains(searchTerm.ToUpper()) || c.CUSTCD.ToUpper().Contains(searchTerm.ToUpper())) && c.CUST_ACTIVE.ToUpper() == "Y").ToList();
            //if (Type == "0")
            //    CUSTDHR = CUSTDHR.Take(30).ToList();
            //else
            //    CUSTDHR = CUSTDHR.Where(c => c.CUSTCAT.ToUpper() == Type.ToUpper()).Take(30).ToList();
            CUSTDHR = _masterService.GetCustomerList(Type, searchTerm).Take(30).ToList();

            var users = from user in CUSTDHR
                        select new
                        {
                            id = user.CUSTCD,
                            text = user.CUSTNM
                        };

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ActiveInActive_Customergroup(string id)
        {
            try
            {
                var data = _masterService.ActiveInActive_Customergroup(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Designation 
        public JsonResult GetDesignationFromCategory(string Category)
        {
            List<CYGNUS_Master_General> DesignationList = _masterService.GetDesignationFromCategory(Category).ToList();

            var SearchList = (from e in DesignationList
                              select new
                              {
                                  Value = e.CodeId,
                                  Text = e.CodeDesc,
                              }).ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetManagerFromDesignationandLocation(string Category, string Location)
        {
            List<CYGNUS_Master_Users> ManagerList = _masterService.GetManagerFromDesignationandLocation(Category, Location).ToList();

            var SearchList = (from e in ManagerList
                              select new
                              {
                                  Value = e.UserId,
                                  Text = e.Name,
                              }).ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region User Master
        public ActionResult UserList()
        {
            return View();
        }
        private static string UserId = ""; private static bool EditFlag = false;
        public ActionResult _AddEditUser(string id)
        {
            //CYGNUS_Master_UsersViewModel WMUVM = new CYGNUS_Master_UsersViewModel();
            //WMUVM.WMU = new CYGNUS_Master_Users();
            UserId = "";
            EditFlag = false;
            if (id != null && id != "")
            {
                EditFlag = true;
                UserId = id.Trim().ToString();
            }
            else
            {
                EditFlag = false;
                UserId = "0";
            }
            return RedirectToAction("AddEditUser");
            //return RedirectToAction("AddEditUser", WMUVM);
        }

        public ActionResult AddEditUser()
        {
            CYGNUS_Master_UsersViewModel WMUVM = new CYGNUS_Master_UsersViewModel();
            WMUVM.WMU = new CYGNUS_Master_Users();
            WMUVM.EditFlag = EditFlag;
            if (EditFlag == true)
            {
                WMUVM.WMU = _masterService.GetUserDetails().Where(c => c.UserId == UserId).FirstOrDefault();

                if (WMUVM.WMU == null)
                {
                    WMUVM.WMU = new CYGNUS_Master_Users();
                    WMUVM.EditFlag = false;
                }
                else
                {
                    WMUVM.MapCompanys = WMUVM.WMU.COMPANY_LIST;
                    WMUVM.DefaultCompany = WMUVM.WMU.DEFAULT_COMPANY;
                }
            }
            //WMUVM.ListEduDoc = new List<CYGNUS_User_EducationalDetails>();
            ViewBag.BaseEmpType = BaseEmpType;
            ViewBag.BaseUserReadWrite = BaseUserReadWrite;
            return View(WMUVM);
        }
        [HttpPost]
        [InitializeSimpleMembership]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditUser(CYGNUS_Master_UsersViewModel VM, HttpPostedFileBase[] files, HttpPostedFileBase[] Voterfiles, HttpPostedFileBase[] PANfiles, HttpPostedFileBase[] Aadharfiles)
        {
            string extension = "", TranXaction = "", UserFileName = "", UserName = "", type = "user";
            var DocumentUploadedPath = "";
            string VoterIDImage = "", PanImage = "", AadharImage = "";
            string VoterIDContentType = "", PanContentType = "", AadharContentType = "";
            DataTable Dt_Name = new DataTable();
            try
            {
                #region User_Image
                try
                {
                    if (((System.Web.HttpPostedFileBase[])(files)) != null)
                    {
                        foreach (var fileobj in files)
                        {
                            var file = ((System.Web.HttpPostedFileBase[])(files));
                            if (fileobj.ContentLength > 0)
                            {
                                extension = System.IO.Path.GetExtension(fileobj.FileName);
                                UserFileName = BaseUserName + extension;
                                DocumentUploadedPath = AzureStorageHelper.UploadBlobFileForUser(fileobj, UserFileName, BaseUserName.ToString(), type);
                            }
                            //var bmp1 = Image.FromFile(fileobj);
                            //bmp1.Save("E:\\Cygnus Projects\\RCPL\\CygnuXTMS\\CygnuXTMS\\Controllers", System.Drawing.Imaging.ImageFormat.Bmp);

                            //ImageToByte(file);
                            //ImageConverter converter = new ImageConverter();
                            //return (byte[])converter.ConvertTo(file, typeof(byte[]));

                        }
                    }
                }
                catch (Exception)
                {
                    DocumentUploadedPath = VM.WMU.User_Image;
                }
                #endregion

                #region VoterID_Image
                try
                {
                    if (((System.Web.HttpPostedFileBase[])(Voterfiles)) != null)
                    {
                        foreach (var fileobj in Voterfiles)
                        {
                            var file = ((System.Web.HttpPostedFileBase[])(Voterfiles));
                            if (fileobj.ContentLength > 0)
                            {
                                VoterIDContentType = fileobj.ContentType;
                                extension = System.IO.Path.GetExtension(fileobj.FileName);
                                UserFileName = fileobj.FileName;
                                VoterIDImage = AzureStorageHelper.UploadBlobFileForUser(fileobj, UserFileName, BaseUserName.ToString(), type);
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    VoterIDImage = VM.WMU.VoterID_Image;
                    VoterIDContentType = VM.WMU.VoterIDContentType;
                }
                #endregion

                #region PAN_Image
                try
                {
                    if (((System.Web.HttpPostedFileBase[])(PANfiles)) != null)
                    {
                        foreach (var fileobj in PANfiles)
                        {
                            var file = ((System.Web.HttpPostedFileBase[])(PANfiles));
                            if (fileobj.ContentLength > 0)
                            {
                                PanContentType = fileobj.ContentType;

                                extension = System.IO.Path.GetExtension(fileobj.FileName);
                                UserFileName = fileobj.FileName;
                                PanImage = AzureStorageHelper.UploadBlobFileForUser(fileobj, UserFileName, BaseUserName.ToString(), type);
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    PanImage = VM.WMU.PAN_Image;
                    PanContentType = VM.WMU.PanContentType;
                }
                #endregion

                #region Aadhar_Image
                try
                {
                    if (((System.Web.HttpPostedFileBase[])(Aadharfiles)) != null)
                    {
                        foreach (var fileobj in Aadharfiles)
                        {
                            var file = ((System.Web.HttpPostedFileBase[])(Aadharfiles));
                            if (fileobj.ContentLength > 0)
                            {
                                AadharContentType = fileobj.ContentType;
                                extension = System.IO.Path.GetExtension(fileobj.FileName);
                                UserFileName = fileobj.FileName;
                                AadharImage = AzureStorageHelper.UploadBlobFileForUser(fileobj, UserFileName, BaseUserName.ToString(), type);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    AadharImage = VM.WMU.Aadhar_Image;
                    AadharContentType = VM.WMU.AadharContentType;
                }
                #endregion

                //VM.WMU.EntryBy = BaseUserName.ToUpper();
                if (VM.WMU.UserPwd == null || VM.WMU.UserPwd == "")
                {
                    VM.WMU.UserPwd = VM.WMU.UserId + "@12345";
                }
                string MstDetails = "<UserMaster>";
                MstDetails = MstDetails + "<UserId>" + VM.WMU.UserId.ToUpper() + "</UserId>";
                MstDetails = MstDetails + "<EmpId>" + (VM.WMU.EmpId ?? "").ToUpper() + "</EmpId>";
                MstDetails = MstDetails + "<UserPwd>" + (VM.EditFlag == true ? VM.WMU.UserPwd : GF.Encrypt(VM.WMU.UserPwd, psSult)) + "</UserPwd>";
                MstDetails = MstDetails + "<Manual_EmployeeNo>" + VM.WMU.Manual_EmployeeNo + "</Manual_EmployeeNo>";
                MstDetails = MstDetails + "<Name>" + VM.WMU.Name + "</Name>";
                MstDetails = MstDetails + "<PasswordQues>" + VM.WMU.PasswordQues + "</PasswordQues>";
                MstDetails = MstDetails + "<PasswordAns>" + VM.WMU.PasswordAns + "</PasswordAns>";
                MstDetails = MstDetails + "<gender>" + VM.WMU.gender + "</gender>";
                MstDetails = MstDetails + "<Category>" + VM.WMU.Category + "</Category>";
                MstDetails = MstDetails + "<DOB>" + VM.WMU.DOB + "</DOB>";
                MstDetails = MstDetails + "<User_Type>" + VM.WMU.User_Type + "</User_Type>";
                MstDetails = MstDetails + "<ManagerId>" + VM.WMU.ManagerId + "</ManagerId>";
                MstDetails = MstDetails + "<BranchCode>" + VM.WMU.BranchCode + "</BranchCode>";
                MstDetails = MstDetails + "<MultiLocation>" + VM.WMU.MultiLocation + "</MultiLocation>";
                MstDetails = MstDetails + "<DOJ_ORG>" + VM.WMU.DOJ_ORG + "</DOJ_ORG>";
                MstDetails = MstDetails + "<emptype>" + VM.WMU.emptype + "</emptype>";
                MstDetails = MstDetails + "<driver_id>" + VM.WMU.driver_id + "</driver_id>";
                MstDetails = MstDetails + "<EmailId>" + VM.WMU.EmailId + "</EmailId>";
                MstDetails = MstDetails + "<mobileno>" + VM.WMU.mobileno + "</mobileno>";
                MstDetails = MstDetails + "<resi_addr>" + VM.WMU.resi_addr + "</resi_addr>";
                MstDetails = MstDetails + "<Name_Of_bank>" + VM.WMU.Name_Of_bank + "</Name_Of_bank>";
                MstDetails = MstDetails + "<Bank_AC_Number>" + VM.WMU.Bank_AC_Number + "</Bank_AC_Number>";
                MstDetails = MstDetails + "<IFSC_Code>" + VM.WMU.IFSC_Code + "</IFSC_Code>";
                MstDetails = MstDetails + "<Designation>" + VM.WMU.Designation + "</Designation>";
                MstDetails = MstDetails + "<Status>" + (VM.WMU.Status == "Y" ? "100" : "200") + "</Status>";
                MstDetails = MstDetails + "<ShowTBBCharges>" + (VM.WMU.ShowTBBCharges == "Y" ? "Y" : "N") + "</ShowTBBCharges>";
                MstDetails = MstDetails + "<User_Image>" + DocumentUploadedPath + "</User_Image>";
                MstDetails = MstDetails + "<Department>" + VM.WMU.Department + "</Department>";
                MstDetails = MstDetails + "<HOD>" + VM.WMU.HOD + "</HOD>";
                MstDetails = MstDetails + "<Grade>" + VM.WMU.Grade + "</Grade>";
                MstDetails = MstDetails + "<EntryBy>" + BaseUserName.ToUpper() + "</EntryBy>";
                MstDetails = MstDetails + "<ConveyanceExpance>" + VM.WMU.ConveyanceExpance + "</ConveyanceExpance>";
                MstDetails = MstDetails + "<Read_Witre>" + VM.WMU.Read_Witre + "</Read_Witre>";

                MstDetails = MstDetails + "<Ratings>" + VM.WMU.Ratings + "</Ratings>";
                MstDetails = MstDetails + "<PersonalEmail>" + VM.WMU.PersonalEmail + "</PersonalEmail>";
                MstDetails = MstDetails + "<BloodGroup>" + VM.WMU.BloodGroup + "</BloodGroup>";
                MstDetails = MstDetails + "<WeddingDate>" + VM.WMU.WeddingDate + "</WeddingDate>";
                MstDetails = MstDetails + "<VoterID>" + VM.WMU.VoterID + "</VoterID>";
                MstDetails = MstDetails + "<VoterIDContentType>" + VoterIDContentType + "</VoterIDContentType>";
                MstDetails = MstDetails + "<VoterID_Image>" + VoterIDImage + "</VoterID_Image>";
                MstDetails = MstDetails + "<PANNumber>" + VM.WMU.PANNumber + "</PANNumber>";
                MstDetails = MstDetails + "<PanContentType>" + PanContentType + "</PanContentType>";
                MstDetails = MstDetails + "<PAN_Image>" + PanImage + "</PAN_Image>";
                MstDetails = MstDetails + "<AadharNumber>" + VM.WMU.AadharNumber + "</AadharNumber>";
                MstDetails = MstDetails + "<AadharContentType>" + AadharContentType + "</AadharContentType>";
                MstDetails = MstDetails + "<Aadhar_Image>" + AadharImage + "</Aadhar_Image>";
                MstDetails = MstDetails + "<OffcialmobileNo>" + VM.WMU.OffcialMobileNo + "</OffcialmobileNo>";
                MstDetails = MstDetails + "<ShiftCode>" + VM.WMU.ShiftCode + "</ShiftCode>";
                MstDetails = MstDetails + "<MapCompanys>" + VM.MapCompanys + "</MapCompanys>";
                MstDetails = MstDetails + "<DefaultCompany>" + VM.DefaultCompany + "</DefaultCompany>";
                MstDetails = MstDetails + "<MultiLocation>" + VM.WMU.MultiLocation + "</MultiLocation>";
                MstDetails = MstDetails + "</UserMaster>";

                Dt_Name = _masterService.InsertUser(MstDetails, VM.EditFlag == true ? "U" : "I");
                TranXaction = Dt_Name.Rows[0][0].ToString();
                UserName = (VM.WMU.UserId + ':' + VM.WMU.UserId);

                if (TranXaction == "Done")
                {
                    //if (VM.EditFlag == false)
                    //{
                    //    WebSecurity.CreateUserAndAccount(VM.WMU.UserId, VM.WMU.UserPwd);
                    //}
                    if (VM.WMU.Status == "N")
                    {
                        string oldPassword = GF.Decrypt(VM.WMU.UserPwd, psSult);
                        string newPassword = GF.GenerateString(6);
                        bool changePasswordSucceeded = WebSecurity.ChangePassword(VM.WMU.UserId, oldPassword, newPassword);
                        string commandText = "Update CYGNUS_Master_Users set LastPwd = UserPwd, UserPwd='" + GF.Encrypt(newPassword, psSult) + "',IsPasswordExpired = 0 , PwdLastChangeOn = GETDATE() Where UserId ='" + VM.WMU.UserId + "'";
                        DataTable dataTable = GF.GetDateTableFromQuery(commandText);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("UserDone", new { UserName = UserName, TranXaction = TranXaction });
        }

        public ActionResult UserDone(string UserName, string TranXaction)
        {
            ViewBag.UserName = UserName;
            ViewBag.TranXaction = TranXaction;

            return View();
        }

        public JsonResult GetUserListJson()
        {
            List<CYGNUS_Master_Users> ListUsers = _masterService.GetUserDetailsForUserMasterList(BaseUserName);
            var data = (from e in ListUsers
                        select new
                        {
                            e.UserId,
                            //e.EmpId,
                            e.BranchCode,
                            e.usertypedesc,
                            e.EmTypDes,
                            e.Name,
                            e.Status,
                            e.User_Type
                        }).ToArray();
            return Json(data.OrderBy(c => c.UserId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PasswordReset(string id, string newPassword, int type)
        {
            bool status = false;
            try
            {
                if (type == 1)
                {
                    string getEncryptedPassword = "SELECT UserPwd FROM CYGNUS_Master_Users Where UserId ='" + id + "'";
                    string oldPassword = GF.Decrypt(GF.GetDateTableFromQuery(getEncryptedPassword).Rows[0][0].ToString(), psSult);

                    bool changePasswordSucceeded = WebSecurity.ChangePassword(id, oldPassword, newPassword);
                    if (changePasswordSucceeded)
                    {
                        string commandText = "Update CYGNUS_Master_Users set LastPwd = UserPwd, UserPwd='" + GF.Encrypt(newPassword, psSult) + "',IsPasswordExpired = 0 , PwdLastChangeOn = GETDATE() Where UserId ='" + id + "'";
                        DataTable dataTable = GF.GetDateTableFromQuery(commandText);
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                }
                else if (type == 2)
                {
                    string commandText = "Update CYGNUS_Master_Users set LastLoginDate = GETDATE() Where UserId ='" + id + "'";
                    DataTable dataTable = GF.GetDateTableFromQuery(commandText);
                    status = true;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public bool CheckDuplicateUser(string Id, string Flag, string User)
        {
            List<CYGNUS_Master_Users> listUsers = _masterService.GetUserDetails();
            if (Flag == "U")
            {
                listUsers = listUsers.Where(c => c.UserId.ToUpper() == Id.ToUpper()).ToList();
            }
            else if (Flag == "M")
            {
                listUsers = listUsers.Where(c => c.mobileno == Id && c.UserId.ToUpper() != User.ToUpper()).ToList();

            }
            else
            {
                listUsers = listUsers.Where(c => c.EmpId == null ? c.EmpId == "" : c.EmpId.ToUpper() == Id.ToUpper()).ToList();

            }

            return listUsers.Count > 0 ? false : true;
        }

        public JsonResult GetDestinationLocationsWithHQTR(string searchTerm)
        {
            searchTerm = searchTerm.ToUpper();

            List<CYGNUS_location> CMP = _masterService.GetDestinationLocationsWithHQTR(searchTerm).Take(5).ToList();

            var users = from user in CMP
                        select new
                        {
                            id = user.LocCode,
                            text = user.LocName
                        };

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerLocationListJson(string str)
        {
            List<CYGNUS_location> ListLocations = new List<CYGNUS_location>();
            ListLocations = _masterService.GetLocationDetails().Where(c => c.ActiveFlag == "Y").ToList().OrderBy(c => c.LocName).ToList();

            var SearchList = (from e in ListLocations
                              select new
                              {
                                  id = e.LocCode,
                                  text = e.LocCode + "~" + e.LocName,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ActiveInActive_User(string id)
        {
            try
            {
                var data = _masterService.ActiveInActive_User(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCompanyListJson(string str)
        {
            List<CYGNUS_COMPANY_MASTER> ListCompany = new List<CYGNUS_COMPANY_MASTER>();
            ListCompany = _masterService.GetCompanyDetails().ToList();
            var SearchList = (from e in ListCompany
                              select new
                              {
                                  id = e.COMPANY_CODE,
                                  text = e.COMPANY_CODE + "~" + e.COMPANY_NAME,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Change Reports Rights
        public ViewResult ChangeRights(string Id)
        {
            ViewBag.UserId = Id;
            return View(_masterService.GetMenusList(false).Where(c => c.IsActive == true).ToList());
        }

        [HttpPost]
        public ActionResult ChangeRights(string UserId, List<CYGNUS_Master_Menu_Access> model)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(model.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, model);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);

                    DataTable Dt = _masterService.InsertMenuRights(xmlDoc.InnerXml, UserId, "1", BaseUserName, BaseFinYear);
                    string Status = Dt.Rows[0][0].ToString();
                    if (Status == "Done")
                    {
                        ViewBag.Status = Status;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("UserList", "Master");
        }
        #endregion

        #region Change Reports Rights

        public ViewResult ChangeReportRights(string Id)
        {
            ViewBag.UserId = Id;
            List<CYGNUS_Master_Reports> List = new List<CYGNUS_Master_Reports>();
            //List = _masterService.GetReportList("", "", BaseUserName.ToUpper(), 2).ToList();
            List = _masterService.GetReportList("", "", Id.ToUpper(), 2).ToList();
            return View(List);
        }

        [HttpPost]
        public ActionResult ChangeReportRights_Submit(string Id, List<CYGNUS_Master_Reports> Report)
        {
            CYGNUS_Master_Reports ObjCMR = new CYGNUS_Master_Reports();
            try
            {
                string XML = "<root>";
                if (Report != null)
                {
                    ObjCMR = Report.First();
                    foreach (var item in Report)
                    {
                        XML = XML + "<Reports>";
                        XML = XML + "<UserId>" + item.UserName + "</UserId>";
                        XML = XML + "<ReportId>" + item.ID + "</ReportId>";
                        XML = XML + "<HasAccess>" + item.HasAccess + "</HasAccess>";
                        XML = XML + "</Reports>";
                    }
                }
                else
                {
                    @ViewBag.StrError = "Please Enter Valid UserName";
                    return View("Error");
                }
                XML = XML + "</root>";

                DataTable DT = new DataTable();
                DT = _masterService.Add_Report_Rights(XML, BaseLocationCode, ObjCMR.UserName, BaseCompanyCode, BaseUserName);

                return RedirectToAction("ChangeReportRights_Done", new { Id = ObjCMR.UserName });
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.ToString().Replace('\n', '_');
                return View("Error");
            }
        }

        public ActionResult ChangeReportRights_Done(string Id)
        {
            ViewBag.UserId = Id;
            return View();
        }

        #endregion

        #region Customer Master

        public ActionResult Customer()
        {
            CYGNUS_CUSTHDRViewModel WCVM = new CYGNUS_CUSTHDRViewModel();
            try
            {
                WCVM.listWCH = new List<CYGNUS_CUSTHDR>();
                WCVM.WCH = new CYGNUS_CUSTHDR();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WCVM);
        }

        public ActionResult CustomerMasters(string CustomerCode)
        {
            CYGNUS_CUSTHDRViewModel WCVM = new CYGNUS_CUSTHDRViewModel();
            WCVM.WCH = new CYGNUS_CUSTHDR();
            WCVM.WCH.CCBD = new CYGNUS_Customer_Bill_Details();
            WCVM.WCH.CCPA = new CYGNUS_Customer_Pickup_Address();
            WCVM.WCH.listCCPA = new List<CYGNUS_Customer_Pickup_Address>();
            WCVM.WCH.CCKD = new CYGNUS_Customer_KYC_Details();
            ViewBag.CustomerCode = CustomerCode;
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
            try
            {
                if (CustomerCode != "0")
                {
                    WCVM.WCH = _masterService.GetCustomerMasterObject(CustomerCode).FirstOrDefault() ?? new CYGNUS_CUSTHDR();

                    WCVM.WCH.listCCKD = DataRowToObject.CreateListFromTable<CYGNUS_Customer_KYC_Details>(_masterService.Get_Customerwise_KYCDetails(CustomerCode));
                    WCVM.WCH.listCCGD = DataRowToObject.CreateListFromTable<CYGNUS_Customer_Geofence_Details>(_masterService.Get_Customerwise_GeofenceDetails(CustomerCode));
                    WCVM.WCH.listCCBD = DataRowToObject.CreateListFromTable<CYGNUS_Customer_Bill_Details>(_masterService.Get_Customerwise_BillDetails(CustomerCode));
                    WCVM.WCH.listCCPA = DataRowToObject.CreateListFromTable<CYGNUS_Customer_Pickup_Address>(_masterService.Get_Customerwise_PickUpDetails(CustomerCode));
                    WCVM.WCH.CCPA = new CYGNUS_Customer_Pickup_Address();

                    // Ensure these are never null for the View binding
                    if (WCVM.WCH.CCBD == null) WCVM.WCH.CCBD = new CYGNUS_Customer_Bill_Details();
                    if (WCVM.WCH.CCKD == null) WCVM.WCH.CCKD = new CYGNUS_Customer_KYC_Details();

                    if (!string.IsNullOrEmpty(WCVM.WCH.CUSTCAT))
                    {
                        List<string> CUSTCAT = WCVM.WCH.CUSTCAT.Split(',').ToList<string>();
                        foreach (var item in CUSTCAT)
                        {
                            if (item == "P01") WCVM.WCH.Paid = true;
                            if (item == "P02") WCVM.WCH.TBB = true;
                            if (item == "P03") WCVM.WCH.ToPay = true;
                            if (item == "P04") WCVM.WCH.FOC = true;
                        }
                    }
                }
                WCVM.WCH.listCCKMA = DataRowToObject.CreateListFromTable<Cygnus_Cutomer_KMA_Details>(_masterService.Get_Customerwise_KMA(CustomerCode));

            }
            catch (Exception)
            {
                return RedirectToAction("Customer");
            }
            //return PartialView("_AddEditCustomer", WCVM.WCH);
            return View("_AddEditCustomer", WCVM.WCH);
        }

        public JsonResult GetBusinessTypeCategory(string str)
        {
            List<CYGNUS_Master_General> ListGnMST = _masterService.GetGeneralMaster().Where(c => c.CodeType.ToUpper() == "BUSINESSCAT" && c.StatusCode == "Y").OrderBy(c => c.CodeDesc).ToList(); ;

            var SearchList = (from e in ListGnMST
                              select new
                              {
                                  id = e.CodeId,
                                  text = e.CodeDesc,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetConsignnorCustomerListJson(string str)
        {
            List<CYGNUS_CUSTHDR> ListConsignnors = new List<CYGNUS_CUSTHDR>();
            ListConsignnors = _masterService.GetConsignnorCustomerListJson().ToList();

            var SearchList = (from e in ListConsignnors
                              select new
                              {
                                  id = e.CUSTCD,
                                  text = e.CUSTNM,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetConsigneeCustomerListJson(string str)
        {
            List<CYGNUS_CUSTHDR> ListConsignees = new List<CYGNUS_CUSTHDR>();
            ListConsignees = _masterService.GetConsigneeCustomerListJson().ToList();

            var SearchList = (from e in ListConsignees
                              select new
                              {
                                  id = e.CUSTCD,
                                  text = e.CUSTNM,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCityWisePincodeForCustomerMater(string id)
        {
            List<CYGNUS_pincode_master> listPincode = new List<CYGNUS_pincode_master>();

            listPincode = _masterService.GetPincodeMaster().Where(c => c.Location == id).ToList();


            var SearchList = (from e in listPincode
                              select new
                              {
                                  Value = e.pincode,
                                  Text = e.pincode + ":" + e.Area,
                              }).Distinct().ToList();
            // return Json(SearchList, JsonRequestBehavior.AllowGet);


            return Json(SearchList.OrderBy(c => c.Text), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocationsJson()
        {
            List<CYGNUS_location> ListLocations = _masterService.GetLocationDetails();
            var SearchList = (from e in ListLocations
                              select new
                              {
                                  Value = e.LocCode,
                                  Text = e.LocName,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeDropDownList(string searchTerm)
        {
            searchTerm = searchTerm.ToUpper();
            List<CYGNUS_Master_Users> CMP = _masterService.Search_Organization_Employee(searchTerm, BaseUserName).Take(5).ToList();
            var users = from user in CMP
                        select new
                        {
                            id = user.EmpId,
                            text = user.Name,
                        };

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public class GSTINInfo
        {
            public string Gstin { get; set; }
            public string AuthStatus { get; set; }
            public string StateCd { get; set; }
        }
        public class ApiResponse
        {
            public bool Error { get; set; }
            public GSTINInfo[] Data { get; set; }
        }
        public async Task<JsonResult> GetGSTINInfoAsync(string pan)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {

                string url = "https://commonapi.mastersindia.co/commonapis/gstinbypan?pan=" + pan;

                string SQRY = "exec USP_GetMasterIndiaEInvoiceAPIToken '" + BaseCompanyCode + "','2'";
                DataTable Dt = GF.GetDataTableFromSP(SQRY);
                string authorizationToken = "Bearer " + Dt.Rows[0]["CodeDesc"].ToString();
                string clientId = "ziZJseFZDRBuBqpxJK";


                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", authorizationToken);
                    client.DefaultRequestHeaders.Add("client_id", clientId);

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                        if (!apiResponse.Error)
                        {
                            foreach (var gstinInfo in apiResponse.Data)
                            {
                                Console.WriteLine($"GSTIN: {gstinInfo.Gstin}, Auth Status: {gstinInfo.AuthStatus}, State Code: {gstinInfo.StateCd}");
                            }
                        }
                        else
                        {
                            apiResponse.Error = true;
                        }
                    }
                    else
                    {
                        apiResponse.Error = true;
                    }
                }

            }
            catch (Exception ex)
            {
                apiResponse.Error = true;
            }
            return Json(apiResponse, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadCustomerKYCDoc()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + fileName;

                        string folderPath = Server.MapPath("~/UploadedDocuments/CustomerKYCDoc");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string fullPath = Path.Combine(folderPath, uniqueFileName);
                        file.SaveAs(fullPath);

                        var result = new
                        {
                            success = true,
                            filePath = "/UploadedDocuments/CustomerKYCDoc/" + uniqueFileName,
                            fileName = uniqueFileName
                        };
                        return Json(result);
                    }
                }
                return Json(new { success = false, message = "No file received. FileCount: " + Request.Files.Count + ", FormCount: " + Request.Form.Count + ", Len: " + Request.ContentLength + ", Type: " + Request.ContentType });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AddEditCustomer(CYGNUS_CUSTHDR WCH, List<Cygnus_Cutomer_KMA_Details> ListCustomerKMAdetails, string BillJsonData, string KYCJsonData, string GeofenceJsonData, string PickupAddressJsonData, string IsRegistered)
        {
            string CustCode = "false,-", Message = "", CustomerCode = "", Status = "0", CustomerName = "";
            try
            {
                List<string> CUSTCAT = new List<string>();

                if (WCH.Paid == true)
                    CUSTCAT.Add("P01");
                if (WCH.TBB == true)
                    CUSTCAT.Add("P02");
                if (WCH.ToPay == true)
                    CUSTCAT.Add("P03");
                if (WCH.FOC == true)
                    CUSTCAT.Add("P04");

                WCH.CUSTCAT = string.Join(",", CUSTCAT);



                if (WCH.CUST_ACTIVE.ToString().ToUpper() == "TRUE")
                    WCH.CUST_ACTIVE = "Y";
                else
                    WCH.CUST_ACTIVE = "N";
                if (WCH.MOBSERV_ENABLED.ToString().ToUpper() == "TRUE")
                    WCH.MOBSERV_ENABLED = "Y";
                else
                    WCH.MOBSERV_ENABLED = "N";

                if (WCH.Consignee.ToString().ToUpper() == "TRUE")
                    WCH.Consignee = "Y";
                else
                    WCH.Consignee = "N";

                if (WCH.Consignnor.ToString().ToUpper() == "TRUE")
                    WCH.Consignnor = "Y";
                else
                    WCH.Consignnor = "N";

                //if (WCH.ThirdParty.ToString().ToUpper() == "TRUE")
                //    WCH.ThirdParty = "Y";
                //else
                //    WCH.ThirdParty = "N";
                WCH.ThirdParty = (WCH.ThirdParty ?? "").ToUpper() == "TRUE" ? "Y" : "N";

                WCH.CUSTNM = WCH.CUSTNM.ToUpper();

                XmlDocument MstDetails = new XmlDocument();
                XmlSerializer xmlSerializerHeader = new XmlSerializer(WCH.GetType());
                using (MemoryStream xmlStreamHeader = new MemoryStream())
                {
                    xmlSerializerHeader.Serialize(xmlStreamHeader, WCH);
                    xmlStreamHeader.Position = 0;
                    MstDetails.Load(xmlStreamHeader);
                }

                XmlDocument GSTDetails = new XmlDocument();
                //if (ListCustomerGSTDetails != null && ListCustomerGSTDetails.Count > 0)
                //{
                //    WCH.listCCGSTD = ListCustomerGSTDetails.Where(c => c.gst_registration_no != null).ToList();
                //}
                //else
                //{
                //    WCH.listCCGSTD = new List<CygnusCustomerGSTDetails>();
                //}

                //WCH.listCCGSTD = ListCustomerGSTDetails.Where(c => c.gst_registration_no != null).ToList();

                XmlDocument KMADetails = new XmlDocument();
                if (ListCustomerKMAdetails != null && ListCustomerKMAdetails.Count > 0)
                {
                    WCH.listCCKMA = ListCustomerKMAdetails.Where(c => c.EmployeeName != null).ToList();
                }
                else
                {
                    WCH.listCCKMA = new List<Cygnus_Cutomer_KMA_Details>();
                }
                XmlSerializer xmlSerializerGSTDetail = new XmlSerializer(WCH.listCCKMA.GetType());
                using (MemoryStream xmlStreamGSTDetail = new MemoryStream())
                {
                    xmlSerializerGSTDetail.Serialize(xmlStreamGSTDetail, WCH.listCCKMA);
                    xmlStreamGSTDetail.Position = 0;
                    KMADetails.Load(xmlStreamGSTDetail);
                }
                // Handle Bill Details
                if (!string.IsNullOrEmpty(BillJsonData))
                {
                    WCH.listCCBD = JsonConvert.DeserializeObject<List<CYGNUS_Customer_Bill_Details>>(BillJsonData);

                }
                else
                {
                    WCH.listCCBD = new List<CYGNUS_Customer_Bill_Details>();
                }

                XmlDocument BillDetails = new XmlDocument();
                XmlSerializer xmlSerializerBill = new XmlSerializer(WCH.listCCBD.GetType());
                using (MemoryStream xmlStreamBill = new MemoryStream())
                {
                    xmlSerializerBill.Serialize(xmlStreamBill, WCH.listCCBD);
                    xmlStreamBill.Position = 0;
                    BillDetails.Load(xmlStreamBill);
                }

                // Handle KYC Details
                if (!string.IsNullOrEmpty(KYCJsonData))
                {
                    WCH.listCCKD = JsonConvert.DeserializeObject<List<CYGNUS_Customer_KYC_Details>>(KYCJsonData);
                }
                else
                {
                    WCH.listCCKD = new List<CYGNUS_Customer_KYC_Details>();
                }
                XmlDocument KYCDetails = new XmlDocument();
                XmlSerializer xmlSerializerKYC = new XmlSerializer(WCH.listCCKD.GetType());
                using (MemoryStream xmlStreamKYC = new MemoryStream())
                {
                    xmlSerializerKYC.Serialize(xmlStreamKYC, WCH.listCCKD);
                    xmlStreamKYC.Position = 0;
                    KYCDetails.Load(xmlStreamKYC);
                }


                // Handle Geofence Details
                if (!string.IsNullOrEmpty(GeofenceJsonData))
                {
                    WCH.listCCGD = JsonConvert.DeserializeObject<List<CYGNUS_Customer_Geofence_Details>>(GeofenceJsonData);
                }
                else
                {
                    WCH.listCCGD = new List<CYGNUS_Customer_Geofence_Details>();
                }

                XmlDocument GeofenceDetails = new XmlDocument();
                XmlSerializer xmlSerializerGeofence = new XmlSerializer(WCH.listCCGD.GetType());
                using (MemoryStream xmlStreamGeofence = new MemoryStream())
                {
                    xmlSerializerGeofence.Serialize(xmlStreamGeofence, WCH.listCCGD);
                    xmlStreamGeofence.Position = 0;
                    GeofenceDetails.Load(xmlStreamGeofence);
                }

                // Handle PickUp Address Details
                if (!string.IsNullOrEmpty(PickupAddressJsonData))
                {
                    WCH.listCCPA = JsonConvert.DeserializeObject<List<CYGNUS_Customer_Pickup_Address>>(PickupAddressJsonData);
                    if (WCH.listCCPA != null && WCH.listCCGD != null)
                    {
                        foreach (var pickupAddress in WCH.listCCPA)
                        {
                            var addressPoints = WCH.listCCGD
                                .Where(x => x.AddID == pickupAddress.AddID)
                                .OrderBy(x => x.srno)
                                .Select(x => x.Latitude + "," + x.Longitude)
                                .ToList();

                            if (addressPoints.Any())
                            {
                                pickupAddress.Geom = string.Join("|", addressPoints);
                            }
                        }
                    }
                }
                else
                {
                    WCH.listCCPA = new List<CYGNUS_Customer_Pickup_Address>();
                }
                XmlDocument PickUpAddressDetails = new XmlDocument();
                XmlSerializer xmlSerializerPickUpAddress = new XmlSerializer(WCH.listCCPA.GetType());
                using (MemoryStream xmlStreamPickUpAddress = new MemoryStream())
                {
                    xmlSerializerPickUpAddress.Serialize(xmlStreamPickUpAddress, WCH.listCCPA);
                    xmlStreamPickUpAddress.Position = 0;
                    PickUpAddressDetails.Load(xmlStreamPickUpAddress);
                }
                DataTable Dt = _masterService.AddEditCustomerMaster(MstDetails.InnerXml, BaseUserName.ToUpper(), KMADetails.InnerXml, BillDetails.InnerXml, KYCDetails.InnerXml, GeofenceDetails.InnerXml, PickUpAddressDetails.InnerXml);
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    CustCode = "true," + Dt.Rows[0][0].ToString();
                    Message = Dt.Rows[0]["TranXaction"].ToString();
                    CustomerCode = Dt.Rows[0][0].ToString();
                    Status = "1";
                    CustomerName = Dt.Rows[0]["CustomerName"].ToString().ToUpper();
                }
                else
                {
                    CustCode = "false,-";
                    Message = Dt.Rows[0]["TranXaction"].ToString();
                    CustomerCode = "0";
                    Status = "0";
                    CustomerName = "";
                }
                return RedirectToAction("CustomerDone", new { CustomerCode = CustomerCode, CustomerName = CustomerName, Status = Status });
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }
        public JsonResult GetEmpDetails(string UserId)
        {
            string IsRecordFound = "", EmailId = "", Designation = "", MobileNo = "";
            DataTable DT = new DataTable();

            try
            {
                DT = _masterService.GetEmpDesignation(UserId);

                if (DT.Rows.Count > 0)
                {
                    IsRecordFound = "1";
                    UserId = DT.Rows[0]["UserId"].ToString();
                    EmailId = DT.Rows[0]["EmailId"].ToString();
                    Designation = DT.Rows[0]["Designation"].ToString();
                    MobileNo = DT.Rows[0]["MobileNo"].ToString();
                }
                else
                {
                    IsRecordFound = "0";
                }
                return new JsonResult()
                {
                    Data = new
                    {
                        IsRecordFound = IsRecordFound,
                        UserId = UserId,
                        EmailId = EmailId,
                        Designation = Designation,
                        MobileNo = MobileNo
                    }
                };
            }
            catch (Exception)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        IsRecordFound = IsRecordFound,
                        UserId = UserId,
                        EmailId = EmailId,
                        Designation = Designation,
                        MobileNo = MobileNo
                    }
                };
            }
        }
        public ActionResult CustomerDone(string CustomerCode, string CustomerName, string Status)
        {
            if (Status == "1")
            {
                ViewBag.CustomerCode = CustomerCode.ToUpper() + " : " + CustomerName.ToUpper();
            }
            ViewBag.Status = Status;
            return View();
        }

        public ActionResult PasswordResetCustomer(string id)
        {
            bool status = false;
            try
            {

                string commandText1 = "Update CYGNUS_CUSTHDR  set CUSTPASS='" + "123456" + "' Where CUSTCD ='" + id + "'";
                DataTable dataTable1 = GF.GetDateTableFromQuery(commandText1);
                status = true;
            }
            catch (Exception)
            {
                throw;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGSTWiseStateDetails(string GSTNo, string CustCode)
        {
            List<CygnusCustomerGSTDetails> ObjCGD = new List<CygnusCustomerGSTDetails>();
            ObjCGD = DataRowToObject.CreateListFromTable<CygnusCustomerGSTDetails>(_masterService.GetGSTWiseStateDetails(GSTNo, CustCode));
            return Json(ObjCGD, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CustomerGSTDetails(int id)
        {
            CygnusCustomerGSTDetails ObjCGD = new CygnusCustomerGSTDetails();
            ObjCGD.id = id;
            return PartialView("_CustomerGSTDetails", ObjCGD);
        }

        public JsonResult ActiveInActive_Customer(string id)
        {
            try
            {
                var data = _masterService.ActiveInActive_Customer(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CustomerToUserCreate(string id)
        {
            bool status = false;
            try
            {
                string Password = GF.Encrypt(id, psSult);
                DataTable DT = _masterService.CustomerToUserCreate(id, BaseUserName, Password);
                if (DT.Rows.Count > 0)
                {
                    status = true;
                    if (status == true)
                    {
                        WebSecurity.CreateUserAndAccount(id, id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerListingNew(string searchTerm, string GRPCD, string State)
        {
            var CMP = new List<CYGNUS_CUSTHDR>();
            CMP = _masterService.GetCustomerListingNew(searchTerm, GRPCD, State);

            var users = from user in CMP
                        select new
                        {
                            id = user.CUSTCD,
                            text = user.CUSTNM
                        };
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pincode Master

        public ActionResult Pincode()
        {
            CYGNUS_pincodemasterViewModel WPVM = new CYGNUS_pincodemasterViewModel();
            try
            {
                WPVM.listWPM = new List<CYGNUS_pincode_master>();
                WPVM.WPM = new CYGNUS_pincode_master();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(WPVM);
        }


        public ActionResult GetPincodeByCity(string Id)
        {
            List<CYGNUS_pincode_master> listPincode = new List<CYGNUS_pincode_master>();
            try
            {
                listPincode = _masterService.GetPincodeMaster();
                if (Id != null && Id.Length > 0)
                {
                    listPincode = listPincode.Where(c => c.cityname == Id).ToList();
                }
            }
            catch (Exception) { }
            return PartialView("_PincodeList", listPincode);
        }

        public ActionResult PincodeMasters(int id)
        {
            CYGNUS_pincodemasterViewModel WPVM = new CYGNUS_pincodemasterViewModel();
            WPVM.WPM = new CYGNUS_pincode_master();
            try
            {
                if (id > 0)
                {
                    WPVM.WPM = _masterService.GetPincodeMaster().FirstOrDefault(c => c.ID == id);
                }

                List<CYGNUS_citymaster> cityList = new List<CYGNUS_citymaster>();
                if (WPVM.WPM != null && !string.IsNullOrEmpty(WPVM.WPM.StateCode))
                {
                    cityList = _masterService.GetCityMaster().Where(c => c.state == WPVM.WPM.StateCode).ToList();
                }
                else
                {
                    cityList = _masterService.GetCityMaster().ToList();
                }

                ViewBag.CityList = cityList;
                ViewBag.LocationList = _masterService.GetLocationDetails().Where(c => c.ActiveFlag == "Y").ToList();
                ViewBag.StateList = _masterService.GetStateMaster().Where(c => c.activeflag == null ? c.activeflag == null : c.activeflag == "Y").OrderBy(c => c.stnm).ToList();
            }
            catch (Exception)
            {

            }
            return PartialView("_AddEditPincode", WPVM.WPM);
        }

        public ActionResult PincodeMastersList()
        {
            CYGNUS_pincodemasterViewModel WPVM = new CYGNUS_pincodemasterViewModel();
            try
            {
                WPVM.listWPM = _masterService.GetPincodeMaster();
            }
            catch (Exception) { }
            return PartialView("_PincodeList", WPVM.listWPM);
        }

        public ActionResult AddEditPincode(CYGNUS_pincode_master WPM)
        {
            bool Status = false;
            try
            {
                if (WPM != null && _masterService.ExistsPincode(WPM.pincode, WPM.ID))
                {
                    return Json(new { Status = false, Message = "Pincode Already Exist...." }, JsonRequestBehavior.AllowGet);
                }

                if (WPM.ActiveFlag != null && WPM.ActiveFlag.ToString() == "true")
                {
                    WPM.ActiveFlag = "Y";
                }
                else
                {
                    WPM.ActiveFlag = "N";
                }
                if (WPM.Is_ODA_Apply != null && WPM.Is_ODA_Apply.ToString() == "true")
                {
                    WPM.Is_ODA_Apply = "Y";
                }
                else
                {
                    WPM.Is_ODA_Apply = "N";
                }

                string MstDetails = "<PincodeMaster>";
                MstDetails = MstDetails + "<pincode>" + WPM.pincode + "</pincode>";
                MstDetails = MstDetails + "<ID>" + WPM.ID + "</ID>";
                MstDetails = MstDetails + "<StateCode>" + WPM.StateCode + "</StateCode>";
                MstDetails = MstDetails + "<cityname>" + WPM.cityname + "</cityname>";
                MstDetails = MstDetails + "<Area>" + WPM.Area + "</Area>";
                MstDetails = MstDetails + "<ActiveFlag>" + WPM.ActiveFlag + "</ActiveFlag>";
                MstDetails = MstDetails + "<EntryBy>" + BaseUserName + "</EntryBy>";
                MstDetails = MstDetails + "<LocCode>" + WPM.LocCode + "</LocCode>";
                MstDetails = MstDetails + "<District>" + WPM.District + "</District>";
                MstDetails = MstDetails + "<Region>" + WPM.Region + "</Region>";
                MstDetails = MstDetails + "<Service_Type>" + WPM.Service_Type + "</Service_Type>";

                /* PinCode Base ODA Changes*/
                MstDetails = MstDetails + "<KM_From_Location>" + WPM.KM_From_Location + "</KM_From_Location>";
                MstDetails = MstDetails + "<Handling_Location>" + WPM.Handling_Location + "</Handling_Location>";
                MstDetails = MstDetails + "<Is_ODA_Apply>" + WPM.Is_ODA_Apply + "</Is_ODA_Apply>";
                MstDetails = MstDetails + "</PincodeMaster>";

                DataTable Dt = _masterService.AddEditPincodeMaster(MstDetails);
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return Json(new { Status = Status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Status = Status }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ActiveInActive_Pincode(int id)
        {
            try
            {
                var data = _masterService.ActiveInActive_Pincode(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Country Master


        public ActionResult MasterCountriesList()
        {
            return View();
        }

        public JsonResult GetCountryDetailsById(int id)
        {
            try
            {
                var data = _masterService.GetCountryMaster().FirstOrDefault(c => c.Id == id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ActiveInActiveCountry(int id)
        {
            try
            {
                var data = _masterService.ActiveInActiveCountry(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddEditCountryMaster(CYGNUS_CountryViewModel CCVM)
        {
            bool Status = false;
            try
            {
                var list = _masterService.GetCountryMaster();
                if (list != null)
                {
                    if (list.Any(c => c.CountryName.Trim().ToUpper() == CCVM.CM.CountryName.Trim().ToUpper() && c.Id != CCVM.CM.Id))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    if (list.Any(c => c.CountryCode.Trim().ToUpper() == CCVM.CM.CountryCode.Trim().ToUpper() && c.Id != CCVM.CM.Id))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }

                string MstDetails = "<Country>";
                MstDetails = MstDetails + "<Id>" + CCVM.CM.Id + "</Id>";
                MstDetails = MstDetails + "<CountryName>" + CCVM.CM.CountryName + "</CountryName>";
                MstDetails = MstDetails + "<CountryCode>" + CCVM.CM.CountryCode + "</CountryCode>";
                MstDetails = MstDetails + "</Country>";
                DataTable Dt = _masterService.AddEditCountryMaster(MstDetails, BaseUserName);
                if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckDuplicateCountryName(string CountryName, int Id = 0)
        {
            string Count = "0";
            try
            {
                var list = _masterService.GetCountryMaster();
                if (list != null)
                {
                    var dupList = list.Where(c => c.CountryName.Trim().ToUpper() == CountryName.Trim().ToUpper() && c.Id != Id).ToList();
                    Count = dupList.Count.ToString();
                }
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckDuplicateCountryCode(string CountryCode, int Id = 0)
        {
            string Count = "0";
            try
            {
                var list = _masterService.GetCountryMaster();
                if (list != null)
                {
                    var dupList = list.Where(c => c.CountryCode.Trim().ToUpper() == CountryCode.Trim().ToUpper() && c.Id != Id).ToList();
                    Count = dupList.Count.ToString();
                }
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Lane Master
        public ActionResult LaneMaster()
        {
            CYGNUS_LaneViewModel CLVM = new CYGNUS_LaneViewModel();
            try
            {
                CLVM.listCL = new List<CYGNUS_LaneMaster>();
                CLVM.listCL = _masterService.GetLaneDetails().ToList();
                CLVM.CL = new CYGNUS_LaneMaster();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(CLVM);
        }

        public ActionResult AddEditLane(int id)
        {
            CYGNUS_LaneMaster CL = new CYGNUS_LaneMaster();
            try
            {
                if (id > 0)
                {
                    CL = _masterService.GetLaneDetails().FirstOrDefault(c => c.Lane_ID == id);
                }
                ViewBag.StateList = _masterService.GetStateMaster().ToList();
                ViewBag.ZoneList = _masterService.GetGeneralMaster().Where(c => c.CodeType.ToUpper() == "ZONE" && c.StatusCode == "Y").ToList();
            }
            catch (Exception)
            {
                return RedirectToAction("LaneMaster");
            }
            return PartialView("_AddEditLane", CL);
        }
        [HttpPost]
        public JsonResult AddEditLane(CYGNUS_LaneMaster CL)
        {
            bool Status = false;
            string Message = "";
            try
            {
                string Query = "EXEC USP_AddEditLaneMaster " + CL.Lane_ID + ",'" + CL.Lane_Name + "','" + CL.State + "','" + CL.City + "','" + CL.Zone + "','" + BaseUserName + "'";
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
        public JsonResult CheckDuplicateLane(string LaneName, int LaneId = 0)
        {
            string Count = "";
            try
            {
                List<CYGNUS_LaneMaster> CYGNUS_LaneList = _masterService.GetLaneDetails().Where(c => c.Lane_Name.ToUpper() == LaneName.ToUpper() && c.Lane_ID != LaneId).ToList();
                Count = CYGNUS_LaneList.Count.ToString();
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetLaneListJson(string searchTerm)
        {
            try
            {
                var lanes = _masterService.GetLaneDetails(searchTerm);
                var SearchList = (from e in lanes
                                  select new
                                  {
                                      id = e.Lane_ID,
                                      text = e.Lane_Name
                                  }).Distinct().Take(30).ToList();
                return Json(SearchList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Vehicle Model Master

        public ActionResult VehicleModel()
        {
            return View();
        }

        public JsonResult GetVehicleModelListJson()
        {
            List<CYGNUS_Vehicle_Model> listVehicleType = _masterService.GetVehicleModelDetails();
            var data = (from e in listVehicleType
                        select new
                        {
                            e.Type_Code,
                            e.Type_Name,
                            e.Made_By,
                            e.ActiveFlag
                        }).ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditVehicleModel(int id)
        {
            CYGNUS_Vehicle_ModelViewModel WVTVM = new CYGNUS_Vehicle_ModelViewModel();
            WVTVM.WVT = new CYGNUS_Vehicle_Model();
            if (id > 0)
            {
                WVTVM.EditFlag = true;
                WVTVM.WVT = _masterService.GetVehicleModelDetails().Where(c => c.Type_Code == id).FirstOrDefault();
            }
            return View(WVTVM);
        }

        [HttpPost]
        public ActionResult AddEditVehicleModel(CYGNUS_Vehicle_ModelViewModel WVTVM)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(WVTVM.WVT.GetType());
                WVTVM.WVT.UPDTBY = BaseUserName;
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, WVTVM.WVT);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    var flag = WVTVM.WVT.Type_Code;
                    if (flag > 0)
                    {
                        DataTable Dt = _masterService.AddEditVehicleModelMaster(xmlDoc.InnerXml, "US", BaseFinYear.Split('-')[0].ToString());
                    }
                    else
                    {
                        DataTable Dt = _masterService.AddEditVehicleModelMaster(xmlDoc.InnerXml, "E", BaseFinYear.Split('-')[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("VehicleModel");
        }

        [HttpPost]
        public JsonResult ActiveInActive_VehicleModel(int id)
        {
            try
            {
                var data = _masterService.ActiveInActive_VehicleModel(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Vehicle Master

        public ActionResult VehicleList()
        {
            return View();
        }

        public JsonResult GetVehicleListJson()
        {
            List<CYGNUS_Vehicle_Master> ListVehicles = _masterService.GetVehicleList("", BaseUserName, "Vehicle_List");

            var ListVehiclesdata = (from e in ListVehicles
                                    select new
                                    {
                                        ID = e.ID,
                                        VehicleNo = e.VehicleNo,
                                        VehicleCode = e.VehicleCode == "P" ? "Puller" : (e.VehicleCode == "T" ? "Trailer" : (e.VehicleCode == "FT" ? "Full Truck" : e.VehicleCode)),
                                        IsRegistered = e.IsRegistered,
                                        IsActive = e.IsActive
                                    }).ToArray();

            return Json(ListVehiclesdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditVehicle(string Id)
        {
            GeneralFunctions GF = new GeneralFunctions();
            GF.DeleteAllFile();
            VehicleViewModel VVM = new VehicleViewModel();
            try
            {
                ViewBag.BaseUserName = BaseUserName;
                VVM.EditFlag = "false";
                VVM.VehicleHDR = new CYGNUS_Vehicle_Master();
                VVM.VehicleHDR.IsActive = true;
                VVM.VehicleHDR.IsRegistered = true;

                ViewBag.StatePermitList = _masterService.GetStateMaster();

                // Data lists for dropdowns - Cleaner MVC architecture
                ViewBag.FuelTypeList = _masterService.GetGeneralMasterWithParam("FUELTY");
                ViewBag.VendorTypeList = _masterService.GetGeneralMasterWithParam("VENDTY", "05,XX");
                ViewBag.ManufactureList = _masterService.GetGeneralMasterWithParam("MFG");
                ViewBag.VehicleTypeList = _masterService.GetGeneralMasterWithParam("FTLTYP");
                ViewBag.InsuranceTypeList = _masterService.GetGeneralMasterWithParam("NCBTYPE");
                ViewBag.VehicalAPIEnable = _masterService.GetGeneralMasterWithParam("VEHAPIENABLE").FirstOrDefault()?.CodeDesc ?? "N";
                ViewBag.FuelUOMList = _masterService.GetGeneralMasterWithParam("UOM", "1,11,12");

                ViewBag.CityList = _masterService.GetCityMaster().Where(c => c.activeflag == "Y").OrderBy(c => c.Location).ToList();
                ViewBag.RouteList = _masterService.GetRutMstDetails();

                ViewBag.VehicleCodeList = new List<SelectListItem> {
                    new SelectListItem { Text = "Puller", Value = "P" },
                    new SelectListItem { Text = "Trailer", Value = "T" },
                    new SelectListItem { Text = "Full Truck", Value = "FT" }
                };

                ViewBag.StatusList = new List<SelectListItem> {
                    new SelectListItem { Text = "New Vehicle", Value = "NewVehicle" },
                    new SelectListItem { Text = "Scrap", Value = "Scrap" },
                    new SelectListItem { Text = "Total Loss", Value = "TotalLoss" }
                };

                ViewBag.VehicleGroupList = new List<SelectListItem> {
                    new SelectListItem { Text = "Transport Vehicle", Value = "TransportVehicle" },
                    new SelectListItem { Text = "Utility Vehicle", Value = "UtilityVehicle" }
                };


                ViewBag.LESSHYPOList = _masterService.GetGeneralMasterWithParam("LESSHYPO");


                ViewBag.ListVendor = new List<CYGNUS_VENDOR_HDR>();
                ViewBag.ListVehicleModel = new List<CYGNUS_Vehicle_Model>();
                ViewBag.ListLocations = new List<CYGNUS_location>();

                if (Id != null && Id != "")
                {
                    VVM.EditFlag = "true";
                    var existingVehicle = _masterService.GetVehicleById(Id);
                    if (existingVehicle != null)
                    {
                        VVM.VehicleHDR = existingVehicle;
                        ViewBag.ListVendor = _masterService.GetVendorObject().Where(m => m.Vendor_Type == VVM.VehicleHDR.VendorType).ToList();
                        ViewBag.ListLocations = _masterService.GetLocationDetails();

                        // Populate Vehicle Model list based on vehicle code
                        string targetCategory = (VVM.VehicleHDR.VehicleCode == "T") ? "02" : "01";
                        ViewBag.ListVehicleModel = _masterService.GetVehicleModel().Where(m => m.Vehicle_Type_Category == targetCategory).ToList();
                    }
                }

                // Safety check: ensure VehicleHDR is never null for the view
                if (VVM.VehicleHDR == null)
                {
                    VVM.VehicleHDR = new CYGNUS_Vehicle_Master();
                    VVM.VehicleHDR.IsActive = true;
                    VVM.VehicleHDR.IsRegistered = true;
                }
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(VVM);
        }

        [HttpPost]
        public ActionResult VehicleMasterSubmit(HttpPostedFileBase[] files, VehicleViewModel VM)
        {
            try
            {
                // 1. Handle Multiple File Uploads
                string uploadBaseDir = Server.MapPath("~/UploadedDocuments/VehicleUpload/");
                string yearMonth = DateTime.Now.ToString("yyyy/MMMM");
                string targetDir = Path.Combine(uploadBaseDir, yearMonth);

                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                // Main Vehicle Photo (files[0] or named "files" in Request.Files)
                var mainPhoto = (files != null && files.Length > 0 && files[0] != null && files[0].ContentLength > 0)
                                ? files[0]
                                : (Request.Files["files"] != null && Request.Files["files"].ContentLength > 0 ? Request.Files["files"] : null);

                if (mainPhoto != null)
                {
                    string extension = Path.GetExtension(mainPhoto.FileName).ToLower();
                    string newFName = Guid.NewGuid().ToString() + extension;
                    mainPhoto.SaveAs(Path.Combine(targetDir, newFName));
                    VM.VehicleHDR.VehiclePhoto = "/" + yearMonth + "/" + newFName;
                }

                // Handle named document files (doc_file_...)
                if (VM.VehicleHDR.DocumentList != null && VM.VehicleHDR.DocumentList.Count > 0)
                {
                    foreach (var doc in VM.VehicleHDR.DocumentList)
                    {
                        string fileKey = "doc_file_" + doc.Document_Id;
                        if (Request.Files[fileKey] != null && Request.Files[fileKey].ContentLength > 0)
                        {
                            var postedFile = Request.Files[fileKey];
                            string extension = Path.GetExtension(postedFile.FileName).ToLower();
                            string newFName = Guid.NewGuid().ToString() + extension;
                            postedFile.SaveAs(Path.Combine(targetDir, newFName));

                            // Map path to DocPath property
                            doc.DocPath = "/" + yearMonth + "/" + newFName;
                        }
                    }
                }

                // 2. Handle Multiple Vehicle Photos (named "photos")
                var uploadedPhotos = new List<string>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files.GetKey(i) == "photos")
                    {
                        var photo = Request.Files[i];
                        if (photo != null && photo.ContentLength > 0)
                        {
                            string extension = Path.GetExtension(photo.FileName).ToLower();
                            string newFName = Guid.NewGuid().ToString() + extension;
                            photo.SaveAs(Path.Combine(targetDir, newFName));
                            uploadedPhotos.Add("/" + yearMonth + "/" + newFName);
                        }
                    }
                }

                if (uploadedPhotos.Count > 0)
                {
                    VM.VehicleHDR.VehiclePhoto = string.Join(",", uploadedPhotos);
                }

                // 3. Date Sanitization & Fallback
                DateTime minDate = new DateTime(1900, 1, 1);
                Func<DateTime, DateTime> sanitize = (d) => (d < minDate) ? minDate : d;

                VM.VehicleHDR.RegistrationDate = sanitize(VM.VehicleHDR.RegistrationDate);
                VM.VehicleHDR.PurchaseDate = sanitize(VM.VehicleHDR.PurchaseDate);
                VM.VehicleHDR.SaleDate = sanitize(VM.VehicleHDR.SaleDate);
                VM.VehicleHDR.WarrantyFrom = sanitize(VM.VehicleHDR.WarrantyFrom);
                VM.VehicleHDR.WarrantyTo = sanitize(VM.VehicleHDR.WarrantyTo);
                VM.VehicleHDR.AMCFrom = sanitize(VM.VehicleHDR.AMCFrom);
                VM.VehicleHDR.AMCTo = sanitize(VM.VehicleHDR.AMCTo);

                // 3. XML Serialization
                string xmlString;
                var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var serializer = new XmlSerializer(VM.VehicleHDR.GetType());
                var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false };

                using (var stringWriter = new StringWriter())
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, VM.VehicleHDR, emptyNamespaces);
                    xmlString = stringWriter.ToString();
                }

                // 4. Save to Database (SP now handles both Vehicle and Documents via XML)
                bool status = _masterService.AddEditVehicle(xmlString, VM.EditFlag, BaseUserName, BaseCompanyCode);

                return Json(new { Status = status, Message = status ? "Vehicle details saved successfully." : "Failed to save vehicle details." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetVehicleDocuments(string VehicleType, string VendorType, string VehicleId = "")
        {
            try
            {
                var masterDocs = _masterService.GetVehicleDocumentTypeWise(VehicleType, VendorType);
                if (!string.IsNullOrEmpty(VehicleId))
                {
                    var savedDocs = _masterService.GetVehicleDocument(VehicleId);
                    foreach (var doc in masterDocs)
                    {
                        var saved = savedDocs.FirstOrDefault(s => s.Document_Id == doc.Document_Id);
                        if (saved != null)
                        {
                            doc.DocPath = saved.DocPath;
                            doc.FromDate = saved.FromDate;
                            doc.ToDate = saved.ToDate;
                        }
                    }
                }
                return Json(masterDocs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<CYGNUS_Vehicle_Document_Type>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ActiveInActive_Vehicle(string id)
        {
            try
            {
                var data = _masterService.ActiveInActive_Vehicle(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetVehicleModelByType(string vehicleType)
        {
            try
            {
                // Category 02 for Trailer (T), Category 01 for others (Puller/FullTruck)
                string targetCategory = (vehicleType == "T") ? "02" : "01";

                var models = _masterService.GetVehicleModel().Where(m => m.Vehicle_Type_Category == targetCategory).Select(m => new
                {
                    Value = m.Type_Code.ToString(),
                    Text = m.Type_Name
                }).ToList();

                return Json(models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Fill Dropdown
        public JsonResult GetFillVendorListJson(string Verndortype)
        {
            List<CYGNUS_VENDOR_HDR> ListVendorHDR = _masterService.GetVendorObject().Where(c => c.Vendor_Type == Verndortype).OrderBy(c => c.VENDORNAME).ToList();

            var ListVendor = (from e in ListVendorHDR
                              select new
                              {
                                  Value = e.VENDORCODE,
                                  Text = e.VENDORNAME,
                              }).ToArray();
            return Json(ListVendor, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateVehicle(string Id)
        {
            // For duplicate check by Vehicle Number, we use the full list
            int count = _masterService.GetVehicleList("", BaseUserName, "CheckDuplicate_Veh").Where(m => (m.VehicleNo ?? "").Trim().ToUpper() == (Id ?? "").Trim().ToUpper()).Count();
            return Json(count);
        }
        public async Task<string> GetFirstTokenAsync()
        {
            var httpClient = new HttpClient();

            var url = "https://services.cygnux.in/api/auth/login";

            string email = ConfigurationManager.AppSettings["API_Email"];
            string password = ConfigurationManager.AppSettings["API_Password"];

            var payload = new
            {
                email = email,
                password = password
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get token. Status Code: {response.StatusCode}\nResponse: {jsonResponse}");

            dynamic result = JsonConvert.DeserializeObject(jsonResponse);
            return result?.token?.ToString();
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetVehicleDetailsFromAPINew(string VEHNO)
        {
            if (string.IsNullOrEmpty(VEHNO) || VEHNO.Trim().Length < 4)
            {
                return Json(new { stautsMessage = "Error", statusDescription = "Vehicle number must be at least 4 characters long." }, JsonRequestBehavior.AllowGet);
            }
            VehicleDetails result = new VehicleDetails();
            string api_key = ConfigurationManager.AppSettings["api_key"];
            string user_id = ConfigurationManager.AppSettings["user_id"];

            try
            {
                // 1. Check database cache first
                string cachedResponse = _masterService.GetVehicleAPICache(VEHNO);
                if (!string.IsNullOrEmpty(cachedResponse))
                {
                    Console.WriteLine("Vehicle details fetched from cache.");
                    result = JsonConvert.DeserializeObject<VehicleDetails>(cachedResponse);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 2. If not in cache, call external API
                string token = await GetFirstTokenAsync();
                string url = "https://services.cygnux.in/api/vahan";

                var jsonContent = new
                {
                    api_key = api_key,
                    user_id = user_id,
                    vehicle_number = VEHNO
                };

                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Content = new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize into VehicleDetails directly since response is flat JSON
                        result = JsonConvert.DeserializeObject<VehicleDetails>(responseBody);

                        // 3. Save successful response to cache
                        if (result != null && result.stautsMessage != "Error") // Ensure valid response
                        {
                            _masterService.AddEditVehicleAPICache(VEHNO, responseBody);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch data. Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult Getlatitude(string loccode)
        //{
        //    try
        //    {
        //        List<CYGNUS_location> result = _masterService.Getlatitude(loccode);

        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //Fill Dropdown
        public JsonResult GetVehicleMasterDocumentListWithJson(string VehicleId)
        {
            List<CYGNUS_Vehicle_Document_Type> VehicleMasterDocsList = _masterService.GetVehicleDocument(VehicleId);

            var ListVehicleMasterDocument = (from e in VehicleMasterDocsList
                              select new
                              {
                                  Value = e.Document_Id,
                                  Text = e.Document_Name,
                              }).ToArray();
            return Json(ListVehicleMasterDocument, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVehicleDocumentListForEventWithJson()
        {
            List<Cygnus_Master_Vehicle_DocumentType> VehicleMasterDocsListForEvent  = _masterService.GetVehicleDocumentTypeById(0);

            var ListVehicleMasterDocument = (from e in VehicleMasterDocsListForEvent
                                             select new
                                             {
                                                 Value = e.Id,
                                                 Text = e.Document_Name,
                                             }).ToArray();
            return Json(ListVehicleMasterDocument, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public JsonResult GetAllUserJson(string searchTerm)
        {
            List<CYGNUS_Master_Users> listUser = new List<CYGNUS_Master_Users>();
            listUser = _masterService.GetUserDetails().Where(c => c.Status == "100" && (c.Name.ToUpper().Contains(searchTerm.ToUpper()) || c.UserId.ToUpper().Contains(searchTerm.ToUpper()))).ToList();
            var SearchList = (from e in listUser
                              select new
                              {
                                  id = e.UserId,
                                  text = e.Name,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }

        #region Designation Mapping
        public ActionResult DesignationMapping()
        {
            List<CYGNUS_Designation_Mapping> listCDM = new List<CYGNUS_Designation_Mapping>();
            listCDM = _masterService.GetDesignationMappingList().ToList();
            ViewBag.GeneralMasterList = _masterService.GetGeneralMaster();
            return View(listCDM);
        }
        public JsonResult GetDesignationListJsonWithIdName(string str)
        {
            List<CYGNUS_Master_General> ListUsers = new List<CYGNUS_Master_General>();
            ListUsers = _masterService.GetGeneralMaster().Where(c => c.StatusCode == "Y" && c.CodeType == "DESIG").ToList();

            var SearchList = (from e in ListUsers
                              select new
                              {
                                  id = e.CodeId,
                                  text = e.CodeDesc,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DesignationMappingSubmit(List<CYGNUS_Designation_Mapping> MappingList)
        {
            string status = "";

            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(MappingList.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, MappingList);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
            }

            try
            {
                status = _masterService.InsertDesignationMappingDetails(xmlDoc.InnerXml, BaseUserName);
                if (!string.IsNullOrEmpty(status) && status == "NOT DONE")
                {
                    ViewBag.StrError = "Cannot update records.";
                    return View("Error");
                }
            }
            catch (Exception e1)
            {
                ViewBag.StrError = e1.Message.Replace('\n', ' ').ToString(); ;
                return View("Error");
            }

            return RedirectToAction("DesignationMappingDone", new { id = status });
        }

        public ActionResult DesignationMappingDone(string id)
        {
            ViewBag.Status = id;
            return View();
        }

        #endregion

        #region Driver master
        public ActionResult DriverList()
        {
            return View();
        }

        public JsonResult GetDriverListJson()
        {
            List<CYGNUS_FLEET_DRIVERMST> listWFDM = _masterService.GetDriverMstDetails();
            return Json(listWFDM, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditDriver(string id)
        {
            DriverViewModel DVM = new DriverViewModel
            {
                WFDDD = new CYGNUS_FLEET_DRIVER_DOCDET
                {
                    License_Verified_Dt = DateTime.Now
                },
                WFDM = new CYGNUS_FLEET_DRIVERMST
                {
                    D_Lic_Current_Issuance_Date = DateTime.Now,
                    D_Lic_Initial_Issuance_Date = DateTime.Now,
                    Date_of_Joining = DateTime.Now,
                    Date_of_ReJoining = DateTime.Now,
                    Date_of_Exit = DateTime.Now,
                    Valdity_dt = DateTime.Now,
                    Valdity_Todt = DateTime.Now,
                }
            };

            var type = "Add";
            if (id != null && id != "0")
            {
                type = "Edit";
                //WMFSDVM.EditFlag = true;
                DVM.WFDM = _masterService.GetDriverMstDetails().Where(c => c.Driver_Id == Convert.ToDecimal(id)).FirstOrDefault();
                var abc = Convert.ToString(DVM.WFDM.Date_of_Joining);
                if (Convert.ToString(DVM.WFDM.Date_of_Joining) == "1/1/0001 12:00:00 AM")
                {
                    DVM.WFDM.Date_of_Joining = System.DateTime.Now;
                }
                if (Convert.ToString(DVM.WFDM.Date_of_ReJoining) == "1/1/0001 12:00:00 AM")
                {
                    DVM.WFDM.Date_of_ReJoining = System.DateTime.Now;
                }
                if (Convert.ToString(DVM.WFDM.Date_of_Exit) == "1/1/0001 12:00:00 AM")
                {
                    DVM.WFDM.Date_of_Exit = System.DateTime.Now;
                }
                DVM.WFDDD = _masterService.GetDriverDetDetails().Where(c => c.Driver_Id == Convert.ToDecimal(id)).FirstOrDefault();
            }

            if (DVM.WFDM == null)
            {
                DVM.WFDM = new CYGNUS_FLEET_DRIVERMST();
            }
            if (DVM.WFDDD == null)
            {
                DVM.WFDDD = new CYGNUS_FLEET_DRIVER_DOCDET { License_Verified_Dt = DateTime.Now };
            }

            if (DVM.WFDM.Driver_Id == 0)
            {
                DVM.WFDM.ActiveFlag = "Y";
                DVM.WFDDD.License_Verified = "Y";
                DVM.WFDDD.Address_Verified = "Y";
            }

            ViewBag.CityList = _masterService.GetCityMaster().Where(c => c.activeflag == "Y").ToList().OrderBy(c => c.Location).ToList();
            ViewBag.ListLocations = _masterService.GetLocationDetailsForDriver().OrderBy(c => c.LocName).ToList();
            ViewBag.PincodeList = _masterService.GetPincodeMaster().ToList();
            ViewBag.VehicleAccountCodeList = _masterService.GetVehicleAccountCodeObject().ToList();
            DVM.WFDM.DriverAccountCode = "LIA0138";
            ViewBag.Type = type;

            ViewBag.CategoryList = _masterService.GetGeneralMasterWithParam("DRIVERCAT");
            ViewBag.EthnicityList = _masterService.GetGeneralMasterWithParam("DRVETH");
            ViewBag.ReasonForExitList = _masterService.GetGeneralMasterWithParam("REFE");

            return View(DVM);
        }

        [HttpPost]
        public ActionResult AddEditDriver(DriverViewModel DVM, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                int id = 0;
                var FolderName = "";
                if (DVM.WFDM.Driver_Id == 0)
                {
                    var Maxcode = _masterService.GetMaxDriverCode().ToString();
                    if (Maxcode == "")
                    {
                        Maxcode = "0";
                    }
                    FolderName = Convert.ToString((Convert.ToInt32(Maxcode) + 1));
                }
                else
                {
                    FolderName = Convert.ToString(DVM.WFDM.Driver_Id);
                }
                string[] array = new string[15];

                if (!string.IsNullOrEmpty(DVM.WFDM.DeiverPhotoBase64))
                {
                    byte[] imageBytes = Convert.FromBase64String(DVM.WFDM.DeiverPhotoBase64);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Image image = Image.FromStream(ms);
                        string fileName = FolderName + "_" + DateTime.Now.Ticks.ToString() + ".Png";
                        DVM.WFDM.Driver_Photo = fileName;
                        string filePath = Server.MapPath("~/Images/Driver/") + FolderName + "/" + fileName;
                        string strDirectoryName = Server.MapPath("~/Images/Driver/") + FolderName;
                        if (Directory.Exists(strDirectoryName) == false)
                            Directory.CreateDirectory(strDirectoryName);
                        image.Save(filePath, ImageFormat.Png);
                    }
                }

                DVM.WFDM.EntryBy = BaseUserName;
                DVM.WFDM.UpdatedBy = BaseUserName;

                array[0] = DVM.WFDDD.Electricity_Bill_File;
                array[1] = DVM.WFDDD.Telephone_Bill_File;
                array[2] = DVM.WFDDD.BankAcc_File;
                array[3] = DVM.WFDDD.Passport_File;
                array[4] = DVM.WFDDD.Rationcard_File;
                array[5] = DVM.WFDDD.Driver_Registration_Form_File;
                array[6] = DVM.WFDDD.ID_Passport_File;
                array[7] = DVM.WFDDD.Driving_lic_File;
                array[8] = DVM.WFDDD.VoterId_File;
                array[9] = DVM.WFDDD.PAN_File;
                array[10] = DVM.WFDM.Driver_Photo;
                array[11] = DVM.WFDDD.Thumb_Impression_File;
                array[12] = DVM.WFDDD.Aadhar_card_File;
                array[13] = DVM.WFDDD.Vaccine_Certificate_File;
                array[14] = DVM.WFDDD.Police_Verification_Certificate_File;

                DataTable DT = _masterService.AddEditDriver(DVM, array, BaseCompanyCode);
            }
            catch (Exception)
            {
            }
            return RedirectToAction("DriverList");
        }
        public JsonResult CheckDuplicateDriverCode(string Code)
        {
            try
            {
                string Count = _masterService.CheckDuplicateDriverManualCode(Code);

                return new JsonResult()
                {
                    Data = new
                    {
                        Count = Count,
                    }
                };
            }
            catch (Exception)
            {
                return Json(0);
            }
        }

        public JsonResult CheckDuplicateDriver(string number)
        {
            try
            {
                string Count = _masterService.CheckDuplicateDriver(number);

                return new JsonResult()
                {
                    Data = new
                    {
                        Count,
                    }
                };
            }
            catch (Exception)
            {
                return Json(0);
            }
        }

        public JsonResult ActiveInActive_Driver(int id)
        {
            try
            {
                var data = _masterService.ActiveInActive_Driver(id, BaseUserName);
                return Json(data);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        #endregion

        #region VEHICLE TYPE WISE DOCUMENT
        public ActionResult VehicleType_WiseDocument()
        {
            List<Cygnus_Master_VehicleType_wise_Document> Listmapping = new List<Cygnus_Master_VehicleType_wise_Document>();
            try
            {
                Listmapping = _masterService.Get_VehicleType_wise_DocumentDetails("");
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(Listmapping);
        }

        public ActionResult AddEditVehicleType_WiseDocument(string id)
        {
            try
            {
                var vehicleTypes = _masterService.GetGeneralMasterWithParam("FTLTYP").ToList();
                ViewBag.VehicleTypeList = new SelectList(vehicleTypes, "CodeId", "CodeDesc", id);
                ViewBag.SelectedVehicleType = id;

                List<Cygnus_Master_VehicleType_wise_Document> docList = new List<Cygnus_Master_VehicleType_wise_Document>();
                if (!string.IsNullOrEmpty(id))
                {
                    var allDocs = _masterService.GetVehicleDocumentTypeById(0).ToList();
                    DataTable dtExisting = GF.GetDataTableFromSP("EXEC USP_Get_VehicleType_wise_DocumentDetails '" + id.Replace("'", "''") + "'");
                    docList = allDocs.Select(d =>
                    {
                        DataRow existing = dtExisting.AsEnumerable()
                            .FirstOrDefault(r => r["Document_id"].ToString() == d.Id.ToString());
                        return new Cygnus_Master_VehicleType_wise_Document
                        {
                            Id = existing != null ? Convert.ToInt32(existing["Id"]) : 0,
                            Vehicle_DocumentType = id,
                            Documnet_Id = d.Id.ToString(),
                            Document_Name = d.Document_Name,
                            ShortName = existing != null ? existing["ShortName"].ToString() : "",
                            Is_RequireFor_Own = existing != null && Convert.ToInt32(existing["Is_RequireFor_Own"]) == 1,
                            Is_RequireFor_Market = existing != null && Convert.ToInt32(existing["Is_RequireFor_Market"]) == 1,
                            Is_Active = existing != null && Convert.ToInt32(existing["Is_Active"]) == 1
                        };
                    }).ToList();
                }
                return View(docList);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }

        public JsonResult GetDocumentListByVehicleType(string vehicleType)
        {
            try
            {
                var allDocs = _masterService.GetGeneralMasterWithParam("VEHDOCTYP").ToList();
                DataTable dtEx = GF.GetDataTableFromSP("EXEC USP_Get_VehicleType_wise_DocumentDetails '" + vehicleType.Replace("'", "''") + "'");
                var docList = allDocs.Select(d =>
                {
                    DataRow existing = dtEx.AsEnumerable()
                        .FirstOrDefault(r => r["Document_id"].ToString() == d.CodeId);
                    return new
                    {
                        Id = existing != null ? Convert.ToInt32(existing["Id"]) : 0,
                        Documnet_Id = d.CodeId,
                        Document_Name = d.CodeDesc,
                        ShortName = existing != null ? existing["ShortName"].ToString() : "",
                        Is_RequireFor_Own = existing != null && Convert.ToInt32(existing["Is_RequireFor_Own"]) == 1,
                        Is_RequireFor_Market = existing != null && Convert.ToInt32(existing["Is_RequireFor_Market"]) == 1,
                        Is_Active = existing != null && Convert.ToInt32(existing["Is_Active"]) == 1
                    };
                }).ToList();
                return Json(docList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveVehicleType_WiseDocument(string vehicleDocumentType, List<Cygnus_Master_VehicleType_wise_Document> docList)
        {
            bool Status = false;
            string Message = "";
            try
            {
                var xmlBuilder = new System.Text.StringBuilder("<Root>");
                foreach (var item in docList)
                {
                    xmlBuilder.Append("<Item>");
                    xmlBuilder.Append("<VehicleType>" + vehicleDocumentType + "</VehicleType>");
                    xmlBuilder.Append("<Document_id>" + item.Documnet_Id + "</Document_id>");
                    xmlBuilder.Append("<Document_Name>" + item.Document_Name + "</Document_Name>");
                    xmlBuilder.Append("<ShortName>" + (item.ShortName ?? "") + "</ShortName>");
                    xmlBuilder.Append("<Is_RequireFor_Own>" + (item.Is_RequireFor_Own ? "1" : "0") + "</Is_RequireFor_Own>");
                    xmlBuilder.Append("<Is_RequireFor_Market>" + (item.Is_RequireFor_Market ? "1" : "0") + "</Is_RequireFor_Market>");
                    xmlBuilder.Append("<Is_Active>" + (item.Is_Active ? "1" : "0") + "</Is_Active>");
                    xmlBuilder.Append("</Item>");
                }
                xmlBuilder.Append("</Root>");
                string spQuery = "EXEC USP_Save_VehicleType_wise_Document '" + xmlBuilder.ToString().Replace("'", "''") + "','" + BaseUserName + "'";
                GF.GetDataTableFromSP(spQuery);
                Status = true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Json(new { Status = Status, Message = Message });
        }
        #endregion
        #region  vehicle Driver Mapping 
        public ActionResult VehicleDriverMappingList()
        {
            List<Cygnus_VehicleDriver_Mapping> Listmapping = new List<Cygnus_VehicleDriver_Mapping>();

            try
            {
                Listmapping = _masterService.Get_VehicleDriverMapping_Details(0, BaseUserName);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(Listmapping);
        }
        public ActionResult AddEditVehicleDriverMapping(int id)
        {
            try
            {
                Cygnus_VehicleDriver_Mapping CVDM = new Cygnus_VehicleDriver_Mapping();
                if (id != 0)
                {
                    CVDM = _masterService.Get_VehicleDriverMapping_Details(id, BaseUserName).FirstOrDefault();
                }

                ViewBag.Driverlist = _masterService.GetAvailableDriver(CVDM.VehicleId);
                //ViewBag.VehicleList = _masterService.Get_VehicleDriverMapping_VehicleList(CVDM.VehicleId);
                ViewBag.VehicleList = _masterService.GetVehicleList(CVDM.VehicleId, BaseUserName, "Vehicle_Driver_Map");

                return View(CVDM);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }
        public ActionResult VehicleDriverMappingSubmit(Cygnus_VehicleDriver_Mapping CCVM)
        {
            try
            {
                DataTable DT = _masterService.VehicleDriverMappingSubmit(CCVM.VehicleId, CCVM.First_Driver, CCVM.Second_Driver, BaseUserName);
            }
            catch (Exception ex)
            {

                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("VehicleDriverMappingList");

        }
        public ActionResult DetachDriverMapping(string vehicleId, string driverType, string driverId, string detachReason = "")
        {
            try
            {
                _masterService.DetachDriverMapping(vehicleId, driverType, driverId, BaseUserName, detachReason);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("VehicleDriverMappingList");
        }
        [HttpPost]
        public JsonResult ValidateDriverDetach(string vehicleId)
        {
            bool showWarning = false;

            // Vehicle Driver Mapping
            DataTable dtDriver = _masterService.GetVehicleDriverMapping(vehicleId);

            if (dtDriver.Rows.Count > 0)
            {
                string firstDriver = Convert.ToString(dtDriver.Rows[0]["First_Driver"]);
                string secondDriver = Convert.ToString(dtDriver.Rows[0]["Second_Driver"]);

                bool onlyOneDriver =
                    (firstDriver == "0" && secondDriver != "0") ||
                    (firstDriver != "0" && secondDriver == "0");

                if (onlyOneDriver)
                {
                    DataTable dtTrip = _masterService.GetActiveTrip(vehicleId);

                    if (dtTrip.Rows.Count > 0)
                    {
                        showWarning = true;
                    }
                }
            }

            return Json(new { ShowTransitWarning = showWarning }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region  vehicle Trailer Mapping 
        public ActionResult VehicTrailerMapping()
        {
            List<CYGNUS_TRAILER_MAPPING> Listmapping = new List<CYGNUS_TRAILER_MAPPING>();
            try
            {
                Listmapping = _masterService.GetTrailerMappingObject("", BaseUserName);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(Listmapping);
        }
        public ActionResult AddEditVehicTrailerMapping(string Vehicleid)
        {
            try
            {
                CYGNUS_TRAILER_MAPPING CTM = new CYGNUS_TRAILER_MAPPING();
                if (!string.IsNullOrEmpty(Vehicleid))
                {
                    CTM = _masterService.GetTrailerMappingObject(Vehicleid, BaseUserName).FirstOrDefault();
                }
                return View(CTM);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }
        public ActionResult VehicleTrailerMappingSubmit(CYGNUS_TRAILER_MAPPING TM)
        {
            try
            {
                string MstDetDetails = "<root>";
                MstDetDetails = MstDetDetails + "<VehicleId>" + TM.VehicleId + "</VehicleId>";
                MstDetDetails = MstDetDetails + "<TrailerId>" + TM.TrailerId + "</TrailerId>";
                MstDetDetails = MstDetDetails + "<Target_Customer>" + TM.Target_Customer + "</Target_Customer>";
                MstDetDetails = MstDetDetails + "<Branch>" + TM.Branch + "</Branch>";
                MstDetDetails = MstDetDetails + "<Primary_Supervisor>" + TM.Primary_Supervisor + "</Primary_Supervisor>";
                MstDetDetails = MstDetDetails + "<Secondary_Supervisor>" + TM.Secondary_Supervisor + "</Secondary_Supervisor>";
                MstDetDetails = MstDetDetails + "<Cluster_Head>" + TM.Cluster_Head + "</Cluster_Head>";

                MstDetDetails += "</root>";
                DataTable DT = _masterService.VehicleTrailerMappingSubmit(MstDetDetails, BaseUserName);
            }
            catch (Exception ex)
            {

                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("VehicTrailerMapping");

        }
        public ActionResult DetachTrailerMapping(string VehicleId)
        {
            try
            {
                _masterService.DetachTrailerMapping(VehicleId, BaseUserName);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return RedirectToAction("VehicTrailerMapping");
        }
        #endregion


        #region Fuel Station Master
        public ActionResult FuelStation()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }

        public JsonResult FuelStationListJson()
        {
            List<CYGNUS_Master_FuelStation> listFS = _masterService.GetFuelStation();
            return Json(listFS, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditFuelStation(int id)
        {
            CYGNUS_Master_FuelStationViewModel CMFSVM = new CYGNUS_Master_FuelStationViewModel();
            try
            {
                ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
                if (id > 0)
                {
                    CMFSVM.FuelStation = _masterService.GetFuelStationById(id);

                    var geofence = _masterService.GetGeofenceByName("FuelStation_" + id);
                    if (geofence != null)
                    {
                        CMFSVM.ShapeType = geofence.ShapeType;
                        CMFSVM.Radius = geofence.Radius;
                        CMFSVM.Geom = geofence.Geom;
                    }
                }
                else
                {
                    CMFSVM.FuelStation = new CYGNUS_Master_FuelStation { IsActive = true };
                }

                ViewBag.CityList = _masterService.GetCityMaster().OrderBy(m => m.Location).ToList();
                ViewBag.StateList = _masterService.GetStateMaster().Where(c => c.activeflag == "Y").ToList();
                ViewBag.PincodeList = _masterService.GetPincodeMaster().Where(c => c.ActiveFlag == "Y").ToList();
            }
            catch (Exception) { }
            return View("_AddEditFuelStation", CMFSVM);
        }

        [HttpPost]
        public ActionResult AddEditFuelStation(CYGNUS_Master_FuelStationViewModel CMFSVM)
        {
            bool Status = false;
            try
            {
                if (CMFSVM.FuelStation.Id == 0)
                {
                    CMFSVM.FuelStation.IsActive = true;
                }

                string MstDetails = "<FuelStation>";
                MstDetails = MstDetails + "<Id>" + CMFSVM.FuelStation.Id + "</Id>";
                MstDetails = MstDetails + "<State>" + CMFSVM.FuelStation.State + "</State>";
                MstDetails = MstDetails + "<City>" + CMFSVM.FuelStation.City + "</City>";
                MstDetails = MstDetails + "<Location>" + CMFSVM.FuelStation.StationName + "</Location>";
                MstDetails = MstDetails + "<Address>" + CMFSVM.FuelStation.GeoAddress + "</Address>";
                MstDetails = MstDetails + "<Pincode>" + CMFSVM.FuelStation.Pincode + "</Pincode>";
                MstDetails = MstDetails + "<Latitude>" + CMFSVM.FuelStation.Latitude + "</Latitude>";
                MstDetails = MstDetails + "<Longitude>" + CMFSVM.FuelStation.Longitude + "</Longitude>";
                MstDetails = MstDetails + "<ContactPersonName>" + CMFSVM.FuelStation.ContactPersonName + "</ContactPersonName>";
                MstDetails = MstDetails + "<ContactPersonNumber>" + CMFSVM.FuelStation.ContactPersonNumber + "</ContactPersonNumber>";
                MstDetails = MstDetails + "<ContactPersonEmail>" + CMFSVM.FuelStation.ContactPersonEmail + "</ContactPersonEmail>";
                MstDetails = MstDetails + "<IsActive>" + CMFSVM.FuelStation.IsActive + "</IsActive>";
                MstDetails = MstDetails + "<UgelroId>" + CMFSVM.FuelStation.UgelroId + "</UgelroId>";
                MstDetails = MstDetails + "<ShapeType>" + CMFSVM.ShapeType + "</ShapeType>";
                MstDetails = MstDetails + "<Radius>" + (CMFSVM.Radius ?? 0) + "</Radius>";
                MstDetails = MstDetails + "<Geom>" + CMFSVM.Geom + "</Geom>";
                MstDetails = MstDetails + "</FuelStation>";

                Status = _masterService.AddEditFuelStation(MstDetails, BaseCompanyCode, BaseUserName);

                return new JsonResult()
                {
                    Data = new
                    {
                        Status,
                    }
                };
            }
            catch (Exception)
            {
                return Json(Status);
            }
        }

        [HttpPost]
        public ActionResult ActiveInActiveFuelStation(int id)
        {
            bool status = false;
            try
            {
                status = _masterService.ActiveInActiveFuelStation(id);
                if (status)
                {
                    try
                    {
                        var existingGeofence = _masterService.GetGeofenceMaster()
                            .FirstOrDefault(x => x.GeofenceName == "FuelStation_" + id);
                        if (existingGeofence != null)
                        {
                            _masterService.ActiveInActiveGeofenceMaster(existingGeofence.Id, BaseUserName);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
            return Json(status);
        }
        #endregion

        #region Card Master
        public ActionResult Card()
        {
            try
            {
                List<CYGNUS_Master_Card> list = _masterService.GetCardMaster();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }

        public JsonResult CardListJson()
        {
            var list = _masterService.GetCardMaster();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult _AddEditCard(int id)
        {
            CYGNUS_Master_CardViewModel CMVM = new CYGNUS_Master_CardViewModel();
            try
            {
                if (id > 0)
                {
                    CMVM.Card = _masterService.GetCardMasterById(id);
                }

                if (CMVM.Card == null)
                {
                    CMVM.Card = new CYGNUS_Master_Card { IsActive = true };
                }

                ViewBag.CardTypes = new List<SelectListItem> {
                    new SelectListItem { Text = "Fuel Card", Value = "Fuel" },
                    new SelectListItem { Text = "Reimbursement Card", Value = "Reimbursement" }
                };
                ViewBag.CardProviders = new List<SelectListItem> {
                    new SelectListItem { Text = "Indian Oil Corporation Limited", Value = "IOCL" },
                    new SelectListItem { Text = "Bharat Petroleum Corporation Limited", Value = "BPCL" },
                    new SelectListItem { Text = "Hindustan Petroleum Corporation Limited", Value = "HPCL" },
                    new SelectListItem { Text = "Reliance Industries Limited", Value = "Reliance" },
                    new SelectListItem { Text = "Nayara Energy", Value = "Nayara" },
                    new SelectListItem { Text = "Shell India", Value = "Shell" }
                };
            }
            catch (Exception) { }
            return PartialView(CMVM);
        }

        [HttpPost]
        public ActionResult AddEditCard(CYGNUS_Master_CardViewModel CMVM)
        {
            bool Status = false;
            try
            {
                if (CMVM.Card == null) CMVM.Card = new CYGNUS_Master_Card();

                string MstDetails = "<Card>";
                MstDetails += "<Id>" + CMVM.Card.Id + "</Id>";
                MstDetails += "<CardNo>" + CMVM.Card.CardNo + "</CardNo>";
                MstDetails += "<CardType>" + (CMVM.Card.CardType ?? "") + "</CardType>";
                MstDetails += "<CardProvider>" + (CMVM.Card.CardProvider ?? "") + "</CardProvider>";
                MstDetails += "<MaxLimit>" + (CMVM.Card.MaxLimit ?? 0) + "</MaxLimit>";
                MstDetails += "<ValidFrom>" + (CMVM.Card.ValidFrom.HasValue ? CMVM.Card.ValidFrom.Value.ToString("yyyy-MM-dd") : "") + "</ValidFrom>";
                MstDetails += "<ValidTill>" + (CMVM.Card.ValidTill.HasValue ? CMVM.Card.ValidTill.Value.ToString("yyyy-MM-dd") : "") + "</ValidTill>";
                MstDetails += "<NameOnCard>" + (CMVM.Card.NameOnCard ?? "") + "</NameOnCard>";
                MstDetails += "<MobileNo>" + (CMVM.Card.MobileNo ?? "") + "</MobileNo>";
                MstDetails += "<IsActive>" + (CMVM.Card.IsActive ? "1" : "0") + "</IsActive>";
                MstDetails += "</Card>";

                Status = _masterService.AddEditCardMaster(MstDetails, BaseCompanyCode, BaseUserName);

                return Json(new { Status });
            }
            catch (Exception ex)
            {
                return Json(Status);
            }
        }

        [HttpPost]
        public ActionResult ActiveInActiveCard(int id)
        {
            bool status = false;
            try
            {
                status = _masterService.ActiveInActiveCardMaster(id);
            }
            catch (Exception) { }
            return Json(status);
        }
        #endregion

        #region Card Assignment 
        public ActionResult CardAssignment()
        {
            try
            {
                var list = _masterService.GetCardAssignment();
                return View(list);
            }
            catch (Exception ex)
            {
                return RedirectToAction("General", "Error", new { message = ex.Message });
            }
        }

        public ActionResult _AddEditCardAssignment(int id)
        {
            var model = new CYGNUS_Master_Card_AssignmentViewModel();
            try
            {
                ViewBag.AssignTypeList = new List<SelectListItem> {
                    new SelectListItem { Text = "Employee", Value = "EMP" },
                    new SelectListItem { Text = "Vehicle", Value = "VEH" },
                    new SelectListItem { Text = "Driver", Value = "DRV" }
                };

                ViewBag.CardTypeList = new List<SelectListItem> {
                    new SelectListItem { Text = "Fuel Card", Value = "Fuel" },
                    new SelectListItem { Text = "Reimbursement Card", Value = "Reimbursement" }
                };

                List<SelectListItem> list = new List<SelectListItem>();

                if (id > 0)
                {
                    model.Assignment = _masterService.GetCardAssignmentById(id);
                    // Pre-load the 'Assign To' list based on existing type
                    if (model.Assignment != null)
                    {
                        ViewBag.AssignToList = GetAssigneeList(model.Assignment.AssignType);

                        list = _masterService.GetCardMasterByType(model.Assignment.CardType).Select(c => new SelectListItem
                        {
                            Text = c.CardNo,
                            Value = c.Id.ToString()
                        }).ToList();

                        list.Add(new SelectListItem
                        {
                            Text = model.Assignment.CardNo,
                            Value = model.Assignment.CardNoId.ToString(),
                        });
                    }
                    else
                    {
                        ViewBag.AssignToList = new List<SelectListItem>();
                    }

                }
                else
                {
                    list = _masterService.GetCardMasterByType(null).Select(c => new SelectListItem
                    {
                        Text = c.CardNo,
                        Value = c.Id.ToString()
                    }).ToList();

                    model.Assignment = new CYGNUS_Master_Card_Assignment { FromDate = DateTime.Today, IsActive = true };
                    ViewBag.AssignToList = new List<SelectListItem>();
                }

                ViewBag.CardNoList = list;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("General", "Error", new { message = ex.Message });
            }
        }

        public ActionResult _AddCardAssignmentForVehicle(string vehicleId)
        {
            var model = new CYGNUS_Master_Card_AssignmentViewModel();
            try
            {
                ViewBag.AssignTypeList = new List<SelectListItem> {
                    new SelectListItem { Text = "Employee", Value = "EMP" },
                    new SelectListItem { Text = "Vehicle", Value = "VEH" },
                    new SelectListItem { Text = "Driver", Value = "DRV" }
                };

                ViewBag.CardTypeList = new List<SelectListItem> {
                    new SelectListItem { Text = "Fuel Card", Value = "Fuel" },
                    new SelectListItem { Text = "Reimbursement Card", Value = "Reimbursement" }
                };

                ViewBag.CardNoList = _masterService.GetCardMasterByType(null).Select(c => new SelectListItem
                {
                    Text = c.CardNo,
                    Value = c.Id.ToString()
                }).ToList();

                ViewBag.AssignToList = GetAssigneeList("VEH");
                ViewBag.IsPreset = true;

                model.Assignment = new CYGNUS_Master_Card_Assignment
                {
                    AssignType = "VEH",
                    AssignPersonId = vehicleId,
                    FromDate = DateTime.Today,
                    IsActive = true
                };

                return PartialView("_AddEditCardAssignment", model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("General", "Error", new { message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetCardsByType(string type)
        {
            try
            {
                var list = _masterService.GetCardMasterByType(type).Select(c => new SelectListItem
                {
                    Text = c.CardNo,
                    Value = c.Id.ToString()
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAssigneeByType(string type)
        {
            try
            {
                var list = GetAssigneeList(type);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckDuplicateCardAssignment(CYGNUS_Master_Card_Assignment model)
        {
            try
            {
                DataTable dt = _masterService.CheckDuplicateCardAssignment(model);
                if (dt != null && dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["Status"]))
                {
                    return Json(new { isDuplicate = true, message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { isDuplicate = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { isDuplicate = false }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<SelectListItem> GetAssigneeList(string type)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (type == "VEH")
            {
                var vehicles = _masterService.GetVehicleList("", BaseUserName, "Vehicle_List");
                list = vehicles.Where(v => v.IsActive && v.VehicleCode != "T").Select(v => new SelectListItem { Text = v.VehicleNo, Value = v.ID.ToString() }).ToList();
            }
            else if (type == "EMP")
            {
                var users = _masterService.GetEmployeeList();
                list = users.Select(u => new SelectListItem { Text = u.Name, Value = u.UserId }).ToList();
            }
            else if (type == "DRV")
            {
                var drivers = _masterService.GetDriverMstDetails();
                list = drivers.Where(d => d.ActiveFlag == "Y").Select(d => new SelectListItem { Text = d.Driver_Name, Value = d.Driver_Id.ToString() }).ToList();
            }
            return list;
        }

        [HttpPost]
        public ActionResult AddEditCardAssignment(CYGNUS_Master_Card_AssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Duplicate Check: Same AssignType/AssignTo OR Same CardType/CardNoId
                    DataTable dt = _masterService.CheckDuplicateCardAssignment(model.Assignment);

                    if (dt != null && dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["Status"]))
                    {
                        return Json(new { success = false, message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
                    }

                    string XML = GF.GetXmlString(model.Assignment);
                    bool isSaved = _masterService.AddEditCardAssignment(XML, BaseCompanyCode, BaseUserName);
                    return Json(new { success = isSaved, message = isSaved ? "Saved successfully." : "Failed to save." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, message = "Invalid data submitted." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ToggleActiveCardAssignment(int id)
        {
            bool status = false;
            try
            {
                status = _masterService.ActiveInActiveCardAssignment(id);
            }
            catch (Exception) { }
            return Json(status);
        }

        public JsonResult GetCardAssignmentByVehicle(string vehicleId)
        {
            try
            {
                var list = _masterService.GetCardAssignment()
                    .Where(c => c.AssignPersonId == vehicleId)
                    .Select(c => new
                    {
                        c.Id,
                        c.AssignType,
                        c.AssignPersonId,
                        c.CardType,
                        c.CardNoId,
                        FromDate = c.FromDate.ToString("dd/MM/yyyy"),
                        ToDate = c.ToDate.HasValue ? c.ToDate.Value.ToString("dd/MM/yyyy") : "-",
                        c.IsActive
                    }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GST Details

        [AllowAnonymous]
        public async Task<JsonResult> GetGSTDetails(string GSTNO)
        {
            // 1. Validation of GST Number
            if (string.IsNullOrEmpty(GSTNO) || GSTNO.Trim().Length < 15)
            {
                return Json(new { Status = false, Message = "GST number must be at least 15 characters long." }, JsonRequestBehavior.AllowGet);
            }

            string cleanGST = GSTNO.Trim().ToUpper();
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanGST, @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$"))
            {
                return Json(new { Status = false, Message = "Invalid GST number format." }, JsonRequestBehavior.AllowGet);
            }

            CygnusCustomerGSTDetails result = new CygnusCustomerGSTDetails();
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            string api_key = ConfigurationManager.AppSettings["api_key"];
            string user_id = ConfigurationManager.AppSettings["user_id"];

            // Default API request payload representation
            var requestPayload = new
            {
                Url = "https://services.cygnux.in/api/gst",
                Method = "POST",
                Headers = new[] { "Accept: application/json" },
                Body = new { api_key = api_key, user_id = user_id, gst = cleanGST }
            };

            try
            {
                // Try fetching from database cache first
                CygnusCustomerGSTDetails Dt = _masterService.GetGSTDetailsByGstNumber(cleanGST);
                if (Dt != null)
                {
                    stopwatch.Stop();
                    return Json(new { Status = true, Data = new[] { Dt } }, JsonRequestBehavior.AllowGet);
                }

                // If not in cache, call external API
                string token = await GetFirstTokenAsync();
                string url = "https://services.cygnux.in/api/gst";

                var jsonContent = new
                {
                    api_key = api_key,
                    user_id = user_id,
                    gst = cleanGST
                };

                requestPayload = new
                {
                    Url = url,
                    Method = "POST",
                    Headers = new[] { "Accept: application/json", $"Authorization: Bearer {token}" },
                    Body = jsonContent
                };

                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Content = new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);
                    stopwatch.Stop();
                    long executionTimeMs = stopwatch.ElapsedMilliseconds;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(responseBody);

                        bool isSuccess = (json["response"]?["code"] != null && Convert.ToInt32(json["response"]["code"]) == 1000) ||
                                         (json["status"] != null && Convert.ToInt32(json["status"]) == 1 &&
                                          json["response"]?["error"] != null && Convert.ToBoolean(json["response"]["error"]) == false);

                        if (isSuccess)
                        {
                            var apiResponse = JsonConvert.DeserializeObject<GSTApiResponse>(responseBody);
                            var gst = apiResponse.response.data;

                            result = new CygnusCustomerGSTDetails
                            {
                                gst_registration_no = gst.gstin,
                                statePrefix = gst.pradr?.addr?.stcd,
                                CSGEAddress = $"{gst.pradr?.addr?.bno}, {gst.pradr?.addr?.bnm}, {gst.pradr?.addr?.st}, {gst.pradr?.addr?.loc}",
                                PinCode = gst.pradr?.addr?.pncd,
                                City = gst.pradr?.addr?.loc,
                                Status = gst.sts,
                                PanNo = gst.gstin.Substring(2, 10),
                                TradeName = gst.tradeNam,
                                legalName = gst.lgnm,
                                registrationDate = gst.rgdt,
                                APIResponse = responseBody
                            };

                            string XML = GF.GetXmlString(result);

                            // Build the wrapped log JSON
                            var wrappedLog = new
                            {
                                GstNumber = cleanGST,
                                RequestPayload = requestPayload,
                                ResponsePayload = responseBody,
                                ApiStatus = "Success",
                                ResponseMessage = "GST details fetched and saved successfully.",
                                ErrorDetails = "",
                                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                ExecutionTimeMs = executionTimeMs
                            };
                            string wrappedLogJson = JsonConvert.SerializeObject(wrappedLog);

                            CygnusCustomerGSTDetails dt = _masterService.AddCustomerGSTDetails(XML, gst.gstin, BaseCompanyCode, BaseUserName, wrappedLogJson);
                            return Json(new { Status = true, Data = new[] { dt } }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            // API returned a non-success response code (e.g. invalid GST number or parameter issue)
                            string errorMsg = json["response"]?["message"]?.ToString() ?? "Please check your GST Number";
                            return Json(new { Status = false, Message = errorMsg }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        // HTTP Error status code
                        return Json(new { Status = false, Message = "Failed to fetch GST details. Please try again." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Please contact your administrator team..!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Expense Master
        public ActionResult Expense()
        {
            try
            {
                var list = _masterService.GetExpenseMaster();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
        }

        public ActionResult _AddEditExpense(int id = 0)
        {
            var vm = new CYGNUS_Master_ExpenseViewModel();
            try
            {
                if (id > 0)
                {
                    vm.Expense = _masterService.GetExpenseMasterById(id);
                }

                if (vm.Expense == null)
                {
                    vm.Expense = new CYGNUS_Master_Expense { Id = id, IsActive = true };
                }
            }
            catch (Exception)
            {
                vm.Expense = new CYGNUS_Master_Expense { Id = id, IsActive = true };
            }

            ViewBag.ExpenseTypeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Trip", Text = "Against Trip" },
                new SelectListItem { Value = "LR", Text = "Against LR" }
            };

            return PartialView(vm);
        }

        [HttpPost]
        public ActionResult AddEditExpense(CYGNUS_Master_ExpenseViewModel model)
        {
            try
            {
                if (model == null || model.Expense == null)
                {
                    return Json(new { Status = false, Message = "Expense details cannot be empty." }, JsonRequestBehavior.AllowGet);
                }

                if (model.Expense.BillableToCustomer)
                {
                    if (!model.Expense.Addition && !model.Expense.Deduction)
                    {
                        return Json(new { Status = false, Message = "Please select either Addition or Deduction for the Freight Bill configuration." }, JsonRequestBehavior.AllowGet);
                    }
                }

                string xmlData = GF.GetXmlString(model.Expense);
                bool status = _masterService.AddEditExpenseMaster(xmlData, BaseCompanyCode, BaseUserName);

                return Json(new { Status = status, Message = status ? "Expense saved successfully." : "Failed to save expense." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Failed to save expense. " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public bool ToggleActiveExpense(int id)
        {
            try
            {
                return _masterService.ActiveInActiveExpenseMaster(id, BaseUserName);
            }
            catch
            {
                return false;
            }
        }

        public JsonResult ExpenseListJson()
        {
            try
            {
                var list = _masterService.GetExpenseMaster();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<CYGNUS_Master_Expense>(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Geofence Master
        [HttpGet]
        public ActionResult Geofence()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GeofenceListJson()
        {
            try
            {
                var list = _masterService.GetGeofenceMaster();
                return Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<CYGNUS_Master_Geofence>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AddEditGeofence(int id = 0)
        {
            ViewBag.StateList = _masterService.GetStateMaster();
            ViewBag.DepartmentTypeList = _masterService.GetGeneralMasterWithParam("GEODPRTYP");
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
            ViewBag.AreaTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ok", Value = "Ok" },
                new SelectListItem { Text = "Danger", Value = "Danger" },
                new SelectListItem { Text = "Modrate", Value = "Modrate" }
            };
            CYGNUS_Master_GeofenceViewModel vm = new CYGNUS_Master_GeofenceViewModel();
            if (id > 0)
            {
                vm.Geofence = _masterService.GetGeofenceMasterById(id);
                if (vm.Geofence != null && !vm.Geofence.IsManual)
                {
                    return RedirectToAction("Geofence");
                }
            }
            else
            {
                vm.Geofence = new CYGNUS_Master_Geofence
                {
                    Id = id,
                    IsActive = true,
                    ShapeType = "Circle"
                };
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddEditGeofence(CYGNUS_Master_GeofenceViewModel model)
        {
            try
            {
                if (model == null || model.Geofence == null)
                {
                    return Json(new { Status = false, Message = "Geofence details cannot be empty." }, JsonRequestBehavior.AllowGet);
                }

                // Duplication check on Geofence Name
                var geofenceList = _masterService.GetGeofenceMaster();
                if (geofenceList != null && geofenceList.Count > 0)
                {
                    bool isDuplicate = geofenceList.Any(a => a.GeofenceName != null && a.GeofenceName.Equals(model.Geofence.GeofenceName, StringComparison.OrdinalIgnoreCase) && a.Id != model.Geofence.Id);
                    if (isDuplicate)
                    {
                        return Json(new { Status = false, Message = "Geofence name already exists. Please choose a different name." }, JsonRequestBehavior.AllowGet);
                    }
                }

                // File upload logic
                if (Request.Files != null && Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
                {
                    var file = Request.Files[0];
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;
                    string relativePath = "/Uploads/Geofence/" + fileName;
                    string absolutePath = Server.MapPath("~" + relativePath);

                    string directory = System.IO.Path.GetDirectoryName(absolutePath);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }

                    file.SaveAs(absolutePath);
                    model.Geofence.AttachmentPath = relativePath;
                }
                else if (model.Geofence.Id > 0)
                {
                    // Preserve existing attachment if no new file is uploaded
                    var existing = _masterService.GetGeofenceMasterById(model.Geofence.Id);
                    if (existing != null)
                    {
                        model.Geofence.AttachmentPath = existing.AttachmentPath;
                    }
                }
                model.Geofence.IsManual = true;
                string xmlData = GF.GetXmlString(model.Geofence);
                if (!string.IsNullOrEmpty(xmlData))
                {
                    xmlData = xmlData.Replace("&amp;amp;", "&amp;");
                }
                bool status = _masterService.AddEditGeofenceMaster(xmlData, BaseCompanyCode, BaseUserName);

                return Json(new { Status = status, Message = status ? "Geofence saved successfully." : "Failed to save geofence." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Failed to save geofence. " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CheckDuplicateGeofenceName(string name, int id)
        {
            try
            {
                var geofenceList = _masterService.GetGeofenceMaster();
                bool isDuplicate = false;
                if (geofenceList != null && geofenceList.Count > 0)
                {
                    isDuplicate = geofenceList.Any(a => a.GeofenceName != null && a.GeofenceName.Equals(name, StringComparison.OrdinalIgnoreCase) && a.Id != id);
                }
                if (isDuplicate)
                {
                    return Json("Geofence name already exists. Please choose a different name.", JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public bool ToggleActiveGeofence(int id)
        {
            try
            {
                var geofence = _masterService.GetGeofenceMasterById(id);
                if (geofence != null && !geofence.IsManual)
                {
                    return false;
                }
                return _masterService.ActiveInActiveGeofenceMaster(id, BaseUserName);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Consignee Master

        public ActionResult ConsigneeList()
        {
            CYGNUS_ConsigneeViewModel CCVM = new CYGNUS_ConsigneeViewModel();
            try
            {
                CCVM.listCCM = _masterService.GetConsigneeDetails("").ToList();
                CCVM.CCM = new Cygnus_Consignee_Master();
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(CCVM);
        }

        public ActionResult ConsigneeMaster(string Consignee_code,
             string gst = null, string name = null, string address = null,
             string pincode = null, string returnUrl = null)
        {
            Cygnus_Consignee_Master CL = new Cygnus_Consignee_Master();
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
            try
            {
                if (!string.IsNullOrEmpty(Consignee_code))
                {
                    // Edit mode — load existing record
                    CL = _masterService.GetConsigneeDetails(Consignee_code).FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(gst))
                {
                    // Add mode with pre-fill from EWayBill / Trip Invoice Upload
                    CL.Consignee_GST = gst;
                    CL.Consignee_Name = name ?? string.Empty;
                    CL.Consignee_Address = address ?? string.Empty;
                    CL.Consignee_Pincode = pincode ?? string.Empty;
                }

                ViewBag.ListLane = _masterService.GetLaneDetails().ToList();
                ViewBag.PincodeList = _masterService.GetPincodeMaster().ToList();
                ViewBag.CountryList = _masterService.GetCountryMaster().ToList();
                ViewBag.Liststate = _masterService.GetStateMaster().Where(c => c.activeflag == "Y").ToList();
                ViewBag.CityList = _masterService.GetCityMaster().Where(c => c.activeflag == "Y").OrderBy(c => c.Location).ToList();
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Failed to load consignee details: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.ReturnUrl = returnUrl;

            // AJAX request = opened inside a modal (GLModal.showForm) → return partial view (no layout)
            // Direct browser navigation → return full page with layout
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AddEditConsignee", CL);
            }

            return View(CL);
        }
        [HttpPost]
        public ActionResult AddEditConsigneeMaster(Cygnus_Consignee_Master CCM)
        {
            try
            {
                // Server-side validation for duplicate GST
                if (!string.IsNullOrEmpty(CCM.Consignee_GST))
                {
                    var list = _masterService.GetConsigneeDetails(null);
                    bool isDuplicate = list.Any(c => !string.IsNullOrEmpty(c.Consignee_GST)
                                                     && c.Consignee_GST.Trim().ToUpper() == CCM.Consignee_GST.Trim().ToUpper()
                                                     && c.Id != CCM.Id
                                                     && (string.IsNullOrEmpty(CCM.Consignee_Code) || c.Consignee_Code != CCM.Consignee_Code));
                    if (isDuplicate)
                    {
                        return Json(new { Status = false, Message = $"Consignee GST No. \"{CCM.Consignee_GST}\" already exists!" });
                    }
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(CCM.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, CCM);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);

                    DataTable Dt;
                    if (CCM.Id > 0)
                    {
                        Dt = _masterService.AddEditConsignee(xmlDoc.InnerXml, "U", BaseCompanyCode, BaseUserName);
                    }
                    else
                    {
                        Dt = _masterService.AddEditConsignee(xmlDoc.InnerXml, "I", BaseCompanyCode, BaseUserName);
                    }
                    string ConsigneeCode = "", Status = "", Consignee_Name = "";
                    if (Dt != null && Dt.Rows.Count > 0 && Dt.Rows[0]["Message"].ToString() == "Done")
                    {
                        ConsigneeCode = Dt.Rows[0]["ConsigneeCode"].ToString();
                        Consignee_Name = Dt.Rows[0]["Consignee_Name"].ToString();
                        Status = Dt.Rows[0]["Status"].ToString();
                    }
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new
                        {
                            Status = Status,
                            Message = "Done",
                            ConsigneeCode = ConsigneeCode,
                            Consignee_Name = CCM.Consignee_Name,
                            Consignee_GST = CCM.Consignee_GST,
                            Consignee_Address = CCM.Consignee_Address,
                            Consignee_Pincode = CCM.Consignee_Pincode,
                            Consignee_PANNo = CCM.Consignee_PANNo,
                            Consignee_City = CCM.Consignee_City
                        });
                    }
                    return RedirectToAction("ConsigneeDone", new { Status = Status, Message = "Done", ConsigneeCode = ConsigneeCode, Consignee_Name = Consignee_Name });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Failed to save consignee: " + ex.Message });
            }
        }

        public ActionResult ConsigneeDone(string Status, string ConsigneeCode, string Consignee_Name)
        {
            if (Status == "1")
            {
                ViewBag.ConsigneeCode = ConsigneeCode.ToUpper() + " : " + Consignee_Name.ToUpper();
            }
            ViewBag.Status = Status;
            return View();
        }

        [HttpPost]
        public JsonResult CheckDuplicateGST(string GSTNo, int ExcludeId, string ExcludeCode = null)
        {
            try
            {
                if (string.IsNullOrEmpty(GSTNo))
                {
                    return Json(new { isDuplicate = false });
                }
                var list = _masterService.GetConsigneeDetails(null);
                bool isDuplicate = list.Any(c => !string.IsNullOrEmpty(c.Consignee_GST)
                                                 && c.Consignee_GST.Trim().ToUpper() == GSTNo.Trim().ToUpper()
                                                 && c.Id != ExcludeId
                                                 && (string.IsNullOrEmpty(ExcludeCode) || c.Consignee_Code != ExcludeCode));
                return Json(new { isDuplicate = isDuplicate });
            }
            catch (Exception ex)
            {
                return Json(new { isDuplicate = false, error = ex.Message });
            }
        }
        #endregion

        #region Get Country State City Zone based on Pincode

        public JsonResult Get_Country_State_City_Zone_on_Pincode(string Code, string Type)
        {
            DataSet ds = _masterService.GetCountryStateCityZoneByPincode(Code, Type);

            DataTable dt = ds.Tables[0];

            var data = dt.AsEnumerable()
                         .Select(row => dt.Columns.Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                         .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetZoneByStateJson(string id)
        {
            try
            {
                var states = _masterService.GetStateMaster();
                var state = states.FirstOrDefault(s => s.stcd == id);
                return Json(state != null ? state.Zone : "", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Notes Category Mapping
        public ActionResult NotesCategoryMapping()
        {
            List<CYGNUS_NotesCategory_Mapping> listCNM = new List<CYGNUS_NotesCategory_Mapping>();
            listCNM = _masterService.GetNotesCategoryMappingList().ToList();
            return View(listCNM);
        }

        public JsonResult GetNotesSubCategoryListJsonWithIdName(string str)
        {
            List<CYGNUS_Master_General> ListItems = new List<CYGNUS_Master_General>();
            ListItems = _masterService.GetGeneralMaster().Where(c => c.StatusCode == "Y" && c.CodeType == "NOTESSUBCAT").ToList();

            var SearchList = (from e in ListItems
                              select new
                              {
                                  id = e.CodeId,
                                  text = e.CodeDesc,
                              }).Distinct().ToList();
            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NotesCategoryMappingSubmit(List<CYGNUS_NotesCategory_Mapping> MappingList)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(MappingList.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, MappingList);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                }

                DataTable dt = _masterService.InsertNotesCategoryMappingDetails(xmlDoc.InnerXml, BaseUserName, BaseCompanyCode);

                if (Convert.ToBoolean(dt.Rows[0]["Status"]))
                {
                    return RedirectToAction("NotesCategoryMappingDone", new { id = "DONE" });
                }
                else
                {
                    ViewBag.StrError = "Cannot update records.";
                    return View("Error");
                }
            }
            catch (Exception e1)
            {
                ViewBag.StrError = "Please Contact Your Support Team";
                return View("Error");
            }
        }

        public ActionResult NotesCategoryMappingDone(string id)
        {
            ViewBag.Status = id;
            return View();
        }
        #endregion

        #region Document type master for vehicle
        public ActionResult VehicleDocumentType_List()
        {
            List<Cygnus_Master_Vehicle_DocumentType> CMVDTList = _masterService.GetVehicleDocumentTypeById(0);
            return View(CMVDTList);
        }
        public ActionResult VehicleDocumentType(int id = 0)
        {
            Cygnus_Master_Vehicle_DocumentType CMVDT = new Cygnus_Master_Vehicle_DocumentType();
            if (id > 0)
            {
                CMVDT = _masterService.GetVehicleDocumentTypeById(id).FirstOrDefault() ?? new Cygnus_Master_Vehicle_DocumentType();
            }
            return View(CMVDT);
        }

        [HttpPost]
        public ActionResult VehicleDocumentType_Submit(Cygnus_Master_Vehicle_DocumentType model)
        {
            string Status = "0", Message = "Failed to save.", Document_Name = "";
            int Id = 0;
            try
            {
                string xml = "<VehicleDocumentType>"
                    + "<Id>" + model.Id + "</Id>"
                    + "<Document_Name>" + model.Document_Name + "</Document_Name>"
                    + "<ShortName>" + model.ShortName + "</ShortName>"
                    + "<Is_Date>" + (model.Is_Date ? "1" : "0") + "</Is_Date>"
                    + "<Is_Insurance>" + (model.Is_Insurance ? "1" : "0") + "</Is_Insurance>"
                    + "</VehicleDocumentType>";

                DataTable dt = _masterService.AddEditVehicleDocumentType(xml, BaseUserName,BaseCompanyCode);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["TranXaction"].ToString() == "Done" && dt.Rows[0]["Status"].ToString() == "1")
                {
                    Status = dt.Rows[0]["Status"].ToString();
                    Message = dt.Rows[0]["TranXaction"].ToString();
                    Document_Name = dt.Rows[0]["Document_Name"].ToString();
                    Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                }
               
                return RedirectToAction("VehicleDocumentTypeDone", new { Status = Status, Message = Message, Document_Name = Document_Name , Id = Id });

            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }
            return Json(new { Status, Message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VehicleDocumentTypeDone(string Status, string Document_Name, int Id)
        {
            if (Status == "1")
            {
                ViewBag.Document_Name = Document_Name;
            }
            ViewBag.Status = Status;
            ViewBag.Id = Id;
            return View();
        }

        #endregion
        #region Vendor Master
        public ActionResult VendorMaster(string Id)
        {
            VendorViewModel VVM = new VendorViewModel();
            try
            {
                VVM.EditFlag = "false";
                VVM.VendorHDR = new CYGNUS_VENDOR_HDR();
                VVM.VendorDET = new CYGNUS_VENDOR_DET();
                VVM.ListVendorDoc = new List<Cygnus_Vendor_Document>();
                VVM.VendorDoc = new Cygnus_Vendor_Document();
                //VVM.VendorDoc.Id = 0;
                if (Id != null && Id != "")
                {
                    VVM.EditFlag = "true";
                    VVM.VendorHDR = MS.GetVendorObject().Where(c => c.VENDORCODE == null ? c.VENDORCODE == "" : c.VENDORCODE.Trim().ToUpper() == Id.ToUpper()).FirstOrDefault();
                    VVM.VendorDET = MS.GetVendorDetObject().Where(c => c.VENDORCODE == null ? c.VENDORCODE == "" : c.VENDORCODE.Trim().ToUpper() == Id.ToUpper()).FirstOrDefault();
                    VVM.ListVendorDoc = MS.GetVendorDoc(Id);
                }
                VVM.stateGSTList = DataRowToObject.CreateListFromTable<CygnusVendorGSTDetails>(MS.GetStateWiseGSTDetails("V", VVM.VendorHDR.VENDORCODE));
            }
            catch (Exception ex)
            {
                ViewBag.StrError = "Error " + ex.Message.ToString().Replace('\n', '_');
                return View("Error");
            }
            return View(VVM);
        }
        [HttpPost]
        public ActionResult VendorMasterSubmit(VendorViewModel VM, List<CygnusVendorGSTDetails> ListVendorGSTDetails, List<Cygnus_Vendor_Document> UploadDoc)
        {
            if (UploadDoc != null)
            {

                foreach (var item in UploadDoc)
                {
                    if (Request.Files["files_" + item.Id] != null)
                    {
                        var file = Request.Files["files_" + item.Id];
                        string extension = Path.GetExtension(file.FileName), FolderPath = "";
                        FolderPath = "/UploadedDocuments/VendorDocument/" + BaseLocationCode + "/" + BaseFinYear + "/" + System.DateTime.Now.Month + "/";
                        string path1 = FolderPath + DateTime.Now.ToFileTime() + "_" + item.Id + extension;
                        if (item.ImagePath == null)
                        {
                            item.ImagePath = path1;
                            path1 = Server.MapPath("~/" + path1);
                            string strDirectoryName = Server.MapPath("~/" + FolderPath);
                            if (Directory.Exists(strDirectoryName) == false)
                                Directory.CreateDirectory(strDirectoryName);
                            file.SaveAs(path1);
                        }
                    }
                }
            }
            XmlDocument xmlUploadDoc = new XmlDocument();
            XmlSerializer xmlUploadSerializer = new XmlSerializer(UploadDoc.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlUploadSerializer.Serialize(xmlStream, UploadDoc);
                xmlStream.Position = 0;
                xmlUploadDoc.Load(xmlStream);
            }

            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(VM.VendorHDR.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, VM.VendorHDR);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
            }
            XmlDocument xmlDoc1 = new XmlDocument();
            XmlSerializer xmlSerializer1 = new XmlSerializer(VM.VendorDET.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer1.Serialize(xmlStream, VM.VendorDET);
                xmlStream.Position = 0;
                xmlDoc1.Load(xmlStream);
            }

            XmlDocument xmlDocGST = new XmlDocument();

            ListVendorGSTDetails = ListVendorGSTDetails.Where(m => m.gst_registration_no != null && m.gst_registration_no != "").ToList();

            XmlSerializer xmlSerializerGST = new XmlSerializer(ListVendorGSTDetails.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializerGST.Serialize(xmlStream, ListVendorGSTDetails);
                xmlStream.Position = 0;
                xmlDocGST.Load(xmlStream);
            }


            DataTable DT = MS.InsertVendor(xmlDoc.InnerXml, xmlDoc1.InnerXml, xmlDocGST.InnerXml, xmlUploadDoc.InnerXml, VM.EditFlag, BaseUserName);

            bool Status = false;

            string Statusstr = DT.Rows[0]["Status"].ToString();

            if (Statusstr == "1")
            {
                Status = true;
            }

            string VendorCode = DT.Rows[0]["VendorCode"].ToString();
            string VendorName = DT.Rows[0]["VendorName"].ToString();

            string ReturnStr = Status + "," + VendorCode;
            return RedirectToAction("VendorDone", new { VendorCode = VendorCode, VendorName = VendorName });
        }
        [HttpPost]
        public ActionResult UploadDoc(int Id)
        {
            Cygnus_Vendor_Document CVD = new Cygnus_Vendor_Document();
            CVD.Id = Id;
            return PartialView("_VendorDocumentDetails", CVD);
        }
        public JsonResult GetVendorListJson()
        {
            List<CYGNUS_VENDOR_HDR> GetVendorlist = MS.GetVendor();

            var ListVendorsdata = (from e in GetVendorlist
                                   select new
                                   {
                                       VENDORCODE = e.VENDORCODE,
                                       VENDORNAME = e.VENDORNAME,
                                       Vendor_Type = e.VENDORTYPE,
                                       Active = e.Active,
                                       VENDORCITY = e.VENDORCITY
                                   }).ToArray();
            return Json(ListVendorsdata, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VendorList()
        {
            return View();
        }
        public ActionResult VendorDone(string VendorCode, string VendorName)
        {
            ViewBag.VendorCode = VendorCode.ToUpper();
            ViewBag.VendorName = VendorName.ToUpper();
            return View();
        }

        [HttpPost]
        public ActionResult ToggleActiveVendor(string id)
        {
            bool status = false;
            status = _masterService.ActiveInActiveVendor(id);
            return Json(status);
        }
        #endregion
    }
}
