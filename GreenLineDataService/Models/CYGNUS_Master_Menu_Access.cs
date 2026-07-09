using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_Master_Menu_Access
    {
        public string UserId { get; set; }
        public int MenuId { get; set; }
        public Nullable<bool> HasAccess { get; set; }
    }
}