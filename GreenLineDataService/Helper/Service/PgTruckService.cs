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
    public class PgTruckService : IPgTruckService
    {
        private string QueryString = string.Empty;
        readonly PgGeneralFunctions PGF = new PgGeneralFunctions();

        public List<PG_Truck> GetAllTrucks()
        {
            QueryString = "SELECT id AS \"Id\", vendor_id AS \"VendorId\", license_plate AS \"LicensePlate\", external_truck_id AS \"ExternalTruckId\" FROM trucks";
            DataTable dataTable = PGF.GetDataTableFromQuery(QueryString);
            List<PG_Truck> TruckList = DataRowToObject.CreateListFromTable<PG_Truck>(dataTable);
            return TruckList;
        }

        public PG_Truck GetTruckById(string id)
        {
            QueryString = "SELECT id AS \"Id\", vendor_id AS \"VendorId\", license_plate AS \"LicensePlate\", external_truck_id AS \"ExternalTruckId\" FROM trucks WHERE id = @id";
            NpgsqlParameter[] parameters = { new NpgsqlParameter("@id", id) };
            DataTable dataTable = PGF.GetDataTableFromQuery(QueryString, parameters);
            PG_Truck TruckData = DataRowToObject.CreateListFromTable<PG_Truck>(dataTable).FirstOrDefault();
            return TruckData;
        }
    }
}
