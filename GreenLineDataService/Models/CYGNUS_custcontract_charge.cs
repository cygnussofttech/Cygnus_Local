using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    [XmlRoot("DocumentElement"), XmlType("DocumentElement")]
    public class CYGNUS_custcontract_charge
    {
        public string ContractId { get; set; }
        public string Custcode { get; set; }
        public string trans_type { get; set; }
        public string rate_type { get; set; }
        public string matrix_type { get; set; }
        public string pkp_dly { get; set; }
        public string fuel_type { get; set; }
        public string cft_weight_type { get; set; }
        public string service_type { get; set; }
        public string lr_type { get; set; }
        public string Dedicated_Vehicle { get; set; }
        public string risktype { get; set; }
        public string Bill_Org { get; set; }
        public DateTime Contract_Stdate { get; set; }

        //Freight Related Services
        //public bool Differ_allowed { get; set; }
        //public bool freight_per_inv { get; set; }
        //public decimal difer_days { get; set; }
        //public decimal difer_disc_per { get; set; }
        //public decimal rate_per_inv { get; set; }
        //public string frt_disc_ratetype { get; set; }
        //public string min_freight_type { get; set; }
        //public bool Codecharge { get; set; }
        //public bool DACcarage { get; set; }
        //public bool ODAchatrge { get; set; }
        //public string oda_type { get; set; }

        //public string cod_chrg_rs { get; set; }
        //public decimal cod_chrg_per { get; set; }
        //public string cod_chrg_ratetype { get; set; }


        //public string dacc_chrg_rs { get; set; }
        //public decimal dacc_chrg_per { get; set; }
        //public string dacc_chrg_ratetype { get; set; }

        //Demurrage Information

        //public bool IsDemChargeApplcbl { get; set; }
        //public string dem_cal_bas { get; set; }
        //public decimal dem_day { get; set; }
        //public decimal dem_chrg_min { get; set; }

        //public decimal dem_chrg_per { get; set; }
        //public string dem_chrg_ratetype { get; set; }
        //public decimal dem_chrg_max { get; set; }


        //Diesel Hike
        public bool IsDieselHikeApplicable { get; set; }
        public string DieselHikeReviewType { get; set; }
        public decimal BaseDieselRate { get; set; }
        public DateTime RateDate { get; set; }
        public decimal CurrentDieselRate { get; set; }
        public decimal Hike_Amount_Rs { get; set; }
        public decimal Effective_Amount_Rs { get; set; }
        public decimal Hike_Amount_Percentage { get; set; }
        public decimal Effective_Amount_Percentage { get; set; }
        public decimal CurrentDieselPercentage { get; set; }
        public DateTime EffectedDate { get; set; }
        public string EffectiveType { get; set; }
        public string ReferenceFuelType { get; set; }
        public string fuelSurchagre { get; set; }


        

       


        //public string use_invno { get; set; }
        //public string comm_bus_period { get; set; }
        //public string spl_inst { get; set; }
        //public string dem_chrg_rs { get; set; }
        //public double surcharge { get; set; }
        //public string vol_yn { get; set; }
        //public double STSRE_chrg { get; set; }
        //public string inst_show_yn { get; set; }
        //public string locmultipoint_yn { get; set; }
        //public string inv_bas { get; set; }
        //public string charge_bas { get; set; }
        //public string remarks { get; set; }
        //public decimal Srno { get; set; }
        //public string del_before_dem_yn { get; set; }
        //public decimal frt_disc_rate { get; set; }
        //public double Hamali_chrg { get; set; }
        //public string flatloading_yn { get; set; }
        //public string billcol_loccode { get; set; }
        //public double DC_DD_chrg { get; set; }
        //public string flatmultidelivery_yn { get; set; }
        //public string diesel_hike_yn { get; set; }
        //public string locmultipickup_yn { get; set; }
        //public string oda_yn { get; set; }
        //public string flatmultipickup_yn { get; set; }
        //public decimal dp_chrg_perkg { get; set; }
        //public string BillLocRule { get; set; }
        //public string frt_disc_contractid { get; set; }
        //public decimal insu_per { get; set; }
        //public decimal max_oct_sur { get; set; }
        //public string bill_freq { get; set; }
        //public string frt_disc_yn { get; set; }

        //public DateTime lasteditdate { get; set; }

        //public string min_subtotal_percent_yn { get; set; }
        //public double OTCHG { get; set; }
        //public string mp_chrg_appl { get; set; }
        //public string billgen_loccode { get; set; }
        //public decimal lowlim_freightrate_road { get; set; }
        //public string billsub_loccode { get; set; }
        //public string flatunloading_yn { get; set; }
        //public string min_freight_percent_yn { get; set; }
        //public decimal dd_chrg_perkg { get; set; }
        //public decimal comm_bus { get; set; }
        //public string oct_sur_bas { get; set; }
        //public string oct_sur_yn { get; set; }
        //public string stax_paidby { get; set; }
        //public decimal upplim_freightrate_air { get; set; }
        //public string contractmode { get; set; }
        //public string difer_disc_yn { get; set; }
        //public string credit_limit_period { get; set; }
        //public string contract_type { get; set; }
        //public double fuelchrg { get; set; }
        //public string sku_yn { get; set; }
        //public string flatmultipoint_yn { get; set; }
        //public decimal dem_minfrt { get; set; }
        //public decimal risk_chrg { get; set; }
        //public decimal rate_oct_sur { get; set; }
        //public decimal min_oct_sur { get; set; }
        //public string locloading_yn { get; set; }
        //public string lasteditby { get; set; }
        //public double oda_chrg { get; set; }
        //public decimal st_chrg { get; set; }
        //public decimal fov_chrg_carrier { get; set; }
        //public string use_invdt { get; set; }
        //public decimal fov_chrg_owner { get; set; }
        //public string frt_disc_rate_yn { get; set; }
        //public decimal FODCharge { get; set; }
        //public string locmultidelivery_yn { get; set; }
        //public string locunloading_yn { get; set; }
        //public string rate_per_inv_yn { get; set; }

        //public string ChargName { get; set; }
        //public string TrnsMode { get; set; }
        //public string Matrixtype { get; set; }
        //public string From { get; set; }
        //public string To { get; set; }

        ////*****
        //public bool Ischeck { get; set; }
        //public bool Volumetric { get; set; }
        //public bool MaxDisc { get; set; }
        //public string MaxDiscountY_N { get; set; }
        //public decimal MaxDiscount { get; set; }
        //public bool UseInvoiceNo { get; set; }
        //public bool chkfuelsurcharge { get; set; } //diesel_hike_yn Fuel Surcharge Applicable
        //public bool chkoctroisurcharge { get; set; } //oct_sur_yn Octroi Surcharge Applicable

        //public string defaultvalue { get; set; }
        //public string BillingState { get; set; }
        //public bool IsPerPieceRate { get; set; }

        //public DateTime GraceUpdate { get; set; }

        //public bool IsLocWiseMinFreight { get; set; }
        //public bool IsCustToCustFreight { get; set; }

        //public decimal CHRGWT { get; set; }
        //public string SService { get; set; }




    }
}