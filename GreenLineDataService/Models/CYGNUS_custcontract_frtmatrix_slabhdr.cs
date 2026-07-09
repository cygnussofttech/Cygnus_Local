using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]

    public class CYGNUS_custcontract_frtmatrix_slabhdr
    {
        public string trans_type { get; set; }
        public int srno { get; set; }
        public string ContractId { get; set; }
        public decimal slab_to { get; set; }
        public decimal slab_from { get; set; }
        public string slab_code { get; set; }
        public string rate_type { get; set; }

        public decimal Slab1From { get; set; }
        public decimal Slab1To { get; set; }
        public string Slab1Type { get; set; }


        public decimal Slab2From { get; set; }
        public decimal Slab2To { get; set; }
        public string Slab2Type { get; set; }

        public decimal Slab3From { get; set; }
        public decimal Slab3To { get; set; }
        public string Slab3Type { get; set; }

        public decimal Slab4From { get; set; }
        public decimal Slab4To { get; set; }
        public string Slab4Type { get; set; }

        public decimal Slab5From { get; set; }
        public decimal Slab5To { get; set; }
        public string Slab5Type { get; set; }

        public decimal Slab6From { get; set; }
        public decimal Slab6To { get; set; }
        public string Slab6Type { get; set; }


        public string slab_code1 { get; set; }
        public string slab_code2 { get; set; }
        public string slab_code3 { get; set; }
        public string slab_code4 { get; set; }
        public string slab_code5 { get; set; }
        public string slab_code6 { get; set; }


        public decimal Slab1 { get; set; }
        public decimal Slab2 { get; set; }
        public decimal Slab3 { get; set; }
        public decimal Slab4 { get; set; }
        public decimal Slab5 { get; set; }
        public decimal Slab6 { get; set; }
        public string matrix_type { get; set; }



        public decimal MIN_ODACHRG { get; set; }
        public string ESS_code { get; set; }
        public decimal ESS_from { get; set; }
        public decimal ESS_to { get; set; }

        public decimal trdays { get; set; }
    }
}