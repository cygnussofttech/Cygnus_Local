

namespace GreenLineDataService.Models
{


    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class Cygnus_Vendor_Document
    {
        public int Id { get; set; }
        public string Remark { get; set; }
        public string ImagePath { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string VendorId { get; set; }

    }
}