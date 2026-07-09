using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_Master_Menu
    {
        public int MenuID { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int ParentID { get; set; }
        public int DisplayRank { get; set; }
        public string NavigationURL { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Action1 { get; set; }
        public string Action2 { get; set; }
        public string Action3 { get; set; }
        public int MenuLevel { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string EntryBy { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsVertical { get; set; }
        public Nullable<bool> IsUserAccess { get; set; }
        public bool IsNewPortal { get; set; }
        public string Type { get; set; }

        public bool Disabled { get; set; }

        public List<CYGNUS_Master_Menu> Childs { get; set; }
    }
}