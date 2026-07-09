using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_VENDOR_HDR
    {
        public string Vendor_Type { get; set; }
        public string VENDORCODE { get; set; }
        public string VENDORTYPE { get; set; }
        public string VENDORNAME { get; set; }
        public Nullable<System.DateTime> CONTRACTSTDT { get; set; }
        public Nullable<System.DateTime> CONTRACTENDDT { get; set; }
        public string VENDORREMARKS { get; set; }
        public string UPDTBY { get; set; }
        public Nullable<System.DateTime> UPDTON { get; set; }
        public string vendcat { get; set; }
        public string PAN_NO { get; set; }
        public string SERVTAXNO { get; set; }
        public string vendorbstno { get; set; }
        public string vendorcstno { get; set; }
        public string Default_Addr { get; set; }
        public string Active { get; set; }
        //public DateTime? Vendor_Date { get; set; }
        public string Contract_Market { get; set; }
        public string Contract_YN { get; set; }
        public Nullable<System.DateTime> Contract_Dt { get; set; }
        public Nullable<decimal> opendebit { get; set; }
        public Nullable<decimal> opencredit { get; set; }
        public string ITEMSSUPLIED { get; set; }
        public string IFSCCODE { get; set; }
        public string TINNO { get; set; }
        public string CINNO { get; set; }
        public string VATNO { get; set; }
        public string ECCNO { get; set; }
        public string EXCISERANGE { get; set; }
        public string EXCISEDIVISION { get; set; }
        public string EXCISECOLLECTORATE { get; set; }
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
        public string EntryBy { get; set; }
        public string VendorGST { get; set; }
        public string IsGstEnable { get; set; }
        //public Nullable<System.DateTime> EntryDate { get; set; }

        public bool activeservice { get; set; }

        public string VENDORCODENAME { get; set; }
        public string NewVENDORCODENAME { get; set; }
        public string vendorcodeValue { get; set; }
        public string vendornameText { get; set; }
        public string BankId { get; set; }
        public string BankIFSCCode { get; set; }
        public string RateType { get; set; }
        public bool Porter { get; set; }
        //public DateTime? Expirydate { get; set; }
        public string Filing_Freq { get; set; }
        //public bool MSME { get; set; }
        //public string MSMEMobNo { get; set; }
        //public string MSMENo { get; set; }
        //public string MSMECat { get; set; }

      //  public DateTime MSMEDate { get; set; }
        public string Taxpayer { get; set; }

        public DateTime? DATEFROM { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public string THCNo { get; set; }
        public string PDCNo { get; set; }
        public int Id { get; set; }

        //balance
        public string RO { get; set; }
        public string location { get; set; }
        public DateTime? DATETO { get; set; }
        public string DOC_NO { get; set; }
        public string THCDate { get; set; }
        public string VehicleNo { get; set; }
        public decimal THCAmount { get; set; }
        public string BalanceLocation { get; set; }
        public string SelectionType { get; set; }
        public bool IsChecked { get; set; }
        public string BaseLoccode { get; set; }

        //Load-Unload
        public string BranchCode { get; set; }
        public string Type { get; set; }

        public string NBLocation { get; set; }
        public bool IsGSTApplied { get; set; }
        public string RateID { get; set; }

        public bool EInvo_App { get; set; }

        public decimal CommissionPercentage { get; set; }
        public string AadhaarNumber { get; set; }
        public string DueDays { get; set; }

       
        public string BusinessNature { get; set; }
        public DateTime? DOCancellation { get; set; }
        public string NatureOfPPoB { get; set; }
        public string LegalName { get; set; }
        public string Ownership { get; set; }

        public string VENDORCITY { get; set; }


    }
}