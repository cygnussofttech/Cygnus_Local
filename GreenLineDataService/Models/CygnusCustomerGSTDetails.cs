using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CygnusCustomerGSTDetails
    {
        public int id { get; set; }
        public string custcode { get; set; }
        public string statecode { get; set; }
        public string statename { get; set; }
        public string gst_registration_no { get; set; }
        public bool isActive { get; set; }
        public string entryby { get; set; }
        public DateTime entrydate { get; set; }
        public string updateby { get; set; }
        public DateTime updatedate { get; set; }
        public string statePrefix { get; set; }
        public string CSGEAddress { get; set; }
        public string PinCode { get; set; }
        public string City { get; set; }
        public string city_code { get; set; }
        public string Status { get; set; }
        public string PanNo { get; set; }
        public string TradeName { get; set; }
        public string legalName { get; set; }
        public string registrationDate { get; set; }
        public string ComapanyCode { get; set; }
        public string F_Col1 { get; set; }
        public string F_Col2 { get; set; }
        public string CountryCode { get; set; }
        public string APIResponse { get; set; }
    }

    public class GSTApiResponse
    {
        public ResponseData response { get; set; }
        public int status { get; set; }
        public int db_res { get; set; }
    }

    public class ResponseData
    {
        public int code { get; set; }
        public bool error { get; set; }
        public string message { get; set; }
        public GSTDetailsData data { get; set; }
    }

    public class GSTDetailsData
    {
        public string ctb { get; set; }
        public string ctj { get; set; }
        public string dty { get; set; }
        public List<string> nba { get; set; }
        public string stj { get; set; }
        public string sts { get; set; }
        public string cxdt { get; set; }
        public string lgnm { get; set; }
        public string rgdt { get; set; }
        public List<AdditionalAddress> adadr { get; set; }
        public string ctjCd { get; set; }
        public string gstin { get; set; }
        public PrimaryAddress pradr { get; set; }
        public string stjCd { get; set; }
        public string lstupdt { get; set; }
        public string tradeNam { get; set; }
        public string einvoiceStatus { get; set; }
    }

    public class AdditionalAddress
    {
        public string ntr { get; set; }
        public Address addr { get; set; }
    }

    public class PrimaryAddress
    {
        public string ntr { get; set; }
        public Address addr { get; set; }
    }

    public class Address
    {
        public string lg { get; set; }
        public string lt { get; set; }
        public string st { get; set; }
        public string bnm { get; set; }
        public string bno { get; set; }
        public string dst { get; set; }
        public string loc { get; set; }
        public string flno { get; set; }
        public string pncd { get; set; }
        public string stcd { get; set; }
        public string landMark { get; set; }
        public string locality { get; set; }
        public string geocodelvl { get; set; }
    }
}