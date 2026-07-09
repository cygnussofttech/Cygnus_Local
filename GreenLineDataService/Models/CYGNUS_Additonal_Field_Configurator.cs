using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_Additonal_Field_Configurator
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Label { get; set; }

        public string DataType { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string Required { get; set; }
        public string ContractId { get; set; }

    }
}