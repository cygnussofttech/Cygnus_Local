using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_Master_CodeTypes
    {
        public int SrNo { get; set; }
        public string HeaderCode { get; set; }
        public string HeaderDesc { get; set; }
        public string HeaderAccess { get; set; }
        public string ID_desc { get; set; }
        public string Name_Desc { get; set; }
        public string Activeflag_YN { get; set; }
        public string ToSupport { get; set; }

        public decimal Id { get; set; }
    }
}