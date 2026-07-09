using System;

namespace GreenLineDataService.Models
{
    public class PG_Geofence
    {
        public int GeofenceId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string GeofenceType { get; set; }
        public string ShapeType { get; set; }
        public decimal Radius { get; set; }
        public string Geom { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
