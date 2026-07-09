using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Trip_Master
    {
        public string TripNo { get; set; }
        public string PRNo { get; set; }
        public int Event_id { get; set; }
        public string EventName { get; set; }

        public int Id { get; set; }

        public DateTime TripDate { get; set; }
        public string TType { get; set; }
        public int Start_City { get; set; }
        public string Start_City_name { get; set; }
        public int End_City { get; set; }
        public string End_City_name { get; set; }

        public string Start_Location { get; set; }
        public string End_Location { get; set; }

        public string Start_Lane { get; set; }
        public string End_Lane { get; set; }

        public string Start_Address { get; set; }
        public string End_Address { get; set; }

        public decimal Start_Km { get; set; }
        public decimal End_Km { get; set; }

        public decimal Start_Latitude { get; set; }
        public decimal Start_Longitude { get; set; }
        public decimal End_Latitude { get; set; }
        public decimal End_Longitude { get; set; }

        public decimal Start_Fuel_Quntity { get; set; }
        public decimal End_Fuel_Quntity { get; set; }

        public string VehicleNo { get; set; }
        public string VehicleId { get; set; }
        public string Vehicle_Mode { get; set; }
        public string TrailerNo { get; set; }

        public int Driver1 { get; set; }
        public int Driver2 { get; set; }
        public string Driver1Name { get; set; }
        public string Driver2Name { get; set; }
        public string Category { get; set; }
        public string Customer_code { get; set; }
        public string ContractId { get; set; }
        public string DocumentNo { get; set; }

        public string Market_Own { get; set; }
        public string CityRouteCode { get; set; }
        public decimal CityRouteKM { get; set; }

        public string Current_Location { get; set; }
        public string Last_Arrived_Location { get; set; }

        public string Attached { get; set; }
        public string Billed { get; set; }
        public string Billno { get; set; }

        public string Billing_Type { get; set; }
        public string Billing_Wise { get; set; }
        public string Billed_Location { get; set; }

        public decimal Advance_Amt { get; set; }
        public decimal Opening_Balance { get; set; }
        public decimal Balance_Amt { get; set; }
        public decimal Rate { get; set; }

        public decimal UFL_Weight { get; set; }
        public decimal AFL_Weight { get; set; }
        public decimal Total_Weight { get; set; }
        public decimal Total_Freight { get; set; }

        public decimal Total_Advance_Expence { get; set; }
        public decimal Total_Oil_Expence { get; set; }
        public decimal Total_Repair_Expence { get; set; }
        public decimal Total_Claims_Amount { get; set; }
        public decimal Total_Enroute_Expence { get; set; }
        public decimal Total_IncDed_Expence { get; set; }
        public decimal Total_Spare_Expence { get; set; }

        public decimal Actual_KMPL { get; set; }
        public decimal Approved_KMPL { get; set; }

        public string PreparedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AuditedBy { get; set; }

        public DateTime Oper_Close_Date { get; set; }
        public decimal Oper_Close_Km { get; set; }

        public DateTime Fin_Close_Date { get; set; }
        public decimal Fin_Close_Km { get; set; }

        public DateTime DriverSettleDt { get; set; }

        public bool IsEmpty { get; set; }

        public string FUEL_SLIP_FLAG { get; set; }
        public string Oper_dt_Updated { get; set; }

        public string Cancel_Status { get; set; }
        public string Cancel_By { get; set; }
        public DateTime Cancel_Date { get; set; }
        public string Cancel_Remarks { get; set; }

        public string COMPANY_CODE { get; set; }

        public string Entry_by { get; set; }
        public DateTime Entry_date { get; set; }

        public string Update_By { get; set; }
        public DateTime Update_Date { get; set; }
        public bool IsRuleConfigured { get; set; }
        public int TripNotesCount { get; set; }
        public int TimeLineCount { get; set; }
        public int VehicleEventCount { get; set; }
        public int DispatchLRCount { get; set; }
        public int TotalExpenceCount { get; set; }
        public string VehicleCode { get; set; }
    }
}