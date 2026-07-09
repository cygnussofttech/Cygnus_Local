using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_FuelStation
    {
        public int Id { get; set; }
        public string StationName { get; set; }
        public string UgelroId { get; set; }
        public string GeoAddress { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string ContactPersonEmail { get; set; }
        public bool IsActive { get; set; }
        public string Company_Code { get; set; }
        public string Entry_By { get; set; }
        public DateTime Entry_On { get; set; }
        public string Update_By { get; set; }
        public DateTime Update_On { get; set; }

        public string StateName { get; set; }
        public string CityName { get; set; }
    }
}