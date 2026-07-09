using GreenLine.Classes;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace GreenLineDataService.Helper
{
    public class MasterService : IMasterService
    {
        private string QueryString = string.Empty;
        readonly GeneralFunctions GF = new GeneralFunctions();
        public string folderPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");

        #region State Master
        public List<CYGNUS_State> GetStateMaster()
        {
            QueryString = "EXEC USP_StateMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_State> CYGNUS_StateList = DataRowToObject.CreateListFromTable<CYGNUS_State>(dataTable);
            return CYGNUS_StateList;
        }

        public List<CYGNUX_Master_Countries> GetCountryMaster()
        {
            QueryString = "EXEC USP_CountryMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUX_Master_Countries> CYGNUS_CountryList = DataRowToObject.CreateListFromTable<CYGNUX_Master_Countries>(dataTable);
            return CYGNUS_CountryList;
        }
        public List<CYGNUS_Master_General> GetGeneralMaster()
        {
            QueryString = "EXEC USP_GeneralMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_General> CYGNUS_Master_GeneralList_XML = DataRowToObject.CreateListFromTable<CYGNUS_Master_General>(dataTable);
            //GF.SerializeParams<CYGNUS_Master_General>(CYGNUS_Master_GeneralList_XML, folderPath + "CYGNUS_Master_General.xml");
            return CYGNUS_Master_GeneralList_XML;
        }

        public List<CYGNUS_Master_General> GetGeneralMasterWithParam(string CodeType, string Codeid)
        {
            QueryString = "EXEC USP_GeneralMasterWithParam_List '" + CodeType + "','" + Codeid + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_General> CYGNUS_Master_GeneralList_XML = DataRowToObject.CreateListFromTable<CYGNUS_Master_General>(dataTable);
            return CYGNUS_Master_GeneralList_XML;
        }

        public bool ActiveInActiveState(int id)
        {
            QueryString = "EXEC USP_ActiveInActiveState " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        public DataTable GetExcelData(string MethodName, string BaseUserName, string BaseCompanyCode, string BaseLocationCode, string BaseYearVal)
        {
            QueryString = "EXEC Usp_GetExcelData '" + MethodName + "','" + BaseUserName + "','" + BaseCompanyCode + "','" + BaseLocationCode + "','" + BaseYearVal + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable AddEditStateMaster(string XML_Main)
        {
            QueryString = "EXEC usp_CYGNUS_STATE_Insert_NewPortal '" + XML_Main + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditStateMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                List<CYGNUS_State> CYGNUS_StateList = DataRowToObject.CreateListFromTable<CYGNUS_State>(DS.Tables[1]);
                GF.SerializeParams<CYGNUS_State>(CYGNUS_StateList, folderPath + "CYGNUS_State.xml");
            }
            return DS.Tables[0];
        }

        public bool ExistsState(string stateName, string stateCode, int srno)
        {
            QueryString = "EXEC USP_ExistsState '" + (stateName ?? "") + "','" + (stateCode ?? "") + "'," + srno;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToInt32(dataTable.Rows[0][0]) > 0;
        }

        public int GetCityCount(string city, string state)
        {
            QueryString = "EXEC USP_GetCityCount '" + (city ?? "") + "','" + (state ?? "") + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToInt32(dataTable.Rows[0][0]);
        }

        #endregion

        #region Organization Master
        public DataTable AddEditOrganization(string BaseCompanyCode, string OrgDetxml, string OrgBnkDetListxml, string UserName)
        {
            string Squery = "EXEC USP_InsertUpdate_Company_Master_Details '" + BaseCompanyCode + "' , '" + OrgDetxml.ReplaceSpecialCharacters() + "' , '" + OrgBnkDetListxml.ReplaceSpecialCharacters() + "' , '" + UserName + "' ";
            int Id = GF.SaveRequestServices(Squery.Replace("'", "''"), "UpdateCompany", "", "");
            return GF.GetDataTableFromSP(Squery);
        }

        public bool ActiveInActiveOrganization(string CompanyCode)
        {
            QueryString = "EXEC USP_ActiveInActiveOrganization '" + CompanyCode + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        public CYGNUS_Organization_Master GetComapanyDetails(string BaseCompanyCode)
        {
            CYGNUS_Organization_Master obj = new CYGNUS_Organization_Master();
            string SQRY = "EXEC USP_GetComapanyDetails '" + BaseCompanyCode + "'";
            DataSet Ds = GF.GetDataSetFromSP(SQRY);

            if (Ds != null && Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
            {
                obj.OrgDet = DataRowToObject.CreateItemFromRow<CYGNUS_COMPANY_MASTER>(Ds.Tables[0].Rows[0]);
            }
            if (Ds.Tables.Count > 1 && Ds.Tables[1].Rows.Count > 0)
            {
                obj.OrgBnkDetList = DataRowToObject.CreateListFromTable<Organization_Bank_Details>(Ds.Tables[1]);
            }
            else
            {
                obj.OrgBnkDetList = new List<Organization_Bank_Details>();
            }
            return obj;
        }

        public List<CYGNUS_COMPANY_MASTER> GetComapanyDetails()
        {
            string SQRY = "EXEC USP_GetComapanyDetails";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);

            if (Dt != null && Dt.Rows.Count > 0)
            {
                return DataRowToObject.CreateListFromTable<CYGNUS_COMPANY_MASTER>(Dt);
            }
            return null;
        }

        public List<Organization_Bank_Details> GetComapanyBankDetails(string BaseCompanyCode)
        {
            string SQRY = "EXEC USP_GetComapanyBankDetails '" + BaseCompanyCode + "'";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);

            if (Dt != null && Dt.Rows.Count > 0)
            {
                return DataRowToObject.CreateListFromTable<Organization_Bank_Details>(Dt);
            }
            return null;
        }

        #endregion

        public DataTable GetIsFinYear()
        {
            var QueryString = "EXEC USP_GetIsFinYear ";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }
        public DataSet CheckValidUserforLogin(string Username, string Loccode, string FinYear, string CompanyCode)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserName", Username ?? (object)DBNull.Value),
                    new SqlParameter("@Loccode", Loccode ?? (object)DBNull.Value),
                    new SqlParameter("@FinYear", FinYear ?? (object)DBNull.Value),
                    new SqlParameter("@CompanyCode", CompanyCode ?? (object)DBNull.Value)
                };
                return GF.GetDataSetFromSP_New("USP_CheckValidUserforLogin", parameters);
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable
                {
                    TableName = "Result"
                };
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
        }

        #region General Master
        public List<CYGNUS_Master_CodeTypes> GetCodetypesMasterList()
        {
            QueryString = "EXEC Usp_GetCodeType_list_NewPortal";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_CodeTypes> CYGNUS_Master_CodeTypesList = DataRowToObject.CreateListFromTable<CYGNUS_Master_CodeTypes>(dataTable);

            return CYGNUS_Master_CodeTypesList;
        }
        public DataTable AddEditGeneralMaster(string XML, string flag, string Finyear)
        {
            QueryString = "EXEC Usp_InsertMstData_NewPortal '" + XML + "','" + flag + "','" + Finyear + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditGeneralMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                if (DS.Tables[1].Rows.Count > 0)
                {
                    List<CYGNUS_Master_General> SSI_Master_GeneralList = DataRowToObject.CreateListFromTable<CYGNUS_Master_General>(DS.Tables[1]);
                    GF.SerializeParams<CYGNUS_Master_General>(SSI_Master_GeneralList, folderPath + "CYGNUS_Master_General.xml");
                }
                else
                {
                    File.Delete(folderPath + "CYGNUS_Master_General.xml");
                }
            }
            return DS.Tables[0];
        }
        public string CheckDuplicateGeneralMaster(string CodeType, string CodeDesc)
        {
            QueryString = "EXEC CheckDuplicateGeneralMaster ' " + CodeDesc + " ','" + CodeType + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return dataTable.Rows[0][0].ToString();
        }
        public List<CYGNUS_location> GetLocationDetails(int? ActiveFlg = 0)
        {
            QueryString = "EXEC USP_GetLocationDetails";

            if (ActiveFlg != null && ActiveFlg > 0)
                QueryString = "EXEC USP_GetLocationDetails '" + ActiveFlg + "'";

            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_location> List = DataRowToObject.CreateListFromTable<CYGNUS_location>(Dt);
            return List;
        }
        public List<CYGNUS_Master_General> GetMasterGeneralObject(string CodeId, string MasterCode)
        {
            QueryString = "EXEC Usp_GetMasterGeneralObject '" + CodeId + "','" + MasterCode + "'";
            DataTable _DT = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_General> MasterList = DataRowToObject.CreateListFromTable<CYGNUS_Master_General>(_DT);

            if (MasterCode != "")
                MasterList = MasterList.Where(c => c.CodeType == MasterCode).ToList();
            else
                MasterList = MasterList.ToList();

            return MasterList;
        }
        #endregion 

        #region Menu Function

        public List<VW_GetUserMenuRights> GetMenuListWithRights(string Userid, bool IsLogin, string Type, string BaseFinYear)
        {
            //Type = "2";
            string curFile = folderPath + "UserMenuRights_" + Userid + ".xml";
            if (!File.Exists(curFile) || IsLogin)
            {
                QueryString = "EXEC Usp_GetUserMenuRights '" + Userid + "','" + Type + "','" + BaseFinYear + "'";
                DataTable Dt = GF.GetDataTableFromSP(QueryString);
                List<VW_GetUserMenuRights> MenuList_XML = DataRowToObject.CreateListFromTable<VW_GetUserMenuRights>(Dt);
                GF.SerializeParams<VW_GetUserMenuRights>(MenuList_XML, folderPath + "UserMenuRights_" + Userid + ".xml");
            }
            List<VW_GetUserMenuRights> MenuList = GF.DeserializeParams<VW_GetUserMenuRights>(folderPath + "UserMenuRights_" + Userid + ".xml");
            return MenuList.Where(c => c.IsNewPortal == true).ToList();
        }

        public List<CYGNUS_Master_Menu> GetMenusList(bool IsLogin)
        {
            //string curFile = folderPath + "Cygnus_Master_Menu.xml";
            //if (!File.Exists(curFile) || IsLogin)
            //{
            QueryString = "EXEC USP_GetMenus_NewPortal";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Menu> MenuList_XML = DataRowToObject.CreateListFromTable<CYGNUS_Master_Menu>(Dt);
            //GF.SerializeParams<CYGNUS_Master_Menu>(MenuList_XML, folderPath + "Cygnus_Master_Menu.xml");
            //}
            //List<CYGNUS_Master_Menu> MenuList = GF.DeserializeParams<CYGNUS_Master_Menu>(folderPath + "Cygnus_Master_Menu.xml");

            return MenuList_XML.Where(c => c.IsNewPortal == true).ToList();
        }

        #endregion

        #region  Change Settings

        public List<ChgangeLoc> GetWorkingLocations(string BaseLocationCode, string MainLocCode)
        {
            string commandText = "Exec usp_WorkingLocations '" + BaseLocationCode + "','" + MainLocCode + "'  ";
            DataTable dataTable = GF.GetDateTableFromQuery(commandText);
            List<ChgangeLoc> ListLocation = DataRowToObject.CreateListFromTable<ChgangeLoc>(dataTable);
            return ListLocation;
        }

        public List<ChgangeLoc> GetWorkingLocationsNewPortal(string BaseLocationCode, string MainLocCode, string UserName)
        {
            //string commandText = "Exec usp_WorkingLocations '" + BaseLocationCode + "','" + MainLocCode + "'  ";
            string commandText = "Exec usp_WorkingLocations_NewPortal '" + BaseLocationCode + "','" + MainLocCode + "' ,'" + UserName + "' ";
            DataTable dataTable = GF.GetDateTableFromQuery(commandText);
            List<ChgangeLoc> ListLocation = DataRowToObject.CreateListFromTable<ChgangeLoc>(dataTable);
            return ListLocation;
        }

        public List<ChgangeCompany> GetCompanyMappedToEmployee(string UserId)
        {
            string commandText = "Exec CompanyMappedToEmployee '" + UserId + "'  ";
            DataTable dataTable = GF.GetDateTableFromQuery(commandText);
            List<ChgangeCompany> ListCompany = DataRowToObject.CreateListFromTable<ChgangeCompany>(dataTable);
            return ListCompany;
        }

        #endregion

        #region Finacial Years

        public List<vw_Get_Finacial_Years> GetFinacialYearDetails()
        {
            QueryString = "Select * from vw_Get_Finacial_Years";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<vw_Get_Finacial_Years> FinacialYearList = DataRowToObject.CreateListFromTable<vw_Get_Finacial_Years>(Dt);
            return FinacialYearList;
        }

        #endregion

        #region Location
        public bool InsertLocation(string XML, string EditFlag, string EntryBy, string XML2)
        {
            bool Status = false;
            //QueryString = "EXEC Usp_InsertLocationMaster_NewPortal '" + XML + "','" + EditFlag + "','" + EntryBy + "','" + XML2 + "'";
            QueryString = "EXEC Usp_InsertLocationMaster '" + XML + "','" + EditFlag + "','" + EntryBy + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "InsertLocation", "", "");

            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0][0].ToString() == "1")
            {
                Status = true;
                if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
                {
                    List<CYGNUS_location> LocationList_XML = DataRowToObject.CreateListFromTable<CYGNUS_location>(DS.Tables[1]);
                    GF.SerializeParams<CYGNUS_location>(LocationList_XML, folderPath + "cygnus_location.xml");
                }
                else
                {
                    File.Delete(folderPath + "cygnus_location.xml");
                }
            }
            return Status;
        }

        public bool ActiveInActiveLocation(string LocCode)
        {
            QueryString = "EXEC USP_ActiveInActive_Location '" + LocCode + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        #endregion

        #region City Master
        public List<CYGNUS_citymaster> GetCityMaster()
        {
            //string curFile = folderPath + "CYGNUS_citymaster.xml";
            //if (!File.Exists(curFile))
            //{
            QueryString = "EXEC USP_CityMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_citymaster> CYGNUS_citymasterList_XML = DataRowToObject.CreateListFromTable<CYGNUS_citymaster>(dataTable);
            //GF.SerializeParams<CYGNUS_citymaster>(CYGNUS_citymasterList_XML, folderPath + "CYGNUS_citymaster.xml");
            //}

            //List<CYGNUS_citymaster> CYGNUS_citymasterList = GF.DeserializeParams<CYGNUS_citymaster>(folderPath + "CYGNUS_citymaster.xml");
            return CYGNUS_citymasterList_XML;
        }
        public DataTable AddEditCityMaster(string XML)
        {
            QueryString = "EXEC Usp_InsertCityMaster_NewPortal '" + XML + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditCityMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                List<CYGNUS_citymaster> SSI_Master_GeneralList = DataRowToObject.CreateListFromTable<CYGNUS_citymaster>(DS.Tables[1]);
                GF.SerializeParams<CYGNUS_citymaster>(SSI_Master_GeneralList, folderPath + "CYGNUS_citymaster.xml");
            }
            return DS.Tables[0];
        }
        public bool ActiveInActive_City(int id)
        {
            QueryString = "EXEC USP_ActiveInActive_City " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        public DataTable GetCityMasterReportDetails()
        {
            QueryString = "EXEC USP_CityMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return dataTable;
        }
        #endregion

        #region Customer Group Master

        public List<CYGNUS_GRPMST> GetCustomerGroupMasterObject()
        {
            //string curFile = folderPath + "CYGNUS_GRPMST.xml";
            //if (!File.Exists(curFile))
            //{
            QueryString = "EXEC USP_CustomerGroupMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_GRPMST> CYGNUS_GRPMST_masterList_XML = DataRowToObject.CreateListFromTable<CYGNUS_GRPMST>(dataTable);
            //    GF.SerializeParams<CYGNUS_GRPMST>(CYGNUS_GRPMST_masterList_XML, folderPath + "CYGNUS_GRPMST.xml");
            //}
            //List<CYGNUS_GRPMST> CYGNUS_GRPMST_masterList = GF.DeserializeParams<CYGNUS_GRPMST>(folderPath + "CYGNUS_GRPMST.xml");
            //return CYGNUS_GRPMST_masterList;
            return CYGNUS_GRPMST_masterList_XML;
        }

        public DataTable AddEditCustomerGroupMaster(string XML)
        {
            QueryString = "EXEC usp_CYGNUS_Customer_Group_Master_Insert_NewPortal '" + XML + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditCustomerGroupMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                List<CYGNUS_GRPMST> CYGNUS_GRPMST_masterList = DataRowToObject.CreateListFromTable<CYGNUS_GRPMST>(DS.Tables[1]);
                GF.SerializeParams<CYGNUS_GRPMST>(CYGNUS_GRPMST_masterList, folderPath + "CYGNUS_GRPMST.xml");
            }
            return DS.Tables[0];
        }

        public List<CYGNUS_CUSTHDR> GetCustomerList(string Type, string Name)
        {

            QueryString = "EXEC sp_GetLike_GetCustomer '" + Name + "','" + Type + "'";
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> itmCustList = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(DT);
            return itmCustList;
        }

        public List<CYGNUS_CUSTHDR> GetCustomerList()
        {
            QueryString = "EXEC USP_GetCustomerList";
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> itmCustList = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(DT);
            return itmCustList;
        }

        public bool ActiveInActive_Customergroup(string id)
        {
            QueryString = "EXEC USP_ActiveInActive_Customergroup '" + id + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        #endregion

        #region Designation 
        public List<CYGNUS_Master_General> GetDesignationFromCategory(string Category)
        {
            QueryString = "EXEC Usp_Get_DesignationFromCategory '" + Category + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_General> DesignationList = DataRowToObject.CreateListFromTable<CYGNUS_Master_General>(dataTable);
            return DesignationList;
        }
        public List<CYGNUS_Master_Users> GetManagerFromDesignationandLocation(string Category, string Location)
        {
            QueryString = "EXEC USP_GetManagerFromDesignationandLocation '" + Category + "','" + Location + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Users> ManagerList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Users>(dataTable);
            return ManagerList;
        }

        #endregion

        #region User Master

        public List<CYGNUS_Master_Users> GetUserDetails()
        {
            QueryString = "EXEC USP_GetUserDetails";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Users> UserList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Users>(Dt);
            return UserList;
        }
        public List<CYGNUS_Master_Users> GetEmployeeList()
        {
            QueryString = "SELECT UserId, Name, emptype, Status FROM CYGNUS_Master_Users WHERE emptype = '1' and Status='100'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Users> UserList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Users>(Dt);
            return UserList;
        }

        public List<CYGNUS_Master_Users> GetUserDetailsForUserMasterList(string userId)
        {
            QueryString = "EXEC USP_GetUserDetailsForUserMasterList '" + userId + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Users> UserList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Users>(Dt);
            return UserList;
        }

        public DataTable InsertUser(string XML, string EditFlag)
        {
            QueryString = "EXEC Usp_InsertUpdate_UserMaster '" + XML + "','" + EditFlag + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "InsertUser", "", "");
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            if (DT.Rows[0][0].ToString() == "Done")
            {
                GetUserDetails();
            }
            return DT;
        }

        public List<CYGNUS_location> GetDestinationLocationsWithHQTR(string Prefix)
        {
            QueryString = "EXEC [USP_Get_GCDestination_With_HQTR] '" + Prefix + "'";
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_location> itmList = DataRowToObject.CreateListFromTable<CYGNUS_location>(DT);
            return itmList;
        }
        public bool ActiveInActive_User(string id)
        {
            QueryString = "EXEC USP_ActiveInActive_User '" + id + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        public List<CYGNUS_COMPANY_MASTER> GetCompanyDetails()
        {
            QueryString = "EXEC USP_GetCompanyDetails";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_COMPANY_MASTER> List = DataRowToObject.CreateListFromTable<CYGNUS_COMPANY_MASTER>(Dt);
            return List;
        }
        public List<CYGNUS_FLEET_DRIVERMST> GetDriverObject()
        {
            QueryString = "SELECT Driver_Id,Driver_Name,ISNULL(UserId,'') AS UserId  FROM CYGNUS_FLEET_DRIVERMST WITH(NOLOCK)";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_FLEET_DRIVERMST> CYGNUS_DriverList_XML = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_DRIVERMST>(dataTable);
            return CYGNUS_DriverList_XML;
        }
        #endregion

        #region User Menu Rights

        public DataTable InsertMenuRights(string XML, string UserId, string isNewPortal, string UserName, string BaseFinYear)
        {
            string SQR = "EXEC Usp_Insert_MenuRights '" + XML + "','" + UserId + "','" + isNewPortal + "','" + UserName + "'";
            int id = GF.SaveRequestServices(SQR.Replace("'", "''"), "InsertMenuRights", "", "");
            DataTable Dt = GF.GetDataTableFromSP(SQR);
            GetMenuListWithRights(UserId, true, "0", BaseFinYear);
            GF.DeleteUserMenuTextFiles(UserId);
            return Dt;
        }

        #endregion

        #region Change Reports Rights
        public List<CYGNUS_Master_Reports> GetReportList(string ReportType, string ReportSubType, string UserName, int Type)
        {
            QueryString = "EXEC USP_GetUserWiseReportAccessListForUser '" + UserName + "','" + ReportType + "','" + ReportSubType + "','" + Type + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(QueryString);
            List<CYGNUS_Master_Reports> EnquiryList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Reports>(Dt1);
            return EnquiryList;
        }
        public DataTable Add_Report_Rights(string XML, string Location, string UserName, string CompanyCode, string BaseUserName)
        {
            QueryString = "EXEC Usp_InsertUpdate_UserWise_Reports_Rights '" + XML + "','" + Location + "','" + UserName + "','" + CompanyCode + "','" + BaseUserName + "' ";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "ChangeReportRightsSubmit", "", "");
            DataTable Dt1 = GF.GetDateTableFromQuery(QueryString);
            return Dt1;
        }
        #endregion

        #region Customer Master
        public List<CYGNUS_CUSTHDR> GetCustomerMasterObject(string CustCode)
        {
            QueryString = "EXEC USP_CustomerMaster_List '" + CustCode + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> CYGNUS_CUSTHDRList_XML = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(dataTable);
            return CYGNUS_CUSTHDRList_XML;
        }

        public DataTable Get_StateWise_GSTDetails_TypeWise(string flag, string Code)
        {
            QueryString = "EXEC USP_Get_StateWise_GSTDetails_TypeWise '" + flag + "','" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable Get_Customerwise_KMA(string Code)
        {
            QueryString = "EXEC USP_Get_Customerwise_KMAType '" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable Get_Customerwise_BillDetails(string Code)
        {
            QueryString = "EXEC USP_Get_Customerwise_BillDetails '" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable Get_Customerwise_KYCDetails(string Code)
        {
            QueryString = "EXEC USP_Get_Customerwise_KYCDetails '" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable Get_Customerwise_PickUpDetails(string Code)
        {
            QueryString = "EXEC USP_Get_Customerwise_PickUpDetails '" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable Get_Customerwise_GeofenceDetails(string Code)
        {
            QueryString = "EXEC USP_Get_Customerwise_GeofenceDetails '" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public DataTable AddEditCustomerMaster(string MstDetailsXML, string BaseUserName, string KMADetailsXML, string BillDetailsXML, string KYCDetailsXML, string GeofenceDetailsXML, string PickUpAddressDetailsXML)
        {
            QueryString = "EXEC usp_CYGNUS_CUSTHDR_Insert_NewPortal '" + MstDetailsXML + "','" + BaseUserName + "','" + KMADetailsXML + "','" + BillDetailsXML + "','" + KYCDetailsXML + "','" + GeofenceDetailsXML + "','" + PickUpAddressDetailsXML + "' ";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditCustomerMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                File.Delete(folderPath + "CYGNUS_CUSTHDR.xml");
            }
            return DS.Tables[0];
        }
        public List<CYGNUS_VENDOR_HDR> GetVendorObject()
        {
            QueryString = "EXEC Sp_GetVendorObject";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_VENDOR_HDR> VendorList = DataRowToObject.CreateListFromTable<CYGNUS_VENDOR_HDR>(Dt);
            return VendorList;
        }
        public List<CYGNUS_CUSTHDR> GetConsignnorCustomerListJson()
        {
            QueryString = "EXEC USP_GetConsignnorCustomerDetails";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> List = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(Dt);
            return List;
        }
        public List<CYGNUS_CUSTHDR> GetConsigneeCustomerListJson()
        {
            QueryString = "EXEC USP_GetConsigneeCustomerDetails";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> List = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(Dt);
            return List;
        }
        public DataTable GetGSTWiseStateDetails(string GSTNo, string CustCode)
        {
            QueryString = "EXEC USP_Get_GSTWise_StateDetails '" + GSTNo + "','" + CustCode + "'";
            return GF.GetDataTableFromSP(QueryString);
        }
        public List<CYGNUS_Master_Users> Search_Organization_Employee(string Prefix, string BaseUserName)
        {
            string SQRY = "EXEC [dbo].[Usp_USER_Select]'" + Prefix + "','" + BaseUserName + "'";
            DataTable dtResponse = GF.GetDataTableFromSP(SQRY);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Users>(dtResponse).ToList();
        }
        public bool ActiveInActive_Customer(string id)
        {
            QueryString = "EXEC USP_ActiveInActive_Customer '" + id + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        public DataTable CustomerToUserCreate(string id, string BaseUserName, string Password)
        {
            QueryString = "EXEC Usp_CustomerToUserCreate '" + id + "','" + BaseUserName + "','" + Password + "'";
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            return DT;
        }

        public List<CYGNUS_CUSTHDR> GetCustomerListingNew(string searchTerm, string GRPCD, string state = null)
        {
            QueryString = "EXEC USP_CustomerMaster_List_New '" + searchTerm + "','" + GRPCD + "','" + state + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> CYGNUS_CUSTHDRList = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(dataTable);
            return CYGNUS_CUSTHDRList;
        }
        public List<CYGNUS_CUSTHDR> GetCustomerListforAddress()
        {

            QueryString = "EXEC USP_GETCUSTOMERS_LIST";
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> itmCustList = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(DT);
            return itmCustList;
        }
        public List<CYGNUS_location> GetLOCATIONByCityJson(int id)
        {

            QueryString = "EXEC USP_GETCITYWISELOCATION " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_location> CYGNUS_citymasterList_XML = DataRowToObject.CreateListFromTable<CYGNUS_location>(dataTable);
            return CYGNUS_citymasterList_XML;
        }
        public List<CYGNUS_State> GetStateByCityJson(int id)
        {

            QueryString = "EXEC USP_GETCITYWISESTATE " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_State> CYGNUS_citymasterList_XML = DataRowToObject.CreateListFromTable<CYGNUS_State>(dataTable);
            return CYGNUS_citymasterList_XML;
        }
        public DataTable GetEmpDesignation(string UserId)
        {
            QueryString = "EXEC USP_GetEmpDetails '" + UserId + "'";
            DataTable DT = GF.GetDataTableFromSP(QueryString);
            return DT;
        }
        #endregion

        #region PinCode
        public List<CYGNUS_pincode_master> GetPincodeMaster()
        {
            QueryString = "EXEC USP_PincodeMaster_List";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_pincode_master> CYGNUS_pincode_masterList = DataRowToObject.CreateListFromTable<CYGNUS_pincode_master>(dataTable);
            return CYGNUS_pincode_masterList;
        }

        public DataTable AddEditPincodeMaster(string XML)
        {
            QueryString = "EXEC usp_CYGNUS_Pincode_Master_Insert_NewPortal '" + XML + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditPincodeMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                List<CYGNUS_pincode_master> CYGNUS_pincode_masterList = DataRowToObject.CreateListFromTable<CYGNUS_pincode_master>(DS.Tables[1]);
                GF.SerializeParams<CYGNUS_pincode_master>(CYGNUS_pincode_masterList, folderPath + "CYGNUS_pincode_master.xml");
            }
            return DS.Tables[0];
        }

        public bool ActiveInActive_Pincode(int id)
        {
            QueryString = "EXEC USP_ActiveInActive_Pincode " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        public bool ExistsPincode(int pincode, int id)
        {
            QueryString = "EXEC USP_ExistsPincode " + pincode + "," + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToInt32(dataTable.Rows[0][0]) > 0;
        }
        #endregion

        #region Country Master
        public bool ActiveInActiveCountry(int id)
        {
            QueryString = "EXEC USP_ActiveInActiveCountry " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        public DataTable AddEditCountryMaster(string XML_Main, string BaseUserName)
        {
            QueryString = "EXEC USP_CYGNUS_Country_Insert_Update '" + XML_Main + "' , '" + BaseUserName + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditCountryMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["TranXaction"].ToString() == "Done")
            {
                List<CYGNUX_Master_Countries> CYGNUS_CountryList = DataRowToObject.CreateListFromTable<CYGNUX_Master_Countries>(DS.Tables[1]);
                GF.SerializeParams<CYGNUX_Master_Countries>(CYGNUS_CountryList, folderPath + "CYGNUS_Country.xml");
            }
            return DS.Tables[0];
        }
        #endregion

        #region Lane Master
        public List<CYGNUS_LaneMaster> GetLaneDetails()
        {
            QueryString = "EXEC USP_GetLaneDetails ";

            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_LaneMaster> List = DataRowToObject.CreateListFromTable<CYGNUS_LaneMaster>(Dt);
            return List;
        }

        public List<CYGNUS_LaneMaster> GetLaneDetails(string term)
        {
            QueryString = "EXEC USP_GetLaneDetails '" + term + "'";

            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_LaneMaster> List = DataRowToObject.CreateListFromTable<CYGNUS_LaneMaster>(Dt);
            return List;
        }
        #endregion

        public List<CYGNUS_CUSTHDR> GetCustomer(string GRPCD)
        {
            QueryString = "EXEC USP_Get_CustomerOn_CustomerGroup '" + GRPCD + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_CUSTHDR> List = DataRowToObject.CreateListFromTable<CYGNUS_CUSTHDR>(dataTable);

            return List;
        }

        #region Vehicle Model

        public List<CYGNUS_Vehicle_Model> GetVehicleModelDetails()
        {
            // string curFile = folderPath + "CYGNUS_Vehicle_Type.xml";
            // if (!File.Exists(curFile))
            // {
            QueryString = "EXEC Usp_GetVehicleTypeList";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Model> VehicleTypeList_XML = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Model>(Dt);
            //     GF.SerializeParams<CYGNUS_Vehicle_Type>(VehicleTypeList_XML, folderPath + "CYGNUS_Vehicle_Type.xml");
            // }
            // List<CYGNUS_Vehicle_Type> VehicleTypeList = GF.DeserializeParams<CYGNUS_Vehicle_Type>(folderPath + "CYGNUS_Vehicle_Type.xml");

            // return VehicleTypeList;
            return VehicleTypeList_XML;
        }

        public List<CYGNUS_Vehicle_Model> GetVehicleModel()
        {
            //string curFile = folderPath + "CYGNUS_Vehicle_Type.xml";
            //if (!File.Exists(curFile))
            //{
            QueryString = "EXEC Sp_GetVehicleTypeObject";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Model> VehicleList_XML = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Model>(Dt);
            //GF.SerializeParams<CYGNUS_Vehicle_Type>(VehicleList_XML, folderPath + "CYGNUS_Vehicle_Type.xml");
            //}
            //List<CYGNUS_Vehicle_Type> VehicleList = GF.DeserializeParams<CYGNUS_Vehicle_Type>(folderPath + "CYGNUS_Vehicle_Type.xml");
            //return VehicleList;
            return VehicleList_XML;
        }

        public DataTable AddEditVehicleModelMaster(string XML, string Entry_EditFlag, string Finyear)
        {
            string QueryStringAdd = "EXEC usp_InsertUpdate_CYGNUS_Vehicle_Type '" + XML + "','" + Entry_EditFlag + "','" + Finyear + "'";
            int id = GF.SaveRequestServices(QueryStringAdd.Replace("'", "''"), "AddEditVehicleTypeMaster", "", "");
            DataTable dataTableAdd = GF.GetDataTableFromSP(QueryStringAdd);
            return dataTableAdd;
        }

        public bool ActiveInActive_VehicleModel(int id)
        {
            QueryString = "EXEC USP_ActiveInActive_VehicleType " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            bool status = Convert.ToBoolean(dataTable.Rows[0]["Status"]);
            return status;
        }

        #endregion

        #region Vehicle

        public List<CYGNUS_Vehicle_Master> GetVehicleList(string VehicleNo, string BaseUserName, string Type)
        {
            QueryString = "EXEC usp_Get_VehicleMaster_List_New'" + VehicleNo + "','" + BaseUserName + "','" + Type + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Master> VehicleList = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(dataTable);
            return VehicleList;
        }

        public CYGNUS_Vehicle_Master GetVehicleById(string id)
        {
            QueryString = "EXEC usp_Get_VehicleMaster_ByID '" + id + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            CYGNUS_Vehicle_Master VehicleData = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(Dt).FirstOrDefault();
            return VehicleData;
        }

        public DataTable AddEditVehicle(string XML, string EditFlag, string EntryBy, string CompanyCode)
        {
            QueryString = "EXEC usp_AddEdit_VehicleMaster_WithDoc '" + XML + "','" + EntryBy + "','" + CompanyCode + "'";
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditVehicle", "", "");
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }

        public List<CYGNUS_location> Getlatitude(string loccode)
        {
            QueryString = "exec USP_GetlatitudeByLocation '" + loccode + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_location> item = DataRowToObject.CreateListFromTable<CYGNUS_location>(dataTable);
            return item;
        }

        public bool ActiveInActive_Vehicle(string id)
        {
            QueryString = "EXEC USP_ActiveInActive_VehicleMaster '" + id + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            bool status = Convert.ToBoolean(dataTable.Rows[0]["Status"]);
            return status;
        }

        public string GetVehicleAPICache(string vehNo)
        {
            QueryString = "EXEC USP_GetVehicleAPICache_ByVehicleNo '" + vehNo + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["APIResponse"].ToString();
            }
            return null;
        }

        public bool AddEditVehicleAPICache(string vehNo, string apiResponse)
        {
            string escapedResponse = apiResponse.Replace("'", "''");
            QueryString = "EXEC USP_AddEdit_VehicleAPICache '" + vehNo + "', '" + escapedResponse + "'";
            GF.GetDataTableFromSP(QueryString);
            return true;
        }

        public List<CYGNUS_Vehicle_Document_Type> GetVehicleDocumentTypeWise(string Vehicle_Type, string Vendor_Type)
        {
            QueryString = "EXEC USP_GetVehicleDocumentsByVehicleAndVendorType '" + Vehicle_Type + "','" + Vendor_Type + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Document_Type> List = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Document_Type>(Dt);
            return List;
        }

        public List<CYGNUS_Vehicle_Document_Type> GetVehicleDocument(string VehicleId)
        {
            QueryString = "SELECT DocTypeId as Document_Id, DocPath, FromDate, ToDate, VMD.InsuranceVendor, cmg.CodeDesc as Document_Name from CYGNUS_Vehicle_Master_Document VMD WITH(NOLOCK) LEFT join CYGNUS_Master_General CMG WITH(NOLOCK) ON CMG.CodeId = VMD.DocTypeId and CodeType = 'VEHDOCTYP' where VMD.VehicleId = '" + VehicleId + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Document_Type>(Dt);
        }

        #endregion

        #region Tyre Size

        public List<CYGNUS_FLEET_TYRESIZEMST> GetTyreSizeList()
        {
            // string curFile = folderPath + "CYGNUS_FLEET_TYRESIZEMST.xml";
            // if (!File.Exists(curFile))
            // {
            QueryString = "EXEC USP_GetTyreSizeList";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_FLEET_TYRESIZEMST> TyreSizeList_XML = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_TYRESIZEMST>(Dt);
            //     GF.SerializeParams<CYGNUS_FLEET_TYRESIZEMST>(TyreSizeList_XML, folderPath + "CYGNUS_FLEET_TYRESIZEMST.xml");
            // }
            // List<CYGNUS_FLEET_TYRESIZEMST> TyreSizeList = GF.DeserializeParams<CYGNUS_FLEET_TYRESIZEMST>(folderPath + "CYGNUS_FLEET_TYRESIZEMST.xml");
            // return TyreSizeList;
            return TyreSizeList_XML;
        }

        public bool InsertUpdateTyreSize(string XML, string TYRE_SIZEID)
        {
            bool Status = false;
            QueryString = "EXEC Usp_InsertUpdateTyreSize_NewPortal '" + XML + "','" + TYRE_SIZEID + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "InsertUpdateTyreSize", "", "");

            DataSet DS = GF.GetDataSetFromSP(QueryString);
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0][0].ToString() == "1")
            {
                Status = true;
                if (DS.Tables.Count > 2 && DS.Tables[1].Rows.Count > 0 && DS.Tables[2].Rows.Count > 0)
                {
                    List<CYGNUS_FLEET_TYRESIZEMST> TyreSizeList_XML = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_TYRESIZEMST>(DS.Tables[1]);
                    GF.SerializeParams<CYGNUS_FLEET_TYRESIZEMST>(TyreSizeList_XML, folderPath + "CYGNUS_FLEET_TYRESIZEMST.xml");
                }
                else
                {
                    File.Delete(folderPath + "CYGNUS_FLEET_TYRESIZEMST.xml");
                }
            }
            return Status;
        }

        #endregion

        #region Route Master location

        public List<CYGNUS_rutmas> GetRutMstDetails()
        {
            //string curFile = folderPath + "CYGNUS_rutmas.xml";
            //if (!File.Exists(curFile))
            //{
            QueryString = "EXEC GetRutMst";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_rutmas> RutMstList_XML = DataRowToObject.CreateListFromTable<CYGNUS_rutmas>(Dt);
            //    GF.SerializeParams<CYGNUS_rutmas>(RutMstList_XML, folderPath + "CYGNUS_rutmas.xml");
            //}
            //List<CYGNUS_rutmas> RutMstList = GF.DeserializeParams<CYGNUS_rutmas>(folderPath + "CYGNUS_rutmas.xml");

            //return RutMstList;
            return RutMstList_XML;
        }

        #endregion

        #region Battery Size

        public List<CYGNUS_FLEET_BATTERYSIZEMST> GetBatterySizeList()
        {
            //string curFile = folderPath + "CYGNUS_FLEET_BATTERYSIZEMST.xml";
            //if (!File.Exists(curFile))
            //{
            QueryString = "EXEC USP_GetBatterySizeList";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_FLEET_BATTERYSIZEMST> BATTERYSizeList_XML = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_BATTERYSIZEMST>(Dt);
            //    GF.SerializeParams<CYGNUS_FLEET_BATTERYSIZEMST>(BATTERYSizeList_XML, folderPath + "CYGNUS_FLEET_BATTERYSIZEMST.xml");
            //}
            //List<CYGNUS_FLEET_BATTERYSIZEMST> BATTERYSizeList = GF.DeserializeParams<CYGNUS_FLEET_BATTERYSIZEMST>(folderPath + "CYGNUS_FLEET_BATTERYSIZEMST.xml");
            //return BATTERYSizeList;
            return BATTERYSizeList_XML;
        }

        #endregion

        #region Designation Mapping

        public List<CYGNUS_Designation_Mapping> GetDesignationMappingList()
        {
            QueryString = "SELECT CDM.SrNo,WMG.CodeId AS Category,ISNULL(CDM.Designation,'') AS Designation,CDM.Active,CDM.Entryby,CDM.Entrydt FROM CYGNUS_Master_General WMG WITH(NOLOCK) LEFT JOIN CYGNUS_Designation_Mapping CDM WITH(NOLOCK) ON CDM.Category = WMG.CodeId WHERE CodeType='HIERARCHY'  AND WMG.StatusCode = 'Y'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Designation_Mapping> LeaveList = DataRowToObject.CreateListFromTable<CYGNUS_Designation_Mapping>(dataTable);
            return LeaveList;
        }
        public string InsertDesignationMappingDetails(string Xml_Mst_Details, string userName)
        {
            string Status;
            try
            {
                string sql = "EXEC USP_InsertDesignationMapping '" + Xml_Mst_Details.ReplaceSpecialCharacters() + "','" + userName + "'";
                int id = GF.SaveRequestServices(sql.Replace("'", "''"), "InsertDesignationMappingDetails", "", "");
                Status = GF.GetDataTableFromSP(sql).Rows[0][0].ToString();
            }
            catch (Exception e)
            {
                throw e;
            }

            return Status;
        }
        #endregion


        #region Driver master
        public List<CYGNUS_FLEET_DRIVER_DOCDET> GetDriverDetDetails()
        {
            string SQRY = "SELECT * FROM CYGNUS_FLEET_DRIVER_DOCDET";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);
            List<CYGNUS_FLEET_DRIVER_DOCDET> DriverDetList = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_DRIVER_DOCDET>(Dt);
            return DriverDetList;
        }
        public List<CYGNUS_FLEET_DRIVERMST> GetDriverMstDetails()
        {
            //string SQRY = "SELECT CONVERT(VARCHAR, Valdity_dt, 106) AS Valdity_date,CONVERT(VARCHAR, Valdity_Todt, 106) AS Valdity_Todate,CONVERT(VARCHAR, Date_of_Joining, 106) AS Date_Joining,CONVERT(VARCHAR, D_DOB, 106) AS Date_DOB,CONVERT(VARCHAR, Date_of_Exit, 106) AS Date_Exit,* FROM CYGNUS_FLEET_DRIVERMST";
            string SQRY = "EXEC USP_GetDriverMstDetails";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);
            List<CYGNUS_FLEET_DRIVERMST> DriverMstList = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_DRIVERMST>(Dt);
            return DriverMstList;
        }
        public string GetMaxDriverCode()
        {
            string QueryString = "select max(Driver_Id) from CYGNUS_FLEET_DRIVERMST";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return dataTable.Rows[0][0].ToString();
        }
        public DataTable GetFileName(int Id, string FileName)
        {
            if (FileName == null)
            {
                FileName = "";
            }
            string QueryString = "EXEC usp_Get_FileName '" + Id + "','" + FileName + "'";
            return GF.GetDataTableFromSP(QueryString);
        }

        public DataTable AddEditDriver(DriverViewModel DVM, string[] array, string BaseCompanyCode)
        {
            var Mode = "I";
            if (DVM.WFDM.Driver_Id != 0)
            {
                Mode = "U";
            }
            string String = "'" + DVM.WFDM.Driver_Id + "','" + Mode + "','" + DVM.WFDM.Driver_Name + "','" + DVM.WFDM.Manual_Driver_Code + "','" + DVM.WFDM.DFather_Name + "','" + DVM.WFDM.VEHNO + "','" + DVM.WFDM.License_No + "','" + GF.FormateDate(DVM.WFDM.Valdity_dt) + "','" + DVM.WFDM.ActiveFlag + "','" + DVM.WFDM.Issue_By_RTO + "','" + DVM.WFDM.Name_Of_bank + "','" + DVM.WFDM.Bank_AC_Number + "','" + DVM.WFDM.IFSC_Code + "',";
            string String1 = "'" + DVM.WFDM.Driver_Location + "','" + DVM.WFDM.Mobileno + "','" + DVM.WFDM.P_Address + "','" + DVM.WFDM.P_City + "','" + DVM.WFDM.P_Pincode + "','" + DVM.WFDM.C_Address + "','" + DVM.WFDM.C_City + "','" + DVM.WFDM.C_Pincode + "','" + DVM.WFDM.D_category + "','" + GF.FormateDate(DVM.WFDM.D_DOB) + "',";
            string String2 = "'" + DVM.WFDM.D_Ethnicity_Id + "','" + GF.FormateDate(DVM.WFDM.D_Lic_Initial_Issuance_Date) + "','" + GF.FormateDate(DVM.WFDM.D_Lic_Current_Issuance_Date) + "','" + DVM.WFDDD.License_Verified + "','" + DVM.WFDDD.Address_Verified + "','" + GF.FormateDate(DVM.WFDDD.License_Verified_Dt) + "','" + array[10] + "','" + DVM.WFDDD.Electricity_Bill_YN + "','" + array[0] + "','" + DVM.WFDDD.Telephone_Bill_YN + "',";
            string String3 = "'" + array[1] + "','" + DVM.WFDDD.BankAcc_YN + "','" + array[2] + "','" + DVM.WFDDD.Passport_YN + "','" + array[3] + "','" + DVM.WFDDD.Rationcard_YN + "','" + array[4] + "','" + DVM.WFDDD.ID_Passport_YN + "','" + array[6] + "','" + DVM.WFDDD.Driving_lic_YN + "',";
            //string String4 = "'" + array[7] + "','" + DVM.WFDDD.VoterId_YN + "','" + array[8] + "','" + DVM.WFDDD.PAN_YN + "','" + array[9] + "','" + DVM.WFDM.Guarantor_Name + "','" + DVM.WFDDD.Thumb_Impression_YN + "','" + array[11] + "','" + DVM.WFDDD.Driver_Registration_Form_YN + "','" + array[5] + "','" + DVM.WFDM.EntryBy + "','" + DVM.WFDM.UpdatedBy + "',''";
            string String4 = "'" + array[7] + "','" + DVM.WFDDD.VoterId_YN + "','" + array[8] + "','" + DVM.WFDDD.PAN_YN + "','" + array[9] + "','" + DVM.WFDM.Guarantor_Name + "','" + DVM.WFDDD.Thumb_Impression_YN + "','" + array[11] + "','" + DVM.WFDDD.Driver_Registration_Form_YN + "','" + array[5] + "','" + DVM.WFDM.EntryBy + "','" + DVM.WFDM.UpdatedBy + "','" + DVM.WFDM.DriverAccountCode + "',";
            string String5 = "'" + GF.FormateDate(DVM.WFDM.Date_of_Joining) + "','" + GF.FormateDate(DVM.WFDM.Date_of_ReJoining) + "','" + GF.FormateDate(DVM.WFDM.Date_of_Exit) + "','" + DVM.WFDM.Reason_for_Exit + "','" + DVM.WFDDD.Aadhar_card_YN + "','" + array[12] + "','" + DVM.WFDDD.Vaccine_Certificate_YN + "','" + array[13] + "','" + DVM.WFDDD.Police_Verification_Certificate_YN + "','" + array[14] + "','" + GF.FormateDate(DVM.WFDM.Valdity_Todt) + "','" + DVM.WFDM.Passport_No + "','" + DVM.WFDM.PAN_No + "','" + DVM.WFDM.Aadhar_No + "','" + DVM.WFDM.Vaccine_No + "','" + DVM.WFDM.Voter_Id + "','" + DVM.WFDM.Police_Verification + "','" + BaseCompanyCode + "'";
            string QueryString = "EXEC usp_Driver_InsertUpdate " + String + String1 + String2 + String3 + String4 + String5;
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditDriver", "", "");
            return GF.GetDataTableFromSP(QueryString);
        }
        public List<CYGNUS_acctinfo> GetVehicleAccountCodeObject()
        {
            QueryString = "EXEC Usp_Get_AccCode_For_Driver";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_acctinfo> VehicleList = DataRowToObject.CreateListFromTable<CYGNUS_acctinfo>(Dt);

            return VehicleList;
        }
        public List<CYGNUS_location> GetLocationDetailsForDriver()
        {
            QueryString = "EXEC USP_GetLocationDetails_List";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_location> List = DataRowToObject.CreateListFromTable<CYGNUS_location>(Dt);
            return List;
        }
        public List<CYGNUS_Vehicle_Master> GetVehicleObject()
        {
            string curFile = folderPath + "CYGNUS_VEHICLE_HDR.xml";
            if (!File.Exists(curFile))
            {
                QueryString = "EXEC Sp_GetVehicleObject";
                DataTable Dt = GF.GetDataTableFromSP(QueryString);
                List<CYGNUS_Vehicle_Master> VehicleList_XML = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(Dt);
                GF.SerializeParams<CYGNUS_Vehicle_Master>(VehicleList_XML, folderPath + "CYGNUS_VEHICLE_HDR.xml");
            }
            List<CYGNUS_Vehicle_Master> VehicleList = GF.DeserializeParams<CYGNUS_Vehicle_Master>(folderPath + "CYGNUS_VEHICLE_HDR.xml");
            return VehicleList;
        }
        public string CheckDuplicateDriverManualCode(string Code)
        {
            string QueryString = "EXEC CheckDuplicateDriverManualCode '" + Code + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return dataTable.Rows[0][0].ToString();
        }
        public string CheckDuplicateDriver(string number)
        {
            string QueryString = "EXEC CheckDuplicateDriver '" + number + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return dataTable.Rows[0][0].ToString();
        }
        public bool ActiveInActive_Driver(int id, string baseusername)
        {
            QueryString = "EXEC USP_ActiveInActive_Driver_Test " + id + ", '" + baseusername + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        #endregion

        #region VEHICLE TYPE WISE DOCUMENT

        public List<Cygnus_Master_VehicleType_wise_Document> Get_VehicleType_wise_DocumentDetails(string id)
        {
            QueryString = "EXEC USP_Get_VehicleType_wise_DocumentDetails '" + id + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<Cygnus_Master_VehicleType_wise_Document> List = DataRowToObject.CreateListFromTable<Cygnus_Master_VehicleType_wise_Document>(Dt);
            return List;
        }

        public DataTable GetVehicleTypeWiseDocumentDetailsDataTable(string vehicleType)
        {
            QueryString = "EXEC USP_Get_VehicleType_wise_DocumentDetails '" + (vehicleType ?? "").Replace("'", "''") + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }

        public bool SaveVehicleType_WiseDocument(string XML, string BaseUserName)
        {
            QueryString = "EXEC USP_Save_VehicleType_wise_Document '" + XML + "','" + BaseUserName + "'";
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "SaveVehicleType_WiseDocument", "", "");
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            if (Dt != null && Dt.Rows.Count > 0)
            {
                return Dt.Rows[0]["Status"].ToString() == "1";
            }
            return false;
        }

        #endregion

        #region Document Type Master for Vehicle

        public List<Cygnus_Master_Vehicle_DocumentType> GetVehicleDocumentTypeById(int id)
        {
            QueryString = "EXEC USP_GetVehicleDocumentTypeById " + id;
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<Cygnus_Master_Vehicle_DocumentType>(Dt);
        }

        public DataTable AddEditVehicleDocumentType(string XML, string BaseUserName, string CompanyCode)
        {
            QueryString = "EXEC USP_AddEdit_VehicleDocumentType '" + XML + "','" + BaseUserName + "','" + CompanyCode + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditVehicleDocumentType", "", "");
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            return Dt;
        }

        #endregion

        #region  vehicle Driver Mapping 
        public List<Cygnus_VehicleDriver_Mapping> Get_VehicleDriverMapping_Details(int id, string BaseUserName)
        {
            QueryString = "EXEC USP_Get_VehicleDriverMapping_Details_New '" + id + "','" + BaseUserName + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<Cygnus_VehicleDriver_Mapping> List = DataRowToObject.CreateListFromTable<Cygnus_VehicleDriver_Mapping>(Dt);
            return List;
        }
        public DataTable VehicleDriverMappingSubmit(string Vehicleid, int First_Driver, int Second_Driver, string Baseusername)
        {
            string QueryString = "EXEC USP_InsertUpdate_VehicleDriver_Mapping '" + Vehicleid + "','" + First_Driver + "','" + Second_Driver + "','" + Baseusername + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "VehicleDriver_Mapping", "", "");
            return GF.GetDataTableFromSP(QueryString);
        }
        public void DetachDriverMapping(string vehicleId, string driverType, string driverId, string userName, string detachReason = "")
        {

            string Qry = "EXEC [usp_Detach_DriverMapping] '" + vehicleId + "','" + driverType + "','" + driverId + "','" + userName + "','" + (detachReason ?? "").Replace("'", "''") + "'";
            GF.SaveRequestServices(Qry.Replace("'", "''"), "DetachDriverMapping", "", "");
            GF.GetDataTableFromSP(Qry);
        }
        public List<CYGNUS_Vehicle_Master> GetVehicleListForMapping(string vehicleId)
        {
            QueryString = "EXEC usp_Get_VehicleMasterList_For_Mapping '" + vehicleId + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Master> VehicleList = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(dataTable);
            return VehicleList;
        }
        public List<CYGNUS_FLEET_DRIVERMST> GetAvailableDriver(string VehicleId)
        {
            string SQRY = "EXEC USP_GetAvailableDriver'" + VehicleId + "'";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);
            List<CYGNUS_FLEET_DRIVERMST> DriverMstList = DataRowToObject.CreateListFromTable<CYGNUS_FLEET_DRIVERMST>(Dt);
            return DriverMstList;
        }
        public List<CYGNUS_Vehicle_Master> Get_VehicleDriverMapping_VehicleList(string vehicleId)
        {
            QueryString = "EXEC usp_Get_VehicleDriverMapping_VehicleList '" + vehicleId + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Master> VehicleList = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(dataTable);
            return VehicleList;
        }
        public DataTable GetActiveTrip(string vehicleId)
        {
            string QueryString = "EXEC USP_GetActiveTrip '" + vehicleId + "'";
            return GF.GetDataTableFromSP(QueryString);

        }
        public DataTable GetVehicleDriverMapping(string vehicleId)
        {
            string QueryString = "EXEC USP_GetVehicleDriverMapping '" + vehicleId + "'";
            return GF.GetDataTableFromSP(QueryString);

        }

        #endregion

        #region  vehicle Trailer Mapping 
        public List<CYGNUS_TRAILER_MAPPING> GetTrailerMappingObject(string VehicleId, string BaseUserName)
        {

            string QueryString = "EXEC usp_Get_VehicleTrailerMapping_Details_New '" + VehicleId + "','" + BaseUserName + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_TRAILER_MAPPING> TrailerList = DataRowToObject.CreateListFromTable<CYGNUS_TRAILER_MAPPING>(Dt);

            return TrailerList;
        }
        public DataTable VehicleTrailerMappingSubmit(string HdrXML, string UserName)
        {
            string Qry = "EXEC [usp_InsertUpdate_TrailerMapping] '" + HdrXML + "','" + UserName + "'";
            GF.SaveRequestServices(Qry.Replace("'", "''"), "AddEditTrailerMapping", "", "");
            DataSet DS = GF.GetDataSetFromSP(Qry);

            return DS.Tables[0];
        }
        public List<CYGNUS_Vehicle_Master> GetTrailerListForMapping()
        {
            QueryString = "EXEC usp_Get_TrailerMasterList_For_Mapping ";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Vehicle_Master> VehicleList = DataRowToObject.CreateListFromTable<CYGNUS_Vehicle_Master>(dataTable);
            return VehicleList;
        }
        public void DetachTrailerMapping(string VehicleId, string userName)
        {
            string Qry = "EXEC [usp_Detach_TrailerMapping] '" + VehicleId + "','" + userName + "'";
            GF.SaveRequestServices(Qry.Replace("'", "''"), "DetachTrailerMapping", "", "");
            GF.GetDataTableFromSP(Qry);
        }
        public List<CYGNUS_Master_Users> GetUserList()
        {
            QueryString = "EXEC usp_Get_UserList ";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_Users> UserList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Users>(dataTable);
            return UserList;
        }
        #endregion

        #region Address Master

        public List<CYGNUS_Master_FuelStation> GetFuelStation()
        {
            QueryString = "EXEC Get_CYGNUS_Master_FuelStation";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_Master_FuelStation> CYGNUS_master_FuelStationList = DataRowToObject.CreateListFromTable<CYGNUS_Master_FuelStation>(dataTable);

            return CYGNUS_master_FuelStationList;
        }

        public CYGNUS_Master_FuelStation GetFuelStationById(int Id)
        {
            QueryString = "EXEC Get_CYGNUS_Master_FuelStation_ById '" + Id + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            CYGNUS_Master_FuelStation _FuelStation = DataRowToObject.CreateListFromTable<CYGNUS_Master_FuelStation>(dataTable).FirstOrDefault();
            return _FuelStation;
        }

        public bool AddEditFuelStation(string XML, string CompanyCode, string BaseUserName)
        {
            bool Status = false;
            QueryString = "EXEC usp_InsertUpdate_Master_FuelStation'" + XML + "','" + CompanyCode + "','" + BaseUserName + "'";

            // Log the request
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditFuelStation", "", "");

            DataSet DS = GF.GetDataSetFromSP(QueryString);

            // Standardize response check (compatible with Status or TranXaction patterns)
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
            {
                var row = DS.Tables[0].Rows[0];
                if (row.Table.Columns.Contains("STATUS"))
                {
                    var status = row["STATUS"].ToString();
                    if (status == "1" || status == "Done")
                    {
                        Status = true;
                    }
                }
                else if (row.Table.Columns.Contains("TranXaction") && row["TranXaction"].ToString() == "Done")
                {
                    Status = true;
                }
            }

            return Status;
        }

        public bool ActiveInActiveFuelStation(int Id)
        {
            QueryString = "EXEC USP_ActiveInActive_FuelStation " + Id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }
        #endregion

        #region Card Master
        public List<CYGNUS_Master_Card> GetCardMaster()
        {
            QueryString = "EXEC Get_CYGNUS_Master_Card";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Card>(dataTable);
        }

        public List<CYGNUS_Master_Card> GetCardMasterByType(string type)
        {
            QueryString = "EXEC Get_CYGNUS_Master_Card_By_Type '" + type + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Card>(dataTable);
        }


        public CYGNUS_Master_Card GetCardMasterById(int Id)
        {
            QueryString = "EXEC Get_CYGNUS_Master_Card_ById " + Id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Card>(dataTable).FirstOrDefault();
        }

        public bool AddEditCardMaster(string XML, string CompanyCode, string BaseUserName)
        {
            bool Status = false;
            QueryString = "EXEC usp_InsertUpdate_Master_Card '" + XML + "','" + CompanyCode + "','" + BaseUserName + "'";

            GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditCardMaster", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);

            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
            {
                var row = DS.Tables[0].Rows[0];
                if (row.Table.Columns.Contains("STATUS"))
                {
                    var status = row["STATUS"].ToString();
                    if (status == "1" || status == "Done")
                    {
                        Status = true;
                    }
                }
            }
            return Status;
        }

        public bool ActiveInActiveCardMaster(int Id)
        {
            QueryString = "EXEC USP_ActiveInActive_CardMaster " + Id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            bool status = Convert.ToBoolean(dataTable.Rows[0]["Status"]);
            return status;
        }
        #endregion

        #region Card Assignment
        public List<CYGNUS_Master_Card_Assignment> GetCardAssignment()
        {
            QueryString = "EXEC USP_GetAll_Card_Assignment";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Card_Assignment>(dataTable);
        }

        public CYGNUS_Master_Card_Assignment GetCardAssignmentById(int id)
        {
            QueryString = "EXEC USP_Get_Card_Assignment_ById " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Card_Assignment>(dataTable).FirstOrDefault();
        }

        public bool AddEditCardAssignment(string XML, string companyCode, string userName)
        {
            QueryString = "EXEC USP_AddEdit_Card_Assignment '" + XML + "','" + companyCode + "','" + userName + "'";
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditCardAssignment", "", "");
            DataSet ds = GF.GetDataSetFromSP(QueryString);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Status"].ToString() == "1";
            }
            return false;
        }

        public bool ActiveInActiveCardAssignment(int id)
        {
            QueryString = "EXEC USP_ActiveInActive_Card_Assignment " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        public DataTable CheckDuplicateCardAssignment(CYGNUS_Master_Card_Assignment model)
        {
            QueryString = "EXEC USP_CheckDuplicate_Card_Assignment " + model.Id + ",'" + model.AssignType + "','" + model.AssignPersonId + "','" + model.CardType + "'";
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return dt;
        }
        #endregion

        public List<CYGNUS_custcontract_hdr> GetCustomerContractList(string customerCode)
        {
            QueryString = "EXEC USP_Get_CustomerContract '" + customerCode + "'";
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_custcontract_hdr> ContractList = DataRowToObject.CreateListFromTable<CYGNUS_custcontract_hdr>(dt);
            return ContractList;
        }

        public CygnusCustomerGSTDetails GetGSTDetailsByGstNumber(string GSTNO)
        {
            QueryString = "EXEC USP_GetGSTDetails_By_GstNumber '" + GSTNO + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CygnusCustomerGSTDetails>(dataTable).FirstOrDefault();
        }

        public CygnusCustomerGSTDetails AddCustomerGSTDetails(string xmlCustGSTInfo, string GST, string BaseCompanyCode, string BaseUserName, string apiResponse)
        {
            QueryString = "EXEC USP_AddCustomerGSTDetails '" + xmlCustGSTInfo + "','" + GST + "','" + BaseCompanyCode + "','" + BaseUserName + "','" + (apiResponse ?? "").Replace("'", "''") + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CygnusCustomerGSTDetails>(dataTable).FirstOrDefault();
        }

        #region Consignee Master
        public DataTable AddEditConsignee(string XML_Main, string Flag, string CompanyCode, string BaseUserName)
        {
            QueryString = "EXEC USP_InsertUpdate_Consignee '" + XML_Main + "' ,'" + Flag + "','" + CompanyCode + "','" + BaseUserName + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditConsignee", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            return DS.Tables[0];
        }
        public List<Cygnus_Consignee_Master> GetConsigneeDetails(string Consignee_code)
        {
            QueryString = "EXEC USP_GetConsigneeDetails '" + Consignee_code + "'";

            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<Cygnus_Consignee_Master> List = DataRowToObject.CreateListFromTable<Cygnus_Consignee_Master>(Dt);
            return List;
        }
        #endregion

        #region Expense Master
        public List<CYGNUS_Master_Expense> GetExpenseMaster()
        {
            QueryString = "EXEC USP_GetAll_Expense";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Expense>(dataTable);
        }

        public CYGNUS_Master_Expense GetExpenseMasterById(int id)
        {
            QueryString = "EXEC USP_Get_Expense_ById " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Expense>(dataTable).FirstOrDefault();
        }

        public bool AddEditExpenseMaster(string xmlData, string companyCode, string userName)
        {
            QueryString = "EXEC USP_AddEdit_Expense '" + xmlData + "','" + companyCode + "','" + userName + "'";
            int reqId = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditExpenseMaster", "", "");
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dt.Rows[0]["Status"]);
        }

        public bool ActiveInActiveExpenseMaster(int id, string userName)
        {
            QueryString = "EXEC USP_ActiveInActive_Expense " + id + ",'" + userName + "'";
            int reqId = GF.SaveRequestServices(QueryString.Replace("'", "''"), "ActiveInActiveExpenseMaster", "", "");
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dt.Rows[0]["Status"]);
        }
        #endregion

        #region Geofence Master
        public List<CYGNUS_Master_Geofence> GetGeofenceMaster()
        {
            QueryString = "EXEC USP_GetAll_Geofence";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Geofence>(dataTable);
        }

        public CYGNUS_Master_Geofence GetGeofenceMasterById(int id)
        {
            QueryString = "EXEC USP_Get_Geofence_ById " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Geofence>(dataTable).FirstOrDefault();
        }

        public CYGNUS_Master_Geofence GetGeofenceByName(string geofenceName)
        {
            QueryString = "EXEC USP_Get_Geofence_By_Name '" + geofenceName + "'";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return DataRowToObject.CreateListFromTable<CYGNUS_Master_Geofence>(dataTable).FirstOrDefault();
        }

        public bool AddEditGeofenceMaster(string xmlData, string companyCode, string userName)
        {
            QueryString = "EXEC USP_AddEdit_Geofence '" + xmlData + "','" + companyCode + "','" + userName + "'";
            int reqId = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddEditGeofenceMaster", "", "");
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dt.Rows[0]["Status"]);
        }

        public bool ActiveInActiveGeofenceMaster(int id, string userName)
        {
            QueryString = "EXEC USP_ActiveInActive_Geofence " + id + ",'" + userName + "'";
            int reqId = GF.SaveRequestServices(QueryString.Replace("'", "''"), "ActiveInActiveGeofenceMaster", "", "");
            DataTable dt = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dt.Rows[0]["Status"]);
        }
        #endregion

        #region Get Country State City Zone based on Pincode
        public DataSet GetCountryStateCityZoneByPincode(string code, string type)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "Code", code },
                { "Type", type }
            };

            return GF.GetdatasetFromParams("USP_GetCountry_state_City_onPincode", parameters);
        }
        #endregion

        #region Notes Category Mapping
        public List<CYGNUS_NotesCategory_Mapping> GetNotesCategoryMappingList()
        {
            QueryString = "EXEC USP_GetNotesCategoryMappingList";
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_NotesCategory_Mapping> NotesCatList = DataRowToObject.CreateListFromTable<CYGNUS_NotesCategory_Mapping>(dataTable);
            return NotesCatList;
        }
        public DataTable InsertNotesCategoryMappingDetails(string Xml_Mst_Details, string BaseUserName, string BaseCompanyCode)
        {
            string QueryString = "EXEC USP_InsertNotesCategoryMapping '" + Xml_Mst_Details.ReplaceSpecialCharacters() + "','" + BaseUserName + "','" + BaseCompanyCode + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "InsertNotesCategoryMappingDetails", "", "");
            return GF.GetDataTableFromSP(QueryString);
        }
        #endregion

        #region Vendor
        public DataTable InsertVendor(string XML, string DetXML, string GSTXML, string VendorDocXML, string EditFlag, string EntryBy)
        {
            DataTable DT = new DataTable();
            QueryString = "EXEC Usp_InsertVendor_NewPortal'" + XML + "','" + DetXML + "','" + GSTXML + "','" + VendorDocXML + "','" + EditFlag + "','" + EntryBy + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "InsertVendor", "", "");
            DataSet DS = GF.GetDataSetFromSP(QueryString);
            return DS.Tables[0];
        }

        public List<CYGNUS_VENDOR_DET> GetVendorDetObject()
        {
            QueryString = "EXEC Sp_GetVendorDetObject";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_VENDOR_DET> VendorList = DataRowToObject.CreateListFromTable<CYGNUS_VENDOR_DET>(Dt);
            return VendorList;
        }

        public List<Cygnus_Vendor_Document> GetVendorDoc(string VendorId)
        {
            QueryString = "EXEC Sp_GetVendorDocument '" + VendorId + "'";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<Cygnus_Vendor_Document> VendorDocList = DataRowToObject.CreateListFromTable<Cygnus_Vendor_Document>(Dt);
            return VendorDocList;
        }

        public DataTable GetStateWiseGSTDetails(string flag, string Code)
        {
            QueryString = "EXEC usp_GetStateWiseGSTDetails '" + flag + "','" + Code + "'";
            return GF.GetDataTableFromSP(QueryString);
        }

        public bool ActiveInActiveVendor(string id)
        {
            QueryString = "EXEC USP_ActiveInActive_Vendor " + id;
            DataTable dataTable = GF.GetDataTableFromSP(QueryString);
            return Convert.ToBoolean(dataTable.Rows[0]["Status"]);
        }

        public List<CYGNUS_VENDOR_HDR> GetVendor()
        {
            QueryString = "EXEC USP_GetVendor";
            DataTable Dt = GF.GetDataTableFromSP(QueryString);
            List<CYGNUS_VENDOR_HDR> VendorList = DataRowToObject.CreateListFromTable<CYGNUS_VENDOR_HDR>(Dt);
            return VendorList;
        }
        #endregion
    }
}
