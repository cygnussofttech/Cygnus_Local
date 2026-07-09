using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for WebXConvert
/// </summary>
public static class SYSConvert
{
    public static double ToDouble(object num)
    {
        try
        {
            return Convert.ToDouble(num);
        }
        catch (Exception)
        {
            return 0;
        }
    }
    public static Int16 ToInt16(object num)
    {
        double iNum;
        try
        {
            iNum = Math.Round(Convert.ToDouble(num));
            return Convert.ToInt16(iNum);
        }
        catch (Exception)
        {
            return 0;
        }
    }
    public static Int32 ToInt32(object num)
    {
        double iNum;
        try
        {
            iNum = Math.Round(Convert.ToDouble(num));
            return Convert.ToInt32(iNum);
        }
        catch (Exception)
        {
            return 0;
        }
    }
    public static Int64 ToInt64(object num)
    {
        double iNum;
        try
        {
            iNum = Math.Round(Convert.ToDouble(num));
            return Convert.ToInt64(iNum);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// Converts to DateTime from string using different cultures
    /// </summary>
    /// <param name="date"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(string date, string culture)
    {
        System.Globalization.CultureInfo cif = new System.Globalization.CultureInfo(culture);
        DateTime dt;
        try
        {
            dt = Convert.ToDateTime(date, cif);
        }
        catch (Exception)
        {
            dt = DateTime.MinValue;
        }
        return dt;
    }

    public static bool ToBoolean(string Y_N)
    {
        return (Y_N.ToUpper().CompareTo("Y") == 0) ? true : false;
    }

    public static string ToY_N(bool flag)
    {
        return flag ? "Y" : "N";
    }

}
