using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_master_CustAddresses
    {
        public string Address { get; set; }
        public string MapAddress { get; set; }
        public string City { get; set; }
        public string AddID { get; set; }
        public string ActiveFlag { get; set; }
        public string Email { get; set; }
        public DateTime UpdateDate { get; set; }
        public decimal srno { get; set; }
        public string Pincode { get; set; }
        public string UpdateBy { get; set; }
        public string PhoneNo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LocState { get; set; }
        public string Loc_Level { get; set; }
        public string LocStateCode { get; set; }
        public string Loc_LevelCode { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string Customer { get; set; }
        public string CUSTCD { get; set; }
        public string CUSTNM { get; set; }
        public string GeoFenceLatitude { get; set; }
        public string GeoFenceLongitude { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }

    }
}