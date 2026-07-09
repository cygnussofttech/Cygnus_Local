using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GreenLineDataService.Models
{
    public class CYGNUS_custcontract_servicecharges  
    {
        public string contractid { get; set; }
        public string custcode { get; set; }
        public string stax_paidby_enabled { get; set; }//stax_paidby_enabled
        public bool staxpaidbyenabled { get; set; }
        public string gstpaidby { get; set; }
        public string trans_type { get; set; }
        //public string stax_paidby_opts { get; set; }
        //public string stax_paidby { get; set; }
        public decimal cft_ratio { get; set; }
        public string cft_measure { get; set; }
        //ptl
        public decimal min_fuelsurchrg { get; set; }
        public decimal max_fuelsurchrg { get; set; }
        public decimal fuelsurchrg { get; set; }
        public string fuelsurchrgbas { get; set; }
        //ftl
        public decimal min_fuelsurchrg_ftl { get; set; }
        public decimal max_fuelsurchrg_ftl { get; set; }
        public decimal fuelsurchrg_ftl { get; set; }
        public string fuelsurchrgbas_ftl { get; set; }

        //Extra
        public string service_type { get; set; }

        //public decimal lowlim_subtot { get; set; }

        //public decimal lowlim_frt { get; set; }
        //public string trans_type { get; set; }
        //public decimal cutoff_trdays { get; set; }
        //public decimal cutoff_hrs { get; set; }
        //public decimal min_frtbasrate { get; set; }
        //public decimal min_subtot_per { get; set; }
        //public decimal upplim_frt { get; set; }
        //public string frt_disc_ratetype { get; set; }
        //public decimal upplim_subtot { get; set; }
        //public decimal min_frtrate_per { get; set; }
        //public decimal frt_disc_rate { get; set; }
        //public string min_frtbas { get; set; }

        //public decimal cutoff_min { get; set; }

        ///* START GST Changes Chirag D */


        ///* END GST Changes Chirag D */
        //public bool InvoiceRateApplay { get; set; }
        //public decimal InvoiceRate { get; set; }

        //public bool delbeforedemyn { get; set; }
        //public string dem_cal_bas { get; set; }
        //public decimal dem_day { get; set; }

        //public decimal dem_chrg_min { get; set; }
        //public decimal dem_chrg_per { get; set; }

        //public string dem_chrg_ratetype { get; set; }
        //public decimal dem_chrg_max { get; set; }

        //public decimal dem_chrg_kg { get; set; }
        //public string dem_chrg_ratetype_Kg { get; set; }

        //public string ModeWiseCharges { get; set; }
        //public decimal MinWeightRate { get; set; }
        //public string MinWeightBas { get; set; }

    }
}