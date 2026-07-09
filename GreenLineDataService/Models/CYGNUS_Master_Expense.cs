using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Expense
    {
        public int Id { get; set; }
        public string ExpenseName { get; set; }
        public string ExpenseType { get; set; }
        public bool BillableToCustomer { get; set; }
        public bool Addition { get; set; }
        public bool Deduction { get; set; }
        public bool AttachmentMandatory { get; set; }
        public bool RemarksMandatory { get; set; }
        public bool IsActive { get; set; }
    }
}