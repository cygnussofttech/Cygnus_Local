using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenLineDataService.Models
{
    public class Cygnus_Master_Event
    {
        public int Id { get; set; }
        public string Event_Name { get; set; }
        public bool IsRequire { get; set; }
        public bool IsActive { get; set; }
    }
}
