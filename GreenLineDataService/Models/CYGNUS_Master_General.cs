using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_Master_General
    {
        public string CodeType { get; set; }
        public string CodeId { get; set; }
        public string CodeDesc { get; set; }
        public string CodeAccess { get; set; }
        public string StatusCode { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<decimal> noofdigits { get; set; }
        public Nullable<decimal> noofchar { get; set; }
        public string codefor { get; set; }
        public string codefor1 { get; set; }
        public string location { get; set; }
        public string locaname { get; set; }
        public string loccode { get; set; }

        public string RUTCD { get; set; }
        public string RUTDESC { get; set; }
       


        //Lavnit 
        public decimal HourlyBasedSlot { get; set; }
        public int Id { get; set; }

        public bool activeservice { get; set; }
        public string Name { get; set; }
        public string Vendor_Type { get; set; }
        public string Code { get; set; }

        /* Added Fro Octroi Bill */
        public string OCT_AMT { get; set; }
        public string oct_percentage { get; set; }
        public string DKTTOT { get; set; }
        public string OCT_RECEIPTNO { get; set; }
        public string recptdt { get; set; }

        public string PKGSTY { get; set; }
        public string PKGSTYName { get; set; }
        public string PRODCD { get; set; }
    }

    public class CYGNUS_Master_General_Tab
    {
        public string CodeType { get; set; }
        public string CodeId { get; set; }
        public string CodeDesc { get; set; }
        public string Version { get; set; }
        public string Link { get; set; }
    }

    public class vw_Master_General
    {
        public string CodeID { get; set; }
        public string Code { get; set; }
        public string Loccode { get; set; }
    }

    public class VW_Hierarchy
    {
        public int CodeId { get; set; }
        public string CodeDesc { get; set; }
    }
    public class CYGNUS_Master_Items
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public Nullable<decimal> TransportRate { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }

    public class CYGNUS_Master_Sop
    {
        public int ID { get; set; }
        public string SopTitle { get; set; }
        public string SopDesc { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; }
        //31-5 TRB
        public string UserId { get; set; }
    }

}