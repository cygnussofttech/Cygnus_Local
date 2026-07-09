using System;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public partial class CYGNUS_State
    {
        public decimal srno { get; set; }
        public string stcd { get; set; }
        public string stnm { get; set; }
        public string countrycd { get; set; }
        public string CountryName { get; set; }
        public string activeflag { get; set; }
        public bool activeflag2 { get; set; }
        public bool IsUnionTerritory { get; set; }
        public string statePrefix { get; set; }
        public string Zone { get; set; }
        public string zonename { get; set; }
        public string entryby { get; set; }
        public Nullable<System.DateTime> entrydt { get; set; }
        public string lasteditby { get; set; }
        public Nullable<System.DateTime> lasteditdate { get; set; }

    }
}

