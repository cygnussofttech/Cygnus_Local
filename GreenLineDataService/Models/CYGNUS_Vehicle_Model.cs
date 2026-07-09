using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Vehicle_Model
    {
        public int Type_Code { get; set; }
        public string Type_Name { get; set; }
        public string Made_By { get; set; }
        public string Model_No { get; set; }
        public string Type_Desc { get; set; }
        public decimal Capacity { get; set; }
        public decimal Rate_Per_KM { get; set; }
        public string Fuel_Type { get; set; }
        public string ActiveFlag { get; set; }
        public string UPDTBY { get; set; }
        public System.DateTime UPDTON { get; set; }
        public decimal usedcapacity { get; set; }
        public decimal Payload { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal Height { get; set; }
        public string Fleet_Type { get; set; }
        public string Vehicle_Type_Category { get; set; }
        public Nullable<decimal> TyreRotate_KM { get; set; }
        public string TYRE_SIZEID { get; set; }
        public Nullable<decimal> CFT { get; set; }
        public string AvgSpeedPerHour { get; set; }
    }
}