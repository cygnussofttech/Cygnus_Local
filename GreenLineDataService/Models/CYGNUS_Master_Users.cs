using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]

    public partial class CYGNUS_Master_Users
    {
        
        public string UserId { get; set; }
        public string User_Type { get; set; }
        public string UserPwd { get; set; }
        public string BranchCode { get; set; }
        public string MultiLocation { get; set; }
        public string OffcialMobileNo { get; set; }
        public string BRCD { get; set; }
        public string Name { get; set; }
        public string PasswordQues { get; set; }
        public string PasswordAns { get; set; }
        public string EmpId { get; set; }
        public string ManagerId { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<System.DateTime> ActiveTillDate { get; set; }
        public Nullable<System.DateTime> PwdLastChangeOn { get; set; }
        public string LastPwd { get; set; }
        public string Status { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string mobileno { get; set; }

        public string gender { get; set; }
        public string resi_addr { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOJ_ORG { get; set; }
        public string ROLEID { get; set; }
        public string IsPassEncrypted { get; set; }
        public string IsPassReset { get; set; }
        public string emptype { get; set; }

        public string confirmPassword { get; set; }
        public string Name_Of_bank { get; set; }
        public string Bank_AC_Number { get; set; }
        public string IFSC_Code { get; set; }
        public string Designation { get; set; }
        public string User_Image { get; set; }
        public string Department { get; set; }
        public string HOD { get; set; }
        public string Grade { get; set; }
        public decimal ConveyanceExpance { get; set; }
        public string Message { get; set; }

        public string BrachName { get; set; }
        public string Menu_1 { get; set; }
        public string Menu_2 { get; set; }
        public string Menu_3 { get; set; }
        public string Category { get; set; }
        public string Read_Witre { get; set; }

        public string PersonalEmail { get; set; }
        public string BloodGroup { get; set; }
        public string VoterID { get; set; }
        public string PANNumber { get; set; }
        public string AadharNumber { get; set; }
        public string Version { get; set; }

        public string EduQualification { get; set; }
        public string VoterID_Image { get; set; }
        public string PAN_Image { get; set; }
        public string Aadhar_Image { get; set; }
        public string Edu_Image { get; set; }
        public string VoterIDContentType { get; set; }
        public string PanContentType { get; set; }
        public string AadharContentType { get; set; }
        public string EduContentType { get; set; }
        public Nullable<System.DateTime> WeddingDate { get; set; }
        public int Ratings { get; set; }
        public string ShowTBBCharges { get; set; }
        public string ShiftCode { get; set; }
        public int LeaveGroupId { get; set; }
        public string Manual_EmployeeNo { get; set; }
        public string usertypedesc { get; set; }
        public string EmTypDes { get; set; }
        public string COMPANY_LIST { get; set; }
        public string DEFAULT_COMPANY { get; set; }

        public decimal driver_id { get; set; }
    }

    public partial class CYGNUS_View_Track
    {
        public string UserId { get; set; }
        public string ReportId { get; set; }
        public string chacked { get; set; }
        public string Level0Text { get; set; }
        public string Level1Text { get; set; }
        public string DOC_Called_AS { get; set; }
        public string L0_App_Module { get; set; }
        public decimal L1_App_Module { get; set; }
        public string App_Module { get; set; }
        public bool New_chacked { get; set; }
    }

    public partial class ModuleAcessModel
    {
        public string UserID { get; set; }
        public string Chk { get; set; }
        public string Name { get; set; }
        public string BranchCode { get; set; }
        public string HasAccess { get; set; }
        public bool New_chacked { get; set; }
        public string Text { get; set; }
        public string App_Module { get; set; }
        public string Level1Text { get; set; }
        public string Level2Text { get; set; }
        public string Level3Text { get; set; }

        public string Level4Text { get; set; }
        public bool HasAccessNew { get; set; }

    }
    public partial class CYGNUS_ChequeCount
    {
        public int ChequeCount { get; set; }

    }
    public partial class CYGNUS_PendingAmount
    {
        public decimal PendingAmount { get; set; }

    }
}