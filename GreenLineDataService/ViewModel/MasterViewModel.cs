using GreenLineDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.ViewModel
{
    public class CYGNUS_StateViewModel
    {
        public List<CYGNUS_State> listWS { get; set; }
        public CYGNUS_State WST { get; set; }
    }

    #region General Master

    public class CYGNUS_Master_GeneralViewModel
    {
        public CYGNUS_Master_CodeTypes WMCT { get; set; }
        public List<CYGNUS_Master_General> listWMG { get; set; }
        public List<CYGNUS_Master_CodeTypes> VPart { get; set; }
        public CYGNUS_Master_General WMG { get; set; }

        //lavnit
        public List<CYGNUS_location> WLocationList { get; set; }
        public List<CYGNUS_location> WLocationROList { get; set; }
        public List<CYGNUS_location> WLocationAOList { get; set; }
        public List<CYGNUS_location> WLocationBankList { get; set; }

        public string RO { get; set; }
        public string AO { get; set; }
        public string LO { get; set; }
        public string Bank { get; set; }
        public string rdio { get; set; }
        public Nullable<decimal> LocLevel { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ChequeNo { get; set; }

        public List<vw_Get_Finacial_Years> ListYears { get; set; }
        public string FinYear { get; set; }

    }

    #endregion

    #region Location Master

    public class LocationViewModel
    {
        public List<CYGNUS_location> listWS { get; set; }
        public CYGNUS_location WL { get; set; }
        public List<CYGNUS_State> ListLocState { get; set; }
        public List<CYGNUS_Master_General> ListGnMST { get; set; }
        public string EditFlag { get; set; }
    }

    #endregion

    #region City master
    public class CYGNUS_citymasterViewModel
    {
        public List<CYGNUS_citymaster> listWCM { get; set; }
        public CYGNUS_citymaster WCM { get; set; }
    }

    #endregion

    #region Group Master

    public class CYGNUS_GRPMSTViewModel
    {
        public List<CYGNUS_GRPMST> listWGM { get; set; }
        public CYGNUS_GRPMST WGM { get; set; }
    }

    #endregion

    #region User Master

    public class CYGNUS_Master_UsersViewModel
    {
        public CYGNUS_Master_Users WMU { get; set; }
        public string UserId { get; set; }
        public bool EditFlag { get; set; }
        public string MapCompanys { get; set; }
        public string DefaultCompany { get; set; }
    }

    #endregion

    #region Customer Master

    public class CYGNUS_CUSTHDRViewModel
    {
        public List<CYGNUS_CUSTHDR> listWCH { get; set; }
        public CYGNUS_CUSTHDR WCH { get; set; }
    }
    #endregion

    #region pincode Master

    public class CYGNUS_pincodemasterViewModel
    {
        public List<CYGNUS_pincode_master> listWPM { get; set; }
        public CYGNUS_pincode_master WPM { get; set; }
    }

    #endregion

    #region Country Master
    public class CYGNUS_CountryViewModel
    {
        public List<CYGNUX_Master_Countries> listCM { get; set; }
        public CYGNUX_Master_Countries CM { get; set; }
    }
    #endregion

    #region Lane Master
    public class CYGNUS_LaneViewModel
    {
        public List<CYGNUS_LaneMaster> listCL { get; set; }
        public CYGNUS_LaneMaster CL { get; set; }
    }
    #endregion

    #region Vehicle Model Master

    public class CYGNUS_Vehicle_ModelViewModel
    {
        public CYGNUS_Vehicle_Model WVT { get; set; }
        public bool EditFlag { get; set; }
    }

    #endregion

    #region Vehicle Master
    public class VehicleViewModel
    {
        public CYGNUS_Vehicle_Master VehicleHDR { get; set; }
        public string EditFlag { get; set; }
    }

    #endregion

    #region Driver master
    public class DriverViewModel
    {
        public List<CYGNUS_FLEET_DRIVERMST> listWFDM { get; set; }
        public CYGNUS_FLEET_DRIVERMST WFDM { get; set; }
        public List<CYGNUS_FLEET_DRIVER_DOCDET> listWFDDD { get; set; }
        public CYGNUS_FLEET_DRIVER_DOCDET WFDDD { get; set; }
    }
    #endregion

    #region  Document Type Master
    public partial class DOCU_TYPEViewModel
    {
        public List<CYGNUS_FLEET_DOCU_TYPE_MST> ListDocType { get; set; }
        public CYGNUS_FLEET_DOCU_TYPE_MST ModelDocType { get; set; }
        public string EditFlag { get; set; }
    }

    public partial class CYGNUS_FLEET_DOCU_TYPE_MST
    {
        public int DOCU_TYPE_ID { get; set; }
        public string DOCU_TYPE { get; set; }
        public string DECS { get; set; }
        public string APPLICABLE_STATE { get; set; }
        public int RENEW_AUTH_ID { get; set; }
        public string COST_CAPTURED { get; set; }
        public string ACTIVE_FLAG { get; set; }
    }
    #endregion

    #region Fuel Station Master

    public class CYGNUS_Master_FuelStationViewModel
    {
        public CYGNUS_Master_FuelStation FuelStation { get; set; }
        public string ShapeType { get; set; }
        public decimal? Radius { get; set; }
        public string Geom { get; set; }
    }
    #endregion

    #region Card Master
    public class CYGNUS_Master_CardViewModel
    {
        public CYGNUS_Master_Card Card { get; set; }
    }
    #endregion

    #region Card Assignment Master
    public class CYGNUS_Master_Card_AssignmentViewModel
    {
        public CYGNUS_Master_Card_Assignment Assignment { get; set; }
    }
    #endregion

    #region Consignee Master
    public class CYGNUS_ConsigneeViewModel
    {
        public List<Cygnus_Consignee_Master> listCCM { get; set; }
        public Cygnus_Consignee_Master CCM { get; set; }
    }
    #endregion

    #region Expense Master
    public class CYGNUS_Master_ExpenseViewModel
    {
        public CYGNUS_Master_Expense Expense { get; set; }
    }
    #endregion

    #region Geofence Master
    public class CYGNUS_Master_GeofenceViewModel
    {
        public CYGNUS_Master_Geofence Geofence { get; set; }
    }
    #endregion

    #region Vendor Master
    public class VendorViewModel
    {
        public List<CYGNUS_VENDOR_HDR> listVendor { get; set; }
        public CYGNUS_VENDOR_HDR VendorHDR { get; set; }
        public CYGNUS_VENDOR_DET VendorDET { get; set; }
        public List<CYGNUS_Master_General> ListGnMST { get; set; }
        public List<CYGNUS_location> locationList { get; set; }
        public string EditFlag { get; set; }
        public List<CygnusVendorGSTDetails> stateGSTList { get; set; }
        public Cygnus_Vendor_Document VendorDoc { get; set; }
        public List<Cygnus_Vendor_Document> ListVendorDoc { get; set; }
    }
    #endregion
}