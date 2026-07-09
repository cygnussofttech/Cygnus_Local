using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
namespace GreenLine.Controllers
{
    /// <summary>
    /// Summary description for ControllersHelper
    /// </summary>
    public class ControllersHelper
    {
        public ControllersHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static SqlParameter GetSqlParameter(string paramName, object value, SqlDbType sqlTyp, int size, ParameterDirection direction)
        {
            SqlParameter prm = new SqlParameter(paramName, sqlTyp);
            prm.Value = value;
            prm.Size = size;
            prm.Direction = direction;
            return prm;
        }
        public static SqlParameter GetSqlParameter(string paramName, object value, SqlDbType sqlTyp, ParameterDirection direction)
        {
            SqlParameter prm = new SqlParameter(paramName, sqlTyp);
            prm.Value = value;
            prm.Direction = direction;
            return prm;
        }
        public static SqlParameter GetSqlParameter(string paramName, object value, SqlDbType sqlTyp, int size)
        {
            SqlParameter prm = new SqlParameter(paramName, sqlTyp);
            prm.Value = value;
            prm.Size = size;
            return prm;
        }
        public static SqlParameter GetSqlParameter(string paramName, object value, SqlDbType sqlTyp)
        {
            SqlParameter prm = new SqlParameter(paramName, sqlTyp);
            prm.Value = value;
            return prm;
        }
        public static SqlParameter GetSqlParameter(string paramName, object value)
        {
            SqlParameter prm = new SqlParameter(paramName, value);
            return prm;
        }
        public static SqlParameter GetSqlParameter(string paramName, SqlDbType sqlTyp, ParameterDirection direction)
        {
            SqlParameter prm = new SqlParameter(paramName, sqlTyp);
            prm.Direction = direction;
            return prm;
        }
    }
}