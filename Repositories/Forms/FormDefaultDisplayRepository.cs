using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Forms;
using API.DataManagement.Interfaces;
using API.DataManagement.Models.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Repositories.Forms
{
    public class FormDefaultDisplayRepository : IFormDefaultDisplayRepository
    {

        #region Fields
        private static FormDefaultDisplayRepository _instance;
        private SqlConnection connection;
        private SqlTransaction transaction;
        //private SqlCommand command;
        //private Object locker = new Object();

        private readonly Logger.Logger log = new Logger.Logger("Forms", "Form");
        #endregion

        #region MSSQL
        public static FormDefaultDisplayRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormDefaultDisplayRepository();
                }
                return _instance;
            }
        }
        public FormDefaultDisplayRepository()
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

        #region Methods

        public async Task<List<DefaultFormDisplay>> GetAllDefaultFormDisplaysAsync()
        {
            OpenConnection();
            List<DefaultFormDisplay> list = new List<DefaultFormDisplay>();
            string query = "SELECT * FROM dbo.defaultFormDisplay";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            DataTable table = new DataTable();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                DefaultFormDisplay defaultForm = new DefaultFormDisplay
                {
                    Id = Convert.ToInt32(row[0])
                };
                if (!Convert.IsDBNull(row[1]))
                {
                    defaultForm.FormId = row[1].ToString();
                }
                if (!Convert.IsDBNull(row[2]))
                {
                    defaultForm.FormName = row[2].ToString();
                }
                if (!Convert.IsDBNull(row[3]))
                {
                    defaultForm.TableName = row[3].ToString();
                }
                if (!Convert.IsDBNull(row[4]))
                {
                    defaultForm.FieldsAttributes = row[4].ToString();
                }
                list.Add(defaultForm);
            }

            CloseConnection();
            return list;
        }

        /// <summary>
        /// Insert new record in table DefaultFormDisplays.
        /// </summary>
        /// <returns></returns>
        public async Task<int> InsertDefaultFormDisplayAsync(DefaultFormDisplayDTO data)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string formId = "Fx" + Convert.ToInt32(DateTimeToUnixTimestamp(DateTime.Now));
                string query = "INSERT INTO dbo.defaultFormDisplay (FormId, FormName, TableName, FieldsAttributes)" +
                    "values('" + formId + "', '" + data.FormName + "', '" + data.TableName + "','" + data.FieldsAttributes + "')";
                SqlCommand command = new SqlCommand(query, connection, transaction);
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
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
        /// Update fields atributs inside seleced From.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateDefaultFormDisplayAsync(UpdateDefaultFormDisplayDTO data)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "UPDATE dbo.defaultFormDisplay SET FieldsAttributes='" + data.FieldsAttributes + "' WHERE FormName = '" + data.FormName + "'";
                SqlCommand command = new SqlCommand(query, connection, transaction);
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result != 0)
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
        /// Return all form data for selected form.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<DefaultFormDisplay> GetDefaultFormDisplayAsync(StringDTO data)
        {
            OpenConnection();

            DefaultFormDisplay defaultForm = new DefaultFormDisplay();
            string query = "SELECT * FROM dbo.defaultFormDisplay WHERE FormName ='"+ data.text +"'";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            DataTable table = new DataTable();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                defaultForm.Id = Convert.ToInt32(row[0]);
                if (!Convert.IsDBNull(row[1]))
                {
                    defaultForm.FormId = row[1].ToString();
                }
                if (!Convert.IsDBNull(row[2]))
                {
                    defaultForm.FormName = row[2].ToString();
                }
                if (!Convert.IsDBNull(row[3]))
                {
                    defaultForm.TableName = row[3].ToString();
                }
                if (!Convert.IsDBNull(row[4]))
                {
                    defaultForm.FieldsAttributes = row[4].ToString();
                }
            }

            CloseConnection();
            return defaultForm;
        }

        /// <summary>
        /// Delete provided row for selected form.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> DeleteDefaultFormDisplayAsync(StringDTO data)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "DELETE FROM dbo.defaultFormDisplay WHERE FormName='"+ data.text +"'";
                SqlCommand command = new SqlCommand(query, connection, transaction);
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result != 0)
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

        public async Task<bool> FormExistAsync(string formName)
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
                    query = "SELECT COUNT(*) FROM dbo.defaultFormDisplay WHERE FormName = '" + formName + "' ";
                }
                catch (Exception ex)
                {
                    log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
                }

                SqlCommand command = new SqlCommand(query, connection, transaction);
                result = (int)await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result > 0)
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
        #endregion

        // Additional methods
        #region Help Methods
        //metoda za konvertovanje UnixTime-a u DateTime -> koristimo je za parametre Create_Date i Last_Modified_Date
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// metoda za konvertovanje DateTime to UnixTime -> koristimo je za parametre Create_Date i Last_Modified_Date
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
            new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
        #endregion
    }
}
