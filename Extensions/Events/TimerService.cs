using API.DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DataManagement.Logger;
using System.Timers;
using System.Threading;
using API.DataManagement.Repositories.EventServiceRepository;
using API.DataManagement.DTOs.Events;
using SQLModifications.Logger;

namespace API.DataManagement.Extensions.Events
{
    public class TimerService
    {
        private readonly FileWriter fw;

        private System.Threading.Timer timer;

        private int seconds = 0;

        /// <summary>
        /// Service Constructor
        /// </summary>
        public TimerService()
        {
            this.fw = new FileWriter("Events", "TimerEventService");
            _ = fw.WriteLine("/////////////////////////////START/////////////////////////////////////////////");
            _ = fw.WriteLine("Interval Service started at " + DateTime.Now);
            _ = fw.WriteLine("///////////////////////////////////////////////////////////////////////////////");


            MethodsThatRunAtStart();
            SetUpStartTimer(new TimeSpan(24, 00, 00));
        }
        /// <summary>
        /// Used for adding new timers.
        /// </summary>
        /// <param name="timeToGo"></param>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        public TimerService(TimeSpan timeToGo, string tableName, string condition, string action)
        {
            this.fw = new FileWriter("Events", "TimerEventService");
            _ = fw.WriteLine("NEW Timer Event added for today:");
            _ = fw.WriteLine("Action: " + action + " || Time to go: " + timeToGo);
            SetUpSpecificTimer(timeToGo, tableName, condition, action);
        }

        private void MethodsThatRunAtStart()
        {
            RunDayOfWeekMethods();
            RunDayOfMonthMethods();
        }

        private void SetUpStartTimer(TimeSpan alertTime )
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            _ = fw.WriteLine("/////////////////////////////CHECK////////////////////////////////////////////");
            _ = fw.WriteLine("CHECKED for timers at: " + DateTime.Now + " || for: Day of month:" +DateTime.Now.Day + " and Day of week:" + DateTime.Now.DayOfWeek + "(" + (int)DateTime.Now.DayOfWeek + ")");
            _ = fw.WriteLine("///////////////////////////////////////////////////////////////////////////////");
            _ = fw.WriteLine("Next check at: "+DateTime.Now.AddDays(1)+" at 00 : 00 : 00");
            _ = fw.WriteLine("Next check in: " + timeToGo);
            _ = fw.WriteLine("ALL TIMER STARTED, WAITING FOR EXECUTION");
            _ = fw.WriteLine("/////////////////////////////WAIT//////////////////////////////////////////////");

            this.timer = new System.Threading.Timer(x =>
            {
                this.MethodRunAt00();

            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void SetUpSpecificTimer(TimeSpan timeToGo, string tableName, string condition, string action)
        {
            if (timeToGo  < TimeSpan.Zero)
            {
                return;//time already passed
            }
            
            this.timer = new System.Threading.Timer(x =>
            {
                this.MethodRunAtProvidedTime(tableName, condition, action);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void MethodRunAtProvidedTime(string tableName, string condition, string action)
        {
            EventServiceRepository.Instance.Escalation(tableName, condition, action, fw);
        }

        private void MethodRunAt00()
        {
            RunDayOfMonthMethods();
            RunDayOfWeekMethods();
            SetUpStartTimer(new TimeSpan(24, 00, 00));
        }

        private void RunDayOfWeekMethods()
        {
            var list = EventServiceRepository.Instance.GetAllEventsTimerWeekDay();
            _ = fw.WriteLine("Schedule for "+DateTime.Now.DayOfWeek+", TODO today:");
            if (list.Count == 0)
            {
                _ = fw.WriteLine("No Events for today.");
                return;
            }
            foreach (var item in list)
            {
                if (item.Hours==0 && item.Minutes==0)
                {
                    seconds = 5;
                }
                TimeSpan timeToGo = new TimeSpan(item.Hours, item.Minutes, seconds) - DateTime.Now.TimeOfDay;
                if (timeToGo < TimeSpan.Zero)
                {
                    _ = fw.WriteLine("WARNING! : ACTION:" + item.Action + " || Will not be done, time of the day allready passed. You can run it manualy.");
                }
                else
                {
                    SetUpSpecificTimer(timeToGo, item.TableName, item.Condition, item.Action);
                    _ = fw.WriteLine("Action: " + item.Action + " || Will run at: "+item.Hours+":"+item.Minutes+":00 || Time to go: " + timeToGo);
                }
            }
        }

        private void RunDayOfMonthMethods()
        {
            var list = EventServiceRepository.Instance.GetAllEventsMonthDay(fw);
            _ = fw.WriteLine("Schedule for every " + DateTime.Now.Day + ". in month, TODO today:");

            if (list.Count == 0)
            {
                _ = fw.WriteLine("No Events for today.");
                return;
            }
            foreach (var item in list)
            {
                if (item.Hours == 0 && item.Minutes == 0)
                {
                    seconds = 5;
                } else
                {
                    seconds = 0;
                }
                TimeSpan timeToGo = new TimeSpan(item.Hours, item.Minutes, seconds) - DateTime.Now.TimeOfDay;
                if (timeToGo  < TimeSpan.Zero)
                {
                    _ = fw.WriteLine("WARNING! : ACTION:" + item.Action + " || Will not be done, time of the day allready passed. You can run it manualy.");
                }
                else
                {
                    SetUpSpecificTimer(timeToGo, item.TableName, item.Condition, item.Action);
                    _ = fw.WriteLine("Will be done in : " + timeToGo + " ||  ACTION : " + item.Action);
                }
            }
        }
    }
}
