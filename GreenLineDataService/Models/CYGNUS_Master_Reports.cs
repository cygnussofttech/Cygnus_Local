using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Reports
    {
        public int ID { get; set; }
        public string ReprotName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ReportType { get; set; }
        public string ReportSubType { get; set; }
        public string RDLName { get; set; }

        public int ParameterSetId { get; set; }
        public bool IsActive { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }

        public bool ShowParameterPrompts { get; set; }
        public bool ShowFindControls { get; set; }
        public bool ShowToolBar { get; set; }

        public bool HasAccess { get; set; }

        public string UserName { get; set; }
        public bool isLinkedReport { get; set; }
        public int ParentReportId { get; set; }
        public string LinkedDateType { get; set; }
        public int SnapshotCount { get; set; }

        public string LinkReportxml { get; set; }

        public DateTime SnapshotDate { get; set; }

        public ArrayOfReportParameter arrayOfReportParameter { get; set; }

        public string SPName { get; set; }
        public bool IsDownLoadZip { get; set; }
        public bool IsAllowSaveReport { get; set; }
        public bool IsGridReport { get; set; }

        public int PageSize { get; set; }
        public string ReportPromt { get; set; }
        public bool IsStoryRequired { get; set; }
        public string StoryLine { get; set; }
        public string TableAlias { get; set; }
        public int ReportCategory { get; set; }
        public string SortBy { get; set; }
        public bool IsSummaryEnable { get; set; }
        public bool IsAPIUseForAI { get; set; }
        public int AssistantId { get; set; }
        public string AITableName { get; set; }
        public bool IsTableCreatedAtAI { get; set; }
        public int ManualID { get; set; }
        public bool IsChart { get; set; }
    }

    public class LinkReport
    {
        public int ID { get; set; }
        public string ReprotName { get; set; }
        public string RDLName { get; set; }
        public int SnapshotCount { get; set; }
        public bool IsGridReport { get; set; }
    }

    public class CYGNUS_Report_Fields
    {
        public decimal SrNo { get; set; }
        public decimal ReportId { get; set; }
        public string SubReportType { get; set; }
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
        public decimal ColumnOrder { get; set; }
        public bool isActive { get; set; }
        public bool IsSelected { get; set; }
        public int? ManualReportId { get; set; }
        public string ReportCategory { get; set; }
        public int? ClWidth { get; set; }
        public string ClAlign { get; set; }
        public string FilterType { get; set; }
        public bool IsShow { get; set; }
        public int? SortOrder { get; set; }
        public string Datatype { get; set; }
        public string AIDescription { get; set; }
        public bool IsSummaryDisplay { get; set; }
    }
}