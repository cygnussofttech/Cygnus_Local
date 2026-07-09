using System;

namespace GreenLineDataService.Models
{
    public class CYGNUS_Master_Vehicle_Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public bool IsBlocking { get; set; }
        public bool RequireClose { get; set; }
        public string Severity { get; set; }
        public bool IsAutoEvent { get; set; }
        public bool IsActive { get; set; }
    }
}
