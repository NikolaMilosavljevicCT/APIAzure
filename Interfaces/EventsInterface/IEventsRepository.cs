using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Events;
using API.DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Interfaces.EventsInterface
{
    public interface IEventsRepository
    {
        #region Methods for Events in MSSQL database
        public Task<List<Event>> GetAllEventsAsync();
        //First part of creation event
        public Task<bool> EventAsync(string tableName, string condition);
        //Second part of creation event with action pushFields
        public Task<bool> PushFieldAsync(string action);
        //Insert into dbo.escalations -> events when Interval is specified
        public Task<int> InsertIntoEventsIntervalAsync(EventsIntervalDTO data);
        //Insert into dbo.escalations -> events when Timer is specified
        public Task<int> InsertIntoEventsTimerAsync(EventsTimerDTO data);
        #endregion
    }
}
