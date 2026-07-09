using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Report_General
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string CodeType { get; set; }
        public string CodeId { get; set; }
        public string CodeDescription { get; set; }
        public bool IsActive { get; set; }
    }
}