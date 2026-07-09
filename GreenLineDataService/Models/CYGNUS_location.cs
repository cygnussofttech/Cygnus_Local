using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public partial class CYGNUS_location
    {
        //public Nullable<decimal> Loc_Level { get; set; }
        //public Nullable<decimal> Report_Level { get; set; }
        //public string LocCode { get; set; }
        //public string LocName { get; set; }
        //public string Report_Loc { get; set; }
        //public string LocAddr { get; set; }
        //public string LocState { get; set; }
        //public string LocCity { get; set; }
        //public string LocPincode { get; set; }
        //public string LocSTDCode { get; set; }
        //public string LocTelno { get; set; }
        //public string LocFaxno { get; set; }
        //public string Locmobile { get; set; }
        //public string LocEmail { get; set; }
        //public string Loc_Account { get; set; }
        //public string ActiveFlag { get; set; }
        //public string Op_Bkg { get; set; }
        //public string Op_Dly { get; set; }
        //public string Op_Tranship { get; set; }
        //public string OutSource_Own { get; set; }
        //public string Octroi_IO { get; set; }
        //public string AirService { get; set; }
        //public string RailService { get; set; }
        //public string Default_NextLoc { get; set; }
        //public string Nearest_PrevLoc { get; set; }
        //public string CutOff_YN { get; set; }
        //public Nullable<System.DateTime> Bkg_CutOffTime { get; set; }
        //public string DocketGen_Auto { get; set; }
        //public string LocAbbrev { get; set; }
        //public string LocRegion { get; set; }
        //public Nullable<System.DateTime> OP_startdt { get; set; }
        //public Nullable<System.DateTime> OP_enddt { get; set; }
        //public string Computerised { get; set; }
        //public string Dataentry { get; set; }
        //public Nullable<System.DateTime> loc_startdt { get; set; }
        //public Nullable<System.DateTime> loc_enddt { get; set; }
        //public string payment_type { get; set; }
        //public string delivery_type { get; set; }
        //public string UPDTBY { get; set; }
        //public Nullable<System.DateTime> UPDTON { get; set; }
        //public string DKT_PFX { get; set; }
        //public string Mode_Surface { get; set; }
        //public string Mode_Sea { get; set; }
        //public string Pickup_Door { get; set; }
        //public string Pickup_Godown { get; set; }
        //public string BilledAt { get; set; }
        //public string Vol { get; set; }
        //public string COD_DOD { get; set; }
        //public string ODA { get; set; }
        //public string Octroi_Area { get; set; }
        //public Nullable<System.DateTime> Dly_StartDt { get; set; }
        //public Nullable<System.DateTime> Dly_EndDt { get; set; }
        //public Nullable<System.DateTime> Transshipment_StartDt { get; set; }
        //public Nullable<System.DateTime> Transshipment_EndDt { get; set; }
        //public string CPC { get; set; }
        //public string cn_prefixcode { get; set; }
        //public string OP_UGD { get; set; }
        //public decimal locid { get; set; }
        //public string Delivery_Control_Loc { get; set; }
        ////public string WebRefNo { get; set; }
        ////public bool IsQuickGCAllow { get; set; }
        ////public string CAR_GC_StockLocation { get; set; }
        ////public Nullable<bool> IsMRAllow { get; set; }
        ////public string GroupId { get; set; }
        ////public Nullable<System.DateTime> UpdateReportDate { get; set; }
        ////public string Category { get; set; }
        ////public string DispatchLocation_Inventory { get; set; }


        public Nullable<decimal> Loc_Level { get; set; }
        public Nullable<decimal> Report_Level { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string Report_Loc { get; set; }
        public string LocAddr { get; set; }
        public string LocState { get; set; }
        public string LocCity { get; set; }
        public string LocPincode { get; set; }
        public string LocSTDCode { get; set; }
        public string LocTelno { get; set; }
        public string LocFaxno { get; set; }
        public string Locmobile { get; set; }
        public string LocEmail { get; set; }
        public string Loc_Account { get; set; }
        public string ActiveFlag { get; set; }
        public string Op_Bkg { get; set; }
        public string Op_Dly { get; set; }
        public string Op_Tranship { get; set; }
        public string OutSource_Own { get; set; }
        public string Octroi_IO { get; set; }
        public string AirService { get; set; }
        public string RailService { get; set; }
        public string Default_NextLoc { get; set; }
        public string Nearest_PrevLoc { get; set; }
        public string CutOff_YN { get; set; }
        public Nullable<System.DateTime> Bkg_CutOffTime { get; set; }
        public string DocketGen_Auto { get; set; }
        public string LocAbbrev { get; set; }
        public string LocRegion { get; set; }
        public Nullable<System.DateTime> OP_startdt { get; set; }
        public Nullable<System.DateTime> OP_enddt { get; set; }
        public string Computerised { get; set; }
        public string Dataentry { get; set; }
        public Nullable<System.DateTime> loc_startdt { get; set; }
        public Nullable<System.DateTime> loc_enddt { get; set; }
        public string payment_type { get; set; }
        public string delivery_type { get; set; }
        public string UPDTBY { get; set; }
        public Nullable<System.DateTime> UPDTON { get; set; }
        public string DKT_PFX { get; set; }
        public string Mode_Surface { get; set; }
        public string Mode_Sea { get; set; }
        public string Pickup_Door { get; set; }
        public string Pickup_Godown { get; set; }
        public string BilledAt { get; set; }
        public string Vol { get; set; }
        public string COD_DOD { get; set; }
        public string ODA { get; set; }
        public string Octroi_Area { get; set; }
        public Nullable<System.DateTime> Dly_StartDt { get; set; }
        public Nullable<System.DateTime> Dly_EndDt { get; set; }
        public Nullable<System.DateTime> Transshipment_StartDt { get; set; }
        public Nullable<System.DateTime> Transshipment_EndDt { get; set; }
        public string CPC { get; set; }
        public string cn_prefixcode { get; set; }
        public string OP_UGD { get; set; }
        public decimal locid { get; set; }
        public string Delivery_Control_Loc { get; set; }
        public string BarcodeScanAllowed { get; set; }

        //add
        public string PocName { get; set; }
        public string LocationInTime { get; set; }
        public string LocationOutTime { get; set; }


        public string Acccode { get; set; }
        public string Column1 { get; set; }
        public string Accdesc { get; set; }

        public bool IsPeritNoReq { get; set; }
        public bool IsKeyNoReq { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string LocName_LocCode { get; set; }

        public string loc_startdt_str { get; set; }
        public string loc_enddt_str { get; set; }

        public string BankName { get; set; }
        public string IfscCode { get; set; }
        public string AccHolderName { get; set; }
        public string Address { get; set; }
        public string AccountNo { get; set; }
        public decimal BookingLoc_MinFreight { get; set; }
        public decimal TowardsLoc_MinFreight { get; set; }
        public string Cust_Code { get; set; }

        public string  Loc_Type { get; set; }
        public string Loc_Leveldec { get; set; }


    }
}