using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_acctinfo
    {
        public string Tel { get; set; }
        public int OrderNoTB { get; set; }
        public string Groupcode { get; set; }
        public string manual_yn { get; set; }
        public string Accdesc { get; set; }
        public string Acccategory { get; set; }
        public string depmethod { get; set; }
        public double deprate { get; set; }
        public decimal secid { get; set; }
        public string Entryby { get; set; }
        public string bankAcct_Deposit { get; set; }
        public string EntryAllows { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public DateTime Entrydt { get; set; }
        public string bkloccode { get; set; }
        public string Email { get; set; }
        public string Company_Acccode { get; set; }
        public string panno { get; set; }
        public string Comments { get; set; }
        public string Pin { get; set; }
        public string State { get; set; }
        public string AccountAccessBy { get; set; }
        public string ACTIVEFLAG { get; set; }
        public decimal secdetailid { get; set; }
        public string Acccode { get; set; }
        public string subgroup { get; set; }
        public DateTime UpdateReportDate { get; set; }
        public string bkAcctNo { get; set; }
        public string Contperson { get; set; }
        public string AccountAccessIn { get; set; }
        public string Brcd { get; set; }
        public string ACCTTYPE { get; set; }
        public string Address { get; set; }
        public decimal orderno { get; set; }

        public string TINNo { get; set; }
        public string CSTNo { get; set; }

        //jemin
        public decimal OpenCredit { get; set; }
        public decimal OpenDebit { get; set; }
        public string FinYear { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public bool IsChecked { get; set; }

        //jemin voucher cansel
        public string VoucherNo { get; set; }
        public DateTime VDate { get; set; }
        public DateTime CDate { get; set; }
        public int SrNo { get; set; }
        public string cancel { get; set; }
        public bool IsCurrencyDisplay { get; set; }
        public string BaseCurrency { get; set; }
        public string ExchangeCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal FCBillAmount { get; set; }
        public string TransactionDateName { get; set; }
    }
}