using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_VehicleDriver_Mapping
    {
        public int Id {  get; set; }   
        public string VehicleId { get; set; }
        public string VehicleNo { get; set; }
        public int First_Driver { get; set; }
        public string First_Driver_name { get; set; }
        public decimal First_Driver_Odometer { get; set; }
        public string First_Driver_Latitude { get; set; }
        public string First_Driver_Longitude { get; set; }
        public int Second_Driver { get; set; }
        public string Second_Driver_name { get; set; }
        public decimal Second_Driver_Odometer { get; set; }
        public string Second_Driver_Latitude { get; set; }
        public string Second_Driver_Longitude { get; set; }
        public bool IsAttached { get; set; }
        public string EntryDate { get; set; }
        public string EntryBy { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}