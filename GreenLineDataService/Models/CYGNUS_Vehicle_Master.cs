using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Vehicle_Master
    {
        public string ID { get; set; }
        public bool IsRegistered { get; set; }
        public string VehicleCode { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string VehicleGroup { get; set; }
        public string VendorType { get; set; }
        public string VendorCode { get; set; }
        public string ManufacturerName { get; set; }
        public string VehicleModel { get; set; }
        public string PermitState { get; set; }
        public string InsuranceType { get; set; }
        public string VehicleStatus { get; set; }
        public bool IsActive { get; set; }
        public int TyreCount { get; set; }
        public int StepneyTyreCount { get; set; }

        public bool IsWarranty { get; set; }
        public DateTime WarrantyFrom { get; set; }
        public DateTime WarrantyTo { get; set; }
        public bool IsAMC { get; set; }
        public DateTime AMCFrom { get; set; }
        public DateTime AMCTo { get; set; }

        public string RCBookNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string ManufactureYearAndMonth { get; set; }
        public string BodyType { get; set; }
        public decimal ULW { get; set; }
        public decimal RLW { get; set; }
        public decimal PLW { get; set; }

        public string FuelType { get; set; }
        public string FuelUOM { get; set; }
        public decimal FuelTankCapacity { get; set; }

        public decimal AvgLoad { get; set; }
        public decimal AvgLessHalf { get; set; }
        public decimal AvgMoreHalf { get; set; }
        public decimal AvgEmpty { get; set; }

        public decimal PurchaseValue { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string EMIPaymentType { get; set; }
        public decimal DownPayAmt { get; set; }
        public decimal EmiAmt { get; set; }
        public int EmiDay { get; set; }
        public int TotalEmi { get; set; }

        public decimal SaleAmt { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal ScrapValue { get; set; }

        public string VehiclePhoto { get; set; }
        public string TrackingId { get; set; }
        public string Location { get; set; }

        public List<CYGNUS_Vehicle_Document_Type> DocumentList { get; set; }
    }

    public class VehicleDetails
    {
        public string stautsMessage { get; set; }
        public string rc_regn_no { get; set; }
        public string rc_regn_dt { get; set; }
        public string rc_regn_upto { get; set; }
        public string rc_purchase_dt { get; set; }
        public string rc_owner_sr { get; set; }
        public string rc_owner_name { get; set; }
        public string rc_f_name { get; set; }
        public string state_cd { get; set; }
        public string rto_cd { get; set; }
        public string rc_present_address { get; set; }
        public string rc_permanent_address { get; set; }
        public string rc_mobile_no { get; set; }
        public string rc_vch_catg { get; set; }
        public string rc_vh_class_desc { get; set; }
        public string rc_chasi_no { get; set; }
        public string rc_eng_no { get; set; }
        public string rc_maker_desc { get; set; }
        public string rc_maker_model { get; set; }
        public string rc_body_type_desc { get; set; }
        public string rc_fuel_desc { get; set; }
        public string rc_color { get; set; }
        public string rc_norms_desc { get; set; }
        public string rc_fit_upto { get; set; }
        public string rc_np_upto { get; set; }
        public string rc_np_issued_by { get; set; }
        public string rc_tax_upto { get; set; }
        public string rc_financer { get; set; }
        public string rc_insurance_comp { get; set; }
        public string rc_insurance_policy_no { get; set; }
        public string rc_insurance_upto { get; set; }
        public string rc_manu_month_yr { get; set; }
        public string rc_unld_wt { get; set; }
        public string rc_gvw { get; set; }
        public string rc_no_cyl { get; set; }
        public string rc_cubic_cap { get; set; }
        public string rc_seat_cap { get; set; }
        public string rc_sleeper_cap { get; set; }
        public string rc_wheelbase { get; set; }
        public string rc_registered_at { get; set; }
        public string rc_status_as_on { get; set; }
        public string rc_pucc_upto { get; set; }
        public string rc_pucc_no { get; set; }
        public string rc_status { get; set; }
        public string rc_permit_no { get; set; }
        public string rc_permit_issue_dt { get; set; }
        public string rc_permit_valid_from { get; set; }
        public string rc_permit_valid_upto { get; set; }
        public string rc_permit_type { get; set; }
        public string rc_permit_code { get; set; }
    }

    public class CYGNUS_Vehicle_Document_Type
    {
        public int Id { get; set; }
        public string Vehicle_DocumentType { get; set; }
        public string Document_Id { get; set; }
        public string Document_Name { get; set; }
        public string ShortName { get; set; }
        public bool Is_RequireFor_Own { get; set; }
        public bool Is_RequireFor_Market { get; set; }
        public bool Is_Active { get; set; }
        public string DocPath { get; set; } // To store the uploaded file name
        public string DocumentNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}