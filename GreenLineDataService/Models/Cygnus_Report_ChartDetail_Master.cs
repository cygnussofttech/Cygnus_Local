using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_Report_ChartDetail_Master
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string ReportCategory { get; set; }
        public string LabelField { get; set; }
        public string DisplayLable { get; set; }
        public string ChartType { get; set; }
        public string ValueFields { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public int TopRecords { get; set; }
        public int ChartOrder { get; set; }
    }
}