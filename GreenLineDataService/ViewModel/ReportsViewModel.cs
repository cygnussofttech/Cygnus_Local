using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.ViewModel
{
    public class ReportsViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Location { get; set; }
        public string LocLevel { get; set; }

        public List<ReportParameter> RDLparameters { get; set; }
    }

    public class ChartViewModel
    {
        public string ReportCategory { get; set; }
        public int ReportId { get; set; }
        public string ChartType { get; set; }  // pie, bar, etc.
        public string LabelField { get; set; } // e.g., "PaymentType"
        public string ValueFields { get; set; } // e.g., ["Profit", "Income", "Expenses"]
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string DisplayLable { get; set; }


        public int? TopRecords { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
    }
}