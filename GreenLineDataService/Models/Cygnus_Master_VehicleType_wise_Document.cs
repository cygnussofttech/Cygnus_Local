using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace GreenLineDataService.Models
{
    public class Cygnus_Master_VehicleType_wise_Document
    {
       public int Id { get; set; }
       public string Vehicle_DocumentType { get; set; }
        public string Vehicle_DocumentTypeID { get; set; }
        public string Documnet_Id { get; set; }
        public string Document_Name { get; set; }
        public string ShortName { get; set; }
        public bool Is_RequireFor_Own { get; set; }
        public bool Is_RequireFor_Market { get; set; }
        public bool Is_Active { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public string EntryBy { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public int Document_Assign {  get; set; }
    }
}