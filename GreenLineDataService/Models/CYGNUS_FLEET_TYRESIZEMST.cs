using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_FLEET_TYRESIZEMST
    {
        public string TYRE_SIZEID { get; set; }
        public string TYRE_SIZENAME { get; set; }
        public string SizeNameFirst { get; set; }
        public string SizeNameSecond { get; set; }
        public string SizeNameThird { get; set; }
        public string SIZE_ACTIVEFLAG { get; set; }
        public string SIZE_ENTRYBY { get; set; }
        public Nullable<System.DateTime> SIZE_ENTRYDT { get; set; }
        public string TYRE_TYPEID { get; set; }
        public string TYRE_PATTERNID { get; set; }
        public string COMPANY_CODE { get; set; }

        public string TYRETYPE { get; set; }
    }
}