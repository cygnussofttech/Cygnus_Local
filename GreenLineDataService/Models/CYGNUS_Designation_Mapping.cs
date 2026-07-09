using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Designation_Mapping
    {
        public decimal SrNo { get; set; }
        public string Category { get; set; }
        public string Designation { get; set; }
        public string Active { get; set; }
        public string Entryby { get; set; }
        public DateTime Entrydt { get; set; }
        public string Updateby { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}