using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs.Events
{
    public class EventsTimerDTO
    {
        public string TableName { get; set; }
        public string Condition { get; set; }
        public string Action { get; set; }
        public string Timer { get; set; }
        public string EventType { get; set; }
        public int DayOfMonth { get; set; }
        public int DayOfWeek { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}
