using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_FLEET_DRIVERMST
    {
        public string Driver_Status { get; set; }
        public string P_Address { get; set; }
        public string Guarantor_Name { get; set; }
        public string D_category { get; set; }
        public DateTime D_Lic_Current_Issuance_Date { get; set; }
        public DateTime Date_of_Joining { get; set; }
        public DateTime Date_of_ReJoining { get; set; }
        public DateTime Date_of_Exit { get; set; }
        public string Reason_for_Exit { get; set; }
        public string ID_Proof { get; set; }
        public string Manual_Driver_Code { get; set; }
        public string Driver_Location_BKP { get; set; }
        public string Driver_Type_ID { get; set; }
        public string DFather_Name { get; set; }
        public decimal Driver_Id { get; set; }
        public string C_City { get; set; }
        public string C_Address { get; set; }
        public DateTime D_Lic_Initial_Issuance_Date { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string P_Pincode { get; set; }
        public string License_No { get; set; }
        public DateTime UpdatedDt { get; set; }
        public DateTime Valdity_dt { get; set; }
        public DateTime Valdity_Todt { get; set; }
        public string Valdity_date { get; set; }
        public string Valdity_Todate { get; set; }
        public string ActiveFlag { get; set; }
        public string Address_Proof { get; set; }
        public string UpdatedBy { get; set; }
        public string Mobileno { get; set; }
        public string Driver_Name { get; set; }
        public string Driver_Location { get; set; }
        public string C_Pincode { get; set; }
        public string Issue_By_RTO { get; set; }
        public DateTime D_DOB { get; set; }
        public string D_Ethnicity_Id { get; set; }
        public string P_City { get; set; }
        public string Telno { get; set; }
        public string Driver_Photo { get; set; }
        public string VEHNO { get; set; }
        public string Name_Of_bank { get; set; }
        public string Bank_AC_Number { get; set; }
        public string IFSC_Code { get; set; }

        public string id { get; set; }
        public string text { get; set; }

        public string DriverAccountCode { get; set; }

        public string Date_Joining { get; set; }

        public string Date_DOB { get; set; }

        public string Date_Exit { get; set; }
        public string Aadhar_No { get; set; }
        public string Passport_No { get; set; }
        public string PAN_No { get; set; }
        public string Vaccine_No { get; set; }
        public string Voter_Id { get; set; }
        public string Police_Verification { get; set; }
        public string DeiverPhotoBase64 { get; set; }
        public string UserId { get; set; }
    }
}