using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Trip_Expense_Master
    {
        public string Trip_Exp_Header { get; set; }
        public string ID { get; set; }
        public DateTime Entry_Date { get; set; }
        public bool Active_Flag { get; set; }
        public string AccCode { get; set; }
        public string Polarity { get; set; }
        public string Operator { get; set; }
        public string Enter_By { get; set; }
        public string CodeId { get; set; }
        public string CodeDesc { get; set; }
        public bool BillableToCustomer { get; set; }
    }
}
