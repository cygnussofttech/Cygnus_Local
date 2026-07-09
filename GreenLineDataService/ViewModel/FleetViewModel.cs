using GreenLineDataService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace GreenLineDataService.ViewModel
{
    public class FleetViewModel
    {
    }

    #region PickUp Request
    public class CYGNUS_PickUpRequestViewModel
    {
        public List<CYGNUS_PickUp_Request> listCPR { get; set; }
        public CYGNUS_PickUp_Request CPR { get; set; }
    }
    #endregion

    public class Driver_Settlement_Attachment
    {
        public int ID { get; set; }
        public string VSlipNo { get; set; }
        public string FilePath { get; set; }
        public string ENTRY_BY { get; set; }
        public string COMPANY_CODE { get; set; }
        public DateTime ENTRY_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string REMARK { get; set; }
        public string FIN_YEAR { get; set; }
    }

    #region Trip

    public class CYGNUS_TripViewModel
    {
        public List<CYGNUS_Trip_Master> listCTM { get; set; }
        public CYGNUS_Trip_Master CTM { get; set; }
        public List<Cygnus_Master_Event> listEvents { get; set; }
        public List<CYGNUS_Master_Vehicle_Event> listVehicleEvents { get; set; }
        public CYGNUS_FLEET_ENROUTE_EXP EnrtExp { get; set; }
        public CYGNUS_TripNotes TripNote { get; set; }
        public CYGNUS_All_Trip_Activity AllTrip { get; set; }
        public List<CYGNUS_TripStageActivity> listStageActivities { get; set; }
        public CYGNUS_Vehicle_Event VehicleEvent { get; set; }

        public List<CYGNUS_TripDocket> listDockets { get; set; }
    }
    public class CYGNUS_TripDocket
    {
        public string LR_No { get; set; }
        public string CustLRNo { get; set; }
        public string invoice_no { get; set; }
        public decimal? Total { get; set; }
        public string Consignee_Name { get; set; }
        public string Consignee_Address { get; set; }
        public string Consignee_City { get; set; }
        public string Consignee_Pincode { get; set; }
        public decimal? gross_weight { get; set; }
        public decimal? pieces_count { get; set; }
        public string LRUpload { get; set; }
        public string PODPath { get; set; }
        public string PODLink { get; set; }
        public int IsUploadAllowed { get; set; }
        public int? StopId { get; set; }
    }

    public class CYGNUS_TripStageActivity
    {
        public int StageId { get; set; }
        public string StageName { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsStopLevel { get; set; }
        public bool IsDocketLevel { get; set; }
        public bool IsFlexible { get; set; }
        public string PRNo { get; set; }
        public string VehicleNo { get; set; }
        public string TripNo { get; set; }
        public string DocketNo { get; set; }
        public int? StopId { get; set; }
        public string StageStatus { get; set; }
        public bool IsCompleted { get; set; }
        public string PreviousStageId { get; set; }
        public string NextStageId { get; set; }
        public bool IsRule { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
    }

    public class CYGNUS_TripNotes
    {
        public long Id { get; set; }
        public string TripNo { get; set; }
        public string VehicleId { get; set; }
        public string NoteType { get; set; }
        public string NoteSubType { get; set; }
        public string Notes { get; set; }
        public string Document { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string NoteTypeDesc { get; set; }
        public string NoteSubTypeDesc { get; set; }
    }

    public class CYGNUS_All_Trip_Activity
    {
        public string PRNo { get; set; }
        public string VehicleID { get; set; }
        public string TripNo { get; set; }
        public string DocketNo { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }

        public string Status { get; set; }
        public string Severity { get; set; }
        public string Remarks { get; set; }
        public string EntryDate { get; set; }
        public string EntryBy { get; set; }
        public decimal? Latitude { get; set; }     // legacy – kept for compatibility
        public decimal? Longitude { get; set; }    // legacy – kept for compatibility
        public decimal? Start_Latitude { get; set; }
        public decimal? Start_Longitute { get; set; }  // matches SP column name (typo intentional)
    }

    public class CYGNUS_Vehicle_Event
    {
        public int EventId { get; set; }
        public string TripNo { get; set; }
        public string PRNo { get; set; }
        public string VehicleId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int NoOfTyers { get; set; }
        public string VehicleType { get; set; }
        public string VehicleBreakDownType { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string FromDate { get; set; }
        public bool IsVehicleReplaceRequired { get; set; }
        public string JobOrderType { get; set; }
        public string Attachment { get; set; }
        public string VehicleDocs { get; set; }
    }

    #endregion
	
	#region Consignor Consignee Lane Master
	public class CCLMModel
	{
		public List<Cygnus_Consignor_Consignee_Lane_Mapping> listCCCLM { get; set; }
		public Cygnus_Consignor_Consignee_Lane_Mapping CCCLM { get; set; }
	}
	#endregion
}