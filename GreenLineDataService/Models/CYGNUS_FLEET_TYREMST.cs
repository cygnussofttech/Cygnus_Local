using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_FLEET_TYREMST
    {
        public string TYRE_ID { get; set; }
        public string TYRE_TYPEID { get; set; }
        public string TYRE_SIZEID { get; set; }
        public Nullable<System.DateTime> TYRE_PURDT { get; set; }
        public Nullable<decimal> TYRE_PUR_KMS { get; set; }
        public string TYRE_OE { get; set; }
        public Nullable<decimal> TYRE_COST { get; set; }
        public string TYRE_VEHNO { get; set; }
        public string TYRE_LOCATION { get; set; }
        public Nullable<System.DateTime> TYRE_WARRANTY_EXPDT { get; set; }
        public Nullable<decimal> TYRE_WARRANTY_EXPKMS { get; set; }
        public string TYRE_VENDOR { get; set; }
        public string TYRE_TREAD_DEPTH_32NDS { get; set; }
        public Nullable<decimal> Last_TREAD_DEPTH_32NDS { get; set; }
        public Nullable<decimal> TYRE_PRESSURE_PSI { get; set; }
        public string TYRE_AXLE_CONFIG { get; set; }
        public string TYRE_WHEEL_CONFIG { get; set; }
        public string TYRE_STATUS { get; set; }
        public string TYRE_ACTIVEFLAG { get; set; }
        public string TYRE_ENTRYBY { get; set; }
        public Nullable<System.DateTime> TYRE_ENTRYDT { get; set; }
        public Nullable<decimal> TYRE_TOTKM_RUN { get; set; }
        public string TYRE_NO { get; set; }
        public string MFG_ID { get; set; }
        public Nullable<decimal> TOTAL_COST { get; set; }
        public string TYRE_CATEGORY { get; set; }
        public string TYRE_TREAD_PATTERN { get; set; }
        public Nullable<int> VEH_INTERNAL_NO { get; set; }
        public Nullable<int> TYREPOS_ID { get; set; }
        public string TYRE_PATTERN_CODE { get; set; }
        public Nullable<int> TYRE_MODEL_ID { get; set; }
        public int POS_CATEGORY { get; set; }
        public Nullable<decimal> TYRE_OPKM_RUN { get; set; }
        public string TYRE_REPLACE_NO { get; set; }
        public Nullable<decimal> MOUNT_KM_RUN { get; set; }
        public Nullable<decimal> DISMOUNT_KM_RUN { get; set; }
        public Nullable<decimal> MANUAL_TYRE_OPKM_RUN { get; set; }
        public Nullable<decimal> MANUAL_MOUNT_KM_RUN { get; set; }
        public Nullable<decimal> MANUAL_DISMOUNT_KM_RUN { get; set; }
        public string COMPANY_CODE { get; set; }


        public string CATEGORY { get; set; } //TYRE_CATEGORY
        public string MFG_NAME { get; set; } //MFG_ID
        public string MODEL_NO { get; set; } //TYRE_MODEL_ID
        public string SIZE_NAME { get; set; } //TYRE_SIZEID
        public string TYREPAT_CODE { get; set; } //TYRE_PATTERN_CODE

        public string MODEL { get; set; }
        public string TYRESIZE { get; set; }
        public string PATTERN { get; set; }
        public string TYRETYPE { get; set; }
        public string PURCHASED_AS { get; set; }
        public string TYREPOS_CODE { get; set; }
        public string TYREPOS_DESC { get; set; }
        public Nullable<decimal> TOTALCOST { get; set; }
        public string COSTKM { get; set; }
        public string MFG { get; set; }
    }
}