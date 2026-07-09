using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_ParameterSet_Details_SP
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string SQLParamName { get; set; }
        public string RDLParameter { get; set; }
        public string DefaultValue { get; set; }
        public string ParameterValue { get; set; }
    }
}