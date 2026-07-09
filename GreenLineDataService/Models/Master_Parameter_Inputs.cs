using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Master_Parameter_Inputs
    {
        public int ParentParameter { get; set; }
        public int Id { get; set; }
        public int ParameterId { get; set; }
        public string ParentColumn { get; set; }
        public string CompareColumn { get; set; }
        public int ParameterType { get; set; }
        public bool IsParentColumnBaseValue { get; set; }
        public bool IsFirstValueSelected { get; set; }
    }
}