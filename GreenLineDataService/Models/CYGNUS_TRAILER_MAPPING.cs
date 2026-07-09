using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_TRAILER_MAPPING
    {
        public int Id { get; set; }
        public string VehicleId { get; set; }
        public string TrailerId { get; set; }
        public string VehicleNo { get; set; }
        public string TrailerNo { get; set; }
        public string type { get; set; }
        public string Target_Customer { get; set; }
        public string Branch { get; set; }
        public string Primary_Supervisor { get; set; }
        public string Secondary_Supervisor { get; set; }
        public string Cluster_Head { get; set; }

        public bool IsAttached { get; set; }
        public string EntryDate { get; set; }
        public string EntryBy { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}