using System;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Payment_Links
    {
        public int Id { get; set; }
        public string PRNo { get; set; }
        public string MerchantOrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentLink { get; set; }
        public string PaymentToken { get; set; }
        public string Status { get; set; }
        public string ExpiryDate { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
