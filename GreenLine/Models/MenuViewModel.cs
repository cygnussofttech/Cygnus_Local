using System.Collections.Generic;

namespace GreenLine.Models
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string DisplayName { get; set; }
        public string NavigationURL { get; set; }
        public int ParentID { get; set; }
        public string Icon { get; set; }

        public List<MenuViewModel> Children { get; set; }

        public MenuViewModel()
        {
            Children = new List<MenuViewModel>();
        }
    }
}
