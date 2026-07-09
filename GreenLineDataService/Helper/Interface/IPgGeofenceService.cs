using GreenLineDataService.Models;
using System.Collections.Generic;

namespace GreenLineDataService.Helper.Interface
{
    public interface IPgGeofenceService
    {
        List<PG_Geofence> GetAllGeofences();
    }
}
