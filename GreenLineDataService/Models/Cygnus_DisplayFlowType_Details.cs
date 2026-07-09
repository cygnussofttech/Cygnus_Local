using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_DisplayFlowType_Details
    {
        public int Id { get; set; }
        public int Srno { get; set; }
        public string CompareText { get; set; }
        public string DisplayText { get; set; }
        public string DocumentType { get; set; }
        public string DisplayType { get; set; }
        public string CLR { get; set; }
    }
}