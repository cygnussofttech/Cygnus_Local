using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Master_ParameterSet_Details
    {
        public int OrderId { get; set; }
        public int SetId { get; set; }
        public int ParameterId { get; set; }
        public string ReportParameterName { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public string ParameterName { get; set; }
        public string SourceTableName { get; set; }
        public bool IsMultipleChoice { get; set; }
        public bool IsMultipleChoiceWithSearch { get; set; }
        public int ParameterType { get; set; }
        public int SubCount { get; set; }
        public bool IsFirstValueSelected { get; set; }
        public string IdColumn { get; set; }
        public string NameColumn { get; set; }
        public string paratype { get; set; }
        public string DefaultValue { get; set; }
        public string BlankValue { get; set; }
        public bool IsRequiredInReport { get; set; }
    }
}