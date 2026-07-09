using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{

    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_VENDOR_DET
    {
        public string VENDORCODE { get; set; }
        public string VENDORBRCD { get; set; }
        public string VENDORCONTACT { get; set; }
        public string VENDORADDRESS { get; set; }
        public string VENDORCITY { get; set; }
        public string VENDORPIN { get; set; }
        public string VENDORPHONE { get; set; }
        public string VENDORMOBILE { get; set; }
        public string VENDOREMAIL { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string BLACKLISTED { get; set; }
        public decimal COST_PERMONTH { get; set; }
    }
}