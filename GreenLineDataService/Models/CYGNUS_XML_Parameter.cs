using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    public class CYGNUS_XML_Parameter
    {
        public string Name { get; set; }
        public string Values { get; set; }
        public string Visible { get; set; }
    }

    [XmlRoot(ElementName = "Values")]
    public class Values
    {

        [XmlElement(ElementName = "string")]
        public string String { get; set; }
    }

    [XmlRoot(ElementName = "ReportParameter")]
    public class ReportParameterNew
    {

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Values")]
        public Values Values { get; set; }

        [XmlElement(ElementName = "Visible")]
        public bool Visible { get; set; }
    }

    [XmlRoot(ElementName = "ArrayOfReportParameter")]
    public class ArrayOfReportParameter
    {

        [XmlElement(ElementName = "ReportParameter")]
        public List<ReportParameterNew> ReportParameter { get; set; }
    }
}