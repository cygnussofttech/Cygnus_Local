using GreenLineDataService.Models;
using GreenLineDataService.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GreenLineDataService.Helper.Service.ReportService;

namespace GreenLineDataService.Helper.Interface
{
    public interface IReportService
    {
        List<Master_ParameterSet_Details> USP_GETParameterDetails_ForSet(int Id);
        List<Master_ParameterSet> Usp_GetAllParameterSet();
        DataTable AddEditParameterSet(int Id, string XMLHeader, string XMLDetails);
        DataTable GetParentMenu(string Id);
        DataTable GetParmeters();
        List<Customer> GetCustomerListFromSearch(string Serach, string SearchType);
        List<Master_ParameterSet_Details> GETParameterDetails(int ParameterSetId, int Id);
        List<Master_Parameter_Inputs> GetSubParameter(int Id);
        CYGNUS_Master_Reports GetReportDetails(int Id);
        List<LinkReport> GetLinkReportDetails(int Id, string UserName);
        List<CYGNUS_Master_Reports> GetReportList(string Id, string type);
        DataTable InsertUpdateReportDetailSubmit(string ReportData);
        List<CYGNUS_Report_Fields> FieldList(int Id);
        DataTable AddEditReportFieldSet(string XMLDetails);
        List<ChartViewModel> GetChartDetails(int Id);
        DataTable Getparametername(string name, int ParentReportId);
        List<CYGNUS_Master_Reports> GetReportList(string ReportType, string ReportSubType, string UserName, int Type);
        List<CYGNUS_Master_Reports_Parameters> GetReportParameters(int Id);
        List<ReportField> GetReportFields(string SQRY);
        List<ReportParameterValue> GetDynamicParameterValue(string SQRY);
        void InsertReportHistory(string ReportId, string ParameterId, string ParameterValue, string EntryBy, string SetId);
        bool AddLinkedReport(string ReprotName, string DisplayName, string Description, bool IsActive, string EntryBy, bool isLinkedReport, int ParentReportId, string LinkedDateType, string @XmlDetail);
        bool EditLinkedReport(string ReprotName, string DisplayName, string Description, bool IsActive, string EntryBy, bool isLinkedReport, int ParentReportId, string LinkedDateType, string @XmlDetail);
        bool RefreshLinkReportdata(string RDLName);
        bool AddDownloadZip(int ReportId, string @XmlDetail, bool IsDownloaded, string DownloadBy, string FileName, bool IsActive);
        List<CYGNUS_Master_DownloadZip> GetDownloadedReportlist();
        List<CYGNUS_Master_ParameterSet_Details_SP> GetReportParameterSet_Details_SP(int ReportId);
        DataTable GetParamtersFromLinkedReport(int ReportId);
        List<Cygnus_Report_ChartDetail_Master> GetChartReportParameterSet_Details(int ReportId);
        DataTable AddEditChartSubmit(int Id, int ReportId, string ReportCategory, string LabelField, string DisplayLable, string ChartType, string ValueFields, string SortBy, string SortOrder, int TopRecords, int ChartOrder, string baseusername);
        List<Cygnus_DisplayFlowType_Details> GetFlowtypeList();
        DataTable AddEditFlowTypeSubmit(int Id, int Srno, string CompareText, string DisplayText, string DocumentType, string DisplayType, string baseusername);
        List<CYGNUS_Report_General> GetReportGeneralList(int ReportId);
        DataTable AddEditReportGeneralSubmit(int Id, int ReportId, string CodeType, string CodeId, string CodeDescription, bool IsActive, string baseusername);
        DataTable AddEditParameterSet_Details(int Id, int ReportId, string SQLParamName, string RDLParameter, string DefaultValue);
    }
}
