using GreenLine.Classes;
using GreenLineDataService.Helper.Interface;
using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Helper.Service
{
    public class ReportService: IReportService
    {
        GeneralFunctions GF = new GeneralFunctions();

        #region Report Setting

        #region Parameter Set

        public List<Master_ParameterSet_Details> USP_GETParameterDetails_ForSet(int Id)
        {
            string SQRY = "EXEC USP_GETParameterDetails_ForSet '" + Id.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<Master_ParameterSet_Details> EnquiryList = DataRowToObject.CreateListFromTable<Master_ParameterSet_Details>(Dt1);
            return EnquiryList;
        }

        public List<Master_ParameterSet> Usp_GetAllParameterSet()
        {
            string SQRY = "EXEC Usp_GetAllParameterSet";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<Master_ParameterSet> ParameterSetList = DataRowToObject.CreateListFromTable<Master_ParameterSet>(Dt1);
            return ParameterSetList;
        }

        public DataTable AddEditParameterSet(int Id, string XMLHeader, string XMLDetails)
        {
            string SQRY = "EXEC Usp_Insert_Master_ParameterSet @ID='" + Id + "',@HeaderXML='" + XMLHeader + "',@DetailXML='" + XMLDetails + "'";
            DataTable Dt = GF.GetDateTableFromQuery(SQRY);

            return Dt;
        }

        #endregion


        #region Add Report

        public DataTable GetParentMenu(string Id)
        {
            string SQRY = "EXEC USP_GetParentMenu '" + Id + "'";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);
            return Dt;
        }

        #endregion


        #region Add Parameter

        public DataTable GetParmeters()
        {
            string SQRY = "EXEC Usp_GetAllParameter";
            DataTable Dt = GF.GetDataTableFromSP(SQRY);
            return Dt;
        }

        #endregion

        #endregion

        #region Required Methods

        public List<Customer> GetCustomerListFromSearch(string Serach, string SearchType)
        {
            string SQR = "EXEC USP_GetEmployeeUserDriverVehicleList_NewPortal_Reports '" + Serach + "','1','" + SearchType + "'  ";
            DataTable Dt = GF.GetDataTableFromSP(SQR);

            List<Customer> GetCustomerVendorList = DataRowToObject.CreateListFromTable<Customer>(Dt);

            return GetCustomerVendorList;
        }

        public class Customer
        {
            public string CUSTCD { get; set; }
            public string CUSTNM { get; set; }
        }

        #endregion

        #region Standard Report Data
        public List<Master_ParameterSet_Details> GETParameterDetails(int ParameterSetId, int Id)
        {
            string SQRY = "EXEC USP_GETParameterDetails '" + ParameterSetId.ToString() + "','" + Id.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<Master_ParameterSet_Details> EnquiryList = DataRowToObject.CreateListFromTable<Master_ParameterSet_Details>(Dt1);
            return EnquiryList;
        }

        public List<Master_Parameter_Inputs> GetSubParameter(int Id)
        {
            string SQRY = "EXEC USP_GetInputParameters '" + Id.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<Master_Parameter_Inputs> EnquiryList = DataRowToObject.CreateListFromTable<Master_Parameter_Inputs>(Dt1);
            return EnquiryList;
        }

        public CYGNUS_Master_Reports GetReportDetails(int Id)
        {
            string SQRY = "EXEC USP_GetReportDetails '" + Id.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Master_Reports> EnquiryList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Reports>(Dt1);
            return EnquiryList.First();
        }

        public List<LinkReport> GetLinkReportDetails(int Id, string UserName)
        {
            string SQRY = "EXEC USP_GetLinkReport_Details '" + Id + "','" + UserName + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<LinkReport> EnquiryList = DataRowToObject.CreateListFromTable<LinkReport>(Dt1);
            return EnquiryList;
        }

        public List<CYGNUS_Master_Reports> GetReportList(string Id, string type)
        {
            string SQRY = "EXEC USP_MasterReports_List '" + Id + "','" + type + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Master_Reports> EnquiryList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Reports>(Dt1);
            return EnquiryList;
        }

        public DataTable InsertUpdateReportDetailSubmit(string ReportData)
        {
            string QueryString = "EXEC USP_Insert_Update_CYGNUS_Master_Reports '" + ReportData + "'";
            GF.SaveRequestServices(QueryString.Replace("'", "''"), "InsertUpdateReportDetailSubmit", "", "");
            return GF.GetDateTableFromQuery(QueryString);
        }

        public List<CYGNUS_Report_Fields> FieldList(int Id)
        {
            string SQRY = "EXEC USP_GetReprotsFields '" + Id + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Report_Fields> ParameterSetList = DataRowToObject.CreateListFromTable<CYGNUS_Report_Fields>(Dt1);
            return ParameterSetList;
        }

        public DataTable AddEditReportFieldSet(string XMLDetails)
        {
            string SQRY = "EXEC Usp_InsertUpdate_ReportField @DetailXML='" + XMLDetails + "'";
            DataTable Dt = GF.GetDateTableFromQuery(SQRY);

            return Dt;
        }

        public List<ChartViewModel> GetChartDetails(int Id)
        {
            string SQRY = "EXEC USP_GetChartDetails '" + Id.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<ChartViewModel> EnquiryList = DataRowToObject.CreateListFromTable<ChartViewModel>(Dt1);
            return EnquiryList;
        }

        public DataTable Getparametername(string name, int ParentReportId)
        {
            string SQRY = "EXEC USP_GetParameter_Name '" + ParentReportId + "','" + name + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            return Dt1;
        }

        public List<CYGNUS_Master_Reports> GetReportList(string ReportType, string ReportSubType, string UserName, int Type)
        {
            string SQRY = "EXEC USP_GetUserWiseReportAccessList '" + UserName + "','" + ReportType + "','" + ReportSubType + "','" + Type + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Master_Reports> EnquiryList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Reports>(Dt1);
            return EnquiryList;
        }

        public List<CYGNUS_Master_Reports_Parameters> GetReportParameters(int Id)
        {
            string SQRY = "SELECT * FROM CYGNUS_Master_Reports_Parameters WITH (NOLOCK) WHERE ReportId= '" + Id.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Master_Reports_Parameters> EnquiryList = DataRowToObject.CreateListFromTable<CYGNUS_Master_Reports_Parameters>(Dt1);
            return EnquiryList;
        }

        public List<ReportField> GetReportFields(string SQRY)
        {
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<ReportField> EnquiryList = DataRowToObject.CreateListFromTable<ReportField>(Dt1);
            return EnquiryList;
        }

        public List<ReportParameterValue> GetDynamicParameterValue(string SQRY)
        {
            DataTable Dt1 = GF.GetDataTableFromSP(SQRY);
            List<ReportParameterValue> EnquiryList = DataRowToObject.CreateListFromTable<ReportParameterValue>(Dt1);
            return EnquiryList;
        }

        public void InsertReportHistory(string ReportId, string ParameterId, string ParameterValue, string EntryBy, string SetId)
        {
            string SQRY = "INSERT INTO [dbo].[CYGNUS_Report_History] ([ReportId] ,[ParameterId],[ParameterValue],[EntryBy],[SetId]) select '" + ReportId + "','" + ParameterId + "','" + ParameterValue + "','" + EntryBy + "','" + SetId + "'";

            DataTable Dt1 = GF.GetDataTableFromSP(SQRY);
        }

        public bool AddLinkedReport(string ReprotName, string DisplayName, string Description, bool IsActive, string EntryBy, bool isLinkedReport, int ParentReportId, string LinkedDateType, string @XmlDetail)
        {
            bool Status = false;
            string SQRY = "exec USP_INSERT_LINKEDREPORT '" + ReprotName + "','" + DisplayName + "','" + Description + "','" + IsActive + "','" + EntryBy + "','" + isLinkedReport + "','" + ParentReportId + "','" + LinkedDateType + "','" + @XmlDetail.ReplaceSpecialCharacters() + "'";
            int Id = GF.SaveRequestServices(SQRY.Replace("'", "''"), "linkedReport", "", "");
            DataSet DS = GF.GetDataSetFromSP(SQRY);
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0][0].ToString() == "1")
            {
                Status = true;
            }
            return Status;
        }

        public bool EditLinkedReport(string ReprotName, string DisplayName, string Description, bool IsActive, string EntryBy, bool isLinkedReport, int ParentReportId, string LinkedDateType, string @XmlDetail)
        {
            bool Status = false;
            string SQRY = "exec USP_UPDATE_LINKEDREPORTPARAMETERS '" + ReprotName + "','" + DisplayName + "','" + Description + "','" + IsActive + "','" + EntryBy + "','" + isLinkedReport + "','" + ParentReportId + "','" + LinkedDateType + "','" + @XmlDetail.ReplaceSpecialCharacters() + "'";
            int Id = GF.SaveRequestServices(SQRY.Replace("'", "''"), "linkedReport", "", "");
            DataSet DS = GF.GetDataSetFromSP(SQRY);
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0][0].ToString() == "1")
            {
                Status = true;
            }
            return Status;
        }

        public bool RefreshLinkReportdata(string RDLName)
        {
            bool Status = false;
            string SQRY = "EXEC USP_GetScheduleID '" + RDLName + "'";
            DataSet DS = GF.GetDataSetFromSP(SQRY);
            if (DS != null)
            {
                Status = true;
            }
            return Status;
        }

        public bool AddDownloadZip(int ReportId, string @XmlDetail, bool IsDownloaded, string DownloadBy, string FileName, bool IsActive)
        {
            bool Status = false;
            string SQRY = "exec USP_INSERT_DownloadZip '" + ReportId + "','" + @XmlDetail.ReplaceSpecialCharacters() + "','" + IsDownloaded + "','" + DownloadBy + "','" + FileName + "','" + IsActive + "'";
            int Id = GF.SaveRequestServices(SQRY.Replace("'", "''"), "AddDownloadZip", "", "");
            DataSet DS = GF.GetDataSetFromSP(SQRY);
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0] != null && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0][0].ToString() == "1")
            {
                Status = true;
            }
            return Status;
        }

        public List<CYGNUS_Master_DownloadZip> GetDownloadedReportlist()
        {
            string SQRY = "Exec USP_Get_Downloaded_Reportlist";
            DataTable Dt1 = GF.GetDataTableFromSP(SQRY);
            List<CYGNUS_Master_DownloadZip> ReportList = DataRowToObject.CreateListFromTable<CYGNUS_Master_DownloadZip>(Dt1);
            return ReportList;
        }

        public class ReportField
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public string ReprotHeader { get; set; }
            public string SubReportType { get; set; }
            public string MatchedFiledTypeCode { get; set; }
            public bool IsSelected { get; set; }
        }

        public class ReportParameterValue
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public List<CYGNUS_Master_ParameterSet_Details_SP> GetReportParameterSet_Details_SP(int ReportId)
        {
            string SQRY = "EXEC USP_GetParamtersFromLinkedReport '" + ReportId.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Master_ParameterSet_Details_SP> RptParamSetDetLst = DataRowToObject.CreateListFromTable<CYGNUS_Master_ParameterSet_Details_SP>(Dt1);
            return RptParamSetDetLst;
        }

        public DataTable GetParamtersFromLinkedReport(int ReportId)
        {
            string SQRY = "EXEC USP_GetParamtersFromLinkedReport '" + ReportId.ToString() + "'";
            DataTable Dt = GF.GetDateTableFromQuery(SQRY);
            return Dt;
        }

        public List<Cygnus_Report_ChartDetail_Master> GetChartReportParameterSet_Details(int ReportId)
        {
            string SQRY = "EXEC USP_GetChartDetails '" + ReportId.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<Cygnus_Report_ChartDetail_Master> RptParamSetDetLst = DataRowToObject.CreateListFromTable<Cygnus_Report_ChartDetail_Master>(Dt1);
            return RptParamSetDetLst;
        }

        public DataTable AddEditChartSubmit(int Id, int ReportId, string ReportCategory, string LabelField, string DisplayLable, string ChartType, string ValueFields, string SortBy, string SortOrder, int TopRecords, int ChartOrder, string baseusername)
        {
            string SQRY = "EXEC Usp_InsertUpdateChart '" + Id + "','" + ReportId + "','" + ReportCategory + "','" + LabelField + "','" + DisplayLable + "','" + ChartType + "','" + ValueFields + "','" + SortBy + "','" + SortOrder + "','" + TopRecords + "','" + ChartOrder + "','" + baseusername + "'";
            GF.SaveRequestServices(SQRY.Replace("'", "''"), "InsertUpdateChart", "", "");

            DataTable Dt = GF.GetDateTableFromQuery(SQRY);

            return Dt;
        }

        public List<Cygnus_DisplayFlowType_Details> GetFlowtypeList()
        {
            string SQRY = "EXEC USP_GET_FlowtypeList";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<Cygnus_DisplayFlowType_Details> FlowTypeList = DataRowToObject.CreateListFromTable<Cygnus_DisplayFlowType_Details>(Dt1);
            return FlowTypeList;
        }

        public DataTable AddEditFlowTypeSubmit(int Id, int Srno, string CompareText, string DisplayText, string DocumentType, string DisplayType, string baseusername)
        {
            string SQRY = "EXEC Usp_InsertUpdateFlowType '" + Id + "','" + Srno + "','" + CompareText + "','" + DisplayText + "','" + DocumentType + "','" + DisplayType + "','" + baseusername + "'";
            GF.SaveRequestServices(SQRY.Replace("'", "''"), "InsertUpdateFlowType", "", "");

            DataTable Dt = GF.GetDateTableFromQuery(SQRY);

            return Dt;
        }

        public List<CYGNUS_Report_General> GetReportGeneralList(int ReportId)
        {
            string SQRY = "EXEC USP_GET_ReportGeneralList'" + ReportId.ToString() + "'";
            DataTable Dt1 = GF.GetDateTableFromQuery(SQRY);
            List<CYGNUS_Report_General> ReportGeneralList = DataRowToObject.CreateListFromTable<CYGNUS_Report_General>(Dt1);
            return ReportGeneralList;
        }

        public DataTable AddEditReportGeneralSubmit(int Id, int ReportId, string CodeType, string CodeId, string CodeDescription, bool IsActive, string baseusername)
        {
            string SQRY = "EXEC Usp_InsertUpdateReportGeneral '" + Id + "','" + ReportId + "','" + CodeType + "','" + CodeId + "','" + CodeDescription + "','" + IsActive + "','" + baseusername + "'";
            GF.SaveRequestServices(SQRY.Replace("'", "''"), "Usp_InsertUpdateReportGeneral", "", "");

            DataTable Dt = GF.GetDateTableFromQuery(SQRY);

            return Dt;
        }

        public DataTable AddEditParameterSet_Details(int Id, int ReportId, string SQLParamName, string RDLParameter, string DefaultValue)
        {
            string SQRY = "EXEC Usp_InsertParameterSet_Details '" + Id + "','" + ReportId + "','" + SQLParamName + "','" + RDLParameter + "','" + DefaultValue + "'";
            GF.SaveRequestServices(SQRY.Replace("'", "''"), "AddEditParameterSet_Details", "", "");

            DataTable Dt = GF.GetDateTableFromQuery(SQRY);

            return Dt;
        }

        #endregion
    }
}