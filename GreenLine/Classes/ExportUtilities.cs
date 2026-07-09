using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Ajax.Utilities;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GreenLine.Classes
{
    public class ExportUtilities
    {
        public static void ExportToZipCSV(DataTable dt, string fileName, string additionHeader = "")
        {
            ExportToZipCSV(dt, fileName.Replace(@"/", "_"), "", additionHeader);
        }
        public static void ExportToZipCSV(DataTable dt, string fileName, string filter, string additionHeader = "")
        {
            ExportToZipCSV(dt, fileName, filter, true, additionHeader);
        }
        public static void ExportToZipCSV(DataTable dt, string fileName, string filter, bool WriteHeader, string additionHeader = "")
        {
            
            ExportToZipCSV(dt, HttpContext.Current.Response, fileName, filter, WriteHeader, additionHeader);
        }
        public static void ExportToZipCSV(string csvString, string fileName)
        {
            ExportToZipCSV(csvString, HttpContext.Current.Response, fileName);
        }

        public static void ExportToZipCSV(DataTable dt, HttpResponse Response, string fileName, string additionHeader = "")
        {
            ExportToZipCSV(dt, Response, fileName, "", additionHeader);
        }
        public static void ExportToZipCSV(DataTable dt, HttpResponse Response, string fileName, string filter, string additionHeader = "")
        {
            ExportToZipCSV(dt, Response, fileName, filter, true, additionHeader);
        }
        public static void ExportToZipCSV(DataTable dt, HttpResponse Response, string fileName, string filter, bool WriteHeader, string additionHeader = "")
        {
            string CSVOutput = "";
            if (filter != "")
                CSVOutput += filter + Environment.NewLine;
            CSVOutput += ProduceCSVString(dt, WriteHeader, additionHeader);
            ExportToZipCSV(CSVOutput, Response, fileName);
        }
        public static void ExportToZipCSV(string csvString, HttpResponse Response, string fileName)
        {
            StringBuilder str = new StringBuilder();

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".zip");
            Response.Charset = "";

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/x-zip-compressed";

            ZipOutputStream outputStream = new ZipOutputStream(Response.OutputStream);
            ZipEntry entry = new ZipEntry(fileName + ".csv"); entry.DateTime = DateTime.Now;

            outputStream.PutNextEntry(entry);
            using (StreamWriter sw = new StreamWriter(outputStream))
            {
                sw.Write(csvString);
            }

            // Close the zip file:
            outputStream.Finish();
            outputStream.Close();

            // Now end the response.
            Response.End();
        }

        public static string ProduceCSVString(DataTable sourceTable, bool includeHeaders, string additionHeader = "")
        {
            var str = new StringBuilder();

            if (!additionHeader.IsNullOrWhiteSpace())
            {
                str.AppendLine(additionHeader);
            }

            if (includeHeaders)
            {
                str.AppendLine(String.Join(",", (from DataColumn column in sourceTable.Columns select QuoteValue(column.ColumnName)).ToArray()));
            }
            string[] items = null;
            foreach (DataRow row in sourceTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString())).ToArray();
                str.AppendLine(String.Join(",", items));
            }
            return str.ToString();
        }
        private static string QuoteValue(string value)
        {
            return String.Concat("\"", value.Replace("\"", "\"\""), "\"");
        }

        public static void ExportToCSV(DataTable dt, string fileName, bool includeHeaders = true, string additionHeader = "")
        {
            HttpResponse response = HttpContext.Current.Response;

            string csvContent = ProduceCSVString(dt, includeHeaders, additionHeader);

            response.Clear();
            response.Buffer = true;
            response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".csv");
            response.Charset = "";
            response.ContentType = "text/csv";

            response.Output.Write(csvContent);
            response.Flush();
            response.End();
        }

    }
}