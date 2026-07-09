using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    public class CYGNUS_custcontract_hdr
    {
        public Decimal Srno { get; set; }
        public string ContractId { get; set; }
        public string Custcode { get; set; }
        public string Bill_Org { get; set; }
        public string PoWoNumber { get; set; }
        public DateTime? Contract_Stdate { get; set; }
        public DateTime? Contract_Eddate { get; set; }
        public string contract_sign_loccode { get; set; }
        public string Cust_represent { get; set; }
        public string Comp_Empname { get; set; }
        public string Remarks { get; set; } 
        public string Contract_Type { get; set; }
        public string Contract_Type1 { get; set; }
        public string conType { get; set; }
        public string Vendor_Code { get; set; }
        public string Status { get; set; }

        public string Doc_Type { get; set; } 
        public string Doc_File { get; set; }

        public string Value { get; set; }
        public string Text { get; set; }
        public bool IsFinalSubmitDone { get; set; }
        public string Primary_Branch { get; set; }
        public string rate_type { get; set; }
    }
}