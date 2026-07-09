using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Customer_Bill_Details
    {
        public string Bill_GSTIN { get; set; }
        public string Bill_GST_State { get; set; }
        public string Bill_Legal_Business_Name { get; set; }
        public string Bill_Address { get; set; }
        public string Bill_State { get; set; }
        public string Bill_City { get; set; }
        public string Bill_Pincode { get; set; }
        public string Bill_Country { get; set; }
        public string Bill_SAP_Code { get; set; }
        public string Bill_PAN { get; set; }
        public string Bill_TAN { get; set; }
        public string Bill_Mobile { get; set; }
        public string Bill_Email { get; set; }
        public string Customer { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string GSTState { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public bool IsDefault { get; set; }

    }
}