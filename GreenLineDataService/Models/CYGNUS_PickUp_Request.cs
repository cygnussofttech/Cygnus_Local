using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_PickUp_Request
    {

        public string PRNo { get; set; }
        public string Customer_Group { get; set; }
        public string CustomerGroup { get; set; }
        public string Customer_Code { get; set; }
        public string Customer { get; set; }
        public int From_Lane_id { get; set; }   
        public int To_Lane_id { get; set; }
        public string From_Lane { get; set; }
        public string To_Lane{ get; set; }
        public string Lane { get; set; }
        public string Contract_Id { get; set; }   
        public string Organization { get; set; }   
        public string Pickup_Address { get; set; }   
        public string DropLocation { get; set; }
        public decimal Drop_Lat { get; set; }
        public decimal Drop_Long { get; set; }
        public string Material { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalKMs { get; set; }
        public string RateType { get; set; }
        public decimal Rate { get; set; }
        public decimal Commission { get; set; }
        public decimal Freight { get; set; }
        public string Remarks { get; set; }
        public string Type { get; set; }
        public string IsApprove { get; set; }
        public string IsReject { get; set; }
        public string EntryBy { get; set; }
        public string ServiceType { get; set; }
        public string IsCancle { get; set; }
        public List<CYGNUS_FLEET_ENROUTE_EXP> EnrtExpList { get; set; }
        public List<CYGNUS_PickUp_Request> listCPR { get; set; }
        public int? AdvancedInstance { get; set; }
        public decimal? AdvancedPercentage { get; set; }

        //public DateTime EntryDate { get; set; }
    }
}