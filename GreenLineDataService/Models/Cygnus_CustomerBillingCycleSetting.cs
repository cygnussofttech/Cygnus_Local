using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_CustomerBillCycleSetting
    {
        public int Id { get; set; }
        public string CUSTCD { get; set; }      
        public int BillGenDate { get; set; }
        public string BillGenDateRange { get; set; }
        public string DateRange { get; set; }
        public string CodeId { get; set; }
        public string CodeType { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }

        public bool ModeWise { get; set; }
        public int CycleType { get; set; }
        public int BillGenType { get; set; }
        public string billgen_loccode { get; set; }
    }
}