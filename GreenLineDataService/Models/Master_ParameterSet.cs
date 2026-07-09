using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Master_ParameterSet
    {
        public int Id { get; set; }
        public string ParameterSet { get; set; }
        public bool IsActive { get; set; }

        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}