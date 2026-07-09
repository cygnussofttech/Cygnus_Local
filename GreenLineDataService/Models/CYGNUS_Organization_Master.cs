using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;


/*
 add class CYGNUS_Organization_Master
 */

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public partial class CYGNUS_Organization_Master
    {
        public CYGNUS_COMPANY_MASTER OrgDet { get; set; } = new CYGNUS_COMPANY_MASTER();
        public Organization_Bank_Details OrgBnkDet { get; set; } = new Organization_Bank_Details();
        public List<Organization_Bank_Details> OrgBnkDetList { get; set; } = new List<Organization_Bank_Details>();
    }

    public class CYGNUS_COMPANY_MASTER
    {
        public decimal SRNO { get; set; }
        public string COMPANY_CODE { get; set; } = "0";
        public string GST_Number { get; set; }
        public string PANNo { get; set; }
        public string BRCD { get; set; }
        public string COMPANY_NAME { get; set; }
        public string Biling_Type { get; set; }
        public string Biling_Name { get; set; }
        public string Country_Name { get; set; }
        public string Country_Code { get; set; }
        public string State_Name { get; set; }
        public string State_Code { get; set; }
        public string City_Name { get; set; }
        public decimal City_Code { get; set; }
        public string Sap_Code { get; set; }
        public string ADDRESS { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PinCode { get; set; }
        public string Location { get; set; }
        public string CONTACTNO { get; set; }
        public string Email { get; set; }
        public string CINNumber { get; set; }
        public string Employee_Code { get; set; }
        public string Invoice_Bottom_Address { get; set; }
        public string Trip_Series { get; set; }
        public string Register_Office_Address { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string TermsAndCondition { get; set; }
        public string LR_Terms_And_Condition { get; set; }
        public string Credit_Debit_Terms_And_Condition { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string EInvoice_User_Name { get; set; }
        public string EInvoice_Password { get; set; }
        public decimal EInvoice_ASP_Id { get; set; }
        public decimal EInvoice_Client_Id { get; set; }
        public string ACTIVEFLAG { get; set; } = "Y";
        public bool IsActive { get; set; } = true;
    }

    public class Organization_Bank_Details
    {
        public int SrNo { get; set; }
        public string Beneficiary_Name { get; set; }
        public string Bank_Name { get; set; }
        public string Bank_Account_No { get; set; }
        public string Branch_Name { get; set; }
        public string IFSC_Code { get; set; }
        public string MICR_Code { get; set; }
        public string RTGS_NEFT_Code { get; set; }
        public string Bank_Address { get; set; }
        public bool IsActive { get; set; }
    }
}