using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Vehicle_Allocation
    {
        public int Id { get; set; }
        public string PRNo { get; set; }
        public DateTime PlacementDate { get; set; }
        public string FirstDriver { get; set; }
        public string SecondDriver { get; set; }
        public string TrailerNo { get; set; }

        public string VehicleType { get; set; }
        public string VehicleId { get; set; }
        public int FirstDriverId { get; set; }
        public int SecondDriverId { get; set; }
        public string CompanyCode { get; set; }
        public string EntryBy { get; set; }
        public string UpdateBy { get; set; }
        public string AllocationType { get; set; }
    }
}