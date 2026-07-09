using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Vehicle_GPS_Current_Details
    {
        public string VehicleAddress { get; set; }
        public string THC_Attached_YN { get; set; }
        public string Ignition { get; set; }
        public string Longitude { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ReportingTime { get; set; }
        public string APIProviderURL { get; set; }
        public string VehicleSpeed { get; set; }
        public string JsonResponse { get; set; }
        public string THCORGN { get; set; }
        public int SRNO { get; set; }
        public string Latitude { get; set; }
        public string THCDEST { get; set; }
        public string THCNO { get; set; }
        public string VehicleNo { get; set; }
        public bool IsChecked { get; set; }
        public string Description { get; set; }
        public string VehicleLat { get; set; }
        public string VehicleLong { get; set; }
    }
}