using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_Consignor_Consignee_Lane_Mapping
    {
        public int Id { get; set; }
        public string ConsignorCode { get; set; }
        public string Consignor { get; set; }
        public string ConsigneeCode { get; set; }
        public string Consignee { get; set; }
        public int LaneId { get; set; }
        public string Lane { get; set; }
        public int Lane_ID { get; set; }
        public bool IsApprove { get; set; }
        
    }
}