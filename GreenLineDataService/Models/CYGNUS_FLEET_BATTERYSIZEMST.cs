using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_FLEET_BATTERYSIZEMST
    {
        public string BATTERY_SIZEID { get; set; }
        public string BATTERY_SIZENAME { get; set; }
        public string SizeNameFirst { get; set; }
        public string SizeNameSecond { get; set; }
        public string SizeNameThird { get; set; }
        public string SIZE_ACTIVEFLAG { get; set; }
        public string SIZE_ENTRYBY { get; set; }
        public Nullable<System.DateTime> SIZE_ENTRYDT { get; set; }
        public string BATTERY_TYPEID { get; set; }
        public string BATTERY_PATTERNID { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BATTERYTYPE { get; set; }
    }
}