using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Events;
using API.DataManagement.Extensions.Events;
using API.DataManagement.Interfaces.EventsInterface;
using API.DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Repositories.EventsRepository
{
    public class EventsRepository: IEventsRepository
    {
        #region Fields
        private static EventsRepository _instance;
        private SqlConnection connection;
        private SqlTransaction transaction;
        //private SqlCommand command;
        //private Object locker = new Object();

        private Logger.Logger log = new Logger.Logger("EventsLogs", "EventsLog");
        #endregion
        //Openning MSSQL base connection
        #region Events - MSSQL
        public static EventsRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventsRepository();
                }
                return _instance;
            }
        }
        public EventsRepository()
        {
            connection = new SqlConnection(@"Data Source=ca172sql;Initial Catalog=ctickets;User ID=ctsmatdb;Password=ctsmatdb");
        }
        public void OpenConnection()
        {
            connection.Open();
        }
        public void CloseConnection()
        {
            connection.Close();
        }
        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            transaction.Commit();
        }
        public void RollbackTransaction()
        {
            transaction.Rollback();
        }
        #endregion
        //Methods for implementation Events in MSSQL database
        #region Methods - MSSQL Events
        /// <summary>
        /// Retrieve all record from dbo.escalations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Event>> GetAllEventsAsync()
        {
            OpenConnection();
            List<Event> list = new List<Event>();
            string query = "SELECT * FROM dbo.escalations";
            SqlCommand command = new SqlCommand(query, connection);
            DataTable table = new DataTable();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                Event newEvent = new Event();
                if (!Convert.IsDBNull(row[0]))
                {
                    newEvent.IdJob = Convert.ToInt32(row[0]);
                }
                if (!Convert.IsDBNull(row[1]))
                {
                    newEvent.TableName = row[1].ToString();
                }
                if (!Convert.IsDBNull(row[2]))
                {
                    newEvent.EventTime = Convert.ToInt32(row[2]);
                }
                if (!Convert.IsDBNull(row[3]))
                {
                    newEvent.Condition = row[3].ToString();
                }
                if (!Convert.IsDBNull(row[4]))
                {
                    newEvent.Action = row[4].ToString();
                }
                if (!Convert.IsDBNull(row[5]))
                {
                   //newEvent.Timer = row[5].ToString();
                }
                if (!Convert.IsDBNull(row[6]))
                {
                   newEvent.EventType = row[6].ToString();
                }
                if (!Convert.IsDBNull(row[7]))
                {
                    newEvent.DayOfMonth = Convert.ToInt32(row[7]);
                }
                if (!Convert.IsDBNull(row[8]))
                {
                    newEvent.DayOfWeek = Convert.ToInt32(row[8]);
                }
                if (!Convert.IsDBNull(row[9]))
                {
                    newEvent.Hours = Convert.ToInt32(row[9]);
                }
                if (!Convert.IsDBNull(row[10]))
                {
                    newEvent.Minutes = Convert.ToInt32(row[10]);
                }
                list.Add(newEvent);
            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// First part of creating events where condition is checked which is given by user
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<bool> EventAsync(string tableName, string condition)
        {
            int result = 0;
            bool isChecked = false;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    OpenConnection();
                }
                BeginTransaction();
                string query = null;

                query = "SELECT * FROM " + tableName + " WHERE " + condition + "";//"SELECT COUNT(" + txtCondition.Text + ") FROM " + cmbAssociatedForms.SelectedItem.ToString() + "";
                                                                                 // + cmbAssociatedForms.SelectedItem.ToString() + 

                SqlCommand command = new SqlCommand(query, connection, transaction);
                result = (int)await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "StackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result < 0)
                {
                    isChecked = true;
                    RollbackTransaction();
                }
                else
                {
                    CommitTransaction();
                }
                CloseConnection();
            }
            return isChecked;
        }
        /// <summary>
        /// Second part (Action -> PushFields) of creating events
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> PushFieldAsync(string action)
        {
            int result = 0;
            bool isChecked = false;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = null;
                try
                {
                    query = action;//cmbAssociatedForms.SelectedItem.ToString()
                }
                catch (Exception ex)
                {
                    log.WriteLine("ERROR : " + ex.Message + "StackTrace : " + ex.StackTrace);
                }

                SqlCommand commad = new SqlCommand(query, connection, transaction);

                result = await commad.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "StackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == 0)
                {
                    isChecked = true;
                    RollbackTransaction();
                }
                else
                {
                    CommitTransaction();
                }
                CloseConnection();
            }
            return isChecked;
        }
        /// <summary>
        /// Method for insert record in dbo.escalations when interval is specified by user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> InsertIntoEventsIntervalAsync(EventsIntervalDTO data)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "INSERT INTO dbo.escalations (TableName, EventTime, Condition , Action, EventType)" +
                              "values('" + data.TableName + "', " + Convert.ToInt32(data.EventTime) + ", '" + data.Condition + "','" + data.Action + "','Interval')";
                SqlCommand command = new SqlCommand(query, connection, transaction);
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "StackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == 1)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
                CloseConnection();
            }
            return result;
        }
        /// <summary>
        /// Method for insert record in dbo.escalations when Timer is specified by user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> InsertIntoEventsTimerAsync(EventsTimerDTO data)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string upit = "INSERT INTO dbo.escalations (TableName, Condition , Action, Timer, EventType, DayOfMonth, DayOfWeek, Hours, Minutes)" +
                                  "values('" + data.TableName + "', '" + data.Condition + "','" + data.Action + "', '" + data.Timer + "', 'Timer', " + data.DayOfMonth + ", " + data.DayOfWeek + ", " + data.Hours + ", " + data.Minutes + ")";

                SqlCommand command = new SqlCommand(upit, connection, transaction); 
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "StackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == 1)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
                CloseConnection();

                ShouldRunToday(data);
            }
            return result;
        }
        #endregion

        #region HelpMethods
        public void ShouldRunToday(EventsTimerDTO data)
        {
            TimeSpan timeToGo = new TimeSpan(data.Hours, data.Minutes, 00) - DateTime.Now.TimeOfDay;
            if ((data.DayOfWeek == (int)DateTime.Today.DayOfWeek || data.DayOfMonth == DateTime.Now.Day) && timeToGo  > TimeSpan.Zero) {
                _ = new TimerService( timeToGo, data.TableName,data.Condition,data.Action.Replace("''", "'"));
            }
        }
        #endregion
    }
}
