using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public partial class CYGNUS_citymaster
    {
        public string Location { get; set; }
        public string Region { get; set; }
        public string book_loc { get; set; }
        public string Del_loc { get; set; }
        public string state { get; set; }
        public string oda_yn { get; set; }
        public Nullable<float> ODAkm { get; set; }
        public string Book_loc_air { get; set; }
        public string Del_loc_air { get; set; }
        public string Book_loc_Rail { get; set; }
        public string Del_loc_Rail { get; set; }
        public string activeflag { get; set; }
        public decimal city_code { get; set; }
        public string permit_yn { get; set; }

        public decimal oldcity_code { get; set; }
        public decimal oldstate { get; set; }
        public string Locations { get; set; }
        public int Code { get; set; }
    }
}