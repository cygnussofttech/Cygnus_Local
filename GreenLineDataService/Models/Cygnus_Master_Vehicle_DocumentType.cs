using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_Master_Vehicle_DocumentType
    {
        public int Id { get; set; }
        public string Document_Name  { get; set; }
        public string ShortName { get; set; }
        public bool Is_Date  { get; set; }
        public bool Is_Insurance  { get; set; }
        public List<Cygnus_Master_Vehicle_DocumentType> CMVDTList { get; set; }
    }
}