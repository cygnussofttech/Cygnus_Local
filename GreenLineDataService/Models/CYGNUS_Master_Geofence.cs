using System;
using System.Collections.Generic;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Geofence
    {
        public int Id { get; set; }
        public string GeofenceName { get; set; }
        public string DepartmentType { get; set; }
        public string DepartmentTypeName { get; set; }
        public string AreaType { get; set; }
        public int Duration { get; set; }
        public decimal Speed { get; set; }
        public string GeofenceLocation { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string State { get; set; }
        public decimal Radius { get; set; }
        public string ShapeType { get; set; }
        public string Geom { get; set; }
        public string Remark { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsManual { get; set; }
        public bool IsActive { get; set; }
        public List<CYGNUS_Master_Geofence_Points> GeofencePoints { get; set; }
    }

    public class CYGNUS_Master_Geofence_Points
    {
        public int Id { get; set; }
        public int GeofenceId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int SequenceNo { get; set; }
    }
}
