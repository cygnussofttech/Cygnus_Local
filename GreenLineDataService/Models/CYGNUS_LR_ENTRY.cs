using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_LR_ENTRY
    {
        //public string InvoiceNo { get; set; }
        //public string EWayBill { get; set; }
        public string InvoiceUpload { get; set; }
        public string PRNo { get; set; }
        public string VehicleNo { get; set; }
        public string TripNo { get; set; }
        public string EventId { get; set; }

        /*Common fields*/
        public string LRUpload { get; set; }
        public string Other_Doc { get; set; }

        /*Invoice data*/
        public string invoice_no { get; set; }
        public DateTime? invoice_date { get; set; }
        public string detected_document_type { get; set; }
        public bool wrong_document { get; set; }
        public int is_blurry { get; set; }
        public string Content { get; set; }
        public string Material { get; set; }
        public string CustLRNo { get; set; }
        public string so_number { get; set; }
        public string do_number { get; set; }
        public string UOM { get; set; }
        public string delivery_no { get; set; }
        public string di_no { get; set; }
        public string shipment_no { get; set; }
        public DateTime? delivery_date { get; set; }

        public string odn_no { get; set; }
        public DateTime? odn_date { get; set; }
        public string obd_no { get; set; }
        public DateTime? obd_date { get; set; }
        public string reference_no { get; set; }
        public DateTime? eta_date { get; set; }
        public decimal? net_weight { get; set; }
        public decimal? gross_weight { get; set; }
        public int pieces_count { get; set; }
        public string flipkart_order_id { get; set; }
        public string tata_document_type { get; set; }

        /*ewaybill data*/
        public string eway_bill_no { get; set; }
        public DateTime? eway_bill_date { get; set; }
        public DateTime? eway_bill_validity { get; set; }
        public string userGstin { get; set; }
        public string consignor_name { get; set; }
        public string consignor_address { get; set; }
        public string consignor_State { get; set; }
        public string Consignee_Name { get; set; }
        public string Consignee_Code { get; set; }
        public string Consignee_PANNo { get; set; }
        public string Consignee_Address { get; set; }
        public string actualDist { get; set; }
        public string noValidDays { get; set; }

        //public string consignee_name { get; set; }
        //public string consignee_address { get; set; }
        public string consignee_city { get; set; }
        public string consignee_pincode { get; set; }

        //public string ship_to_name { get; set; }
        //public string ship_to_address { get; set; }
        public string Consignee_GST { get; set; }

        //public string ship_to_city { get; set; }
        //public string ship_to_pincode { get; set; }
        public string ship_to_state_code { get; set; }
        //public string ship_to_gst { get; set; }
        public string transporter_name { get; set; }
        public string vehicle_number { get; set; }
        public string lr_no { get; set; }
        public DateTime? lr_date { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerGST { get; set; }
        public string Rate_Type { get; set; }
        public string Contract_Id { get; set; }
        public string Customer_Code { get; set; }
        public string Organization { get; set; }
        public int To_Lane_id { get; set; }
        public int From_Lane_id { get; set; }
        public decimal Freight { get; set; }
        public decimal Total { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Rate { get; set; }
        public decimal totInvValue  { get; set; }
        public decimal totalValue { get; set; }
        public string EwayBillJson { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }
        public string udf7 { get; set; }
        public string udf8 { get; set; }
        public string udf9 { get; set; }
        public string udf10 { get; set; }

        /*Dynamic Charges*/
        public Nullable<decimal> SCHG01 { get; set; }
        public Nullable<decimal> SCHG02 { get; set; }
        public Nullable<decimal> SCHG03 { get; set; }
        public Nullable<decimal> SCHG04 { get; set; }
        public Nullable<decimal> SCHG05 { get; set; }
        public Nullable<decimal> SCHG06 { get; set; }
        public Nullable<decimal> SCHG07 { get; set; }
        public Nullable<decimal> SCHG08 { get; set; }
        public Nullable<decimal> SCHG09 { get; set; }
        public Nullable<decimal> SCHG10 { get; set; }
        public Nullable<decimal> SCHG11 { get; set; }
        public Nullable<decimal> SCHG12 { get; set; }
        public Nullable<decimal> SCHG13 { get; set; }
        public Nullable<decimal> SCHG14 { get; set; }
        public Nullable<decimal> SCHG15 { get; set; }
        public Nullable<decimal> SCHG16 { get; set; }
        public Nullable<decimal> SCHG17 { get; set; }
        public Nullable<decimal> SCHG18 { get; set; }
        public Nullable<decimal> SCHG19 { get; set; }
        public Nullable<decimal> SCHG20 { get; set; }
        public Nullable<decimal> SCHG21 { get; set; }
        public Nullable<decimal> SCHG22 { get; set; }
        public Nullable<decimal> SCHG23 { get; set; }
        public Nullable<decimal> SCHG24 { get; set; }
    }
}
