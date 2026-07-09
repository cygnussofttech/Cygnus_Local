using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_Cutomer_KMA_Details
    {
        public int SRNO { get; set; }
        public string KMATypedesc { get; set; }

        public string KMATypeId { get; set; }
        public string CustomerCode { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}