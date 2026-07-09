using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public partial class CYGNUS_pincode_master
    {
        public int pincode { get; set; }
        public int ID { get; set; }
        public string StateCode { get; set; }
        public string cityname { get; set; }
        public string Area { get; set; }
        public string ActiveFlag { get; set; }
        public string EntryBy { get; set; }
        public Nullable<System.DateTime> Entrydt { get; set; }
        public string LocCode { get; set; }
        public string District { get; set; }
        public string Region { get; set; }
        public string Service_Type { get; set; }

        /* PinCode Base ODA Changes*/
        public decimal KM_From_Location { get; set; }
        public string Handling_Location { get; set; }
        public string Is_ODA_Apply { get; set; }
        public string PinArea { get; set; }
        public int pincode1 { get; set; }
        public string Location { get; set; }
        public string StateName { get; set; }
    }
}
