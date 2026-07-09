using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_Consignee_Master
    {
        public int Id { get; set; }
        public string Consignee_State_Name { get; set; }
        public string Consignee_City_Name { get; set; }
        public string Consignee_Country_Name { get; set; }
        public string Consignee_Lane_Name { get; set; }
        public string Consignee_Code { get; set; }
        public string Consignee_Name { get; set; }
        public string Consignee_GST { get; set; }
        public string Consignee_PANNo { get; set; }
        public string Consignee_Address { get; set; }
        public string Consignee_Pincode { get; set; }
        public string Consignee_Country { get; set; }
        public string Consignee_State { get; set; }
        public string Consignee_City { get; set; }
        public string Consignee_Location { get; set; }
        public decimal Unloading_Lat { get; set; }
        public decimal Unloading_Long { get; set; }
        public string Consignee_Lane { get; set; }
        public decimal Radius { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GeoFenceLatitude { get; set; }
        public string GeoFenceLongitude { get; set; }
        public string Contract_Id { get; set; }
        public string ActiveFlag { get; set; }
        public string EntryBy { get; set; }
        public string UpdateBy { get; set; }
        public string ShapeType { get; set; }
        public string Geom { get; set; }
    }
}
