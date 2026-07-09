using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Reports_Parameters
    {
        public int ID { get; set; }
        public string ParameterName { get; set; }
        public string ReplaceParameterName { get; set; }
        public string DefaultValue { get; set; }

        public int ReportID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefaultParameter { get; set; }
    }
}