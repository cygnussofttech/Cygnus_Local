using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]

    public class VW_GetUserMenuRights
    {
        public string UserId { get; set; }
        public int DisplayRank { get; set; }
        public string NavigationURL { get; set; }
        public int ParentID { get; set; }
        public int MenuId { get; set; }
        public bool HasAccess { get; set; }
        public string DisplayName { get; set; }
        public int MenuLevel { get; set; }
        public bool IsNewPortal { get; set; }
        public bool IsFavorite { get; set; }
    }
}