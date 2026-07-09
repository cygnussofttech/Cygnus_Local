using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]

    public class CYGNUS_CustContract_FRTMatrix_SingleSlab
    {
        public string Custcode { get; set; }
        public string ContractId { get; set; } 
        public string SlabTransMode { get; set; }
        public string chargecode { get; set; }
        public string FilterTrnsMode { get; set; }
        public string FilterMatrixtype { get; set; }

        public int Id { get; set; }
        public string ratetype { get; set; }
        public string chargetype { get; set; }
        public string basedon1 { get; set; }
        public string BillLoc { get; set; }
        public decimal trdays { get; set; }
        public string basecode1 { get; set; }
        public string basecode2 { get; set; }
        public string trans_type { get; set; }
        public string FromLoc { get; set; }
        public string toloc { get; set; }

        public decimal rate { get; set; }
        public int srno { get; set; }
        public string basedon2 { get; set; }

        public string Matrixtype { get; set; }
        public string TransMode { get; set; }
        public string OldFTLType { get; set; }
        public string FTL { get; set; }

        public string SlabMS { get; set; }
        public string InsertType { get; set; }

        public decimal ODACharge { get; set; }
        public string Slabtype { get; set; }

        public bool isNewZoneContractApplied { get; set; }

        public int PinCode { get; set; }

        public string ItemCode { get; set; }
        //public decimal Itemrate { get; set; }
        public decimal ChargesWight { get; set; }
        public decimal ActualWeight { get; set; }
        public decimal MinFreightCharges { get; set; }

        public bool IsItemApplied { get; set; }
        public decimal Min_Freight_BaseWise { get; set; }
        public string Customer_Base_Freight { get; set; }
        public string BillingState { get; set; }
        public string BillingState1 { get; set; }
        public bool isChecked { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string ProductGroup { get; set; }
        public string UOM { get; set; }
        public string FromLane { get; set; }
        public string ToLane { get; set; }
        public string LaneCode { get; set; }
        public int FromQty { get; set; }
        public int ToQty { get; set; }
        public decimal MapDistance { get; set; }
        public decimal ContractualDistance { get; set; }
        public string ChargeableOn { get; set; }
        public int MinimumGuarantee { get; set; }
        public string Attachment { get; set; }
        public decimal? EstVolume { get; set; }

        public bool IsDatePicker { get; set; }

        public DateTime? Contract_Stdate { get; set; }
        public int IsEffective { get; set; }
        public int IsFuturedDate { get; set; }
        public decimal old_rate { get; set; }
        public decimal rate_differnce { get; set; }
    }

    
}