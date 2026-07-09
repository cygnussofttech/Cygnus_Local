using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_custcontract_charge_constraint
    {
        public string ContractId { get; set; }
        public string Custcode { get; set; }
        public string chargecode { get; set; }
        public string chargename { get; set; }
        public string bill_to_customer { get; set; }
        public string use_rate_type { get; set; }
        public bool IsRequired { get; set; }
        public bool useratetype { get; set; }
        //public string operator1 { get; set; }
        public string basedon { get; set; }
        public string slab_type { get; set; }

        //public string basecode { get; set; }
        public string chargetype { get; set; }
        public string use_to { get; set; }
        public string use_trans_type { get; set; }
        public string use_from { get; set; }
        public string use_slab { get; set; }
        ////*****
        //public bool usetrantype { get; set; }
        //public bool usefrom { get; set; }
        //public bool useto { get; set; }
        //public bool useslab { get; set; }
        //public bool slabtype { get; set; }
        public string InsertType { get; set; }

        public string FilterFromLoc { get; set; }
        public string FilterToLoc { get; set; }
        public string FilterTrnsMode { get; set; }
        public string FilterMatrixtype { get; set; }
        public string Matrixtype { get; set; }
        public string TransMode { get; set; }
        //public string OldFTLType { get; set; }
        //public string FTL { get; set; }

        public string SlabMS { get; set; }
        //public bool isNewZoneContractApplied { get; set; }
    }
}