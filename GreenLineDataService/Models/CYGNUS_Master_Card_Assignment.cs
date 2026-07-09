using System;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Card_Assignment
    {
        public int Id { get; set; }
        public string AssignType { get; set; }
        public string AssignPersonId { get; set; }
        public string CardType { get; set; }
        public int CardNoId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
        public string Company_Code { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EntryBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string AssignToName { get; set; }
        public string CardNo { get; set; }
    }
}
