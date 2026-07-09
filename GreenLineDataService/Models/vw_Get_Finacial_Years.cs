using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class vw_Get_Finacial_Years
    {
        public string FinYear { get; set; }
        public string CurrentFinYear { get; set; }
        public string YearVal { get; set; }
    }
}