using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Linq;

namespace GreenLine.Classes
{
    public class PgGeneralFunctions
    {
        private string Connstr
        {
            get { return ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ConnectionString; }
        }

        public string GetConnstr()
        {
            return Connstr;
        }

        public void ExecuteNonQuery(string Squery, NpgsqlParameter[] parameters = null)
        {
            using (var conn = new NpgsqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetDataTableFromQuery(string Squery, NpgsqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable
            {
                TableName = "datatable1"
            };
            using (var conn = new NpgsqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 6000;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        public string ExecuteScalerQuery(string Squery, NpgsqlParameter[] parameters = null)
        {
            object Scaler = null;
            using (var conn = new NpgsqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    conn.Open();
                    Scaler = cmd.ExecuteScalar();
                }
            }
            return Scaler != null ? Scaler.ToString() : "";
        }

        /// <summary>
        /// Safely reads a value from a DataRow, returning default if column doesn't exist or is DBNull
        /// </summary>
        public T GetValueFromRow<T>(DataRow row, string columnName)
        {
            try
            {
                if (!row.Table.Columns.Contains(columnName) || row.IsNull(columnName))
                    return default(T);

                return (T)Convert.ChangeType(row[columnName], typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
