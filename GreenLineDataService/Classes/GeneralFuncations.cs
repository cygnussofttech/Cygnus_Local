using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
namespace GreenLine.Classes
{
    public class GeneralFunctions
    {
        private string Connstr
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
        }

        public string FolderPath
        {
            get { return HttpContext.Current.Server.MapPath("~/App_Data/"); }
        }

        public string BackYearUser
        {
            get { return ConfigurationManager.AppSettings["BackYearUser"].ToString(); }
        }

        public string GetConnstr()
        {
            return Connstr;
        }

        public string ExecuteScalerQuery(string Squery)
        {
            ExecuteDBQueryLog(Squery);

            object Scaler = null;
            using (var conn = new SqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Scaler = cmd.ExecuteScalar();
                }
            }
            return Scaler != null ? Scaler.ToString() : "";
        }

        public DataTable GetDataTableFromObjects(object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                Type t = objects[0].GetType();
                DataTable dt = new DataTable(t.Name);
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    dt.Columns.Add(new DataColumn(pi.Name));
                }
                foreach (var o in objects)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;
        }

        public void ExecuteNonQuery(string Squery)
        {
            ExecuteDBQueryLog(Squery);

            using (var conn = new SqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private string ExtractObjectName(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;

            var match = System.Text.RegularExpressions.Regex.Match(
                            message,
                            @"'([^']+)'");

            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }
        public DataTable GetDateTableFromQuery(string Squery)
        {
            ExecuteDBQueryLog(Squery);

            DataTable dataTable = new DataTable
            {
                TableName = "datatable1"
            };

            using (SqlConnection conn = new SqlConnection(Connstr))
            {
                using (SqlCommand cmd = new SqlCommand(Squery, conn))
                {
                    cmd.CommandTimeout = 6000;

                    try
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dataTable);
                    }
                    catch (SqlException ex)
                    {
                        // 208 = Invalid object name
                        // 2812 = Stored procedure not found
                        if (ex.Number == 208 || ex.Number == 2812)
                        {
                            string missingObject = ExtractObjectName(ex.Message);

                            if (!string.IsNullOrEmpty(missingObject))
                            {
                                try
                                {
                                    using (SqlCommand fixCmd = new SqlCommand("AutoRenameRemoveNIU", conn))
                                    {
                                        fixCmd.CommandType = CommandType.StoredProcedure;

                                        fixCmd.Parameters.AddWithValue("@ObjectName", missingObject);
                                        fixCmd.Parameters.AddWithValue("@AppUser", "SNEHU");

                                        conn.Open();
                                        fixCmd.ExecuteNonQuery();
                                        conn.Close();
                                    }

                                    // Retry query once after rename
                                    SqlDataAdapter daRetry = new SqlDataAdapter(cmd);
                                    daRetry.Fill(dataTable);
                                }
                                catch
                                {
                                    throw; // If rename fails, throw original error
                                }
                            }
                            else
                            {
                                throw;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            return dataTable;
        }
        //public DataTable GetDateTableFromQuery(string Squery)
        //{
        //    ExecuteDBQueryLog(Squery);

        //    DataTable dataTable = new DataTable
        //    {
        //        TableName = "datatable1"
        //    };
        //    using (var conn = new SqlConnection(Connstr))
        //    {
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = Squery;
        //            cmd.CommandTimeout = 6000;
        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {
        //                da.Fill(dataTable);
        //            }
        //        }
        //    }
        //    return dataTable;
        //}

        public async Task<DataTable> GetDateTableFromQueryAsync(string Squery)
        {
            ExecuteDBQueryLog(Squery);

            DataTable dataTable = new DataTable
            {
                TableName = "datatable1"
            };
            using (var conn = new SqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandTimeout = 6000;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
            }
            await Task.Delay(100); // Reduced delay
            return dataTable;
        }

        public DataTable GetDataTableFromSP(string Squery)
        {
            return GetDateTableFromQuery(Squery);
        }

        public DataSet GetDataSetFromSP(string Squery)
        {
            return GetDataSetFromQuery(Squery);
        }

        public DataSet GetDataSetFromSP_New(string storedProcedureName, SqlParameter[] parameters = null)
        {
            string queryLog = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(Connstr))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 60;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        conn.Open();
                        DataSet dataSet = new DataSet();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataSet);
                        }

                        // Prepare the log query
                        queryLog = $"EXEC {storedProcedureName} {string.Join(", ", parameters?.Select(p => $"{p.ParameterName}='{p.Value}'") ?? new string[0])}";

                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable { TableName = "Result" };
                dt.Columns.Add("Error", typeof(string));
                dt.Columns.Add("Message", typeof(string));
                DataRow row = dt.NewRow();
                row["Error"] = "1";
                row["Message"] = ex.Message.ToString().Replace('\n', ' ');
                dt.Rows.Add(row);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            }
            finally
            {
                // Execute log after main connection is closed
                if (!string.IsNullOrEmpty(queryLog))
                {
                    ExecuteDBQueryLog(queryLog);
                }
            }
        }

        public DataSet GetDataSetFromQuery(string Squery)
        {
            ExecuteDBQueryLog(Squery);

            DataSet dataSet = new DataSet();
            using (var conn = new SqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataSet);
                    }
                }
            }
            return dataSet;
        }

        public string StreamToJsonString(Stream jsonStream)
        {
            Encoding encoding = Encoding.UTF8;

            // Read the stream into a byte array
            byte[] data = ToByteArray(jsonStream);

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));
                content = content.Replace(delimiter, "");
                delimiter = content.Substring(0, content.IndexOf("\r\n\r\n"));
                content = content.Replace(delimiter, "").Replace("\r\n\r\n", "");
                content = content.Replace("--", "");
            }
            return content;
        }

        private byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public List<T> DictionaryToObjectList<T>(object[] objArr) where T : new()
        {
            List<T> t = new List<T>();
            foreach (var itm in objArr)
            {
                T CMQ = DictionaryToObject<T>((Dictionary<string, object>)itm);
                t.Add(CMQ);
            }
            return t;
        }

        public T DictionaryToObject<T>(IDictionary<string, object> dict) where T : new()
        {
            T t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();
            int i = 0;
            foreach (PropertyInfo property in properties)
            {
                int j = i;
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;
                KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                if (item.Value != null)
                {
                    object newA = Convert.ChangeType(item.Value, newT);
                    t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
                }
                i++;
            }
            return t;
        }

        public Stream ReturnJSONStream<T>(T t)
        {
            var jsonObj = new
            {
                Success = 1,
                Message = "",
                Response = t
            };
            JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string returnValue = jScriptSerializer.Serialize(jsonObj);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(returnValue));
            return memoryStream;
        }

        public string GetTypeInt(int Id)
        {
            string Type;
            if (Id == 0)
            {
                Type = "Add";
            }
            else
            {
                Type = "Edit";
            }
            return Type;
        }

        public string FormateDate(DateTime date)
        {
            return ((date != null && date != DateTime.MinValue) ? date.ToString("dd MMM yy") : "");
        }

        public string FormateDateWithFullYear(DateTime date)
        {
            return ((date != null && date != DateTime.MinValue) ? date.ToString("dd MMM yyyy") : "");
        }

        public string FormateString(string str)
        {
            return ((str != null && str != "") ? str.Trim() : "");
        }

        public string NextKeyCode(string KeyCode)
        {
            byte[] ASCIIValues = ASCIIEncoding.ASCII.GetBytes(KeyCode);
            int StringLength = ASCIIValues.Length;
            bool isAllZed = true;
            bool isAllNine = true;
            //Check if all has ZZZ.... then do nothing just return empty string.

            for (int i = 0; i < StringLength - 1; i++)
            {
                if (ASCIIValues[i] != 90)
                {
                    isAllZed = false;
                    break;
                }
            }
            if (isAllZed && ASCIIValues[StringLength - 1] == 57)
            {
                ASCIIValues[StringLength - 1] = 64;
            }

            // Check if all has 999... then mak/e it A0
            for (int i = 0; i < StringLength; i++)
            {
                if (ASCIIValues[i] != 57)
                {
                    isAllNine = false;
                    break;
                }
            }
            if (isAllNine)
            {
                ASCIIValues[StringLength - 1] = 47;
                ASCIIValues[0] = 65;
                for (int i = 1; i < StringLength - 1; i++)
                {
                    ASCIIValues[i] = 48;
                }
            }


            for (int i = StringLength; i > 0; i--)
            {
                if (i - StringLength == 0)
                {
                    ASCIIValues[i - 1] += 1;
                }
                if (ASCIIValues[i - 1] == 58)
                {
                    ASCIIValues[i - 1] = 48;
                    if (i - 2 == -1)
                    {
                        break;
                    }
                    ASCIIValues[i - 2] += 1;
                }
                else if (ASCIIValues[i - 1] == 91)
                {
                    ASCIIValues[i - 1] = 65;
                    if (i - 2 == -1)
                    {
                        break;
                    }
                    ASCIIValues[i - 2] += 1;

                }
                else
                {
                    break;
                }
            }
            KeyCode = ASCIIEncoding.ASCII.GetString(ASCIIValues, 0, ASCIIValues.Length);
            return KeyCode;
        }

        public string NextKeyCode(string KeyCode, int IncreaseBy)
        {
            string newCode = KeyCode;
            for (int i = 0; i < IncreaseBy; i++)
            {
                newCode = NextKeyCode(newCode);
            }
            string strRet = newCode;
            return strRet;
        }

        public string GetNextSuffix(string suffix)
        {
            string str = suffix;

            if (str == ".")
                str = "A";
            else
                str = NextKeyCode(suffix, 1);
            return str;
        }

        #region Serialize Deserialize

        public void SerializeParams<T>(List<T> paramList, string folderPath)
        {
            XDocument doc = new XDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(paramList.GetType());
            System.Xml.XmlWriter writer = doc.CreateWriter();
            serializer.Serialize(writer, paramList);
            writer.Close();
            doc.Save(folderPath);
        }

        public List<T> DeserializeParams<T>(string folderPath)
        {
            XDocument doc = XDocument.Load(folderPath);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            System.Xml.XmlReader reader = doc.CreateReader();
            List<T> result = (List<T>)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }

        public string GetXmlString(dynamic Object)
        {
            XmlDocument xd = new XmlDocument();
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(Object.GetType());

            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, Object);
                xmlStream.Position = 0;
                xd.Load(xmlStream);
            }
            return xd.InnerXml.ReplaceSpecialCharacters();
        }

        public Stream ReturnJSONStreamError<T>(T t, string ErrorMessage)
        {
            if (ErrorMessage == null)
                ErrorMessage = "Some Error in Submission";

            var jsonObj = new
            {
                Success = 0,
                Message = ErrorMessage,
                Response = t
            };

            JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string returnValue = jScriptSerializer.Serialize(jsonObj);

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(returnValue));

            return memoryStream;
        }

        #endregion

        #region Save SQL to Table

        public int SaveRequestServices(string SaveXMLString, string ModuleName, string Succes, string ErrorMessage)
        {
            int id = 0;
            try
            {
                string sql = "EXEC Usp_CalledServicesAndriod '" + SaveXMLString + "','" + ModuleName + "','" + Succes + "','" + ErrorMessage + "'";
                DataTable Dt = GetDataTableFromSP(sql);
                id = Convert.ToInt32(Dt.Rows[0][0].ToString());
            }
            catch (Exception)
            {
                //  throw;
            }

            return id;
        }

        #endregion

        public string Encrypt(string strToEncrypt, string strKey)
        {
            try
            {
                using (TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider())
                {
                    using (MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider())
                    {
                        byte[] byteHash, byteBuff;
                        string strTempKey = strKey;

                        byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                        objDESCrypto.Key = byteHash;
                        objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                        byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                        return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public string Decrypt(string strEncrypted, string strKey)
        {
            try
            {
                using (TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider())
                {
                    using (MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider())
                    {
                        byte[] byteHash, byteBuff;
                        string strTempKey = strKey;

                        byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                        objDESCrypto.Key = byteHash;
                        objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                        byteBuff = Convert.FromBase64String(strEncrypted);
                        string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));

                        return strDecrypted;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public void DeleteAllFile()
        {
            try
            {
                if (Directory.Exists(FolderPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(FolderPath);
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.IsReadOnly = false;
                        file.Delete();
                    }
                    foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                    {
                        subDirectory.Delete(true);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public T StreamToObject<T>(Stream jsonStream)
        {
            Encoding encoding = Encoding.UTF8;
            // Copy to a string for header parsing

            // Read the stream into a byte array
            byte[] data = ToByteArray(jsonStream);

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));
                content = content.Replace(delimiter, "");
                delimiter = content.Substring(0, content.IndexOf("\r\n\r\n"));
                content = content.Replace(delimiter, "").Replace("\r\n\r\n", "");
                content = content.Replace("--", "");
            }
            T obj = JsonConvert.DeserializeObject<T>(content);
            return obj;
        }

        public Stream ReturnJSONStream_DT<T>(T t, DataTable DT)
        {
            var jsonObj = new
            {
                Success = DT.Rows[0]["Statuss"].ToString(),
                Message = DT.Rows[0]["Message"].ToString(),
                Response = t
            };

            JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string returnValue = jScriptSerializer.Serialize(jsonObj);

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(returnValue));

            return memoryStream;
        }

        public DataSet GetdatasetFromParams(string Squery, IDictionary<string, string> parameters)
        {
            DataSet dataTable = new DataSet();
            using (var conn = new SqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Squery;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 6000;
                    if (parameters != null)
                    {
                        foreach (var itm in parameters)
                        {
                            cmd.Parameters.AddWithValue("@" + itm.Key, itm.Value);
                        }
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        public DataTable GetDateTableFromQuery_Async(string Squery)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "datatable1";
            using (var conn = new SqlConnection(Connstr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 6000;
                    cmd.CommandText = Squery;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        readonly Random random = new Random();
        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string GenerateString(int size)
        {
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[random.Next(Alphabet.Length)];
            }
            return new string(chars);
        }

        public Stream ReturnJSONStreamDRS<T>(T t)
        {
            var jsonObj = new
            {
                Success = 1,
                Message = "",
                DRSList = t
            };
            JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string returnValue = jScriptSerializer.Serialize(jsonObj);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(returnValue));
            return memoryStream;
        }

        public Stream ReturnJSONStreamBank<T>(T t)
        {
            var jsonObj = new
            {
                Success = 1,
                Message = "",
                bankDetailList = t
            };
            JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string returnValue = jScriptSerializer.Serialize(jsonObj);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(returnValue));
            return memoryStream;
        }

        public bool DeleteUserMenuTextFiles(string UserId)
        {
            try
            {
                string folderPath = HttpContext.Current.Server.MapPath("~/App_Data/UserMenu/");
                string curFile1 = folderPath + "UserMenu_" + UserId + "_1.txt";
                string curFile2 = folderPath + "UserMenu_" + UserId + "_2.txt";
                if (File.Exists(curFile1))
                {
                    File.Delete(curFile1);
                }
                if (File.Exists(curFile2))
                {
                    File.Delete(curFile2);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Stored Execute Query Log - CR
        public void ExecuteDBQueryLog(string Squery)
        {
            if (string.IsNullOrWhiteSpace(Squery)) return;
            // Fire-and-forget: log asynchronously to avoid blocking the main request
            string connStr = Connstr; // Capture connection string for the closure
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    string StoredLog = "EXEC USP_Stored_DB_Query_Log '" + Squery.ReplaceSpecialCharacters() + "'";
                    using (var conn = new SqlConnection(connStr))
                    {
                        using (var cmd = new SqlCommand(StoredLog, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandTimeout = 10; // Fast timeout for logging
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception)
                {
                    // Logging should never break main transaction — silently ignore
                }
            });
        }
        #endregion

        #region Excel Upload
        public DataTable GetDatatableFromExcel(string extension, string path, bool flag)
        {
            string conStr = "";
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"]
                             .ConnectionString;
                    break;
                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"]
                              .ConnectionString;
                    break;
            }
            conStr = String.Format(conStr, path, flag ? "Yes" : "No");

            using (OleDbConnection connExcel = new OleDbConnection(conStr))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter oda = new OleDbDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        DataTable dtTablesList = new DataTable();
                        string sSheetName = "";
                        cmdExcel.Connection = connExcel;
                        if (!(connExcel.State.Equals(ConnectionState.Open)))
                        {
                            connExcel.Open();
                        }
                        dtTablesList = connExcel.GetSchema("Tables");
                        if (dtTablesList.Rows.Count > 0)
                        {
                            sSheetName = dtTablesList.Rows[0]["TABLE_NAME"].ToString();
                        }

                        cmdExcel.CommandText = "SELECT * From [" + sSheetName + "]";
                        oda.SelectCommand = cmdExcel;
                        oda.Fill(dt);
                        return RemoveNullOrBlankRowsFromDataTable(dt);
                    }
                }
            }
        }

        public DataTable RemoveNullOrBlankRowsFromDataTable(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.Trim();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][0] == DBNull.Value && dt.Rows[i][1] == DBNull.Value)
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
            return dt;
        }

        public string ConvertDatatableToXML(DataTable dt)
        {
            MemoryStream str = new MemoryStream();
            dt.WriteXml(str, true);
            str.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(str);
            string xmlstr;
            xmlstr = sr.ReadToEnd();
            return (xmlstr);
        }

        #endregion
    }

    public static class ExtensionMethods
    {
        public static string ReplaceSpecialCharacters(this string value)
        {
            return value.Replace("&", "&amp;").Replace("'", "&#39;").Replace("’", "&#39;").Replace("”", "").Replace("–", "-").Replace("�", " ").Replace(System.Environment.NewLine, " ").Replace(" ", " ").Trim();
        }
    }
}