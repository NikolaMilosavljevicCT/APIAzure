﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.DTOs.Events
{
    public class EventsIntervalDTO
    {
        public string TableName { get; set; }
        public int EventTime { get; set; }
        public string Condition { get; set; }
        public string Action { get; set; }
        public string EventType { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }
}
