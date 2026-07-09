using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Customer_Pickup_Address
    {
        public string PickUp_Address { get; set; }
        public string PickUp_City { get; set; }
        public string PickUp_Country { get; set; }
        public string PickUp_Pincode { get; set; }
        public string PickUp_State { get; set; }
        public string Zone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Radius { get; set; }
        public string GeoFenceLatitude { get; set; }
        public string GeoFenceLongitude { get; set; }


        public string AddID { get; set; }
        public string Geom { get; set; }
        public string ActiveFlag { get; set; }
        public int srno { get; set; }
        public string UpdateBy { get; set; }
        //public DateTime UpdateDate { get; set; }
        public string Customer { get; set; }
        public string EntryBy { get; set; }
        public int Lane_id { get; set; }
        public bool IsDeleted { get; set; }
        //public DateTime EntryDate { get; set; }
    }
}