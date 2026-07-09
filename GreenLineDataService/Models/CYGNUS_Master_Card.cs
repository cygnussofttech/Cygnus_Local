using System;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Card
    {
        public int Id { get; set; }
        public string CardNo { get; set; }
        public string CardType { get; set; }
        public string CardProvider { get; set; }
        public decimal? MaxLimit { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public string NameOnCard { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public string Company_Code { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
