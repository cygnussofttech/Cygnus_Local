using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_rutmas
    {
        public decimal RUTKM { get; set; }
        public decimal SCHDEP_HR { get; set; }
        public string RUTCD { get; set; }
        public DateTime RUTSTDT { get; set; }
        public decimal STD_CONTAMT { get; set; }
        public decimal TransHrs { get; set; }
        public DateTime RUTENDDT { get; set; }
        public DateTime UPDTON { get; set; }
        public string RUTCAT { get; set; }
        public string RUTMOD { get; set; }
        public string UPDTBY { get; set; }
        public string RUTNM { get; set; }
        public string RUTENDBR { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string ControlLoc { get; set; }
        public string RUTSTBR { get; set; }
        public decimal SCHDEP_MIN { get; set; }
        public decimal Early { get; set; }
        public decimal OnTime { get; set; }
        public decimal Late { get; set; }
    }
}