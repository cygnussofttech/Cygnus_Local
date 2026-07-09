using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    public class CYGNUS_CUSTHDR
    {
        public string BankAccountNo { get; set; }
        public string Decision_Mobile { get; set; }

        public bool IsRegistered { get; set; }

        public string EMAILIDS { get; set; }
        public string OLD_GRPCD { get; set; }
        public string BankName { get; set; }
        public string Website { get; set; }
        public string CUST_ACTIVE { get; set; }
        public string TOPAYELEGIBILITY { get; set; }
        public decimal TBBCRDAYS { get; set; }
        public string InvoicewiseBill_YN { get; set; }
        public string SERVICETAX { get; set; }
        public string GRPCD { get; set; }
        public string Ownership { get; set; }
        public bool IsGstEnable { get; set; }
        public bool IsGTA { get; set; }
        public bool IsBusiness { get; set; }
        public string CustUser { get; set; }
        public string RATEMATRIX { get; set; }
        public string CSTNO { get; set; }
        public string CUSTCD { get; set; }
        public string Decision_Email { get; set; }
        public string pincode { get; set; }
        public string Decision_Name { get; set; }
        public string BranchName { get; set; }
        public string RATETYPE { get; set; }
        public string Industry { get; set; }
        public string customer_hierarchy { get; set; }
        public string Businessname { get; set; }

        public string CUSTNM { get; set; }
        public string TIN_No { get; set; }
        public decimal OCTCRDAYS { get; set; }
        public string CUSTPASS { get; set; }
        public string OLD_CUSTCD { get; set; }
        public string OCTROIELIGIBILITY { get; set; }
        public string pan_no { get; set; }
        public string CntLoc { get; set; }
        public string MOBILENO { get; set; }
        public decimal opendebit { get; set; }
        public decimal DKTCHG { get; set; }
        public string isSysGenerated { get; set; }
        public string CustAddress { get; set; }
        public string MOBSERV_ENABLED { get; set; }
        public string Pincode_Bill { get; set; }
        public decimal opencredit { get; set; }
        public string TBBELEGIBILITY { get; set; }
        public string city { get; set; }
        public string CUSTCAT { get; set; }
        public string BDEREF { get; set; }
        public string ServiceTaxNumber { get; set; }
        public string svc_opted { get; set; }
        public decimal Turnover { get; set; }
        public string ServiceOptFor { get; set; }
        public string Address_Bill { get; set; }
        public decimal TOPAYCRDAYS { get; set; }
        public string STNUM { get; set; }
        public string contract_made { get; set; }
        public string CUSTLOC { get; set; }
        public string MAP_CSGN { get; set; }
        public string MAP_CSGE { get; set; }
        public string Decision_Designation { get; set; }
        public string UPDTBY { get; set; }
        public decimal FOVCARRIER { get; set; }
        public string telno { get; set; }
        public string Cust_State { get; set; }
        public string City_Bill { get; set; }
        public string FaxNo { get; set; }
        public string CUST_ABRV { get; set; }
        public decimal FOVOWNER { get; set; }
        public string IsSpecialBill_YN { get; set; }
        public bool Paid { get; set; }
        public bool TBB { get; set; }
        public bool ToPay { get; set; }
        public bool FOC { get; set; }
        public bool Transportation { get; set; }
        public bool Warehouse { get; set; }
        public string ScheduleApply { get; set; }
        public string Consignee { get; set; }
        public string Consignnor { get; set; }
        public string ThirdParty { get; set; }
        public string ddl_MappingMode { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public string ddl_Case { get; set; }
        public string lbl_Case { get; set; }
        public bool IsChecked { get; set; }
        public string Code { get; set; }
        public string Active { get; set; }
        public string OwnershipStatus { get; set; }

        public string legnamebusiness { get; set; }
        public string TaxpayerType { get; set; }
        public bool Status { get; set; }
        public string Mode { get; set; }
        public string OppCode { get; set; }
        public string StringName { get; set; }
        public string ModeType { get; set; }
        public string Name { get; set; }
        public string Service_Code { get; set; }

        public bool IsAllowForChequeCollection { get; set; }

        /* For TDS*/
        public bool IsTDSApplicable { get; set; }
        public string TANNo { get; set; }
        public decimal TDSPercentage { get; set; }
        public string Paybas { get; set; }

        public List<Cygnus_Cutomer_KMA_Details> listCCKMA { get; set; }
        public int FirstTimeLogin { get; set; }
        public bool BillingAtDestination { get; set; }
        public bool AutoBillAllowed { get; set; }
        public bool ModeWise { get; set; }
        public int CycleType { get; set; }
        public int BillGenType { get; set; }
        public string Remark { get; set; }
        public string BookedType { get; set; }
        public string BookedByFraVendor { get; set; }
        public string BookedByBaVendor { get; set; }
        public string BookedByEmp { get; set; }
        public string BookedBy { get; set; }
        public string EmployeeID { get; set; }
        public string GSTNO { get; set; }
        public bool EInvoice { get; set; }
        public bool EWayBillAPIEnable { get; set; }
        public bool LoadingSlipRecieved { get; set; }

        public bool SEZ { get; set; }
        public bool GSTExempted { get; set; }
        public bool Rail { get; set; }
        public bool Road { get; set; }
        public string BusinessType { get; set; }
        public string BusinessCAT { get; set; }
        public CYGNUS_Customer_Pickup_Address CCPA { get; set; }
        public List<CYGNUS_Customer_Pickup_Address> listCCPA { get; set; }
        public CYGNUS_Customer_Bill_Details CCBD { get; set; }
        public List<CYGNUS_Customer_Bill_Details> listCCBD { get; set; }
        public CYGNUS_Customer_KYC_Details CCKD { get; set; }
        public List<CYGNUS_Customer_KYC_Details> listCCKD { get; set; }
        public string citycode { get; set; }
        public string Cust_Type { get; set; }
        public string AadhaarNumber { get; set; }
        public int BaseCurrency { get; set; }


        /*Add by sneha */
        public string LR_Type { get; set; }
        public string CINNo { get; set; }
        public string LR_Creation { get; set; }
        public string SAP_Code { get; set; }
        public string Customer_Category { get; set; }
        public List<CYGNUS_Customer_Geofence_Details> listCCGD { get; set; }

        /*Add by Hitesh */
        public int AdvancedInstance { get; set; }
        public decimal AdvancedPercentage { get; set; }
    }
}