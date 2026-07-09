using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Data;
using GreenLine.Models;
using GreenLine.Classes;
using System.Runtime.Serialization.Json;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Helper;

namespace GreenLine.Security
{
    public class CustomIdentity : ICustomIdentity
    {
        /// <summary>
        /// Authenticate and get identity out with roles
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Instance of identity</returns>
        /// 
        //public static CustomIdentity GetCustomIdentity(string userName, string password, string Loccode, string CompanyCode, DataTable userDetails)
        public static CustomIdentity GetCustomIdentity(LoginModel loginModel, string Loccode, string CompanyCode, DataTable userDetails)
        {
            if (userDetails != null && userDetails.Rows.Count > 0)
            {
                //GreenLineDataService.MasterService MS = new GreenLineDataService.MasterService();
                string SessionID = Guid.NewGuid().ToString();
                GeneralFunctions GF = new GeneralFunctions();
                string SQR = "EXEC Usp_InsertLoginDetails '" + loginModel.UserName.ToString() + "','" + SessionID + "','" + loginModel.LoginCity + "','" + loginModel.LoginCountry + "','" + loginModel.LoginCountryCode + "','" + loginModel.LoginIP + "','" + loginModel.Latitude + "','" + loginModel.Longitude + "','" + userDetails.Rows[0][15].ToString() + "'";
                DataTable Dt = GF.GetDataTableFromSP(SQR);
                CustomIdentity identity = new CustomIdentity
                {
                    IsAuthenticated = true,
                    Name = loginModel.UserName.ToUpper(),
                    LocationCode = userDetails.Rows[0][0].ToString(),
                    MainLocCode = userDetails.Rows[0][0].ToString(),
                    User_Image = userDetails.Rows[0][1].ToString(),
                    UserType = userDetails.Rows[0][2].ToString(),
                    UserReadWrite = userDetails.Rows[0][3].ToString(),
                    LocationName = userDetails.Rows[0][4].ToString(),
                    HeadOfficeCode = userDetails.Rows[0][5].ToString(),
                    LocationLevel = Convert.ToDecimal(userDetails.Rows[0][6].ToString()),
                    FinYear = userDetails.Rows[0][7].ToString(),
                    YearVal = userDetails.Rows[0][8].ToString(),
                    CurrFinYear = userDetails.Rows[0][9].ToString(),
                    CurrYearVal = userDetails.Rows[0][10].ToString(),
                    CompanyCode = userDetails.Rows[0][11].ToString(),
                    CompanyName = userDetails.Rows[0][12].ToString(),
                    EmpType = userDetails.Rows[0][13].ToString(),
                    GSTNToken = userDetails.Rows[0][14].ToString(),
                    DomainName = userDetails.Rows[0][15].ToString(),
                    Currency = userDetails.Rows[0][18].ToString(),
                    CurrencyName = userDetails.Rows[0][19].ToString()
                };
                //var roles = System.Web.Security.Roles.GetRolesForUser(userName);
                // identity.Roles = roles;
                /*
                CYGNUS_Master_Users WMU = MS.GetUserDetails().FirstOrDefault(c => c.UserId.ToUpper() == userName.ToUpper() && c.Status == "100");

                identity.LocationCode = WMU.BranchCode;
                identity.MainLocCode = WMU.BranchCode;
                identity.User_Image = WMU.User_Image;
                identity.UserType = MS.GetLoginUserType("", userName.ToUpper());
                identity.UserReadWrite = WMU.Read_Witre;
                List<CYGNUS_location> WL = MS.GetLocationDetails();
                identity.LocationName = WL.FirstOrDefault(c => c.LocCode == identity.LocationCode).LocName;
                //identity.LocationGroup = SL.lo;
                //CYGNUS_location HeadLoc = WL.FirstOrDefault(c => c.Loc_Level == 1 && c.ActiveFlag == "Y");
                identity.HeadOfficeCode = WL.FirstOrDefault(c => c.Loc_Level == 1 && c.ActiveFlag == "Y").LocCode;
                identity.LocationLevel = WL.FirstOrDefault(c => c.LocCode == identity.LocationCode).Loc_Level;

                vw_Get_Finacial_Years VFY = MS.GetFinacialYearDetails().FirstOrDefault(c => c.CurrentFinYear == "T");

                identity.FinYear = VFY.FinYear;
                identity.YearVal = VFY.YearVal;

                vw_Get_Finacial_Years VFYCURR = MS.GetFinacialYearDetails().FirstOrDefault(c => c.CurrentFinYear == "T");
                identity.CurrFinYear = VFYCURR.FinYear;
                identity.CurrYearVal = VFYCURR.YearVal;

                GeneralFunctions GF = new GeneralFunctions();
                string commandText = "Exec CompanyMappedToEmployee '" + userName + "'  ";
                DataTable dataTable = GF.GetDateTableFromQuery(commandText);
                string compCode = "";

                if (dataTable.Rows.Count == 0 || string.IsNullOrEmpty(dataTable.Rows[0][0].ToString()))
                {
                    //compCode = "C004";
                    compCode = "C003";
                }
                else
                {
                    compCode = dataTable.Rows[0][0].ToString();
                }

                //CYGNUS_COMPANY_MASTER WMM = MS.GetComapanyDetails().FirstOrDefault(c => c.COMPANY_CODE == WMU.BranchCode);
                CYGNUS_COMPANY_MASTER WMM = MS.GetComapanyDetails().FirstOrDefault(c => c.COMPANY_CODE == compCode);
                identity.CompanyCode = WMM.COMPANY_CODE;
                identity.CompanyName = WMM.COMPANY_NAME;
                */

                return identity;
            }
            return new CustomIdentity();
        }

        public static CustomIdentity GetCustomIdentity(string userName, string Loccode, string FinYear, string CompanyCode, string ext)
        {
            IMasterService MS = new MasterService();
            //DataTable userDetails = MS.CheckValidUserforLogin(userName, Loccode, FinYear, CompanyCode,"").Tables[1];
            DataTable userDetails = MS.CheckValidUserforLogin(userName, Loccode, FinYear, CompanyCode).Tables[1];

            CustomIdentity identity = new CustomIdentity
            {
                IsAuthenticated = true,
                Name = userName.ToUpper(),
                MainLocCode = userDetails.Rows[0][0].ToString(),
                User_Image = userDetails.Rows[0][1].ToString(),
                UserType = userDetails.Rows[0][2].ToString(),
                UserReadWrite = userDetails.Rows[0][3].ToString(),
                LocationCode = Loccode,
                //List<CYGNUS_location> WL = MS.GetLocationDetails();
                LocationName = userDetails.Rows[0][4].ToString(),
                HeadOfficeCode = userDetails.Rows[0][5].ToString(),
                LocationLevel = Convert.ToDecimal(userDetails.Rows[0][6].ToString()),

                //vw_Get_Finacial_Years VFY = MS.GetFinacialYearDetails().FirstOrDefault(c => c.FinYear == FinYear);
                FinYear = userDetails.Rows[0][7].ToString(),
                YearVal = userDetails.Rows[0][8].ToString(),

                CurrFinYear = userDetails.Rows[0][9].ToString(),
                CurrYearVal = userDetails.Rows[0][10].ToString(),
                CompanyCode = userDetails.Rows[0][11].ToString(),
                CompanyName = userDetails.Rows[0][12].ToString(),
                EmpType = userDetails.Rows[0][13].ToString(),
                GSTNToken = userDetails.Rows[0][14].ToString(),
                DomainName = userDetails.Rows[0][15].ToString(),
                Currency = userDetails.Rows[0][18].ToString(),
                CurrencyName = userDetails.Rows[0][19].ToString()
            };

            return identity;
        }

        private CustomIdentity() { }

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; }
        public string LocationName { get; private set; }
        public Nullable<decimal> LocationLevel { get; private set; }
        public string LocationCode { get; private set; }
        public string LocationGroup { get; private set; }
        public string MainLocCode { get; private set; }
        public string FinYear { get; private set; }
        public string YearVal { get; private set; }
        public string CurrFinYear { get; private set; }
        public string CurrYearVal { get; private set; }
        public string HeadOfficeCode { get; private set; }
        public string CompanyCode { get; private set; }
        public string CompanyName { get; private set; }
        public string User_Image { get; private set; }
        private string[] Roles { get; set; }
        public string UserType { get; private set; }
        public string UserReadWrite { get; private set; }
        public string EmpType { get; private set; }
        public string GSTNToken { get; set; }
        public string DomainName { get; set; }
        public string Currency { get; set; }
        public string CurrencyName { get; set; }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("Role is null");
            }
            return Roles.Where(one => one.ToUpper().Trim() == role.ToUpper().Trim()).Any();
        }

        /// <summary>
        /// Create serialized string for storing in a cookie
        /// </summary>
        /// <returns>String representation of identity</returns>
        public string ToJson()
        {
            string returnValue = string.Empty;
            IdentityRepresentation representation = new IdentityRepresentation()
            {
                IsAuthenticated = this.IsAuthenticated,
                Name = this.Name,
                LocationCode = this.LocationCode,
                MainLocCode = this.MainLocCode,
                LocationName = this.LocationName,
                LocationLevel = this.LocationLevel,
                FinYear = this.FinYear,
                YearVal = this.YearVal,
                HeadOfficeCode = this.HeadOfficeCode,
                CompanyCode = this.CompanyCode,
                CompanyName = this.CompanyName,
                LocationGroup = this.LocationGroup,
                User_Image = this.User_Image,
                CurrFinYear = this.CurrFinYear,
                CurrYearVal = this.CurrYearVal,
                UserType = this.UserType,
                UserReadWrite = this.UserReadWrite,
                EmpType = this.EmpType,
                GSTNToken = this.GSTNToken,
                DomainName = this.DomainName,
                Currency = this.Currency,
                CurrencyName = this.CurrencyName
                //,
                //Roles = string.Join("|", this.Roles)
            };
            DataContractJsonSerializer jsonSerializer =
                new DataContractJsonSerializer(typeof(IdentityRepresentation));
            using (MemoryStream stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, representation);
                stream.Flush();
                byte[] json = stream.ToArray();
                returnValue = Encoding.UTF8.GetString(json, 0, json.Length);
            }

            return returnValue;
        }

        /// <summary>
        /// Create identity from a cookie data
        /// </summary>
        /// <param name="cookieString">String stored in cookie, created via ToJson method</param>
        /// <returns>Instance of identity</returns>
        public static ICustomIdentity FromJson(string cookieString)
        {

            IdentityRepresentation serializedIdentity = null;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(cookieString)))
            {
                DataContractJsonSerializer jsonSerializer =
                    new DataContractJsonSerializer(typeof(IdentityRepresentation));
                serializedIdentity = jsonSerializer.ReadObject(stream) as IdentityRepresentation;
            }
            CustomIdentity identity = new CustomIdentity()
            {
                IsAuthenticated = serializedIdentity.IsAuthenticated,
                Name = serializedIdentity.Name,
                LocationCode = serializedIdentity.LocationCode,
                MainLocCode = serializedIdentity.MainLocCode,
                LocationName = serializedIdentity.LocationName,
                LocationLevel = serializedIdentity.LocationLevel,
                FinYear = serializedIdentity.FinYear,
                YearVal = serializedIdentity.YearVal,
                HeadOfficeCode = serializedIdentity.HeadOfficeCode,
                CompanyCode = serializedIdentity.CompanyCode,
                CompanyName = serializedIdentity.CompanyName,
                LocationGroup = serializedIdentity.LocationGroup,
                User_Image = serializedIdentity.User_Image,
                CurrFinYear = serializedIdentity.CurrFinYear,
                CurrYearVal = serializedIdentity.CurrYearVal,
                UserType = serializedIdentity.UserType,
                UserReadWrite = serializedIdentity.UserReadWrite,
                EmpType = serializedIdentity.EmpType,
                GSTNToken = serializedIdentity.GSTNToken,
                DomainName = serializedIdentity.DomainName,
                Currency = serializedIdentity.Currency,
                CurrencyName = serializedIdentity.CurrencyName
                //,
                //Roles = serializedIdentity.Roles
                //    .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
            };
            return identity;
        }

    }
}