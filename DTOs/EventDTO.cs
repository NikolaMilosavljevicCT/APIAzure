using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs
{
    public class EventDTO
    {   
        public string TableName { get; set; }
        public string IntervalType { get; set; }
        public string Condition { get; set; }
        public string Action { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }
}
