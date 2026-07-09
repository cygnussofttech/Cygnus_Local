using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using GreenLine.Classes;
using GreenLineDataService.Helper;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using ImageResizer.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;

namespace GreenLine.Controllers
{
    [Authorize]
    public class ContractController : BaseController
    {

        GeneralFunctions GF = new GeneralFunctions();
        private readonly IContractService _contractService;
        private readonly IMasterService _masterService;

        public ContractController() : this(new ContractService(), new MasterService())
        {
        }

        public ContractController(IContractService contractService, IMasterService masterService)
        {
            _contractService = contractService;
            _masterService = masterService;
        }
        // GET: CustomerContract

        #region Customer Contract Page
        public ActionResult CustomerContract()
        {
            CustomerContractViewModel objCustomerContract = new CustomerContractViewModel()
            {
                CustomerContract = new CYGNUS_custcontract_hdr(),
                ListCustContract = new List<CYGNUS_custcontract_hdr>()
            };
            ViewBag.ListContractType = _masterService.GetGeneralMasterWithParam("PAYTYP");
            ViewBag.CustomerList = _masterService.GetCustomerList();
            objCustomerContract.CustomerContract.Contract_Type = "P02";
            objCustomerContract.CustomerContract.Contract_Type1 = "P02";
            return View(objCustomerContract);
        }

        public string GetCustomerName(string custcode, string ContId)
        {
            string CustomerName = "";
            DataTable dt = _contractService.GetCustContName(custcode, ContId);
            if (dt.Rows.Count > 0)
            {
                CustomerName = "<b> Customer Code & Name :</b> " + custcode + ": " + dt.Rows[0]["Name"].ToString();
            }
            return CustomerName;
        }

        [HttpPost]
        public JsonResult GetCustomerContractListJson(CustomerContractViewModel model)
        {
            // Example: adapt to your service method and property names
            var list = _contractService.GetCustContList(model.CustomerContract.Custcode, model.CustomerContract.Contract_Type);

            var result = list.Select(x => new
            {
                Id = x.Srno,
                ContractID = x.ContractId,
                StartDate = x.Contract_Stdate?.ToString("dd MMM yyyy"),
                EndDate = x.Contract_Eddate?.ToString("dd MMM yyyy"),
                ContractType = x.Contract_Type,
                IsActive = x.Status,
                CustCode = x.Custcode
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContractActiveInActive(string SrNo)
        {
            DataTable dt = _contractService.CustomerChnageStatus(SrNo, BaseUserName);
            return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddContarct(string PaymentBasis, string StartDate, string EndDate, string CustCode)
        {
            try
            {
                List<CYGNUS_custcontract_hdr> CusthdrList = new List<CYGNUS_custcontract_hdr>();
                string contractID = _contractService.getCodeContractID();
                DataTable dt = _contractService.AddNewContract(CustCode, PaymentBasis, StartDate, EndDate, contractID, BaseUserName, BaseCompanyCode);
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Customer Contract Edit Page
        public ActionResult UpdateContract(string CustCode, string ContId, string ContType, string Mode)
        {
            CustomerContractViewModel objCustomerContract = new CustomerContractViewModel();
            DataTable dt = _contractService.GetCustomerContractDetails(ContId, "BasicInfo");
            if (dt != null && dt.Rows.Count > 0)
            {
                objCustomerContract.CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }

            DataTable dtservice = _contractService.GetServiceType(ContId);
            if (dtservice.Rows.Count > 0)
            {
                ViewBag.ServiceType = dtservice.Rows[0]["service_type"].ToString();
            }

            //objCustomerContract.billConfiguration.Item_Description = "Freight for Transportation of Coil ";

            DataTable dt1 = _contractService.GetCustContName(CustCode, ContId);
            ViewBag.OrgList = _masterService.GetComapanyDetails().ToList();
            ViewBag.ListLocations = _masterService.GetLocationDetails().ToList();
            ViewBag.contractid = dt1.Rows[0]["contractid"];
            ViewBag.cutscode = dt1.Rows[0]["custcode"].ToString() + " : " + dt1.Rows[0]["name"].ToString();
            ViewBag.type = dt1.Rows[0]["contracttype"].ToString();
            ViewBag.Mode = Mode;

            return View(objCustomerContract);
        }

        #endregion

        #region Contract Basic Information

        [HttpPost]
        public JsonResult AddContractbasicInfo(CYGNUS_custcontract_hdr Custcontract)
        {
            try
            {
                string contractid = Custcontract.ContractId;
                XmlDocument xmlBasicInfo = new XmlDocument();
                if (Custcontract != null)
                {
                    XmlSerializer xmlBasicInfoSerializer = new XmlSerializer(Custcontract.GetType());
                    using (MemoryStream xmlStream = new MemoryStream())
                    {
                        xmlBasicInfoSerializer.Serialize(xmlStream, Custcontract);
                        xmlStream.Position = 0;
                        xmlBasicInfo.Load(xmlStream);
                    }
                }

                DataTable Dt = _contractService.UpdateContractBasicInfo(
                    contractid,
                    BaseUserName,
                    BaseCompanyCode,
                    xmlBasicInfo.InnerXml.ReplaceSpecialCharacters()
                    );
                return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ContractBasicInfo(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
            CYGNUS_custcontract_hdr CustomerContract = new CYGNUS_custcontract_hdr();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }
            ViewBag.OrgList = _masterService.GetComapanyDetails().ToList();
            ViewBag.ListLocations = _masterService.GetLocationDetails().ToList();

            return PartialView("_ContractBasicInformation", CustomerContract);
        }

        #endregion

        #region Documents

        public ActionResult ContractDocuments(string ContractId)
        {
            ContractDocumentsViewModel Documents = new ContractDocumentsViewModel();
            Documents.ContractId = ContractId;
            Documents.Documents = new List<DocumentModel>();
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "ContractDocs");
            if (dt != null && dt.Rows.Count > 0)
            {
                Documents.ExistingDocuments = DataRowToObject.CreateListFromTable<DocumentModel>(dt);
            }
            ViewBag.docType = _masterService.GetGeneralMasterWithParam("CONTDOCS");
            return PartialView("_Documents", Documents);
        }
        [HttpPost]
        public JsonResult AddContractDocuments(ContractDocumentsViewModel Custcontract)
        {
            try
            {
                string contractid = Custcontract.ContractId;

                XmlDocument xmlDocumentInfo = new XmlDocument();
                string xmlDocString = "<ArrayOfDocument>";
                if (Custcontract.Documents != null && Custcontract.Documents.Count > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/Contract/BasicDocuments");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    foreach (var doc in Custcontract.Documents)
                    {
                        string fileName = "";
                        if (doc.fileUpload != null && doc.fileUpload.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(doc.fileUpload.FileName);
                            string fullPath = Path.Combine(folderPath, fileName);
                            doc.fileUpload.SaveAs(fullPath);
                        }
                        else
                        {
                            fileName = !string.IsNullOrEmpty(doc.Doc_File)
                                ? Path.GetFileName(doc.Doc_File)
                                : "";
                        }

                        if (!string.IsNullOrEmpty(doc.Doc_Type) && !string.IsNullOrEmpty(fileName))
                        {
                            xmlDocString += "<Document>";
                            xmlDocString += "<Doc_Type>" + doc.Doc_Type + "</Doc_Type>";
                            xmlDocString += "<Doc_File>" + fileName + "</Doc_File>";
                            xmlDocString += "</Document>";
                        }
                    }
                }
                xmlDocString += "</ArrayOfDocument>";
                xmlDocumentInfo.LoadXml(xmlDocString);

                DataTable Dt = _contractService.UpdateContractDocuments(
                    contractid,
                    BaseUserName,
                    BaseCompanyCode,
                    xmlDocumentInfo.InnerXml.ReplaceSpecialCharacters()
                    );
                return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteContractDocument(int docId)
        {
            try
            {
                DataTable dt = _contractService.DeleteContractDocument(docId, BaseUserName);
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region    Additional Field Configurator
        public ActionResult AdditionalFieldConfig(string ContractId)
        {
            List<CYGNUS_Additonal_Field_Configurator> addFieldConfigList = new List<CYGNUS_Additonal_Field_Configurator>();
            ViewBag.ContractId = ContractId;
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "AdditionalField");

            // Initialize exactly 10 slots for udf1 to udf10
            for (int i = 0; i < 10; i++)
            {
                addFieldConfigList.Add(new CYGNUS_Additonal_Field_Configurator()
                {
                    Id = 0,
                    ContractId = ContractId,
                    Name = "udf" + (i + 1)
                });
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                List<CYGNUS_Additonal_Field_Configurator> dbList = DataRowToObject.CreateListFromTable<CYGNUS_Additonal_Field_Configurator>(dt);
                foreach (var dbItem in dbList)
                {
                    if (!string.IsNullOrEmpty(dbItem.Name))
                    {
                        string numPart = dbItem.Name.ToLower().Replace("udf", "");
                        int index;
                        if (int.TryParse(numPart, out index) && index >= 1 && index <= 10)
                        {
                            addFieldConfigList[index - 1] = dbItem;
                        }
                    }
                }
            }

            return PartialView("_AdditionalFieldConfigurator", addFieldConfigList);
        }

        [HttpPost]
        public JsonResult AddContractAdditionalFieldConfigurartor(List<CYGNUS_Additonal_Field_Configurator> addFieldConfigList)
        {
            try
            {
                string contractid = addFieldConfigList.FirstOrDefault()?.ContractId;

                XmlDocument xmlAdditionFieldInfo = new XmlDocument();
                if (addFieldConfigList != null)
                {
                    var populatedFields = addFieldConfigList.Where(x => !string.IsNullOrEmpty(x.Label)).ToList();
                    for (int i = 0; i < populatedFields.Count; i++)
                    {
                        populatedFields[i].Name = "udf" + (i + 1);
                    }
                    addFieldConfigList = populatedFields;

                    XmlSerializer xmlAdditionFieldInfoSerializer = new XmlSerializer(addFieldConfigList.GetType());
                    using (MemoryStream xmlStream = new MemoryStream())
                    {
                        xmlAdditionFieldInfoSerializer.Serialize(xmlStream, addFieldConfigList);
                        xmlStream.Position = 0;
                        xmlAdditionFieldInfo.Load(xmlStream);
                    }
                }
                DataTable Dt = _contractService.UpdateContractAdditinalFieldConfigurator(
                    contractid,
                    BaseUserName,
                    BaseCompanyCode,
                    xmlAdditionFieldInfo.InnerXml.ReplaceSpecialCharacters()
                    );
                return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Service Selection

        public ActionResult ServiceSelection(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "ServiceSelection");
            CYGNUS_custcontract_charge CustomerContractCharge = new CYGNUS_custcontract_charge();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContractCharge = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_charge>(dt.Rows[0]);
            }
            else
            {
                DataTable dt1 = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
                CustomerContractCharge.Contract_Stdate = Convert.ToDateTime(dt1.Rows[0]["Contract_Stdate"]);
            }
            ViewBag.trans_type = _masterService.GetGeneralMasterWithParam("TRN");
            ViewBag.objMasterMatrixList = _masterService.GetGeneralMasterWithParam("MXTYPE").ToList();
            ViewBag.FUELTY = _masterService.GetGeneralMasterWithParam("FUELTY");
            ViewBag.SVCTYPE = _masterService.GetGeneralMasterWithParam("SVCTYP").ToList();
            ViewBag.LRTYPE = _masterService.GetGeneralMasterWithParam("LRTYPE").ToList();
            //ViewBag.freeStorageDays = _masterService.GetGeneralMasterWithParam("FREEDAYS");
            //ViewBag.demarrageRate = _masterService.GetGeneralMasterWithParam("SCHG14");
            return PartialView("_ServiceSelection", CustomerContractCharge);
        }
        [HttpPost]
        public ActionResult AddServiceSelection(CYGNUS_custcontract_charge CustomerCharge)
        {
            try
            {
                string contractid = CustomerCharge.ContractId;
                string custcode = CustomerCharge.Custcode;

                XmlDocument xmlServiceSelectionInfo = new XmlDocument();
                if (CustomerCharge != null)
                {
                    XmlSerializer xmlServiceSelectionInfoSerializer = new XmlSerializer(CustomerCharge.GetType());
                    using (MemoryStream xmlStream = new MemoryStream())
                    {
                        xmlServiceSelectionInfoSerializer.Serialize(xmlStream, CustomerCharge);
                        xmlStream.Position = 0;
                        xmlServiceSelectionInfo.Load(xmlStream);
                    }
                }
                DataTable Dt = _contractService.AddContractService(contractid, custcode, BaseUserName, BaseCompanyCode, xmlServiceSelectionInfo.InnerXml.ReplaceSpecialCharacters());
                return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString(), ServiceType = CustomerCharge.service_type }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.Message.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region Mode Vice Service

        public ActionResult ModeWiseSelection(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "ModeWise");
            CYGNUS_custcontract_servicecharges CustomerContractServiceCharge = new CYGNUS_custcontract_servicecharges();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContractServiceCharge = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_servicecharges>(dt.Rows[0]);
            }
            ViewBag.trans_type = _masterService.GetGeneralMasterWithParam("TRN");
            return PartialView("_ModeViceService", CustomerContractServiceCharge);
        }
        [HttpPost]
        public ActionResult AddModeViceService(CYGNUS_custcontract_servicecharges Custcontract)
        {

            string contractid = Custcontract.contractid;
            string custcode = Custcontract.custcode;

            Custcontract.stax_paidby_enabled = (Custcontract.staxpaidbyenabled == true ? "Y" : "N");

            XmlDocument xmlModeWiseSelectionInfo = new XmlDocument();
            if (Custcontract != null)
            {
                XmlSerializer xmlModeWiseSelectionInfoSerializer = new XmlSerializer(Custcontract.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlModeWiseSelectionInfoSerializer.Serialize(xmlStream, Custcontract);
                    xmlStream.Position = 0;
                    xmlModeWiseSelectionInfo.Load(xmlStream);
                }
            }
            DataTable Dt = _contractService.AddContractModeViceService(contractid, custcode, BaseUserName, BaseCompanyCode, xmlModeWiseSelectionInfo.InnerXml.ReplaceSpecialCharacters());
            return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Freight PTL Charge
        public ActionResult FreightChargePTL(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
            CYGNUS_custcontract_hdr CustomerContract = new CYGNUS_custcontract_hdr();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }
            ContarctPTLViewModel CustomerContractPTLCharge = new ContarctPTLViewModel();
            CYGNUS_CustContract_FRTMatrix_SingleSlab data = new CYGNUS_CustContract_FRTMatrix_SingleSlab();
            List<CYGNUS_CustContract_FRTMatrix_SingleSlab> datalist = new List<CYGNUS_CustContract_FRTMatrix_SingleSlab>();
            data.ContractId = CustomerContract.ContractId;
            data.Custcode = CustomerContract.Custcode;
            data.EffectiveTo = CustomerContract.Contract_Eddate;
            data.Contract_Stdate = CustomerContract.Contract_Stdate;
            data.chargecode = "1";
            data.FilterTrnsMode = "2";
            data.FilterMatrixtype = "L";
            data.chargetype = "BKG";
            CustomerContractPTLCharge.CustsingleSlab = data;
            CustomerContractPTLCharge.ListsingleSlab = datalist;

            ViewBag.objMasterList = _masterService.GetGeneralMasterWithParam("TRN").ToList();
            ViewBag.objMasterMatrixList = _masterService.GetGeneralMasterWithParam("MXTYPE").ToList();
            var rateTypes = CustomerContract.rate_type.Split(',');
            ViewBag.objRateList = _masterService.GetGeneralMasterWithParam("RATETYP").Where(m => rateTypes.Contains(m.CodeId)).ToList();
            return PartialView("_FreightCharge", CustomerContractPTLCharge);
        }

       
        public ActionResult GetfreightCharge(ContarctPTLViewModel custcontract)
        {
            CYGNUS_CustContract_FRTMatrix_SingleSlab ObjSlab = new CYGNUS_CustContract_FRTMatrix_SingleSlab();
            ObjSlab.chargecode = "1";
            ObjSlab.chargetype = "BKG";
            ObjSlab.ContractId = custcontract.CustsingleSlab.ContractId;
            ObjSlab.Custcode = custcontract.CustsingleSlab.Custcode;
            ObjSlab.FilterTrnsMode = custcontract.CustsingleSlab.FilterTrnsMode;
            ObjSlab.FilterMatrixtype = custcontract.CustsingleSlab.FilterMatrixtype;

            List<CYGNUS_CustContract_FRTMatrix_SingleSlab> ListSingleSlab = new List<CYGNUS_CustContract_FRTMatrix_SingleSlab>();

            ListSingleSlab = _contractService.GetSinlgeslab(
                ObjSlab.ContractId,
                ObjSlab.chargetype,
                ObjSlab.chargecode,
                custcontract.CustsingleSlab.FilterMatrixtype,
                custcontract.CustsingleSlab.FilterTrnsMode,
                custcontract.CustsingleSlab.ratetype
                );
            int Id = 0;
            foreach (var item in ListSingleSlab)
            {
                Id++;
                item.Id = Id;
                item.rate = item.rate;
                item.trdays = item.trdays;
                item.FromLane = item.FromLane;
                item.ToLane = item.ToLane;
                item.LaneCode = item.LaneCode;
                item.ProductGroup = item.ProductGroup;
                item.UOM = item.UOM;
                item.BillLoc = item.BillLoc;
                item.FromQty = item.FromQty;
                item.ToQty = item.ToQty;
                item.MapDistance = item.MapDistance;
                item.ContractualDistance = item.ContractualDistance;
                item.ChargeableOn = item.ChargeableOn;
                item.MinimumGuarantee = item.MinimumGuarantee;
                item.srno = item.srno;
                item.Custcode = ObjSlab.Custcode;
                item.chargecode = "1";
                item.chargetype = "BKG";
                item.ContractId = ObjSlab.ContractId;
                item.FilterTrnsMode = ObjSlab.FilterTrnsMode;
                item.FilterMatrixtype = ObjSlab.FilterMatrixtype;
                item.IsDatePicker = false;
            }

            if (ListSingleSlab.Count == 0)
            {
                ObjSlab.Id = 1;
                ObjSlab.rate = 0.00M;
                ObjSlab.FromLane = custcontract.CustsingleSlab.FromLane;
                ObjSlab.ToLane = custcontract.CustsingleSlab.ToLane;
                ObjSlab.LaneCode = custcontract.CustsingleSlab.LaneCode;
                ObjSlab.ProductGroup = custcontract.CustsingleSlab.ProductGroup;
                ObjSlab.UOM = custcontract.CustsingleSlab.UOM;
                ObjSlab.FromQty = 0;
                ObjSlab.ToQty = 10;
                ObjSlab.MapDistance = custcontract.CustsingleSlab.MapDistance;
                ObjSlab.ContractualDistance = custcontract.CustsingleSlab.ContractualDistance;
                ObjSlab.ChargeableOn = custcontract.CustsingleSlab.ChargeableOn;
                ObjSlab.MinimumGuarantee = custcontract.CustsingleSlab.MinimumGuarantee;
                ObjSlab.srno = custcontract.CustsingleSlab.srno;
                ObjSlab.Custcode = ObjSlab.Custcode;
                ObjSlab.EffectiveFrom = custcontract.CustsingleSlab.EffectiveFrom;
                ObjSlab.EffectiveTo = custcontract.CustsingleSlab.EffectiveTo;
                ObjSlab.IsDatePicker = true;
                ObjSlab.IsEffective = 1;
                ListSingleSlab.Add(ObjSlab);
            }
            else
            {
                ObjSlab.Attachment = ListSingleSlab[0].Attachment;
            }
            custcontract.ListsingleSlab = ListSingleSlab;
            custcontract.CustsingleSlab = ObjSlab;
            
            FreightChargeDropDown(custcontract.CustsingleSlab.ContractId);
            return PartialView("_FreightCharge", custcontract);
        }

        public ActionResult ADDfreightMatrix(int id, string TrnsMode, string chargecode, string Matrixtype, string ContractId, string custcode, string endDate, string startDate)
        {
            CYGNUS_CustContract_FRTMatrix_SingleSlab ObjMatrix = new CYGNUS_CustContract_FRTMatrix_SingleSlab
            {
                FilterMatrixtype = Matrixtype,
                FilterTrnsMode = TrnsMode,
                chargecode = chargecode,
                Id = id,
                ContractId = ContractId,
                Custcode = custcode,
                EffectiveTo = Convert.ToDateTime(endDate),
                EffectiveFrom = Convert.ToDateTime(startDate),
                IsDatePicker = true,
                IsEffective = 1
            };
            return PartialView("_PartialFreightCharge", ObjMatrix);
        }

        public ActionResult AddfreightCharge(ContarctPTLViewModel custcontract, List<CYGNUS_CustContract_FRTMatrix_SingleSlab> FreightCharge, HttpPostedFileBase fileUpload, string SaveType)
        {
            try
            {
                string xmlFRIGHTCCM = "";
                string ContractId = custcontract.CustsingleSlab.ContractId;
                string Custcode = custcontract.CustsingleSlab.Custcode;

                string relativePath = "";
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(fileUpload.FileName);
                    string folderPath = Server.MapPath("~/Uploads/Contract/PTLCharges");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string physicalPath = Path.Combine(folderPath, uniqueFileName);
                    fileUpload.SaveAs(physicalPath);
                    relativePath = uniqueFileName;
                }
                else
                {
                    relativePath = !string.IsNullOrEmpty(custcontract.CustsingleSlab.Attachment)
                        ? Path.GetFileName(custcontract.CustsingleSlab.Attachment)
                        : "";
                }

                custcontract.CustsingleSlab.Attachment = relativePath;

                if (SaveType == "multiple")
                {
                    FreightCharge = FreightCharge.Where(m => m.srno == 0).ToList();
                }

                xmlFRIGHTCCM = "<ArrayOfCCMChargeMatrix>";
                foreach (var item in FreightCharge)
                {
                    item.ratetype = custcontract.CustsingleSlab.ratetype;
                    item.Attachment = custcontract.CustsingleSlab.Attachment;

                    xmlFRIGHTCCM = xmlFRIGHTCCM + "<CCMChargeMatrix>" +
                        "<contractid>" + ContractId + "</contractid>" +
                        "<custcode>" + Custcode + "</custcode>" +
                        "<chargetype>" + "BKG" + "</chargetype>" +
                        "<chargecode>" + "1" + "</chargecode>" +
                        "<srno>" + item.srno + "</srno>" +
                        "<rate>" + item.rate + "</rate>" +
                        "<ratetype>" + item.ratetype + "</ratetype>" +
                        "<trdays>" + item.trdays + "</trdays>" +
                        "<trans_type>" + item.FilterTrnsMode + "</trans_type>" +
                        "<BillLoc>" + item.BillLoc + "</BillLoc>" +
                        "<EffectiveFrom>" + item.EffectiveFrom.ToString("dd MMM yyyy") + "</EffectiveFrom>" +
                        "<EffectiveTo>" + item.EffectiveTo.Value.ToString("dd MMM yyyy") + "</EffectiveTo>" +
                        "<ProductGroup>" + item.ProductGroup + "</ProductGroup>" +
                        "<UOM>" + item.UOM + "</UOM>" +
                        "<FromLane>" + item.FromLane + "</FromLane>" +
                        "<ToLane>" + item.ToLane + "</ToLane>" +
                        "<LaneCode>" + item.LaneCode + "</LaneCode>" +
                        "<FromQty>" + item.FromQty + "</FromQty>" +
                        "<ToQty>" + item.ToQty + "</ToQty>" +
                        "<MapDistance>" + item.MapDistance + "</MapDistance>" +
                        "<ContractualDistance>" + item.ContractualDistance + "</ContractualDistance>" +
                        "<ChargeableOn>" + item.ChargeableOn + "</ChargeableOn>" +
                        "<MinimumGuarantee>" + item.MinimumGuarantee + "</MinimumGuarantee>" +
                        "<Attachment>" + item.Attachment + "</Attachment>" +
                        "<Matrixtype>" + item.FilterMatrixtype + "</Matrixtype>" +
                        "<EstVolume>" + item.EstVolume.Value + "</EstVolume>" +
                        "</CCMChargeMatrix>";
                }
                xmlFRIGHTCCM = xmlFRIGHTCCM + "</ArrayOfCCMChargeMatrix>";
                DataTable Dt = _contractService.AddFreightChargeChargeMatrix(xmlFRIGHTCCM.Trim(), ContractId, BaseUserName, BaseCompanyCode, custcontract.CustsingleSlab.ratetype);
                return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
                //return PartialView("_PartialBlank", custcontract);
            }
            catch (Exception ex)
            {
                ViewBag.StrError = ex.Message;
                return View("Error");
            }
        }

        public void GetExcelDataFreightChargeList(string contractID)
        {
            string FileName = "Freight Charge List";
            DataTable dt = _contractService.GetExcelDataFreightCharge(contractID);
            ExportUtilities.ExportToCSV(dt, FileName);
        }

        [HttpPost]
        public ActionResult GetfreightChargeForPTLUpdate(ContarctPTLViewModel custcontract)
        {
            List<CYGNUS_CustContract_FRTMatrix_SingleSlab> ListSingleSlab = new List<CYGNUS_CustContract_FRTMatrix_SingleSlab>();

            List<RateUpdateModelForFTLPTL> ListForUpdateSlab = new List<RateUpdateModelForFTLPTL>();

            ListSingleSlab = _contractService.GetSinlgeslab(
                custcontract.CustsingleSlab.ContractId,
                custcontract.CustsingleSlab.chargetype,
                custcontract.CustsingleSlab.chargecode,
                custcontract.CustsingleSlab.FilterMatrixtype,
                custcontract.CustsingleSlab.FilterTrnsMode,
                custcontract.CustsingleSlab.ratetype
                );

            foreach (var item in ListSingleSlab)
            {
                RateUpdateModelForFTLPTL objForUpdateSlab = new RateUpdateModelForFTLPTL();
                objForUpdateSlab.srno = item.srno;
                objForUpdateSlab.FromLane = item.FromLane;
                objForUpdateSlab.ToLane = item.ToLane;
                objForUpdateSlab.ProductGroup = item.ProductGroup;
                objForUpdateSlab.EffectiveFrom = item.EffectiveFrom;
                objForUpdateSlab.EffectiveTo = item.EffectiveTo;
                objForUpdateSlab.FromQty = item.FromQty;
                objForUpdateSlab.ToQty = item.ToQty;
                objForUpdateSlab.old_rate = item.rate;
                objForUpdateSlab.IsEffective = item.IsEffective;
                //objForUpdateSlab.old_rate = item.old_rate;
                //objForUpdateSlab.rate_differnce = item.rate_differnce;
                ListForUpdateSlab.Add(objForUpdateSlab);
            }
            ViewBag.Custcode = custcontract.CustsingleSlab.Custcode;
            return PartialView("_UpdateFreightCharge", ListForUpdateSlab);
        }

        [HttpPost]
        public JsonResult UpdatePTLRateChange(string ContractId, List<RateUpdateModelForFTLPTL> RateUpdates)
        {
            try
            {
                if (RateUpdates == null || RateUpdates.Count == 0)
                {
                    return Json(new { Status = false, Message = "Please select at least one record to update." }, JsonRequestBehavior.AllowGet);
                }

                string xmlData = "<ArrayOfRateUpdate>";
                foreach (var item in RateUpdates)
                {
                    xmlData += "<RateUpdate>" +
                               "<Srno>" + item.srno + "</Srno>" +
                               "<Rate>" + item.rate + "</Rate>" +
                               "<OldRate>" + item.old_rate + "</OldRate>" +
                               "<RateDiff>" + item.rate_differnce + "</RateDiff>" +
                               "<EffectiveFrom>" + item.EffectiveFrom.ToString("dd MMM yyyy") + "</EffectiveFrom>" +
                               "<EffectiveTo>" + item.EffectiveTo?.ToString("dd MMM yyyy") + "</EffectiveTo>" +
                               "</RateUpdate>";
                }
                xmlData += "</ArrayOfRateUpdate>";

                DataTable dt = _contractService.UpdatePTLRateChange(xmlData, ContractId, BaseUserName.ToUpper(), BaseCompanyCode.ToUpper());
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteFreightChargeRow(int srno, string contractId)
        {
            try
            {

                DataTable dt = _contractService.DeleteFreightCharges(srno, contractId, BaseUserName.ToUpper());
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region Sundry FTL Charge
        public ActionResult FreightChargeFTL(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
            CYGNUS_custcontract_hdr CustomerContract = new CYGNUS_custcontract_hdr();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }
            ContarctFTLViewModel CustomerContractFTLCharge = new ContarctFTLViewModel();
            CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR data = new CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR();
            List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> datalist = new List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR>();
            data.ContractId = CustomerContract.ContractId;
            data.Custcode = CustomerContract.Custcode;
            data.EffectiveTo = CustomerContract.Contract_Eddate;
            data.chargecode = "2";
            data.trans_type = "2";
            data.Matrixtype = "L";
            data.chargetype = "BKG";
            data.Contract_Stdate = CustomerContract.Contract_Stdate;
            CustomerContractFTLCharge.FTLfreightCharge = data;
            CustomerContractFTLCharge.FTLfreightChargeList = datalist;

            ViewBag.objMasterList = _masterService.GetGeneralMasterWithParam("TRN").ToList();
            ViewBag.objMasterMatrixList = _masterService.GetGeneralMasterWithParam("MXTYPE").ToList();

            var rateTypes = CustomerContract.rate_type.Split(',');
            ViewBag.objRateList = _masterService.GetGeneralMasterWithParam("RATETYP").Where(m => rateTypes.Contains(m.CodeId)).ToList();

            ViewBag.FtlTypList = _masterService.GetGeneralMasterWithParam("FTLTYP").ToList();
            // Fetch Customer Wise Bill Details using CustCode
            var dtBill = _masterService.Get_Customerwise_BillDetails(CustomerContract.Custcode);
            ViewBag.stateDetails = dtBill != null
                ? DataRowToObject.CreateListFromTable<CYGNUS_Customer_Bill_Details>(dtBill)
                : new List<CYGNUS_Customer_Bill_Details>();
            return PartialView("_SundryChargeFTL", CustomerContractFTLCharge);
        }
        public ActionResult GetSundryFTL(ContarctFTLViewModel custcontract)
        {
            CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR ObjSlab = new CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR();

            ObjSlab.chargetype = "BKG";
            ObjSlab.chargecode = "2";
            ObjSlab.trans_type = custcontract.FTLfreightCharge.trans_type;
            ObjSlab.Matrixtype = custcontract.FTLfreightCharge.Matrixtype;
            ObjSlab.ftltype = custcontract.FTLfreightCharge.ftltype;
            ObjSlab.ftl1_trip_ratetype = custcontract.FTLfreightCharge.ftl1_trip_ratetype;
            ObjSlab.ContractId = custcontract.FTLfreightCharge.ContractId;
            ObjSlab.Custcode = custcontract.FTLfreightCharge.Custcode;


            List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> ListFRTMatrixSingleSlab = new List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR>();
            ListFRTMatrixSingleSlab = _contractService.ListSundryFTL(
                ObjSlab.chargecode,
                ObjSlab.trans_type,
                ObjSlab.Matrixtype,
                 ObjSlab.ftltype,
                 ObjSlab.ftl1_trip_ratetype,
                 ObjSlab.chargetype,
                 ObjSlab.ContractId
                );
            if (ListFRTMatrixSingleSlab.Count == 0)
            {
                ObjSlab.Id = 1;
                ObjSlab.FTL1_TRIP_RATE = 0.00M;
                ObjSlab.FromLane = custcontract.FTLfreightCharge.FromLane;
                ObjSlab.ToLane = custcontract.FTLfreightCharge.ToLane;
                ObjSlab.LaneCode = custcontract.FTLfreightCharge.LaneCode;
                ObjSlab.ProductGroup = custcontract.FTLfreightCharge.ProductGroup;
                ObjSlab.UOM = custcontract.FTLfreightCharge.UOM;
                ObjSlab.FromQty = 0;
                ObjSlab.ToQty = 10;
                ObjSlab.MapDistance = custcontract.FTLfreightCharge.MapDistance;
                ObjSlab.ContractualDistance = custcontract.FTLfreightCharge.ContractualDistance;
                ObjSlab.ChargeableOn = custcontract.FTLfreightCharge.ChargeableOn;
                ObjSlab.MinimumGuarantee = custcontract.FTLfreightCharge.MinimumGuarantee;
                ObjSlab.srno = custcontract.FTLfreightCharge.srno;
                ObjSlab.EffectiveFrom = custcontract.FTLfreightCharge.EffectiveFrom;
                ObjSlab.EffectiveTo = custcontract.FTLfreightCharge.EffectiveTo;
                ObjSlab.IsDatePicker = true;
                ObjSlab.IsEffective = 1;
                ListFRTMatrixSingleSlab.Add(ObjSlab);
            }
            else
            {
                int Id = 0;
                foreach (var item in ListFRTMatrixSingleSlab)
                {
                    Id++;
                    item.Id = Id;
                    item.FTL1_TRIP_RATE = item.FTL1_TRIP_RATE;
                    item.FTL1_TRDAYS = item.FTL1_TRDAYS;
                    item.FromLane = item.FromLane;
                    item.ToLane = item.ToLane;
                    item.LaneCode = item.LaneCode;
                    item.ProductGroup = item.ProductGroup;
                    item.UOM = item.UOM;
                    item.BillingState = item.BillingState;
                    item.FromQty = item.FromQty;
                    item.ToQty = item.ToQty;
                    item.MapDistance = item.MapDistance;
                    item.ContractualDistance = item.ContractualDistance;
                    item.ChargeableOn = item.ChargeableOn;
                    item.MinimumGuarantee = item.MinimumGuarantee;
                    item.srno = item.srno;
                    item.Custcode = ObjSlab.Custcode;
                    item.ContractId = ObjSlab.ContractId;
                    item.chargetype = ObjSlab.chargetype;
                    item.chargecode = ObjSlab.chargecode;
                    item.trans_type = ObjSlab.trans_type;
                    item.Matrixtype = ObjSlab.Matrixtype;
                    item.IsDatePicker = false;
                }
                ObjSlab.Attachment = ListFRTMatrixSingleSlab[0].Attachment;
            }
            custcontract.FTLfreightCharge = ObjSlab;
            custcontract.FTLfreightChargeList = ListFRTMatrixSingleSlab;
            FreightChargeDropDown(custcontract.FTLfreightCharge.ContractId);
            ViewBag.FtlTypList = _masterService.GetGeneralMasterWithParam("FTLTYP").ToList();
            return PartialView("_SundryChargeFTL", custcontract);
        }

        public void FreightChargeDropDown(string contractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(contractId, "BasicInfo");
            CYGNUS_custcontract_hdr CustomerContract = new CYGNUS_custcontract_hdr();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }
            ViewBag.objMasterList = _masterService.GetGeneralMasterWithParam("TRN").ToList();
            ViewBag.objMasterMatrixList = _masterService.GetGeneralMasterWithParam("MXTYPE").ToList();
            var rateTypes = CustomerContract.rate_type.Split(',');
            ViewBag.objRateList = _masterService.GetGeneralMasterWithParam("RATETYP").Where(m => rateTypes.Contains(m.CodeId)).ToList();
        }


        public ActionResult ADDFTL(int id, string FTL, string ratetype, string ContractID, string custcode, string endDate,string startDate)
        {
            CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR ObjMatrix = new CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR
            {
                Id = id,
                chargecode = "2",
                chargetype = "BKG",
                trans_type = "2",
                Matrixtype = "L",
                ftltype = FTL,
                ftl1_trip_ratetype = ratetype,
                ContractId = ContractID,
                Custcode = custcode,
                EffectiveTo = Convert.ToDateTime(endDate),
                EffectiveFrom = Convert.ToDateTime(startDate),
                IsDatePicker = true,
                IsEffective = 1
            };
            return PartialView("_PartialSundryFlat", ObjMatrix);
        }


        public ActionResult AddSundryFTL_New(ContarctFTLViewModel Custcontract, List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> FTLList, HttpPostedFileBase fileUpload, string SaveType)
        {
            string relativePath = "";
            string ftltype = Custcontract.FTLfreightCharge.ftltype;
            string ratetype = Custcontract.FTLfreightCharge.ftl1_trip_ratetype;

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(fileUpload.FileName);
                string folderPath = Server.MapPath("~/Uploads/Contract/FTLCharges");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string physicalPath = Path.Combine(folderPath, uniqueFileName);
                fileUpload.SaveAs(physicalPath);
                relativePath = uniqueFileName;
            }
            else
            {
                relativePath = !string.IsNullOrEmpty(Custcontract.FTLfreightCharge.Attachment)
                    ? Path.GetFileName(Custcontract.FTLfreightCharge.Attachment)
                    : "";
            }

            FTLList.ForEach(m => m.Attachment = relativePath);


            Custcontract.FTLfreightCharge.Attachment = relativePath;

            if (SaveType == "multiple")
            {
                FTLList = FTLList.Where(m => m.srno == 0).ToList();
            }

            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(FTLList.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, FTLList);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
            }
            DataTable Dt = _contractService.InsertSundryFTL_NEW(xmlDoc.InnerXml, Custcontract.FTLfreightCharge.ContractId, BaseUserName.ToUpper(), BaseCompanyCode.ToUpper(), ftltype, ratetype);

            return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("ContractDone", new { ContID = Custcontract.CustomerContract.ContractId, CustCode = Custcontract.CustomerContract.Custcode, Type = "" });
        }

        [HttpPost]
        public ActionResult GetfreightChargeForFTLUpdate(ContarctFTLViewModel custcontract)
        {
            List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> ListFRTMatrixSingleSlab = new List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR>();

            ListFRTMatrixSingleSlab = _contractService.ListSundryFTL(
                 custcontract.FTLfreightCharge.chargecode,
                 custcontract.FTLfreightCharge.trans_type,
                 custcontract.FTLfreightCharge.Matrixtype,
                 custcontract.FTLfreightCharge.ftltype,
                 custcontract.FTLfreightCharge.ftl1_trip_ratetype,
                 custcontract.FTLfreightCharge.chargetype,
                 custcontract.FTLfreightCharge.ContractId
                );

            List<RateUpdateModelForFTLPTL> ListForUpdateSlab = new List<RateUpdateModelForFTLPTL>();

            foreach (var item in ListFRTMatrixSingleSlab)
            {
                RateUpdateModelForFTLPTL objForUpdateSlab = new RateUpdateModelForFTLPTL();
                objForUpdateSlab.srno = (int)item.srno;
                objForUpdateSlab.FromLane = item.FromLane;
                objForUpdateSlab.ToLane = item.ToLane;
                objForUpdateSlab.ProductGroup = item.ProductGroup;
                objForUpdateSlab.EffectiveFrom = item.EffectiveFrom;
                objForUpdateSlab.EffectiveTo = item.EffectiveTo;
                objForUpdateSlab.FromQty = item.FromQty;
                objForUpdateSlab.ToQty = item.ToQty;
                objForUpdateSlab.old_rate = item.FTL1_TRIP_RATE;
                objForUpdateSlab.IsEffective = item.IsEffective;
                //objForUpdateSlab.old_rate = item.old_rate;
                //objForUpdateSlab.rate_differnce = item.rate_differnce;
                ListForUpdateSlab.Add(objForUpdateSlab);
            }
            ViewBag.Custcode = custcontract.FTLfreightCharge.Custcode;
            return PartialView("_UpdateFreightCharge", ListForUpdateSlab);
        }

        [HttpPost]
        public JsonResult UpdateFTLRateChange(string ContractId, List<RateUpdateModelForFTLPTL> RateUpdates)
        {
            try
            {
                if (RateUpdates == null || RateUpdates.Count == 0)
                {
                    return Json(new { Status = false, Message = "Please select at least one record to update." }, JsonRequestBehavior.AllowGet);
                }

                string xmlData = "<ArrayOfRateUpdate>";
                foreach (var item in RateUpdates)
                {
                    xmlData += "<RateUpdate>" +
                               "<Srno>" + item.srno + "</Srno>" +
                               "<Rate>" + item.rate + "</Rate>" +
                               "<OldRate>" + item.old_rate + "</OldRate>" +
                               "<RateDiff>" + item.rate_differnce + "</RateDiff>" +
                               "<EffectiveFrom>" + item.EffectiveFrom.ToString("dd MMM yyyy") + "</EffectiveFrom>" +
                               "<EffectiveTo>" + item.EffectiveTo?.ToString("dd MMM yyyy") + "</EffectiveTo>" +
                               "</RateUpdate>";
                }
                xmlData += "</ArrayOfRateUpdate>";

                DataTable dt = _contractService.UpdateFTLRateChange(xmlData, ContractId, BaseUserName.ToUpper(), BaseCompanyCode.ToUpper());
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public void GetExcelDataFreightChargeFTL_List(string contractID)
        {
            string FileName = "Freight Charge FTL List";
            DataTable dt = _contractService.GetExcelDataFreightChargeFTL(contractID);
            ExportUtilities.ExportToCSV(dt, FileName);
            dt.TableName = FileName;
        }

        [HttpPost]
        public JsonResult DeleteFTLChargeRow(int srno, string contractId)
        {
            try
            {
                DataTable dt = _contractService.DeleteFreightCharges(srno, contractId, BaseUserName.ToUpper());
                return Json(new { Status = Convert.ToBoolean(dt.Rows[0]["Status"]), Message = dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public void DownloadSundryFTLExcelFormat(string contractId)
        {
            DownloadSundryExcelFormat(contractId, "2");
        }

        public void DownloadSundryExcelFormat(string contractId, string serviceType)
        {
            string FileName = serviceType == "2" ? "Sundry FTL Charge Format" : "Freight Charge PTL Format";
            DataTable dt = new DataTable();
            dt.TableName = FileName;

            dt.Columns.Add("From", typeof(string)); //A-1
            dt.Columns.Add("To", typeof(string)); //B-2
            dt.Columns.Add("Lane Code", typeof(string)); //C-3
            dt.Columns.Add("Map Dist(km)", typeof(string)); //D-4
            dt.Columns.Add("Contractual Dist(km)", typeof(string)); //E-5
            dt.Columns.Add("Start Date", typeof(string)); //F-6
            dt.Columns.Add("End Date", typeof(string)); //G-7
            dt.Columns.Add("Estimated Volume", typeof(string));//H-8
            dt.Columns.Add("Product Group", typeof(string)); //I-9
            dt.Columns.Add("UOM", typeof(string)); //J-10
            dt.Columns.Add("Rate", typeof(string)); //K-11
            dt.Columns.Add("Transit days", typeof(string)); //L-12
            dt.Columns.Add("Billing State", typeof(string)); //M-13
            dt.Columns.Add("From Qty", typeof(string)); //N-14
            dt.Columns.Add("To Qty", typeof(string)); //O-15
            dt.Columns.Add("Chargeable on", typeof(string)); //P-16
            dt.Columns.Add("Minimum Guarantee", typeof(string)); //Q-17

            string custCode = "";
            DateTime? contractEdDate = null;
            if (!string.IsNullOrEmpty(contractId))
            {
                DataTable dtContract = _contractService.GetCustomerContractDetails(contractId, "BasicInfo");
                if (dtContract != null && dtContract.Rows.Count > 0)
                {
                    var customerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dtContract.Rows[0]);
                    custCode = customerContract.Custcode ?? "";
                    contractEdDate = customerContract.Contract_Eddate;
                }
            }

            // Create default sample row
            DataRow sampleRow = dt.NewRow();
            sampleRow["From"] = "";
            sampleRow["To"] = "";
            sampleRow["Lane Code"] = "L001";
            sampleRow["Map Dist(km)"] = "100";
            sampleRow["Contractual Dist(km)"] = "100";
            sampleRow["Start Date"] = DateTime.Today.ToString("dd-MM-yyyy");
            sampleRow["End Date"] = contractEdDate.HasValue ? contractEdDate.Value.ToString("dd-MM-yyyy") : DateTime.Today.AddDays(30).ToString("dd-MMM-yyyy");
            sampleRow["Estimated Volume"] = "10.00";
            sampleRow["Product Group"] = "";
            sampleRow["UOM"] = "";
            sampleRow["Rate"] = "2.50";
            sampleRow["Transit days"] = "2";
            sampleRow["Billing State"] = "";
            sampleRow["From Qty"] = "0";
            sampleRow["To Qty"] = "100";
            sampleRow["Chargeable on"] = "1:Gross Qty";
            sampleRow["Minimum Guarantee"] = "1";
            dt.Rows.Add(sampleRow);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                var worksheet2 = wb.Worksheets.Add("Sheet1");

                List<CYGNUS_LaneMaster> GetStartLaneDetails = !string.IsNullOrEmpty(custCode)
                    ? _contractService.GetCustomerLaneId(custCode)
                    : new List<CYGNUS_LaneMaster>();

                List<CYGNUS_LaneMaster> GetEndLaneDetails = _masterService.GetLaneDetails() ?? new List<CYGNUS_LaneMaster>();
                List<CYGNUS_Master_General> ProductList = _masterService.GetGeneralMasterWithParam("PROD").ToList();
                List<CYGNUS_Master_General> UOMList = _masterService.GetGeneralMasterWithParam("UOM").ToList();

                List<CYGNUS_Customer_Bill_Details> stateDetails = new List<CYGNUS_Customer_Bill_Details>();
                if (!string.IsNullOrEmpty(custCode))
                {
                    var dtBill = _masterService.Get_Customerwise_BillDetails(custCode);
                    if (dtBill != null)
                    {
                        stateDetails = DataRowToObject.CreateListFromTable<CYGNUS_Customer_Bill_Details>(dtBill);
                    }
                }

                List<string> ChargeableList = new List<string> { "1:Gross Qty", "2:Net Qty", "3:Delivered Qty" };

                int rowFrom = 1;
                foreach (var item in GetStartLaneDetails)
                {
                    worksheet2.Cell("A" + rowFrom.ToString()).Value = item.Lane_ID + ":" + item.Lane_Name;
                    rowFrom++;
                }

                int rowTo = 1;
                foreach (var item in GetEndLaneDetails)
                {
                    worksheet2.Cell("B" + rowTo.ToString()).Value = item.Lane_ID + ":" + item.Lane_Name;
                    rowTo++;
                }

                int rowProd = 1;
                foreach (var item in ProductList)
                {
                    worksheet2.Cell("C" + rowProd.ToString()).Value = item.CodeId + ":" + item.CodeDesc;
                    rowProd++;
                }

                int rowUOM = 1;
                foreach (var item in UOMList)
                {
                    worksheet2.Cell("D" + rowUOM.ToString()).Value = item.CodeId + ":" + item.CodeDesc;
                    rowUOM++;
                }

                int rowState = 1;
                foreach (var item in stateDetails)
                {
                    worksheet2.Cell("E" + rowState.ToString()).Value = item.Bill_State + ":" + item.State;
                    rowState++;
                }

                int rowCharge = 1;
                foreach (var item in ChargeableList)
                {
                    worksheet2.Cell("F" + rowCharge.ToString()).Value = item;
                    rowCharge++;
                }

                var mainSheet = wb.Worksheet(1);

                // Populate first row cells with default selection from lookups
                if (GetStartLaneDetails.Count > 0)
                {
                    mainSheet.Cell(2, 1).Value = GetStartLaneDetails[0].Lane_ID + ":" + GetStartLaneDetails[0].Lane_Name;
                }
                if (GetEndLaneDetails.Count > 0)
                {
                    mainSheet.Cell(2, 2).Value = GetEndLaneDetails[0].Lane_ID + ":" + GetEndLaneDetails[0].Lane_Name;
                }
                if (ProductList.Count > 0)
                {
                    mainSheet.Cell(2, 9).Value = ProductList[0].CodeId + ":" + ProductList[0].CodeDesc;
                }
                if (UOMList.Count > 0)
                {
                    mainSheet.Cell(2, 10).Value = UOMList[0].CodeId + ":" + UOMList[0].CodeDesc;
                }
                if (stateDetails.Count > 0)
                {
                    mainSheet.Cell(2, 13).Value = stateDetails[0].Bill_State + ":" + stateDetails[0].State;
                }

                if (rowFrom > 1)
                {
                    mainSheet.Column(1).SetDataValidation().List(worksheet2.Range("A1:A" + (rowFrom - 1).ToString()), true);
                }
                if (rowTo > 1)
                {
                    mainSheet.Column(2).SetDataValidation().List(worksheet2.Range("B1:B" + (rowTo - 1).ToString()), true);
                }
                if (rowProd > 1)
                {
                    mainSheet.Column(9).SetDataValidation().List(worksheet2.Range("C1:C" + (rowProd - 1).ToString()), true);
                }
                if (rowUOM > 1)
                {
                    mainSheet.Column(10).SetDataValidation().List(worksheet2.Range("D1:D" + (rowUOM - 1).ToString()), true);
                }
                if (rowState > 1)
                {
                    mainSheet.Column(13).SetDataValidation().List(worksheet2.Range("E1:E" + (rowState - 1).ToString()), true);
                }
                if (rowCharge > 1)
                {
                    mainSheet.Column(16).SetDataValidation().List(worksheet2.Range("F1:F" + (rowCharge - 1).ToString()), true);
                }

                // Format Date columns as Date type
                mainSheet.Column(6).Style.NumberFormat.Format = "dd-MM-yyyy";
                mainSheet.Column(7).Style.NumberFormat.Format = "dd-MM-yyyy";

                worksheet2.Hide();

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".xlsx");

                using (MemoryStream mymemorystream = new MemoryStream())
                {
                    wb.SaveAs(mymemorystream);
                    mymemorystream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        [HttpPost]
        public ActionResult ImportSundryFTLExcel(HttpPostedFileBase file, string contractId)
        {
            return ImportSundryExcel(file, contractId);
        }

        [HttpPost]
        public ActionResult ImportSundryExcel(HttpPostedFileBase file, string contractId)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { Status = false, Message = "Please select a valid Excel file." }, JsonRequestBehavior.AllowGet);
                }

                string serviceType = Request.Form["serviceType"] ?? "2"; // 1 = PTL, 2 = FTL
                string custCode = "";
                string ftltype = Request.Form["ftltype"] ?? "";
                string ratetype = Request.Form["ratetype"] ?? "";
                string startDateStr = Request.Form["startDate"] ?? "";
                string endDateStr = Request.Form["endDate"] ?? "";
                string trans_type = Request.Form["trans_type"] ?? "";
                string matrixtype = Request.Form["matrixtype"] ?? "";

                DateTime? contractStDate = null;
                DateTime? contractEdDate = null;

                if (!string.IsNullOrEmpty(contractId))
                {
                    DataTable dtContract = _contractService.GetCustomerContractDetails(contractId, "BasicInfo");
                    if (dtContract != null && dtContract.Rows.Count > 0)
                    {
                        var customerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dtContract.Rows[0]);
                        custCode = customerContract.Custcode ?? "";
                        contractStDate = customerContract.Contract_Stdate;
                        contractEdDate = customerContract.Contract_Eddate;
                    }
                }

                DateTime? effectiveFrom = null;
                DateTime? effectiveTo = null;
                if (!string.IsNullOrEmpty(startDateStr)) effectiveFrom = Convert.ToDateTime(startDateStr);
                if (!string.IsNullOrEmpty(endDateStr)) effectiveTo = Convert.ToDateTime(endDateStr);

                string htmlContent = "";
                int startId = Convert.ToInt32(Request.Form["startId"] ?? "1");

                using (var workbook = new XLWorkbook(file.InputStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var range = worksheet.RangeUsed();
                    var rowCount = range.RowCount();

                    for (int rowNum = 2; rowNum <= rowCount; rowNum++)
                    {
                        var row = worksheet.Row(rowNum);
                        if (row.IsEmpty()) continue;

                        string fromVal = row.Cell(1).Value.ToString() ?? "";
                        string fromLane = fromVal;
                        if (fromVal.Contains(":"))
                        {
                            fromLane = fromVal.Split(':')[0].Trim();
                        }
                        else if (fromVal.Contains("~"))
                        {
                            fromLane = fromVal.Split('~')[0].Trim();
                        }

                        string toVal = row.Cell(2).Value.ToString() ?? "";
                        string toLane = toVal;
                        if (toVal.Contains(":"))
                        {
                            toLane = toVal.Split(':')[0].Trim();
                        }
                        else if (toVal.Contains("~"))
                        {
                            toLane = toVal.Split('~')[0].Trim();
                        }

                        string laneCode = row.Cell(3).Value.ToString() ?? "";

                        // Parse Map Distance
                        decimal mapDistance = 0;
                        string mapDistanceStr = row.Cell(4).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(mapDistanceStr) && !decimal.TryParse(mapDistanceStr, out mapDistance))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Map Distance must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (mapDistance < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Map Distance cannot be negative ({mapDistance})." }, JsonRequestBehavior.AllowGet);
                        }

                        // Parse Contractual Distance
                        decimal contractualDistance = 0;
                        string contractualDistanceStr = row.Cell(5).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(contractualDistanceStr) && !decimal.TryParse(contractualDistanceStr, out contractualDistance))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Contractual Distance must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (contractualDistance < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Contractual Distance cannot be negative ({contractualDistance})." }, JsonRequestBehavior.AllowGet);
                        }

                        // Parse Start Date
                        DateTime? rowStartDate = null;
                        string rowStartStr = row.Cell(6).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(rowStartStr))
                        {
                            DateTime tempDate;
                            if (DateTime.TryParse(rowStartStr, out tempDate))
                            {
                                rowStartDate = tempDate;
                            }
                            else
                            {
                                return Json(new { Status = false, Message = $"Row {rowNum}: Start Date must be a valid date format." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            rowStartDate = effectiveFrom;
                        }

                        if (rowStartDate != null && rowStartDate.Value.Date < DateTime.Today)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Start Date ({rowStartDate.Value.ToString("dd-MMM-yyyy")}) cannot be less than today's date ({DateTime.Today.ToString("dd-MMM-yyyy")})." }, JsonRequestBehavior.AllowGet);
                        }

                        // Parse End Date
                        DateTime? rowEndDate = null;
                        string rowEndStr = row.Cell(7).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(rowEndStr))
                        {
                            DateTime tempDate;
                            if (DateTime.TryParse(rowEndStr, out tempDate))
                            {
                                rowEndDate = tempDate;
                            }
                            else
                            {
                                return Json(new { Status = false, Message = $"Row {rowNum}: End Date must be a valid date format." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            rowEndDate = effectiveTo;
                        }

                        if (rowEndDate != null && contractEdDate != null && rowEndDate.Value.Date > contractEdDate.Value.Date)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: End Date ({rowEndDate.Value.ToString("dd-MMM-yyyy")}) cannot exceed the Contract End Date ({contractEdDate.Value.ToString("dd-MMM-yyyy")})." }, JsonRequestBehavior.AllowGet);
                        }

                        if (rowStartDate != null && rowEndDate != null && rowStartDate.Value.Date > rowEndDate.Value.Date)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Start Date ({rowStartDate.Value.ToString("dd-MMM-yyyy")}) cannot be greater than End Date ({rowEndDate.Value.ToString("dd-MMM-yyyy")})." }, JsonRequestBehavior.AllowGet);
                        }

                        // Parse Estimated Volume
                        decimal estVolume = 0;
                        string estVolumeStr = row.Cell(8).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(estVolumeStr) && !decimal.TryParse(estVolumeStr, out estVolume))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Estimated Volume must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (estVolume < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Estimated Volume cannot be negative ({estVolume})." }, JsonRequestBehavior.AllowGet);
                        }

                        string productGroupVal = row.Cell(9).Value.ToString() ?? "";
                        string productGroup = productGroupVal;
                        if (productGroupVal.Contains(":"))
                        {
                            productGroup = productGroupVal.Split(':')[0].Trim();
                        }

                        string uomVal = row.Cell(10).Value.ToString() ?? "";
                        string uom = uomVal;
                        if (uomVal.Contains(":"))
                        {
                            uom = uomVal.Split(':')[0].Trim();
                        }

                        // Parse Rate
                        decimal rate = 0;
                        string rateStr = row.Cell(11).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(rateStr) && !decimal.TryParse(rateStr, out rate))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Rate must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (rate < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Rate cannot be negative ({rate})." }, JsonRequestBehavior.AllowGet);
                        }

                        // Parse Transit Days
                        int transitDays = 0;
                        string transitDaysStr = row.Cell(12).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(transitDaysStr) && !int.TryParse(transitDaysStr, out transitDays))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Transit Days must be a valid integer." }, JsonRequestBehavior.AllowGet);
                        }
                        if (transitDays < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Transit Days cannot be negative ({transitDays})." }, JsonRequestBehavior.AllowGet);
                        }

                        string stateVal = row.Cell(13).Value.ToString() ?? "";
                        string billingState = stateVal;
                        if (stateVal.Contains(":"))
                        {
                            billingState = stateVal.Split(':')[0].Trim();
                        }

                        // Parse From Qty
                        decimal fromQty = 0;
                        string fromQtyStr = row.Cell(14).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(fromQtyStr) && !decimal.TryParse(fromQtyStr, out fromQty))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: From Qty must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (fromQty < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: From Qty cannot be negative ({fromQty})." }, JsonRequestBehavior.AllowGet);
                        }

                        // Parse To Qty
                        decimal toQty = 0;
                        string toQtyStr = row.Cell(15).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(toQtyStr) && !decimal.TryParse(toQtyStr, out toQty))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: To Qty must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (toQty < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: To Qty cannot be negative ({toQty})." }, JsonRequestBehavior.AllowGet);
                        }

                        if (fromQty > toQty)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: From Qty ({fromQty}) cannot be greater than To Qty ({toQty})." }, JsonRequestBehavior.AllowGet);
                        }

                        string chargeableVal = row.Cell(16).Value.ToString() ?? "";
                        string chargeableOn = chargeableVal;
                        if (chargeableVal.Contains(":"))
                        {
                            chargeableOn = chargeableVal.Split(':')[0].Trim();
                        }

                        // Parse Minimum Guarantee
                        decimal minGuarantee = 0;
                        string minGuaranteeStr = row.Cell(17).Value.ToString() ?? "";
                        if (!string.IsNullOrEmpty(minGuaranteeStr) && !decimal.TryParse(minGuaranteeStr, out minGuarantee))
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Minimum Guarantee must be a valid number." }, JsonRequestBehavior.AllowGet);
                        }
                        if (minGuarantee < 0)
                        {
                            return Json(new { Status = false, Message = $"Row {rowNum}: Minimum Guarantee cannot be negative ({minGuarantee})." }, JsonRequestBehavior.AllowGet);
                        }

                        if (serviceType == "2")
                        {
                            // FTL Row
                            var model = new CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR
                            {
                                Id = startId,
                                chargecode = "2",
                                chargetype = "BKG",
                                trans_type = "2",
                                Matrixtype = "L",
                                ftltype = ftltype,
                                ftl1_trip_ratetype = ratetype,
                                ContractId = contractId,
                                Custcode = custCode,
                                FromLane = fromLane,
                                ToLane = toLane,
                                LaneCode = laneCode,
                                MapDistance = mapDistance,
                                ContractualDistance = contractualDistance > 0 ? contractualDistance : mapDistance,
                                EffectiveFrom = rowStartDate ?? DateTime.Now,
                                EffectiveTo = rowEndDate ?? DateTime.Now,
                                EstVolume = estVolume,
                                ProductGroup = productGroup,
                                UOM = uom,
                                FTL1_TRIP_RATE = rate,
                                FTL1_TRDAYS = transitDays,
                                BillingState = billingState,
                                FromQty = (int)fromQty,
                                ToQty = (int)toQty,
                                ChargeableOn = chargeableOn,
                                MinimumGuarantee = (int)minGuarantee,
                                IsDatePicker = true,
                                IsEffective = 1
                            };
                            htmlContent += RenderPartialViewToString("_PartialSundryFlat", model);
                        }
                        else
                        {
                            // PTL Row
                            var model = new CYGNUS_CustContract_FRTMatrix_SingleSlab
                            {
                                Id = startId,
                                chargecode = "1",
                                chargetype = "BKG",
                                FilterTrnsMode = trans_type,
                                FilterMatrixtype = matrixtype,
                                ratetype = ratetype,
                                ContractId = contractId,
                                Custcode = custCode,
                                FromLane = fromLane,
                                ToLane = toLane,
                                LaneCode = laneCode,
                                MapDistance = mapDistance,
                                ContractualDistance = contractualDistance > 0 ? contractualDistance : mapDistance,
                                EffectiveFrom = rowStartDate ?? DateTime.Now,
                                EffectiveTo = rowEndDate ?? DateTime.Now,
                                EstVolume = estVolume,
                                ProductGroup = productGroup,
                                UOM = uom,
                                rate = rate,
                                trdays = transitDays,
                                BillLoc = billingState,
                                FromQty = (int)fromQty,
                                ToQty = (int)toQty,
                                ChargeableOn = chargeableOn,
                                MinimumGuarantee = (int)minGuarantee,
                                IsDatePicker = true,
                                IsEffective = 1
                            };
                            htmlContent += RenderPartialViewToString("_PartialFreightCharge", model);
                        }
                        startId++;
                    }
                }

                int totalCount = startId - Convert.ToInt32(Request.Form["startId"] ?? "1");
                return Json(new { Status = true, Html = htmlContent, Count = totalCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion


        #region Charge Matrix

        public ActionResult ChargeWiseMatrix(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
            CYGNUS_custcontract_hdr CustomerContract = new CYGNUS_custcontract_hdr();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }
            ContractChargeConstraintViewModel objCustomerContract = new ContractChargeConstraintViewModel();
            CYGNUS_custcontract_charge_constraint data = new CYGNUS_custcontract_charge_constraint();
            data.ContractId = CustomerContract.ContractId;
            data.Custcode = CustomerContract.Custcode;
            data.chargetype = "BKG";
            objCustomerContract.chargeContraint = data;
            List<CYGNUS_custcontract_charge_constraint> datalist = new List<CYGNUS_custcontract_charge_constraint>();
            var list = _contractService.GetchargeMatrixList(ContractId, "BKG");
            if (list.Count > 0)
            {
                datalist = list;
            }
            objCustomerContract.ListContraint = datalist;
            return PartialView("_ChargeMatrix", objCustomerContract);
        }

        [HttpPost]
        public ActionResult AddModeChargeMatrix(ContractChargeConstraintViewModel Custcontract, List<CYGNUS_custcontract_charge_constraint> ChargeMatrixList)
        {

            try
            {
                string contractid = Custcontract.chargeContraint.ContractId;
                ChargeMatrixList.ForEach(m => m.chargetype = Custcontract.chargeContraint.chargetype);
                XmlDocument xmlChargeMatrixListInfo = new XmlDocument();
                if (ChargeMatrixList != null)
                {
                    XmlSerializer xmlChargeMatrixListSerializer = new XmlSerializer(ChargeMatrixList.GetType());
                    using (MemoryStream xmlStream = new MemoryStream())
                    {
                        xmlChargeMatrixListSerializer.Serialize(xmlStream, ChargeMatrixList);
                        xmlStream.Position = 0;
                        xmlChargeMatrixListInfo.Load(xmlStream);
                    }
                }

                DataTable Dt = _contractService.InsertChargeMatrix1(
                    contractid,
                    BaseUserName,
                    BaseCompanyCode,
                    xmlChargeMatrixListInfo.InnerXml.ReplaceSpecialCharacters()
                    );
                return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Standard Charge
        public ActionResult StandardCharge(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
            CYGNUS_custcontract_hdr CustomerContract = new CYGNUS_custcontract_hdr();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dt.Rows[0]);
            }
            CYGNUS_custcontract_charge_constraint data = new CYGNUS_custcontract_charge_constraint();
            ContractStandardChargeViewModel objCustomerContract = new ContractStandardChargeViewModel();
            data.ContractId = CustomerContract.ContractId;
            data.Custcode = CustomerContract.Custcode;
            data.TransMode = "2";
            data.Matrixtype = "L";
            objCustomerContract.chargeContraint = data;
            ViewBag.objMasterList = _masterService.GetGeneralMasterWithParam("TRN").ToList();
            ViewBag.objMasterMatrixList = _masterService.GetGeneralMasterWithParam("MXTYPE").ToList();
            ViewBag.objListCharge = _contractService.GetStandaradChargeList(ContractId, "BKG");
            return PartialView("_StandardCharge", objCustomerContract);
        }

        [HttpGet]
        public JsonResult GetContractStandardCharges(string contractId)
        {
            try
            {
                if (string.IsNullOrEmpty(contractId))
                {
                    return Json(new { success = false, charges = new List<CYGNUS_custcontract_charge_constraint>() }, JsonRequestBehavior.AllowGet);
                }
                var list = _contractService.GetStandaradChargeList(contractId, "BKG");
                var chargeList = list.Select(c => new {
                    c.chargecode,
                    c.chargename,
                    IsRequired = c.IsRequired
                }).ToList();
                return Json(new { success = true, charges = chargeList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStandardCharge(ContractStandardChargeViewModel custcontract)
        {
            CYGNUS_custcontract_charge_constraint ObjChargeContraint = new CYGNUS_custcontract_charge_constraint();
            string chargetype = "BKG";
            string contractid = custcontract.chargeContraint.ContractId;
            string custcode = custcontract.chargeContraint.Custcode;
            string chargecode = custcontract.chargeContraint.basedon;
            string transmode = custcontract.chargeContraint.TransMode;
            string matrixtype = custcontract.chargeContraint.Matrixtype;

            string serviceType = "";
            DataTable dtservice = _contractService.GetServiceType(contractid);
            if (dtservice.Rows.Count > 0)
            {
                serviceType = dtservice.Rows[0]["service_type"].ToString();
            }

            List<CYGNUS_CustContract_FRTMatrix_SingleSlab> ListSingleSlab = new List<CYGNUS_CustContract_FRTMatrix_SingleSlab>();

            ListSingleSlab = _contractService.GetSinlgeslabForStandardCharges(contractid, chargecode, serviceType);

            CYGNUS_CustContract_FRTMatrix_SingleSlab ObjSlab = new CYGNUS_CustContract_FRTMatrix_SingleSlab();
            int Id = 0;
            foreach (var item in ListSingleSlab)
            {
                Id++;
                item.Id = Id;
                item.chargetype = chargetype;
                item.Matrixtype = matrixtype;
                item.TransMode = transmode;
                item.chargecode = chargecode;
                item.ContractId = contractid;
                item.Custcode = custcode;
            }

            if (ListSingleSlab.Count == 0)
            {
                ObjSlab.Id = 1;
                ObjSlab.rate = 0.00M;
                ObjSlab.chargetype = chargetype;
                ObjSlab.Matrixtype = matrixtype;
                ObjSlab.TransMode = transmode;
                ObjSlab.chargecode = chargecode;
                ObjSlab.ContractId = contractid;
                ObjSlab.Custcode = custcode;
                ListSingleSlab.Add(ObjSlab);
            }
            custcontract.ListsingleSlab = ListSingleSlab;
            FreightChargeDropDown(custcontract.chargeContraint.ContractId);
            return PartialView("_Standrad_PartialStandard_SundrySlab_obj", ListSingleSlab);
        }

        public ActionResult ADDfreightChargeMatrix(int id, string contractid, string chargecode, string MatrixType, string TransMode, string custcode)
        {
            CYGNUS_CustContract_FRTMatrix_SingleSlab ObjMatrix = new CYGNUS_CustContract_FRTMatrix_SingleSlab
            {
                Id = id,
                rate = 0.00M,
                chargetype = "BKG",
                Matrixtype = MatrixType,
                TransMode = TransMode,
                chargecode = chargecode,
                ContractId = contractid,
                Custcode = custcode,
            };
            return PartialView("_Standrad_PartialStandard_SundrySlab", ObjMatrix);
        }

        public ActionResult AddStandardCharge(ContractStandardChargeViewModel custcontract, List<CYGNUS_CustContract_FRTMatrix_SingleSlab> FTLList)
        {
            string ContractId = custcontract.chargeContraint.ContractId;
            string Custcode = custcontract.chargeContraint.Custcode;
            string chargecode = custcontract.chargeContraint.basedon;
            string transmode = custcontract.chargeContraint.TransMode;
            string matrixtype = custcontract.chargeContraint.Matrixtype;

            string serviceType = "";
            DataTable dtservice = _contractService.GetServiceType(ContractId);
            if (dtservice.Rows.Count > 0)
            {
                serviceType = dtservice.Rows[0]["service_type"].ToString();
            }

            string xmlFRIGHTCCM = "";

            xmlFRIGHTCCM = "<ArrayOfCCMChargeMatrix>";
            foreach (var item in FTLList)
            {
                xmlFRIGHTCCM = xmlFRIGHTCCM + "<CCMChargeMatrix>" +
                    "<contractid>" + ContractId + "</contractid>" +
                    "<custcode>" + Custcode + "</custcode>" +
                    "<chargetype>" + item.chargetype + "</chargetype>" +
                    "<chargecode>" + item.chargecode + "</chargecode>" +
                    "<rate>" + item.rate + "</rate>" +
                    "<ratetype>" + item.ratetype + "</ratetype>" +
                    "<trans_type>" + item.TransMode + "</trans_type>" +
                    "<FromLane>" + item.FromLane + "</FromLane>" +
                    "<ToLane>" + item.ToLane + "</ToLane>" +
                    "<LaneCode>" + item.LaneCode + "</LaneCode>" +
                    "<Matrixtype>" + item.Matrixtype + "</Matrixtype>" +
                    "</CCMChargeMatrix>";
            }
            xmlFRIGHTCCM = xmlFRIGHTCCM + "</ArrayOfCCMChargeMatrix>";
            DataTable Dt = _contractService.AddStandardChargeMatrix(xmlFRIGHTCCM.Trim(), chargecode, ContractId, BaseUserName, BaseCompanyCode, serviceType);

            return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString() }, JsonRequestBehavior.AllowGet);
            //return PartialView("_PartialBlank", custcontract);
        }

        #endregion

        #region Bill Configuration

        public ActionResult BillConfiguration(string ContractId)
        {
            DataTable dt = _contractService.GetCustomerContractDetails(ContractId, "BillConfiguration");
            CYGNUS_BILL_CONFIGURATION CustomerContract = new CYGNUS_BILL_CONFIGURATION();
            if (dt != null && dt.Rows.Count > 0)
            {
                CustomerContract = DataRowToObject.CreateItemFromRow<CYGNUS_BILL_CONFIGURATION>(dt.Rows[0]);
                //CustomerContract.isDone = true;
            }
            if (string.IsNullOrEmpty(CustomerContract.Item_Description))
            {
                CustomerContract.Item_Description = "Freight for Transportation of Coil ";
            }

            DataTable dataTable = _contractService.GetCustomerContractDetails(ContractId, "BasicInfo");
            CYGNUS_custcontract_hdr Customer = new CYGNUS_custcontract_hdr();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                Customer = DataRowToObject.CreateItemFromRow<CYGNUS_custcontract_hdr>(dataTable.Rows[0]);
            }
            CustomerContract.CustCode = Customer.Custcode;
            ViewBag.bankDetails = _masterService.GetComapanyBankDetails(Customer.Bill_Org) ?? new List<Organization_Bank_Details>();
            ViewBag.ServiceHSN = _masterService.GetGeneralMasterWithParam("HSNCAT");
            ViewBag.ListLocations = _masterService.GetLocationDetails().ToList();
            ViewBag.VehicleDocumentType = _masterService.GetVehicleDocumentTypeById(0);

            // Fetch Customer Wise Bill Details using CustCode
            var dtBill = _masterService.Get_Customerwise_BillDetails(Customer.Custcode);
            ViewBag.billDetails = dtBill != null
                ? DataRowToObject.CreateListFromTable<CYGNUS_Customer_Bill_Details>(dtBill)
                : new List<CYGNUS_Customer_Bill_Details>();

            return PartialView("_BillConfiguration", CustomerContract);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBillConfiguration(CYGNUS_BILL_CONFIGURATION Custcontract)
        {
            string contractid = Custcontract.ContractId;
            string cutomerCode = Custcontract.CustCode;
            XmlDocument xmlBillConfigInfo = new XmlDocument();
            if (Custcontract != null)
            {
                XmlSerializer xmlBillConfigSerializer = new XmlSerializer(Custcontract.GetType());
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlBillConfigSerializer.Serialize(xmlStream, Custcontract);
                    xmlStream.Position = 0;
                    xmlBillConfigInfo.Load(xmlStream);
                }
            }
            DataTable Dt = _contractService.AddBillConfigurationInfo(
                contractid,
                cutomerCode,
                BaseUserName,
                BaseCompanyCode,
                xmlBillConfigInfo.InnerXml.ToString()
                );
            return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString(), ContractId = contractid, CustCode = cutomerCode }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public ActionResult FinalContractSubmit(string ContractId)
        {
            DataTable Dt = _contractService.FinalContractSubmit(ContractId);
            string contractid = Dt.Rows[0]["ContractId"].ToString();
            string cutomerCode = Dt.Rows[0]["CustCode"].ToString();
            return Json(new { Status = Convert.ToBoolean(Dt.Rows[0]["Status"]), Message = Dt.Rows[0]["Message"].ToString(),ContractId = contractid, CustCode = cutomerCode }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContractDone(string ContID, string CustCode)
        {
            DataTable dt = _contractService.GetCustContName(CustCode, ContID);

            ViewBag.ContaractID = ContID;
            ViewBag.ContarctName = dt.Rows[0]["custcode"].ToString() + " : " + dt.Rows[0]["name"].ToString(); ;
            ViewBag.ConType = dt.Rows[0]["contracttype"].ToString();
            ViewBag.CustCode = CustCode;
            return View("ContractDone");
        }
        
       

        public JsonResult GetMasterList(string str)
        {
            var MasterList = _masterService.GetGeneralMasterWithParam(str);

            var SearchList = (from e in MasterList
                              select new
                              {
                                  id = e.CodeId,
                                  text = e.CodeDesc,
                              }).Distinct().ToList();

            return Json(SearchList, JsonRequestBehavior.AllowGet);
        }


    }
}
