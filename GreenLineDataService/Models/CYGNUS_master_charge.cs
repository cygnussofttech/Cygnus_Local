using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_master_charge
    {
        public string chargecode { get; set; }
        public string chargename { get; set; }
        //public string operator1 { get; set; }
        //public string varybyloc { get; set; }
        //public string basecode { get; set; }
        //public string deltimeflag { get; set; }
        //public string chargetype { get; set; }
        //public string booktimeflag { get; set; }
        //public DateTime lasteditdate { get; set; }
        //public string lasteditby { get; set; }
        //public string entryby { get; set; }
        //public DateTime entrydate { get; set; }
        //public string basedon { get; set; }
        //public string Narration { get; set; }
        //public string Acccode { get; set; }
        //public string activeflag { get; set; }
        //public string use_to { get; set; }
        //public string use_trans_type { get; set; }
        //public string slab_type { get; set; }
        //public string use_from { get; set; }
        //public string use_slab { get; set; }
        //public string use_rate_type { get; set; }
    }
}