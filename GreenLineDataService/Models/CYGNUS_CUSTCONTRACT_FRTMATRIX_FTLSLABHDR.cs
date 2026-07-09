using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_CUSTCONTRACT_FRTMATRIX_FTLSLABHDR
    {
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Custcode { get; set; }
        public string chargetype { get; set; }
        public string chargecode { get; set; }
        public string trans_type { get; set; }
        public string Matrixtype { get; set; }
        public string ftltype { get; set; }
        public string ftl1_trip_ratetype { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public string Attachment { get; set; }

        public string FromLane { get; set; }
        public string ToLane { get; set; }
        public string LaneCode { get; set; }
        public string ProductGroup { get; set; }
        public string UOM { get; set; }
        public decimal FTL1_TRIP_RATE { get; set; }
        public decimal FTL1_TRDAYS { get; set; }
        public string BillingState { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int FromQty { get; set; }
        public int ToQty { get; set; }
        public decimal MapDistance { get; set; }
        public decimal ContractualDistance { get; set; }
        public string ChargeableOn { get; set; }
        public int MinimumGuarantee { get; set; }
        public decimal srno { get; set; }
        public decimal? EstVolume { get; set; }
        public bool IsDatePicker { get; set; }
        public DateTime? Contract_Stdate { get; set; }

        public int IsEffective { get; set; }
        public int IsFuturedDate { get; set; }
        public decimal old_rate { get; set; }
        public decimal rate_differnce { get; set; }

    }
}