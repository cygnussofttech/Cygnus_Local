using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_GRPMST
    {
        public string GRPCD { get; set; }
        public string GRPNM { get; set; }
        public string GRP_Pwd { get; set; }
        public string locregion { get; set; }
        public string ActiveFlag { get; set; }
        public string OLD_GRPCD { get; set; }
        public string UPDTBY { get; set; }
        public Nullable<System.DateTime> UPDTON { get; set; }
        public string isSysGenerated { get; set; }
    }
}