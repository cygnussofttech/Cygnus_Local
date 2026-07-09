using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_LaneMaster
    {
        public int Lane_ID { get; set; }
        public string Lane_Name { get; set; }
        public string State { get; set; }
        public string City{ get; set; }
        public string State_name { get; set; }
        public string City_name { get; set; }
        public string Zone { get; set; } 
        public string Zone_desc { get; set; } 
    }
}