using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLine.Classes;

namespace GreenLineDataService.Helper.Service
{
    public class PgGeofenceService : IPgGeofenceService
    {
        private string QueryString = string.Empty;
        readonly PgGeneralFunctions PGF = new PgGeneralFunctions();

        public List<PG_Geofence> GetAllGeofences()
        {
            QueryString = @"SELECT 
                                geofence_id AS ""GeofenceId"", 
                                name AS ""Name"", 
                                state AS ""State"", 
                                geofence_type AS ""GeofenceType"", 
                                shape_type AS ""ShapeType"", 
                                radius AS ""Radius"", 
                                geom::text AS ""Geom"", 
                                created_by AS ""CreatedBy"", 
                                created_date AS ""CreatedDate"", 
                                modified_by AS ""ModifiedBy"", 
                                modified_date AS ""ModifiedDate"" 
                            FROM geofence_master";
            
            DataTable dataTable = PGF.GetDataTableFromQuery(QueryString);
            List<PG_Geofence> GeofenceList = DataRowToObject.CreateListFromTable<PG_Geofence>(dataTable);
            return GeofenceList;
        }
    }
}
