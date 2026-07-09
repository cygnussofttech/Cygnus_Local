using GreenLine.Classes;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Helper
{
    public class ContractService : IContractService
    {
        private string QueryString = string.Empty;
        readonly GeneralFunctions GF = new GeneralFunctions();
        public string folderPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");

        #region General & Customer Contract Master Setup
        public List<CYGNUS_LaneMaster> GetCustomerLaneId(string CustomerCode)
        {
            DataTable dt = new DataTable();
            string strsql = "SELECT lm.Lane_ID, lm.Lane_Name FROM CYGNUS_Customer_Pickup_Address pkp";
            strsql += " INNER JOIN CYGNUS_LaneMaster lm ON lm.Lane_Id = pkp.Lane_id";
            strsql += " WHERE pkp.Customer = '" + CustomerCode + "'";
            DataTable dataTable = GF.GetDataTableFromSP(strsql);
            List<CYGNUS_LaneMaster> LaneList = DataRowToObject.CreateListFromTable<CYGNUS_LaneMaster>(dataTable);
            return LaneList;
        }

        public List<CYGNUS_custcontract_hdr> GetCustContList(string CoustomerCode, string ContType)
        {
            string sqlstr = "SELECT Srno, Custcode,ContractId,ISNULL(activeflag,'N') AS Status, ";
            sqlstr = sqlstr + "Contract_Stdate,";
            sqlstr = sqlstr + "Contract_Eddate,";
            sqlstr = sqlstr + "Contract_Type=(SELECT codedesc FROM CYGNUS_Master_General WHERE codetype='PAYTYP' AND codeid=contract_type)";
            sqlstr = sqlstr + " FROM CYGNUS_custcontract_hdr WHERE custcode='" + CoustomerCode + "' and Contract_Type='" + ContType + "'";

            string SQRY = sqlstr;
            DataTable dataTable = GF.GetDataTableFromSP(SQRY);
            List<CYGNUS_custcontract_hdr> ContractList = DataRowToObject.CreateListFromTable<CYGNUS_custcontract_hdr>(dataTable);
            return ContractList;
        }

        public string getCodeContractID()
        {
            string contractID = "";
            string SQLQuery = "SELECT MAX(contractid) FROM CYGNUS_custcontract_hdr WHERE contractid NOT LIKE 'P0%' AND LEN(CONTRACTID)>8";
            DataTable dt = GF.GetDataTableFromSP(SQLQuery);
            string NewContractID = "";
            contractID = dt.Rows[0]["column1"].ToString();

            if (string.IsNullOrEmpty(contractID))
            {
                contractID = "CN0000000000";
            }
            Int64 MAXNumber = Convert.ToInt64(contractID.Substring(2, contractID.Length - 2));
            MAXNumber++;
            NewContractID = "CN" + MAXNumber.ToString().PadLeft(10, '0');
            return NewContractID;
        }

        public DataTable CustomerChnageStatus(string SrNo, string BaseUserName)
        {
            string QueryString = "Execute USP_Customer_Contract_Active_Deactive '" + SrNo + "' , '" + BaseUserName + "'";
            return GF.GetDataTableFromSP(QueryString);
        }

        public DataTable GetCustContName(string CoustomerCode, string ContarctId)
        {
            string sqlstr = "SELECT CUST.CUSTNM as Name, custcode,contractid, contract_type,contracttype=codedesc ";
            sqlstr = sqlstr + "FROM CYGNUS_CUSTHDR CUST ";
            sqlstr = sqlstr + "LEFT JOIN CYGNUS_CUSTCONTRACT_HDR CONT WITH(NOLOCK) ON CUST.CUSTCD = CONT.Custcode ";
            sqlstr = sqlstr + "LEFT JOIN CYGNUS_Master_General CM WITH(NOLOCK) ON CM.CodeId = CONT.Contract_Type AND CM.CodeType = 'PAYTYP' ";
            sqlstr = sqlstr + "WHERE custcode='" + CoustomerCode + "'";
            if (!string.IsNullOrEmpty(ContarctId))
            {
                sqlstr = sqlstr + " and cont.ContractId = '" + ContarctId + "'";
            }
            return GF.GetDataTableFromSP(sqlstr);
        }

        public DataTable GetCustomerContractDetails(string ContarctId, string QueryType)
        {
            string sqlstr = "Execute USP_GetCustomerContractDetails '" + ContarctId + "','" + QueryType + "'";
            return GF.GetDataTableFromSP(sqlstr);
        }

        public DataTable GetServiceType(string ContractID)
        {
            string strsql = "SELECT service_type FROM dbo.CYGNUS_custcontract_charge";
            strsql = strsql + " WHERE ContractID='" + ContractID + "'";
            return GF.GetDataTableFromSP(strsql);
        }

        public DataTable AddNewContract(string custcode, string Paybas, string startdate, string enddate, string ContarctID, string BaseUserName, string BaseCompanyCode)
        {
            string sqlstr = "Execute USP_AddNewCustomerContract '" + custcode + "','" + Paybas + "','" + startdate + "','" + enddate + "','" + ContarctID + "','" + BaseUserName + "','" + BaseCompanyCode + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "AddNewContract", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        public DataTable FinalContractSubmit(string ContractId)
        {
            string sqlstr = "EXEC USP_Update_CYGNUS_Custcontract_FinalSubmit'" + ContractId + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "UpdateFinalSubmitContractInfo", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion

        #region Tab 1: Contract Information

        public DataTable UpdateContractBasicInfo(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlBasicInfo)
        {
            string sqlstr = "EXEC Usp_Update_Contract_BasicDetais '" + ContractId + "' , '" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlBasicInfo.Trim() + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "UpdateContractBasicInfo", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion

        #region Tab 2: Documents

        public DataTable UpdateContractDocuments(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlDocumentInfo)
        {
            string sqlstr = "EXEC Usp_Update_Contract_Documents '" + ContractId + "' , '" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlDocumentInfo.Trim() + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "UpdateContractDcuments", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        public DataTable DeleteContractDocument(int docId, string BaseUserName)
        {
            string sqlstr = "EXEC USP_Delete_Contract_Document " + docId + ", '" + BaseUserName + "'";
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion

        #region Tab 3: Additional Field Configurator

        public DataTable UpdateContractAdditinalFieldConfigurator(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlAdditionFieldInfo)
        {
            string sqlstr = "EXEC Usp_Update_Contract_AdditinalFieldConfigurator '" + ContractId + "' , '" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlAdditionFieldInfo.Trim() + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "UpdateContractAdditinalFieldConfigurator", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion

        #region Tab 4: Service Selection

        public DataTable AddContractService(string ContractId, string Custcode, string BaseUserName, string BaseCompanyCode, string xmlServiceSelectionInfo)
        {
            string sqlstr = "EXEC USP_INSERT_CCMSERVICES '" + ContractId + "' , '" + Custcode + "' , '" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlServiceSelectionInfo.Trim() + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "AddContractService", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion

        #region Tab 5: Mode Wise Selection

        public DataTable AddContractModeViceService(string ContractId, string Custcode, string BaseUserName, string BaseCompanyCode, string xmlModeWiseSelectionInfo)
        {
            string QueryString = "";
            QueryString = "EXEC USP_INSERT_CCMMODEWISE_SERVICECHARGES_NewPortal '" + ContractId + "' , '" + Custcode + "' , '" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlModeWiseSelectionInfo.Trim() + "'";
            int id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddContractModeViceService", "", "");
            return GF.GetDataTableFromSP(QueryString);
        }

        #endregion

        #region Tab 6: Freight Charge - PTL

        public List<CYGNUS_CustContract_FRTMatrix_SingleSlab> GetSinlgeslab(string ContractID, string ChargeType, string ChargeCode, string MatrixType, string TransMode, string RateType)
        {
            string strsql = "Execute USP_FrightCharge_List '" + ContractID + "','" + ChargeType + "','" + ChargeCode + "','" + MatrixType + "','" + TransMode + "','" + RateType + "'";
            int Id = GF.SaveRequestServices(strsql.Replace("'", "''"), "GetSinlgeslab", "", "");
            DataTable dataTable = GF.GetDataTableFromSP(strsql);
            List<CYGNUS_CustContract_FRTMatrix_SingleSlab> GetSinlgeslabList = DataRowToObject.CreateListFromTable<CYGNUS_CustContract_FRTMatrix_SingleSlab>(dataTable);
            return GetSinlgeslabList;
        }

        public DataTable AddFreightChargeChargeMatrix(string xmlFRIGHTCCM, string ContractID, string BaseUserName, string BaseCompanyCode, string ratetype)
        {
            string QueryString = "EXEC Usp_ChargeMatrix_Entry '" + xmlFRIGHTCCM + "','" + ContractID + "','" + BaseUserName + "','" + BaseCompanyCode + "','" + ratetype + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddFreightChargeChargeMatrix", "", "");
            return GF.GetDataTableFromSP(QueryString);
        }

        public DataTable GetExcelDataFreightCharge(string contractID)
        {
            string SQRY = "EXEC USP_CustContract_FRTMatrix_SingleSlab_ExcelData '" + contractID + "'";
            return GF.GetDataTableFromSP(SQRY);
        }

        public DataTable UpdatePTLRateChange(string xmlData, string ContractId, string BaseUserName, string BaseCompanyCode)
        {
            string strsql = "EXEC USP_Update_PTL_Rates '" + xmlData + "', '" + ContractId + "', '" + BaseUserName + "', '" + BaseCompanyCode + "'";
            int Id = GF.SaveRequestServices(strsql.Replace("'", "''"), "UpdatePTLRateChange", "", "");
            return GF.GetDataTableFromSP(strsql);
        }

        public DataTable DeleteFreightCharges(int srno, string contractId, string BaseUserName)
        {
            string strsql = "EXEC USP_DeleteFreightCharges '" + srno + "', '" + contractId + "', '" + BaseUserName + "'";
            int Id = GF.SaveRequestServices(strsql.Replace("'", "''"), "DeleteFreightCharges", "", "");
            return GF.GetDataTableFromSP(strsql);
        }

        #endregion

        #region Tab 7: Freight Charge - FTL

        public List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> ListSundryFTL(string chargecode, string TranMode, string MatrixType, string ftltype, string rate_type, string chargetype, string ConntractId)
        {
            string strsql = "Execute USP_FrightChargeFTL_List '" + chargecode + "','" + TranMode + "','" + MatrixType + "','" + ftltype + "','" + rate_type + "','" + chargetype + "','" + ConntractId + "'";
            DataTable dataTable = GF.GetDataTableFromSP(strsql);
            List<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR> CYGNUS_SingleList = DataRowToObject.CreateListFromTable<CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR>(dataTable);
            return CYGNUS_SingleList;
        }

        public DataTable GetExcelDataFreightChargeFTL(string contractID)
        {
            string SQRY = "EXEC USP_custcontract_frtmatrix_ftlslabhdr_ExcelData '" + contractID + "'";
            return GF.GetDataTableFromSP(SQRY);
        }

        public DataTable InsertSundryFTL_NEW(string XML, string ContractId, string BaseUserName, string BaseCompanyCode, string ftltype, string ratetype)
        {
            string str = "EXEC USP_INSERT_CCM_FTLFREIGHT_RATE_NewPortal '" + XML + "','" + ContractId + "','" + BaseUserName + "','" + BaseCompanyCode + "','" + ftltype + "','" + ratetype + "'";
            int Id = GF.SaveRequestServices(str.Replace("'", "''"), "InsertSundryFTL", "", "");
            DataTable dt = GF.GetDataTableFromSP(str);
            return dt;
        }

        public DataTable UpdateFTLRateChange(string xmlData, string ContractId, string BaseUserName, string BaseCompanyCode)
        {
            string strsql = "EXEC USP_Update_FTL_Rates '" + xmlData + "', '" + ContractId + "', '" + BaseUserName + "', '" + BaseCompanyCode + "'";
            int Id = GF.SaveRequestServices(strsql.Replace("'", "''"), "UpdateFTLRateChange", "", "");
            return GF.GetDataTableFromSP(strsql);
        }

        #endregion

        #region Tab 8: Charge Matrix

        public List<CYGNUS_custcontract_charge_constraint> GetchargeMatrixList(string ContractId, string type)
        {
            string sqlstr = "Execute USP_GetchargeMatrixList '" + ContractId + "' , '" + type + "'";
            DataTable dt = GF.GetDataTableFromSP(sqlstr);
            List<CYGNUS_custcontract_charge_constraint> ContractMatrixList = DataRowToObject.CreateListFromTable<CYGNUS_custcontract_charge_constraint>(dt);
            return ContractMatrixList;
        }

        public DataTable InsertChargeMatrix1(string ContractId, string BaseUserName, string BaseCompanyCode, string xmlChargeMatrixListInfo)
        {
            string sqlstr = "EXEC USP_INSERT_ChargeMatrix '" + ContractId + "' , '" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlChargeMatrixListInfo.Trim() + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "AddContractService", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion

        #region Tab 9: Standard Charge

        public List<CYGNUS_custcontract_charge_constraint> GetStandaradChargeList(string ContractId, string type)
        {
            string sqlstr = "Execute GetStandaradChargeList '" + ContractId + "' , '" + type + "'";
            DataTable dt = GF.GetDataTableFromSP(sqlstr);
            List<CYGNUS_custcontract_charge_constraint> ContractMatrixList = DataRowToObject.CreateListFromTable<CYGNUS_custcontract_charge_constraint>(dt);
            return ContractMatrixList;
        }

        public List<CYGNUS_CustContract_FRTMatrix_SingleSlab> GetSinlgeslabForStandardCharges(string ContractID, string chargecode, string serviceType)
        {
            string strsql = "Execute USP_GetStandardCharge_PTLFTLList '" + ContractID + "','" + chargecode + "','" + serviceType + "'";
            int Id = GF.SaveRequestServices(strsql.Replace("'", "''"), "GetSinlgeslabForStandardCharges", "", "");
            DataTable dataTable = GF.GetDataTableFromSP(strsql);
            List<CYGNUS_CustContract_FRTMatrix_SingleSlab> GetSinlgeslabList = DataRowToObject.CreateListFromTable<CYGNUS_CustContract_FRTMatrix_SingleSlab>(dataTable);
            return GetSinlgeslabList;
        }

        public DataTable AddStandardChargeMatrix(string xmlFRIGHTCCM, string chargecode, string ContractID, string BaseUserName, string BaseCompanyCode, string serviceType)
        {
            string QueryString = "EXEC Usp_StandardChargeMatrix_Entry '" + xmlFRIGHTCCM + "','" + chargecode + "','" + ContractID + "','" + BaseUserName + "','" + BaseCompanyCode + "','" + serviceType + "'";
            int Id = GF.SaveRequestServices(QueryString.Replace("'", "''"), "AddStandardChargeMatrix", "", "");
            return GF.GetDataTableFromSP(QueryString);
        }

        #endregion

        #region Tab 10: Billing Configuration

        public DataTable AddBillConfigurationInfo(string ContractId, string CustCode, string BaseUserName, string BaseCompanyCode, string xmlBillConfigInfo)
        {
            string sqlstr = "EXEC Usp_Update_CYGNUS_Custcontract_Bill_Configuration '" + ContractId + "' , '" + CustCode + "' ,'" + BaseUserName + "' , '" + BaseCompanyCode + "' , '" + xmlBillConfigInfo.Trim() + "'";
            int Id = GF.SaveRequestServices(sqlstr.Replace("'", "''"), "UpdateContractBillConfigInfo", "", "");
            return GF.GetDataTableFromSP(sqlstr);
        }

        #endregion
    }
}