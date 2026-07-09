using System;

namespace GreenLineDataService.Models
{
    public class PG_Truck
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string LicensePlate { get; set; }
        public string ExternalTruckId { get; set; }
    }
}
