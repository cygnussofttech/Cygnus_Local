using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_DownloadZip
    {
        public int ReportId { get; set; }
        public string ParameterXml { get; set; }
        public string Filename { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsActive { get; set; }
        public DateTime Date { get; set; }
        public string DownloadBy { get; set; }
        public string ReportName { get; set; }
    }
}