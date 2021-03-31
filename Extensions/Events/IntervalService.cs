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
    public class IntervalService
    {
        List<EventsIntervalDTO> allEventsInterval;
        private readonly FileWriter fw;
        private readonly int interval; 

        int numberOfNewEscalations = 0;
        int numberOfEscalationsInApp = 0;

        public IntervalService()
        {
            this.allEventsInterval = EventServiceRepository.Instance.GetAllEventsInterval();
            this.fw = new FileWriter("Events", "IntervalEventService");
            this.interval = 60000; //Intervar izvrsavanja timera 60000 = 1min

            CreateEscalationsOnServiceStart();
            NewEscalationsChecker();
        }

        //CreateEscalationsOnServiceStart();
        //NewEscalationsChecker();
        private void OnElapsedTime( EventsIntervalDTO item)
        {
            try
            {
                EventServiceRepository.Instance.Escalation(item.TableName, item.Condition, item.Action, fw);
            }
            catch (Exception ex)
            {
                _ = fw.WriteLine("ExMsg: " + DateTime.Now);
                _ = fw.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
        }
        public async void CreateEscalationsOnServiceStart()
        {
            EventServiceRepository.Instance.OtvoriKonekciju();
            _ = fw.WriteLine("/////////////////////////////START/////////////////////////////////////////////");
            _ = fw.WriteLine("Interval Service is started at " + DateTime.Now);
            _ = fw.WriteLine("///////////////////////////////////////////////////////////////////////////////");
            try
            {
                int broj = await EventServiceRepository.Instance.CountAllEscalationsAsync();
                //allEventsInterval = EventServiceRepository.Instance.ReturnAllEscalations();
                _ = fw.WriteLine("Count : " + broj);


                foreach (var item in allEventsInterval)
                {
                    CreateTimmer(item);
                }
            }
            catch (Exception ex)
            {
                _ = fw.WriteLine("ExMsg: " + DateTime.Now);
                _ = fw.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
        }
        public async void NewEscalationsChecker()
        {
            await Task.Delay(EventServiceRepository.Instance.timeOut + (numberOfNewEscalations++ * 5));
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnElepsedTimeNewEscalationsCheckerAsync);
            timer.Interval = interval; //number in milisecinds  
            timer.Enabled = true;
        }
        private async void OnElepsedTimeNewEscalationsCheckerAsync(object source, ElapsedEventArgs e)
        {
            int numberOfEscalationsInDb = await EventServiceRepository.Instance.CountAllEscalationsAsync();

            //ispis
            _ = fw.WriteLine("/////////////////////////////COUNT/////////////////////////////////////////////");
            _ = fw.WriteLine("COUNT Events in App:" + numberOfEscalationsInApp);
            _ = fw.WriteLine("COUNT Events in DB :" + numberOfEscalationsInDb);
            _ = fw.WriteLine("///////////////////////////////////////////////////////////////////////////////");

            if (numberOfEscalationsInDb > numberOfEscalationsInApp)
            {
                try
                {
                    allEventsInterval = EventServiceRepository.Instance.GetAllEventsInterval();
                    for (int i = numberOfEscalationsInApp; i < numberOfEscalationsInDb; i++)
                    {
                        CreateTimmer(allEventsInterval[i]);
                        _ = fw.WriteLine("/////////////////////SUCCES//////////////////////");
                        _ = fw.WriteLine("NEW ESCALATION CONDTION:" + allEventsInterval[i].Condition);
                        _ = fw.WriteLine("NEW ESCALATION ACTION:" + allEventsInterval[i].Action);
                    }
                }
                catch (Exception ex)
                {
                    _ = fw.WriteLine("Error:" + ex.Message);
                }
            }
        }
        private void CreateTimmer(EventsIntervalDTO item)
        {
            Thread thread = new Thread(() =>
            {
                item.Timer = new System.Timers.Timer();
                item.Timer.Elapsed += (sender, e) => OnElapsedTime(item);//new ElapsedEventHandler(OnElapsedTime);
                item.Timer.Interval = item.EventTime * interval; //number in milisecinds  
                item.Timer.Enabled = true;
            });
            thread.Start();
            thread.Join();
            numberOfEscalationsInApp++;
        }
    }
}
