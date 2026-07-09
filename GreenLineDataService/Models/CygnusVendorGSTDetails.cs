using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CygnusVendorGSTDetails
    {
        public int id { get; set; }
        public string vendcode { get; set; }
        public string statecode { get; set; }
        public string gst_registration_no { get; set; }
        public bool isActive { get; set; }
        public string entryby { get; set; }
        public DateTime entrydate { get; set; }
        public string updateby { get; set; }
        public DateTime updatedate { get; set; }
        public string statename { get; set; }
        public string statePrefix { get; set; }
    }
}