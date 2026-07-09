using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLineDataService.Models
{
    public class ChangeSettingsViewModel
    {
        public List<ChgangeCompany> ListCompany { get; set; }
        public List<ChgangeLoc> ListLocation { get; set; }
        public List<vw_Get_Finacial_Years> ListYears { get; set; }
        public ChangeSetting changeSetting { get; set; }
        public int submitType { get; set; }
    }

    public partial class ChgangeLoc
    {
        public string LocCode { get; set; }
        public string Location { get; set; }
    }

    public partial class ChgangeCompany
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
    }

    public partial class ChangeSetting
    {
        public string LocCode { get; set; }
        public string FinYear { get; set; }
        public string CompanyCode { get; set; }
    }
}