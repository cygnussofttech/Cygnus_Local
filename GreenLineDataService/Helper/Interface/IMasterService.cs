using GreenLine.Classes;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenLineDataService.Helper.Interface
{
    public interface IMasterService
    {
        #region State Master
        List<CYGNUS_State> GetStateMaster();
        List<CYGNUX_Master_Countries> GetCountryMaster();
        List<CYGNUS_citymaster> GetCityMaster();
        List<CYGNUS_Master_General> GetGeneralMaster();
        List<CYGNUS_Master_General> GetGeneralMasterWithParam(string CodeType = "", string Codeid = "");
        bool ActiveInActiveState(int id);
        DataTable AddEditStateMaster(string XML_Main);
        bool ExistsState(string stateName, string stateCode, int srno);
        int GetCityCount(string city, string state);
        DataTable GetExcelData(string MethodName, string BaseUserName, string BaseCompanyCode, string BaseLocationCode, string BaseYearVal);
        #endregion
        #region Organization Master
        DataTable AddEditOrganization(string BaseCompanyCode, string OrgDetxml, string OrgBnkDetListxml, string UserName);
        bool ActiveInActiveOrganization(string CompanyCode);

        CYGNUS_Organization_Master GetComapanyDetails(string BaseCompanyCode);

        List<CYGNUS_COMPANY_MASTER> GetComapanyDetails();
        List<Organization_Bank_Details> GetComapanyBankDetails(string BaseCompanyCode);
        #endregion
        DataSet CheckValidUserforLogin(string Username, string Loccode, string FinYear, string CompanyCode);
        DataTable GetIsFinYear();
        List<CYGNUS_Master_CodeTypes> GetCodetypesMasterList();
        DataTable AddEditGeneralMaster(string XML, string flag, string Finyear);
        string CheckDuplicateGeneralMaster(string CodeType, string CodeDesc);
        List<CYGNUS_location> GetLocationDetails(int? ActiveFlg = 0);
        List<CYGNUS_Master_General> GetMasterGeneralObject(string CodeId, string MasterCode);
        #region Menu Function
        List<VW_GetUserMenuRights> GetMenuListWithRights(string Userid, bool IsLogin, string Type, string BaseFinYear);
        List<CYGNUS_Master_Menu> GetMenusList(bool IsLogin);

        #endregion

        #region  Change Settings
        List<ChgangeLoc> GetWorkingLocations(string BaseLocationCode, string MainLocCode);
        List<ChgangeLoc> GetWorkingLocationsNewPortal(string BaseLocationCode, string MainLocCode, string UserName);
        List<ChgangeCompany> GetCompanyMappedToEmployee(string UserId);
        #endregion

        #region Finacial Years
        List<vw_Get_Finacial_Years> GetFinacialYearDetails();
        #endregion

        DataTable AddEditCityMaster(string XML);
        bool ActiveInActive_City(int id);
        DataTable GetCityMasterReportDetails();
        bool ActiveInActive_Customergroup(string id);

        List<CYGNUS_GRPMST> GetCustomerGroupMasterObject();
        List<CYGNUS_CUSTHDR> GetCustomer(string GRPCD);
        DataTable AddEditCustomerGroupMaster(string XML);
        List<CYGNUS_CUSTHDR> GetCustomerList(string Type, string Name);
        List<CYGNUS_CUSTHDR> GetCustomerList();

        #region Location
        bool InsertLocation(string XML, string EditFlag, string EntryBy, string XML2);
        bool ActiveInActiveLocation(string LocCode);
        #endregion

        #region Designation 
        List<CYGNUS_Master_General> GetDesignationFromCategory(string Category);
        List<CYGNUS_Master_Users> GetManagerFromDesignationandLocation(string Category, string Location);
        #endregion

        #region User Master
        List<CYGNUS_Master_Users> GetUserDetails();
        List<CYGNUS_Master_Users> GetEmployeeList();
        List<CYGNUS_Master_Users> GetUserDetailsForUserMasterList(string userId);
        DataTable InsertUser(string XML, string EditFlag);
        List<CYGNUS_location> GetDestinationLocationsWithHQTR(string Prefix);
        bool ActiveInActive_User(string id);
        List<CYGNUS_COMPANY_MASTER> GetCompanyDetails();
        #endregion

        #region User Menu Rights
        DataTable InsertMenuRights(string XML, string UserId, string isNewPortal, string UserName, string BaseFinYear);
        #endregion

        #region Change Reports Rights
        List<CYGNUS_Master_Reports> GetReportList(string ReportType, string ReportSubType, string UserName, int Type);
        DataTable Add_Report_Rights(string XML, string Location, string UserName, string CompanyCode, string BaseUserName);

        #endregion
        DataTable GetGSTWiseStateDetails(string GSTNo, string CustCode);
        List<CYGNUS_CUSTHDR> GetCustomerMasterObject(string CustCode);
        DataTable Get_StateWise_GSTDetails_TypeWise(string flag, string Code);
        DataTable Get_Customerwise_KMA(string Code);
        DataTable Get_Customerwise_BillDetails(string Code);
        DataTable Get_Customerwise_KYCDetails(string Code);
        DataTable Get_Customerwise_GeofenceDetails(string Code);
        DataTable Get_Customerwise_PickUpDetails(string Code);
        DataTable GetEmpDesignation(string UserId);
        DataTable AddEditCustomerMaster(string MstDetailsXML, string BaseUserName, string KMADetailsXML, string BillDetailsXML, string KYCDetailsXML, string GeofenceDetailsXML, string PickUpAddressDetailsXML);
        List<CYGNUS_VENDOR_HDR> GetVendorObject();
        List<CYGNUS_CUSTHDR> GetConsignnorCustomerListJson();
        List<CYGNUS_CUSTHDR> GetConsigneeCustomerListJson();
        List<CYGNUS_Master_Users> Search_Organization_Employee(string Prefix, string BaseUserName);
        bool ActiveInActive_Customer(string id);
        DataTable CustomerToUserCreate(string id, string BaseUserName, string Password);
        List<CYGNUS_CUSTHDR> GetCustomerListingNew(string searchTerm, string GRPCD, string state = null);
        List<CYGNUS_CUSTHDR> GetCustomerListforAddress();
        List<CYGNUS_location> GetLOCATIONByCityJson(int id);
        List<CYGNUS_State> GetStateByCityJson(int id);
        #region PinCode
        List<CYGNUS_pincode_master> GetPincodeMaster();
        DataTable AddEditPincodeMaster(string XML);
        bool ActiveInActive_Pincode(int id);
        bool ExistsPincode(int pincode, int id);
        #endregion

        #region Country Master
        bool ActiveInActiveCountry(int id);
        DataTable AddEditCountryMaster(string XML_Main, string BaseUserName);

        #endregion

        #region Lane Master
        List<CYGNUS_LaneMaster> GetLaneDetails();
        List<CYGNUS_LaneMaster> GetLaneDetails(string searchTerm);
        #endregion

        #region Vehicle Model
        List<CYGNUS_Vehicle_Model> GetVehicleModelDetails();
        List<CYGNUS_Vehicle_Model> GetVehicleModel();
        DataTable AddEditVehicleModelMaster(string XML, string Entry_EditFlag, string Finyear);
        bool ActiveInActive_VehicleModel(int id);
        List<CYGNUS_Vehicle_Document_Type> GetVehicleDocumentTypeWise(string Vehicle_Type, string Vendor_Type);
        #endregion

        #region Vehicle
        List<CYGNUS_Vehicle_Master> GetVehicleList(string VehicleNo, string BaseUserName, string Type);
        CYGNUS_Vehicle_Master GetVehicleById(string id);
        bool AddEditVehicle(string XML, string EditFlag, string EntryBy, string CompanyCode);
        List<CYGNUS_location> Getlatitude(string loccode);
        bool ActiveInActive_Vehicle(string vehNo);
        string GetVehicleAPICache(string vehNo);
        bool AddEditVehicleAPICache(string vehNo, string apiResponse);
        List<CYGNUS_Vehicle_Document_Type> GetVehicleDocument(string VehicleId);
        #endregion

        #region Tyre Size
        List<CYGNUS_FLEET_TYRESIZEMST> GetTyreSizeList();
        bool InsertUpdateTyreSize(string XML, string TYRE_SIZEID);
        #endregion

        #region Route Master location
        List<CYGNUS_rutmas> GetRutMstDetails();
        #endregion

        #region Battery Size
        List<CYGNUS_FLEET_BATTERYSIZEMST> GetBatterySizeList();
        #endregion

        #region Designation Mapping
        List<CYGNUS_Designation_Mapping> GetDesignationMappingList();
        string InsertDesignationMappingDetails(string Xml_Mst_Details, string userName);
        #endregion

        #region Driver master
        List<CYGNUS_FLEET_DRIVER_DOCDET> GetDriverDetDetails();
        List<CYGNUS_FLEET_DRIVERMST> GetDriverMstDetails();
        string GetMaxDriverCode();
        DataTable GetFileName(int Id, string FileName);
        DataTable AddEditDriver(DriverViewModel DVM, string[] array, string BaseCompanyCode);
        string CheckDuplicateDriverManualCode(string Code);
        string CheckDuplicateDriver(string number);
        bool ActiveInActive_Driver(int id, string baseUserName);
        List<CYGNUS_acctinfo> GetVehicleAccountCodeObject();
        List<CYGNUS_location> GetLocationDetailsForDriver();
        #endregion
        #region VEHICLE TYPE WISE DOCUMENT
        List<Cygnus_Master_VehicleType_wise_Document> Get_VehicleType_wise_DocumentDetails(string id);
        #endregion

        #region Document Type Master for Vehicle
        List<Cygnus_Master_Vehicle_DocumentType> GetVehicleDocumentTypeById(int id);
        DataTable AddEditVehicleDocumentType(string XML, string BaseUserName, string CompanyCode);
        #endregion

        #region  vehicle Driver Mapping 
        List<Cygnus_VehicleDriver_Mapping> Get_VehicleDriverMapping_Details(int id, string BaseUserName);
        DataTable VehicleDriverMappingSubmit(string Vehicleid, int First_Driver, int Second_Driver, string Baseusername);
        void DetachDriverMapping(string vehicleId, string driverType, string driverId, string userName, string detachReason = "");
        List<CYGNUS_Vehicle_Master> GetVehicleListForMapping(string vehicleId);
        List<CYGNUS_Vehicle_Master> Get_VehicleDriverMapping_VehicleList(string vehicleId);
        List<CYGNUS_FLEET_DRIVERMST> GetAvailableDriver(string VehicleId);
        DataTable GetActiveTrip(string vehicleId);
        DataTable GetVehicleDriverMapping(string vehicleId);
        #endregion
        #region  vehicle Trailer Mapping 
        List<CYGNUS_TRAILER_MAPPING> GetTrailerMappingObject(string VehicleId, string BaseUserName);
        DataTable VehicleTrailerMappingSubmit(string HdrXML, string UserName);
        void DetachTrailerMapping(string VehicleId, string userName);

        #endregion

        #region Fuel Station Master
        List<CYGNUS_Master_FuelStation> GetFuelStation();
        CYGNUS_Master_FuelStation GetFuelStationById(int Id);
        bool AddEditFuelStation(string XML, string CompanyCode, string BaseUserName);
        bool ActiveInActiveFuelStation(int Id);
        #endregion

        #region Card Master
        List<CYGNUS_Master_Card> GetCardMaster();
        List<CYGNUS_Master_Card> GetCardMasterByType(string type);
        CYGNUS_Master_Card GetCardMasterById(int Id);
        bool AddEditCardMaster(string XML, string CompanyCode, string BaseUserName);
        bool ActiveInActiveCardMaster(int Id);
        #endregion

        #region Card Assignment
        List<CYGNUS_Master_Card_Assignment> GetCardAssignment();
        CYGNUS_Master_Card_Assignment GetCardAssignmentById(int id);
        bool AddEditCardAssignment(string XML, string companyCode, string userName);
        bool ActiveInActiveCardAssignment(int id);
        DataTable CheckDuplicateCardAssignment(CYGNUS_Master_Card_Assignment model);
        #endregion

        List<CYGNUS_custcontract_hdr> GetCustomerContractList(string customerCode);
        CygnusCustomerGSTDetails GetGSTDetailsByGstNumber(string GSTNO);
        CygnusCustomerGSTDetails AddCustomerGSTDetails(string xmlCustGSTInfo, string GST, string BaseCompanyCode, string BaseUserName, string apiResponse);

        #region Consignee Master
        DataTable AddEditConsignee(string XML_Main, string Flag, string CompanyCode, string BaseUserName);
        List<Cygnus_Consignee_Master> GetConsigneeDetails(string Consignee_code);
        #endregion
        #region Expense Master
        List<CYGNUS_Master_Expense> GetExpenseMaster();
        CYGNUS_Master_Expense GetExpenseMasterById(int id);
        bool AddEditExpenseMaster(string xmlData, string companyCode, string userName);
        bool ActiveInActiveExpenseMaster(int id, string userName);
        #endregion

        #region Geofence Master
        List<CYGNUS_Master_Geofence> GetGeofenceMaster();
        CYGNUS_Master_Geofence GetGeofenceMasterById(int id);
        CYGNUS_Master_Geofence GetGeofenceByName(string geofenceName);
        bool AddEditGeofenceMaster(string xmlData, string companyCode, string userName);
        bool ActiveInActiveGeofenceMaster(int id, string userName);
        #endregion
        #region Get Country State City Zone based on Pincode
        DataSet GetCountryStateCityZoneByPincode(string code, string type);
        #endregion

        #region Notes Category Mapping
        List<CYGNUS_NotesCategory_Mapping> GetNotesCategoryMappingList();
        DataTable InsertNotesCategoryMappingDetails(string Xml_Mst_Details, string BaseUserName, string BaseCompanyCode);
        #endregion

        #region Vendor
        DataTable InsertVendor(string XML, string DetXML, string GSTXML, string VendorDocXML, string EditFlag, string EntryBy);
        List<CYGNUS_VENDOR_DET> GetVendorDetObject();
        List<Cygnus_Vendor_Document> GetVendorDoc(string VendorId);
        DataTable GetStateWiseGSTDetails(string flag, string Code);
        bool ActiveInActiveVendor(string id);
        List<CYGNUS_VENDOR_HDR> GetVendor();
        #endregion
    }
}
