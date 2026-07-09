using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]

    public class CYGNUS_BILL_CONFIGURATION
    {
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string CustCode { get; set; }
        public string BillTo { get; set; }
        public string HSN_Code { get; set; }
        public string Bill_Format { get; set; }
        public int Max_Trip_Count { get; set; }
        public string Bank_Details { get; set; }
        public string Item_Description { get; set; }
        public string Single_Line_Item { get; set; }
        public string Trip_Wise { get; set; }
        public string  Rate_Wise { get; set; }
        public string Special_Notes { get; set; }
        public string CompanyCode { get; set; }
        public string Bill_Org { get; set; }

        //Billing Information
        public string PodRequired { get; set; }
        public string PodSubmission { get; set; }
        public string BillSubmission { get; set; }
        public int DocumentType { get; set; }

        public bool CreditLimitActive { get; set; }
        public decimal credit_limit { get; set; }
        public int credit_day { get; set; }
        public int GracePeriod { get; set; }
        public decimal GraceLimit { get; set; }

        public string bill_inst { get; set; }
        public string billgen_loccode { get; set; }
        public string billsub_loccode { get; set; }
        public string billcol_loccode { get; set; }
        public string bill_schedule { get; set; }

        public string bank_guarantee_Y_N { get; set; }
        public decimal bank_amount { get; set; }
        public string bank_bill_details { get; set; }
        public DateTime? bank_Validity_date { get; set; }
        public string deposit_Y_N { get; set; }
        public decimal deposit_Amount { get; set; }
        public int termination_days { get; set; }
        public string panlty_clause_Y_N { get; set; }
        public string panlty_details { get; set; }

        //public bool isDone { get; set; }
    }
}