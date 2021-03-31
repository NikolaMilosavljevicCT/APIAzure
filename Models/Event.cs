using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataManagement.Models
{
  
    public class Event
    {
        public int IdJob { get; set; }
        public string TableName { get; set; }
        public int EventTime { get; set; }
        public string Condition { get; set; }
        public string Action { get; set; }
        public System.Timers.Timer Timer { get; set; }
        public string EventType { get; set; }
        public int DayOfMonth { get; set; }
        public int DayOfWeek { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}
