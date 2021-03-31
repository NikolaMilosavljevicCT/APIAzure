using API.DataManagement.DTOs.Events;
using API.DataManagement.Models;
using SQLModifications.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataManagement.Repositories.EventServiceRepository
{
    public class EventServiceRepository
    {
        #region Fields
        private static EventServiceRepository instanca;
        private SqlConnection connection;
        private SqlTransaction transakcija;


        private FileWriter fw = new FileWriter("Events", "EventService");
        private Object locker = new Object();

        public int timeOut { get; set; } = 1000;
        #endregion

        #region BrokerEscalation
        public static EventServiceRepository Instance
        {
            get
            {
                if (instanca == null)
                {
                    instanca = new EventServiceRepository();
                }
                return instanca;
            }
        }

        private EventServiceRepository()
        {
            connection = new SqlConnection(@"Data Source=ca172sql;Initial Catalog=ctickets;User ID=ctsmatdb;Password=ctsmatdb");
        }

        public void OtvoriKonekciju()
        {
            connection.Open();
        }

        public void ZatvoriKonekciju()
        {
            connection.Close();
        }

        public void PocniTransakciju()
        {
            transakcija = connection.BeginTransaction();
        }

        public void PotvrdiTransakciju()
        {
            transakcija.Commit();
        }

        public void PonistiTransakciju()
        {
            transakcija.Rollback();
        }
        #endregion

        #region Methods
        public List<EventsIntervalDTO> GetAllEventsInterval()
        {
            if (connection.State == ConnectionState.Closed)
            {
                OtvoriKonekciju();
            }
            List<EventsIntervalDTO> list = new List<EventsIntervalDTO>();

            string query = "SELECT * FROM dbo.escalations WHERE EventType = 'Interval'";
            SqlCommand command = new SqlCommand(query, connection);

            DataTable table = new DataTable();
            SqlDataReader reader = command.ExecuteReader();

            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                list.Add(
                    new EventsIntervalDTO
                    {
                        TableName = row[1].ToString(),
                        EventTime = Convert.ToInt32(row[2]),
                        Condition = row[3].ToString(),
                        Action = row[4].ToString()
                    });
            }
            ZatvoriKonekciju();
            return list;
        }

        public List<EventsTimerDTO> GetAllEventsTimer()
        {
            if (connection.State == ConnectionState.Closed)
            {
                OtvoriKonekciju();
            }
            List<EventsTimerDTO> list = new List<EventsTimerDTO>();

            string query = "SELECT * FROM dbo.escalations WHERE EventType = 'Timer'";
            SqlCommand command = new SqlCommand(query, connection);

            DataTable table = new DataTable();
            SqlDataReader reader = command.ExecuteReader();

            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                list.Add(
                    new EventsTimerDTO
                    {
                        TableName = row[1].ToString(),
                        Condition = row[3].ToString(),
                        Action = row[4].ToString()
                    });
            }
            ZatvoriKonekciju();
            return list;
        }

        public List<EventsTimerDTO> GetAllEventsTimerWeekDay()
        {
            if (connection.State == ConnectionState.Closed)
            {
                OtvoriKonekciju();
            }
            List<EventsTimerDTO> list = new List<EventsTimerDTO>();
            string query = "SELECT * FROM dbo.escalations WHERE EventType = 'Timer' and  DayOfWeek= '" + (int)DateTime.Today.DayOfWeek + "'";
            SqlCommand command = new SqlCommand(query, connection);

            DataTable table = new DataTable();
            SqlDataReader reader = command.ExecuteReader();

            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                list.Add(
                    new EventsTimerDTO
                    {
                        TableName = row[1].ToString(),
                        Condition = row[3].ToString(),
                        Action = row[4].ToString(),
                        Hours = Convert.ToInt32(row[9]),
                        Minutes = Convert.ToInt32(row[10])
                    });
            }
            ZatvoriKonekciju();
            return list;
        }

        public List<EventsTimerDTO> GetAllEventsMonthDay(FileWriter fw)
        {
            if (connection.State == ConnectionState.Closed)
            {
                OtvoriKonekciju();
            }
            List<EventsTimerDTO> list = new List<EventsTimerDTO>();
            string query = "SELECT * FROM dbo.escalations WHERE EventType = 'Timer' and  DayOfMonth = '" + DateTime.Now.Day + "'";
            SqlCommand command = new SqlCommand(query, connection);

            DataTable table = new DataTable();
            SqlDataReader reader = command.ExecuteReader();

            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                var item = new EventsTimerDTO
                {
                    TableName = row[1].ToString(),
                    Condition = row[3].ToString(),
                    Action = row[4].ToString(),
                    Hours = Convert.ToInt32(row[9]),
                    Minutes = Convert.ToInt32(row[10]),
                    DayOfWeek = Convert.ToInt32(row[8])
                };
                if (!(item.DayOfWeek == (int)DateTime.Today.DayOfWeek)) list.Add(item);
                else _ = fw.WriteLine("Event with action: "+item.Action+" is a DUPLICATE || will be executed in other section today. Check above."); 
            }
            ZatvoriKonekciju();
            return list;
        }

        public async void Escalation(string tableName, string condition, string action, FileWriter fw)
        {
            //provera
            int rezultat = 0;


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                try
                {
                    lock (locker)
                    {
                        //OtvoriKonekciju();
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        SqlCommand command = new SqlCommand();
                        command = connection.CreateCommand();
                        transakcija = connection.BeginTransaction();
                        command.Connection = connection;
                        command.Transaction = transakcija;

                        command.CommandText = "SELECT * FROM " + tableName + " WHERE " + condition + "";
                        rezultat = (int)command.ExecuteScalar(); //rezultat = (int)command.ExecuteScalar();

                        if (rezultat != 0)
                        {
                            command.CommandText = action;
                            command.ExecuteNonQuery(); //rezultat = command.ExecuteNonQuery();
                            transakcija.Commit();
                            // = _ = fw.WriteLine("Both records are written to database.");
                            _ = fw.WriteLine("SUCCESS :" + action);
                        }
                    }
                    //konekcija.Close(); pravi gresku
                    break;

                }
                catch (Exception ex)
                {
                    _ = fw.WriteLine("Commit Exception Type: {0}" + ex.GetType() + action);
                    _ = fw.WriteLine(" Message: {0}" + ex.Message + action);
                    try
                    {
                        // PonistiTransakciju();
                        transakcija.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        _ = fw.WriteLine("Rollback Exception Type: {0}" + ex2.GetType() + action);
                        _ = fw.WriteLine(" Message: {0}" + ex2.Message + action);

                        if (stopwatch.ElapsedMilliseconds > timeOut)
                        {
                            //Give up.
                            _ = fw.WriteLine("GIVEUP : Rollback Exception Type: {0}" + ex2.GetType() + action);
                            _ = fw.WriteLine("GIVEUP : Message: {0}" + ex2.Message + action);
                            break;
                        }
                        connection.Close();
                        await Task.Delay(5);
                    }
                }
            }
            stopwatch.Stop();
        }
        public async Task<int> CountAllEscalationsAsync()
        {
            int rez = 0;
            while (true)
            {
                try
                {
                    lock (locker)
                    {
                        //OtvoriKonekciju();
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        string upit = "SELECT COUNT(*) FROM dbo.escalations WHERE EventType = 'Interval'";
                        SqlCommand komanda = new SqlCommand(upit, connection);
                        rez = (int)komanda.ExecuteScalar();
                        connection.Close();
                    }
                    //konekcija.Close(); pravi gresku
                    break;

                }
                catch (Exception ex)
                {
                    _ = fw.WriteLine("ERROR: CountAllEscalations()" + ex);
                    transakcija.Rollback();
                    connection.Close();
                    await Task.Delay(5);
                }
            }
            return rez;
        }
        #endregion
    }
}

