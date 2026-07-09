using GreenLineDataService.Models;
using System.Collections.Generic;

namespace GreenLineDataService.Helper.Interface
{
    public interface IPgTruckService
    {
        List<PG_Truck> GetAllTrucks();
        PG_Truck GetTruckById(string id);
    }
}
