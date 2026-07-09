using GreenLineDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.ViewModel
{
    public class TrackViewModel
    {
        public vw_OperationTrack opTrackModel { get; set; }
        public CYGNUS_FLEET_VEHICLE_ISSUE TripTrackModel { get; set; }
        public List<ChgangeLoc> ListLocation { get; set; }
        public List<vw_OperationTrack> ListOPTrack { get; set; }

        public string TripSheetStatus { get; set; }
        public string Branch { get; set; }
        public string Vehiclewise { get; set; }
        public string Driverwise { get; set; }
        public string Type { get; set; }
        public string ManualDocNo { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string DocketNo { get; set; }
        public List<CYGNUS_FLEET_VEHICLE_ISSUE> TripTrackModelList { get; set; }
        public TripsheetTrackingViewModel TripView { get; set; }
    }

    public class vw_OperationTrack
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string DockType { get; set; }
        public string From_loc { get; set; }
        public string From_br { get; set; }
        public string To_loc { get; set; }
        public string To_br { get; set; }
        public string DOCKNO { get; set; }
        public string ManualNo { get; set; }
        public string VehicleNo { get; set; }
        public string FreeText { get; set; }
        public string MaxLocVal { get; set; }
        public string MinLocVal { get; set; }
        public string invno { get; set; }
        public Int64 ROWNO { get; set; }
        public string Docno { get; set; }
        public string ManualDocNo { get; set; }
        public string cancelled { get; set; }
        public decimal totdockets { get; set; }
        public string Origin { get; set; }
        public string Dest { get; set; }
        public int TotalRecords { get; set; }
        public string doc_dt { get; set; }
        public string Deldt { get; set; }
        public string Origin_dest { get; set; }
        public string Curr_Next { get; set; }
        public string From_to { get; set; }
        public string Type { get; set; }
        public string docsf { get; set; }
        public string AWBNO { get; set; }
        public string Cnor { get; set; }
        public string Cnee { get; set; }
        public string doc_no { get; set; }
        public int TotalPkg { get; set; }
        public string Thcdt { get; set; }
        public string thcno { get; set; }
        public string Vehno { get; set; }
        public string ACK { get; set; }
        public string ACKDT { get; set; }
        public string doc_veh { get; set; }
        public string status { get; set; }
    }

    public partial class TripsheetTrackingViewModel
    {
        public CYGNUS_FLEET_VEHICLE_ISSUE FVI { get; set; }
        public Driver_Settlement_Attachment DSA { get; set; }
    }

    public class CYGNUS_FLEET_DOCUMENT_MST
    {
        public string DOCUMENT_NO { get; set; }
        public string DOCUTYPE { get; set; }
        public string DOCUDESC { get; set; }
        public string STDT { get; set; }
        public string EXDT { get; set; }
        public string RENEWAUTU { get; set; }
        public string RENEWAL_AUTU_NAME { get; set; }
        public string APPLICABLE_STATE { get; set; }
        public string DOCU_COST { get; set; }
        public int Will_ExpiredInDays { get; set; }
        public string LOCATION { get; set; }

        public string SCANDOCUMENT { get; set; } 
        public string ENTRY_BY { get; set; } 
    }

    public class vw_VT_tripsheet
    {
        public string VSlipNo { get; set; }
        public string Vslipdt { get; set; }
        public string Manual_tripsheetno { get; set; }
        public string Driver_Name1 { get; set; }
        public string Driver_Name2 { get; set; }
        public string Tripsheet_startLocName { get; set; }
        public decimal f_issue_startKM { get; set; }
        public string Tripsheet_EndLocName { get; set; }
        public decimal f_closure_CloseKM { get; set; }
        public decimal TotDiesel { get; set; }
        public decimal Actual_KMPL { get; set; }
        public decimal Approved_KMPL { get; set; }
        public decimal TotExpense { get; set; }
        public decimal AmtPaidToDriver { get; set; }
        public string category { get; set; }
        public string Status { get; set; }
    }

    public class VW_VEH_FUEL_FILL_HISTORY
    {
        public string VSlipNo { get; set; }
        public string BILLFROMDT { get; set; }
        public string BILLNO { get; set; }
        public string DIESEL_LTR { get; set; }
        public decimal Diesel_Rate { get; set; }
        public string EXE_AMT { get; set; }
        public string KM_READING { get; set; }
        public string Card_Cash { get; set; }
        public decimal EXE_Diesel_Ltr { get; set; }
        public decimal EXE_Diesel_Rate { get; set; }
    }

    public partial class VW_ODOMETER_HISTORY
    {
        public string Odometer_Reading_Dt { get; set; }
        public decimal Odometer_Reading { get; set; }
        public decimal Last_Km_Reading { get; set; }
        public string Odometer_Reason { get; set; }
    }

    public partial class vm_JobTripViewPrint_Veh
    {
        public string JOB_ORDER_NO { get; set; }
        public string JOB_ORDER_DT { get; set; }
        public string JOB_ORDER_CLOSEDT { get; set; }
        public string VEHNO { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SERVICE_CENTER_TYPE { get; set; }
        public string ORDER_STATUS { get; set; }
        public string VENDOR_CODE { get; set; }
        public string WS_LOCCODE { get; set; }
    }
}