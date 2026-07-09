using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_FLEET_ENROUTE_EXP
    {
        public string id { get; set; }
        public string Trip_Exp_Header { get; set; }
        public decimal Amount_Spent { get; set; }
        public decimal Exe_Appr_Amt { get; set; }
        public string Polarity { get; set; }
        public bool IsBilled { get; set; }
        public bool IsApprove { get; set; }
        public int SRNO { get; set; }
        public decimal Total { get; set; }
        public string PymentType { get; set; }
        public string PRNO { get; set; }
        public string TripsheetNo { get; set; }
        public string VehicleNo { get; set; }
        public string DocPath { get; set; }
        public string ExpenseName { get; set; }
        public DateTime? Entrydt { get; set; }
    }
}