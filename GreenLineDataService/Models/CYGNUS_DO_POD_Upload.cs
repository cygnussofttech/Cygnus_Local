using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_DO_POD_Upload
    {
        public long Id { get; set; }
        public string TripNo { get; set; }
        public string DocketNo { get; set; }
        public string ActivityType { get; set; }
        public string Remarks { get; set; }
        public string FilePath { get; set; }
        public string FilePathBack { get; set; }
        public decimal? Pieces { get; set; }
        public decimal? Qty { get; set; }
        public bool IsVerify { get; set; }
    }
}
