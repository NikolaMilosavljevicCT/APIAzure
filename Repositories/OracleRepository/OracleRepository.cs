using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Views;
using API.DataManagement.Interfaces.OracleInterface;
using API.DataManagement.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Repositories.OracleRepository
{
    public class OracleRepository: IOracleRepository
    {
        #region Fields
        private static OracleRepository _instance;
        private OracleConnection connection;
        private OracleTransaction transaction;
        //private OracleCommand command;
        //private Object locker = new Object();

        private Logger.Logger log = new Logger.Logger("OracleLogs", "OracleLog");
        #endregion
        //Openning Oracle base connection
        #region Oracle
        public static OracleRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OracleRepository();
                }
                return _instance;
            }
        }
        public OracleRepository()
        {
            connection = new OracleConnection(@"TNS_ADMIN=C:\Users\mminic\Oracle\network\admin;USER ID=CTICKETSADMIN;Password=CT#Admin#;DATA SOURCE=10.20.128.162:1521/orclpdb"); 
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
        //Create table and column methods in Oracle database
        #region Methods - Create
        /// <summary>
        /// Returns Max ID from tbl_list.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMaxIDAsync()
        {
            OpenConnection();
            string query = "SELECT MAX(id) FROM dbo.tbl_list";
            OracleCommand command = new OracleCommand(query, connection)
            {
                Transaction = transaction
            };
            try
            {
                int number = Convert.ToInt32(await command.ExecuteScalarAsync());
                return number + 1;
            }
            catch (Exception)
            {
                return 1;
            }
            finally
            {
                RollbackTransaction();
                CloseConnection();
            }
        }
        /// <summary>
        /// Insert new record in table TblList.
        /// </summary>
        /// <param name="newTable">Data recived from Form</param>
        /// <returns></returns>
        public async Task<int> InsertIntoTblListAsync(CreateTableDto newTable)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                int createDate = Convert.ToInt32(DateTimeToUnixTimestamp(DateTime.Now));
                string query = "INSERT INTO tbl_list (table_name, ootb, create_date, created_by)" +
                              "values('" + newTable.TableName + "', 0, " + createDate + ",'C#Nindze')";
                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
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
        /// Create new table in database ctickets.
        /// </summary>
        /// <param name="newTable">Data recived from Form</param>
        /// <returns></returns>
        public async Task<int> CreateTableAsync(CreateTableDto newTable)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "CREATE TABLE " + newTable.TableName + " (id int not null primary key, name varchar(50), createDate int, createdBy varchar(50), " +
                    "lastModDate int, lastModBy varchar(50))";
                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == -1)
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
        /// Retrive all table names from tblList.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TableList>> GetAllTableNamesAsync()
        {
            OpenConnection();
            List<TableList> list = new List<TableList>();
            string query = "SELECT * from tbl_list";
            OracleCommand command = new OracleCommand(query, connection);
            command.Transaction = transaction;
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                TableList tableNameList = new TableList();
                //tableNameList.Id = Convert.ToInt32(red[0]);
                tableNameList.TableName = row[1].ToString();
                //tableList.DBMSName = red[2].ToString();
                //tableList.Description = red[3].ToString();
                //tableList.DisplayName = red[4].ToString();
                //tableList.DisplayGroup = red[5].ToString();
                //tableList.FunctionGroup = red[6].ToString();
                //tableList.RelAttr = red[7].ToString();
                //tableList.CommonName = red[8].ToString();
                //tableList.SortBy = red[9].ToString();
                //tableList.Methods = red[10].ToString();
                //tableList.Triggers = red[11].ToString();
                //tableList.IsLocal = Convert.ToInt32(red[12]);
                //tableList.LastModDate = UnixTimeStampToDateTime(Convert.ToDouble(red[13]));
                //tableList.LastModBy = red[14].ToString();
                //tableList.TableType = Convert.ToInt32(red[15]);
                //tableList.TenancyType = Convert.ToInt32(red[16]);
                //tableList.SlSortBy = red[17].ToString();
                //tableList.SLWhere = red[18].ToString();
                //tableList.OOTB = Convert.ToInt32(red[19]);
                //tableList.CreateDate = UnixTimeStampToDateTime(Convert.ToDouble(red[20]));
                //tableList.CreatedBy = red[21].ToString();

                list.Add(tableNameList);
            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// Retrive all column from colList.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllColumnsAsync()
        {
            OpenConnection();
            List<string> list = new List<string>();
            string query = "SELECT * from col_list";
            OracleCommand command = new OracleCommand(query, connection)
            {
                Transaction = transaction
            };
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataColumn col in table.Columns)
            {
                list.Add(col.ColumnName);

            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// Retrive all data types from ctickets.
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataTypes>> GetAllDataTypesAsync()
        {
            //OpenConnection();
            if (connection.State == ConnectionState.Closed)
            {
                OpenConnection();
            }
            List<DataTypes> list = new List<DataTypes>();
            string query = "SELECT * from data_type";
            OracleCommand command = new OracleCommand(query, connection)
            {
                Transaction = transaction
            };
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                DataTypes dataTypes = new DataTypes();
                dataTypes.Id = Convert.ToInt32(row[0]);
                dataTypes.TypeName = row[1].ToString();
                dataTypes.Enum = Convert.ToInt32(row[2]);
                dataTypes.SqlDataType = row[3].ToString();
                //dataTypes.AttrMaxLength = Convert.ToInt32(red[4]);
                //dataTypes.AttrMaxSize = Convert.ToInt32(red[5]);
                list.Add(dataTypes);
            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// Check if column allready exist in table.
        /// </summary>
        /// <param name="data">Data recived from Form</param>
        /// <returns></returns>
        public async Task<bool> IfColumnExistInTableAsync(DataTransferObjectBase data)
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
                    query = "SELECT COUNT(*) FROM col_list WHERE table_name = '" + data.TableName + "' " +
                        "AND column_name= '" + data.ColumnName + "'";
                }
                catch (Exception ex)
                {
                    log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
                }

                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
                result = Convert.ToInt32(await command.ExecuteScalarAsync());

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
        /// <summary>
        /// Insert new record in ColList
        /// </summary>
        /// <param name="data">Data recived from Form</param>
        /// <returns></returns>
        public async Task<int> InsertIntoColListAsync(DataTransferObjectBase data)
        {
            int result = 0;

            try
            {
                int type = 0;
                foreach (var item in await GetAllDataTypesAsync())
                {
                    if (item.TypeName == data.DataType)
                    {
                        type = item.Enum;
                    }
                }

                OpenConnection();
                BeginTransaction();
                int createDate = Convert.ToInt32(DateTimeToUnixTimestamp(DateTime.Now));
                string query = "INSERT INTO col_list (table_name, column_name, type, create_date, created_by, max_string_len)" +
                              "values('" + data.TableName + "', '" + data.ColumnName + "'," + type + ", " + createDate + ",'C#Nindze', '" + data.MaxStringLength + "')";
                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
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
        /// Create new column in selected table.
        /// </summary>
        /// <param name="data">Data recived from Form</param>
        /// <returns></returns>
        public async Task<int> CreateColumnAsync(DataTransferObjectBase data)
        {
            int result = 0;
            string type = null;
            try
            {

                foreach (var item in await GetAllDataTypesAsync())
                {
                    if (item.TypeName == data.DataType)
                    {
                        type = item.SqlDataType;
                    }
                }
                if (data.DataType == "Character")
                {
                    type = "varchar2(" +data.MaxStringLength+ ")"; 
                }

                OpenConnection();
                BeginTransaction();
                string query = "ALTER TABLE " + data.TableName + " add " + data.ColumnName+ " " + type + "";
                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
                result = await command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == -1)
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
        /// Create view based on one table.
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="table"></param>
        /// <param name="listOfFields"></param> -> list obtained from the form
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<int> CreateViewOneTableAsync(CreateViewDTO data)
        {
            int result = 0;

            try
            {
                OpenConnection();
                BeginTransaction();

                string select = "";

                foreach (var item in data.ListOfFieldsTable1)
                {
                    select += "t1." + item + " " + data.Table1 + "_" + item + ",";
                }

                string selectFinal = select.Remove(select.Length - 1, 1);
                
                string createViewQuery = "CREATE VIEW " + data.ViewName + " AS SELECT " + selectFinal + " FROM " + data.Table1 + " t1" +
                                         " WHERE " + data.Condition;

                OracleCommand command = new OracleCommand(createViewQuery, connection);
                command.Transaction = transaction;
                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == -1)
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
        /// Create view based on two tables
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="table1"></param>
        /// <param name="table2"></param>
        /// <param name="listOfFieldsTable1"></param> -> list obtained from the form
        /// <param name="listOfFieldsTable2"></param> -> list obtained from the form
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<int> CreateViewTwoTablesAsync(CreateViewDTO data)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();

                string select1 = "";
                string select2 = "";

                foreach (var item in data.ListOfFieldsTable1)
                {
                    select1 += "t1." + item + " " + data.Table1 + "_" + item + ",";
                }

                foreach (var item in data.ListOfFieldsTable2)
                {
                    select2 += "t2." + item + " " + data.Table2 + "_" + item + ",";
                }

                string select2Final = select2.Remove(select2.Length - 1, 1);

                string createViewQuery = "CREATE VIEW  " + data.ViewName + " AS SELECT " + select1 + select2Final + " FROM " + data.Table1 + " t1, " + data.Table2 + " t2" +
                                         " WHERE " + data.Condition;

                OracleCommand command = new OracleCommand(createViewQuery, connection);
                command.Transaction = transaction;

                result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == -1)
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
        #endregion
        //Update table and column methods in Oracle database
        #region Methods - Update
        /// <summary>
        /// Check if table have records.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<bool> IsTableEmptyAsync(string tableName)
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
                    query = "SELECT COUNT(*) FROM " + tableName + "";

                }
                catch (Exception ex)
                {
                    log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
                }

                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
                result = Convert.ToInt32(await command.ExecuteScalarAsync());

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
        /// <summary>
        /// Change table name in ctickets
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="newTableName"></param>
        /// <returns></returns>
        public async Task<int> ChangeTableNameAsync(string tableName, string newTableName)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "ALTER TABLE " + tableName + " RENAME TO " + newTableName + "";//"EXEC sp_rename '" + comboTabela.SelectedItem.ToString() + "','" + txtNazivTabele.Text + "'";
                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
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
        /// Update table name in tblList.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="newTableName"></param>
        /// <returns></returns>
        public async Task<int> ChangeTableNameInTblListAsync(string tableName, string newTableName)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "UPDATE tbl_list  SET table_name='" + newTableName+ "' WHERE table_name = '" + tableName + "'";
                OracleCommand command = new OracleCommand(query, connection);
                command.Transaction = transaction;
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
        /// Update table name in ColList.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="newTableName"></param>
        /// <returns></returns>
        public async Task<int> ChangeTableNameInColListAsync(string tableName, string newTableName)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "UPDATE col_list  SET table_name='" + newTableName + "' WHERE table_name = '" + tableName + "'";
                OracleCommand command = new OracleCommand(query, connection)
                {
                    Transaction = transaction
                };
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
        /// Retrive all columns from selected table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAllColumnsFromSelectedTableAsync(string tableName)
        {
            OpenConnection();
            List<string> list = new List<string>();
            string upit = "SELECT * FROM " + tableName + "";
            OracleCommand command = new OracleCommand(upit, connection)
            {
                Transaction = transaction
            };
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataColumn row in table.Columns)
            {
                list.Add(row.ColumnName.ToString());
            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// Retrive all columns from ColList table for selected table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAllColumnsFromSelectedTableFromColListAsync(TableNamesDTO data)
        {
            OpenConnection();
            List<string> list = new List<string>();
            string query = "SELECT column_name FROM col_list WHERE table_name='" + data.TableName + "'";
            OracleCommand command = new OracleCommand(query, connection);
            command.Transaction = transaction;
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                list.Add(row[0].ToString());
            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// Retrive all columns from ColList
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<ColumnsList>> GetAllColumnsAsync(string tableName)
        {
            OpenConnection();
            List<ColumnsList> list = new List<ColumnsList>();
            string query = "SELECT column_name FROM col_list";
            OracleCommand command = new OracleCommand(query, connection)
            {
                Transaction = transaction
            };
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                ColumnsList cl = new ColumnsList
                {
                    ColumnName = row[0].ToString()
                };
                list.Add(cl);
            }
            CloseConnection();
            return list;
        }
        /// <summary>
        /// Check if Column is empty.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<bool> IsColumnEmptyAsync(string columnName, string tableName)
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
                    query = "SELECT COUNT(" + columnName + ") FROM " + tableName + "";

                }
                catch (Exception ex)
                {
                    log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
                }
                OracleCommand command = new OracleCommand(query, connection)
                {
                    Transaction = transaction
                };
                result = Convert.ToInt32(await command.ExecuteScalarAsync());
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
        /// <summary>
        /// Change Column name in colList table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="newColumnName"></param>
        /// <returns></returns>
        public async Task<int> ChangeColumnNameInColListAsync(string tableName, string columnName, string newColumnName)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "UPDATE col_list  SET column_name='" + newColumnName+ "' WHERE table_name = '" + tableName + "' AND column_name = '" + columnName + "'";
                OracleCommand command = new OracleCommand(query, connection)
                {
                    Transaction = transaction
                };
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
        /// Change column name inside selected table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="newColumnName"></param>
        /// <returns></returns>
        public async Task<int> ChangeColumnNameInTableAsync(string tableName, string columnName, string newColumnName)
        {
            int result = 0;
            try
            {
                OpenConnection();
                BeginTransaction();
                string query = "ALTER TABLE " + tableName + " RENAME COLUMN " + columnName + " TO " + newColumnName + "";//"EXEC sp_RENAME '" + comboTabela.SelectedItem.ToString() + "." + comboKolona.SelectedItem.ToString() + "', '" + txtKolona.Text + "', 'COLUMN'";
                OracleCommand command = new OracleCommand(query, connection)
                {
                    Transaction = transaction
                };
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
        /// Retrive data type of selected column.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public async Task<string> GetColumnTypeForSelectedColumnAsync(string tableName, string columnName)
        {
            //OpenConnection();
            int type = 0;
            string typeFormat = null;
            string query = "SELECT type FROM col_list WHERE table_name='" + tableName + "' AND column_name = '" + columnName + "'";
            OracleCommand command = new OracleCommand(query, connection)
            {
                Transaction = transaction
            };
            DataTable table = new DataTable();
            if (connection.State == ConnectionState.Closed)
            {
                OpenConnection();
            }
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                type = Convert.ToInt32(row[0]);
            }

            CloseConnection();

            foreach (var item in await GetAllDataTypesAsync())
            {
                if (item.Enum == type)
                {
                    typeFormat = item.TypeName;
                }
            }
            return typeFormat;
        }
        /// <summary>
        /// Update date type inside ColList for selected table and column.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="oldDataType"></param>
        /// <param name="newColumnName"></param>
        /// <param name="newDataType"></param>
        /// <returns></returns>
        public async Task<int> ChangeColumnTypeInColListAsync(string tableName, string columnName, string oldDataType, string newColumnName, string newDataType)
        {
            int result = 0;
            try
            {
                int type = 0;
                foreach (var item in await GetAllDataTypesAsync())
                {
                    if (item.TypeName == newDataType)
                    {
                        type = item.Enum;
                    }
                }

                if (connection.State == ConnectionState.Closed)
                {
                    OpenConnection();
                }

                BeginTransaction();
                string query = "UPDATE col_list  SET type=" + type + " WHERE table_name = '" + tableName+ "' AND column_name = '" + columnName + "'";
                OracleCommand command = new OracleCommand(query, connection)
                {
                    Transaction = transaction
                };
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
        /// Change column type inside selected table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="oldDataType"></param>
        /// <param name="newColumnName"></param>
        /// <param name="newDataType"></param>
        /// <returns></returns>
        public async Task<int> ChangeColumnTypeInTableAsync(string tableName, string columnName, string oldDataType, string newColumnName, string newDataType)
        {
            int result = 0;
            try
            {
                string type = "";
                foreach (var item in await GetAllDataTypesAsync())
                {
                    if (item.TypeName == newDataType)
                    {
                        type = item.SqlDataType;
                    }
                    //uslov samo za Character da mu se fiksira vrednost na 1000 karaktera, jer ne moze da primi MAX datatype
                    if (item.TypeName == "Character")
                    {
                        type = "VARCHAR2(1000)";
                    }
                }

                if (connection.State == ConnectionState.Closed)
                {
                    OpenConnection();
                }

                BeginTransaction();
                string query = "ALTER TABLE " + tableName + " MODIFY (" + columnName + " " + type + ")";
                OracleCommand command = new OracleCommand(query, connection)
                {
                    Transaction = transaction
                };
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
        /// GetAllColumnsFromCollistTableForSelectedTable -> FullColumnsData
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<ColumnsList>> FullColumnsDataForSelectedTableFromColListAsync(string tableName)
        {
            OpenConnection();
            List<ColumnsList> list = new List<ColumnsList>();
            string query = "SELECT * FROM col_list WHERE table_name='" + tableName + "'";
            OracleCommand command = new OracleCommand(query, connection)
            {
                Transaction = transaction
            };
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                var newColumn = new ColumnsList();
                newColumn.Id = Convert.ToInt32(row[0]);
                newColumn.TableName = row[1].ToString();
                newColumn.ColumnName = row[2].ToString();
                if (!Convert.IsDBNull(row[3]))
                {
                    newColumn.SchemaName = row[3].ToString();
                }
                if (!Convert.IsDBNull(row[4]))
                {
                    newColumn.DBMSName = row[4].ToString();
                }
                if (!Convert.IsDBNull(row[5]))
                {
                    newColumn.Description = row[5].ToString();
                }
                if (!Convert.IsDBNull(row[6]))
                {
                    newColumn.DisplayName = row[6].ToString();
                }
                if (!Convert.IsDBNull(row[7]))
                {
                    newColumn.Type = Convert.ToInt32(row[7]);
                }
                if (!Convert.IsDBNull(row[8]))
                {
                    newColumn.MaxStringLength = Convert.ToInt32(row[8]);
                }
                if (!Convert.IsDBNull(row[9]))
                {
                    newColumn.XRELTable = row[9].ToString();
                }
                if (!Convert.IsDBNull(row[10]))
                {
                    newColumn.ADDLInfo = row[10].ToString();
                }
                if (!Convert.IsDBNull(row[11]))
                {
                    newColumn.OnNewDefault = row[11].ToString();
                }
                if (!Convert.IsDBNull(row[12]))
                {
                    newColumn.OnCiSet = row[12].ToString();
                }
                if (!Convert.IsDBNull(row[13]))
                {
                    newColumn.ISIndexed = Convert.ToInt32(row[13]);
                }
                if (!Convert.IsDBNull(row[14]))
                {
                    newColumn.ISUnique = Convert.ToInt32(row[14]);
                }
                if (!Convert.IsDBNull(row[15]))
                {
                    newColumn.ISLocal = Convert.ToInt32(row[15]);
                }
                if (!Convert.IsDBNull(row[16]))
                {
                    newColumn.ISNotNull = Convert.ToInt32(row[16]);
                }
                if (!Convert.IsDBNull(row[17]))
                {
                    newColumn.ISRequired = Convert.ToInt32(row[17]);
                }
                if (!Convert.IsDBNull(row[18]))
                {
                    newColumn.ISWriteNew = Convert.ToInt32(row[18]);
                }
                if (!Convert.IsDBNull(row[19]))
                {
                    newColumn.LastModDate = UnixTimeStampToDateTime(Convert.ToDouble(row[19]));
                }
                if (!Convert.IsDBNull(row[20]))
                {
                    newColumn.LastModBy = row[20].ToString();
                }
                if (!Convert.IsDBNull(row[21]))
                {
                    newColumn.ISServProv = Convert.ToInt32(row[21]);
                }
                if (!Convert.IsDBNull(row[22]))
                {
                    newColumn.ServProvCode = Convert.ToInt32(row[22]);
                }
                if (!Convert.IsDBNull(row[23]))
                {
                    newColumn.UiInfo = row[23].ToString();
                }
                if (!Convert.IsDBNull(row[24]))
                {
                    newColumn.CreateDate = UnixTimeStampToDateTime(Convert.ToDouble(row[24]));
                }
                if (!Convert.IsDBNull(row[25]))
                {
                    newColumn.CreatedBy = row[25].ToString();

                    list.Add(newColumn);
                }
            }

            CloseConnection();
            return list;
        }

        /// <summary>
        /// Retrive all Views
        /// </summary>
        /// <param></param>
        /// <param></param>
        /// <returns></returns>
        public async Task<List<StringDTO>> GetAllViewsAsync()
        {
            if (connection.State == ConnectionState.Closed)
            {
                OpenConnection();
            }
            List<StringDTO> viewList = new List<StringDTO>();
            string query = "select view_name from user_views";
            OracleCommand command = new OracleCommand(query, connection);
            command.Transaction = transaction;
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataRow row in table.Rows)
            {
                viewList.Add(new StringDTO { text = row[0].ToString() });
            }
            CloseConnection();
            return viewList;
        }

        /// <summary>
        /// Get details for a view that is selected in frontend
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public async Task<DetailsViewDTO> GetViewDetailsAsync(StringDTO data)
        {
            return new DetailsViewDTO
            {
                FirstTable = await GetViewTableNameAsync(data.text, 0),
                SecondTable = await GetViewTableNameAsync(data.text, 1),
                FirstTableColumns = await FillViewTableAsync(data.text, 0),
                SecondTableColumns = await FillViewTableAsync(data.text, 1)
            };
        }

        /// <summary>
        /// Update view where only one table is selected.
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="listOfFieldsTable1"></param> -> list obtained from the form
        /// <returns></returns>

        public async Task<int> AlterViewOneTableAsync(AlterViewDTO data)
        {

            int result = 0;

            try
            {
                OpenConnection();
                BeginTransaction();

                string select = "";
                string viewDefinition = "";
                string updateViewLastPart = "";

                foreach (var item in data.ListOfFieldsTable1)
                {
                    select += "t1." + item + " " + data.Table1 + "_" + item + ",";
                }

                string selectFinal = select.Remove(select.Length - 1, 1);

                string viewDefinitionQuery = "SELECT TEXT_VC FROM USER_VIEWS WHERE VIEW_NAME = '" + data.ViewName + "'";

                OracleCommand command = new OracleCommand(viewDefinitionQuery, connection);
                command.Transaction = transaction;
                DataTable table = new DataTable();
                OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
                table.Load(reader);

                foreach (DataRow row in table.Rows)
                {
                    viewDefinition = row[0].ToString();
                }


                updateViewLastPart = viewDefinition[viewDefinition.IndexOf("FROM")..];

                string alterViewQuery = "CREATE OR REPLACE VIEW " + data.ViewName + " AS SELECT " + selectFinal + " " + updateViewLastPart + "";

                OracleCommand alterCommand = new OracleCommand(alterViewQuery, connection);
                alterCommand.Transaction = transaction;
                result = await alterCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == -1)
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
        /// Update view based on two tables
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="table1"></param>
        /// <param name="table2"></param>
        /// <param name="listOfFieldsTable1"></param> -> list obtained from the form
        /// <param name="listOfFieldsTable2"></param> -> list obtained from the form
        /// <param name="condition"></param>
        /// <returns></returns>

        public async Task<int> AlterViewTwoTablesAsync(AlterViewDTO data)
        {
            int result = 0;

            try
            {
                OpenConnection();
                BeginTransaction();

                string select1 = "";
                string select2 = "";
                string viewDefinition = "";
                string updateViewLastPart = "";

                foreach (var item in data.ListOfFieldsTable1)
                {
                    select1 += "t1." + item + " " + data.Table1 + "_" + item + ", ";
                }

                foreach (var item in data.ListOfFieldsTable2)
                {
                    select2 += "t2." + item + " " + data.Table2 + "_" + item + ",";
                }

                string select2Final = select2.Remove(select2.Length - 1, 1);


                string viewDefinitionQuery = "SELECT TEXT_VC FROM USER_VIEWS WHERE VIEW_NAME = '" + data.ViewName + "'";

                OracleCommand command = new OracleCommand(viewDefinitionQuery, connection);
                command.Transaction = transaction;
                DataTable table = new DataTable();
                OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
                table.Load(reader);

                foreach (DataRow row in table.Rows)
                {
                    viewDefinition = row[0].ToString();
                }

                updateViewLastPart = viewDefinition[viewDefinition.IndexOf("FROM")..];

                string alterViewQuery = "CREATE OR REPLACE VIEW  " + data.ViewName + " AS SELECT " + select1 + select2Final +
                                        " " + updateViewLastPart + "";

                OracleCommand alterCommand = new OracleCommand(alterViewQuery, connection);
                alterCommand.Transaction = transaction;
                result = await alterCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                if (result == -1)
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
        #endregion
        //Additional methods
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

        /// <summary>
        /// metoda za prikupljanje svih kolona za prosledjenu tabelu, s tim sto vraca dictionary gde nam je kljuc naziv kolone
        /// a value ce biti false za svaku kolonu -> pomocna metoda koji koristimo za UpdateView kada korisnik izabere view
        /// pa treba da mu se stikliraju checkboxovi pored svake kolone koja je koriscena za kreiranje tog viewa za trazenu tabelu
        /// </summary>
        /// <param name="tabela"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, bool>> GetColumnsForSelectedTableAsync(string tableName)
        {
            if (connection.State == ConnectionState.Closed)
            {
                OpenConnection();
            }
            Dictionary<string, bool> tableColumnsDict = new Dictionary<string, bool>();
            string query = "SELECT * FROM " + tableName + "";
            OracleCommand command = new OracleCommand(query, connection);
            command.Transaction = transaction;
            DataTable table = new DataTable();
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            table.Load(reader);

            foreach (DataColumn column in table.Columns)
            {
                tableColumnsDict.Add((column.ColumnName.ToString()), false);
            }
            CloseConnection();
            return tableColumnsDict;
        }

        /// <summary>
        /// Retrieve the table name(first or second - based on index passed to the function) for chosen view
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public async Task<string> GetViewTableNameAsync(string viewName, int index)
        {
            string viewTable = "";
            List<string> viewTables = new List<string>();

            try
            {
                OpenConnection();
                BeginTransaction();

                string getViewTablesQuery = "SELECT REFERENCED_NAME FROM all_dependencies WHERE type = 'VIEW' and name = '" + viewName + "'";
                OracleCommand command = new OracleCommand(getViewTablesQuery, connection);
                command.Transaction = transaction;
                DataTable table = new DataTable();
                OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
                table.Load(reader);

                foreach (DataRow row in table.Rows)
                {
                    viewTables.Add(row[0].ToString());
                }

                //ako je view napravljen od samo jedne tabele
                if (viewTables.Count == 1)
                {
                    if (index == 1)
                    {
                        viewTable = null;
                        return viewTable;
                    }
                }

                viewTable = viewTables[index];

                viewTable = viewTables[0];
            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }

            return viewTable;
        }

        /// <summary>
        /// Retrieve the table(first or second - based on index passed to the function) for chosen view and it's all columns as a dictionary,
        /// those columns that were used to create the view will be value=true(mark checkbox on form), 
        /// otherwise value=false(don't mark checkbox on form)
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, bool>> FillViewTableAsync(string viewName, int index)
        {
            string viewTable;
            string tableName;
            string columnName;
            List<string> checkedColumnsViewTable = new List<string>();
            Dictionary<string, bool> columnsViewTable = new Dictionary<string, bool>(); ;
            List<string> viewTables = new List<string>();

            try
            {
                OpenConnection();
                BeginTransaction();

                string getViewTablesQuery = "SELECT REFERENCED_NAME FROM all_dependencies WHERE type = 'VIEW' and name = '" + viewName + "'";
                OracleCommand command = new OracleCommand(getViewTablesQuery, connection);
                command.Transaction = transaction;
                DataTable table = new DataTable();
                OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
                table.Load(reader);

                foreach (DataRow row in table.Rows)
                {
                    viewTables.Add(row[0].ToString());
                }

                //ako je view napravljen od samo jedne tabele
                if (viewTables.Count == 1)
                {
                    if (index == 1)
                    {
                        columnsViewTable = null;
                        return columnsViewTable;
                    }
                }

                viewTable = viewTables[0];
                columnsViewTable = await GetColumnsForSelectedTableAsync(viewTable);

                string query = "SELECT * FROM " + viewName + "";
                OracleCommand cmd = new OracleCommand(query, connection);
                command.Transaction = transaction;
                DataTable tbl = new DataTable();
                OracleDataReader rdr = (OracleDataReader)await command.ExecuteReaderAsync();
                tbl.Load(rdr);

                foreach (DataColumn column in tbl.Columns)
                {
                    string wholeColumn = column.ColumnName.ToString();
                    tableName = wholeColumn.Substring(0, wholeColumn.IndexOf("_"));
                    columnName = wholeColumn.Substring(wholeColumn.IndexOf("_") + 1);

                    if (tableName == viewTable)
                        checkedColumnsViewTable.Add(columnName);
                }

                foreach (var item in checkedColumnsViewTable)
                {
                    if (columnsViewTable.ContainsKey(item))
                    {
                        columnsViewTable[item] = true;
                    }
                }

            }
            catch (Exception ex)
            {
                log.WriteLine("ERROR : " + ex.Message + "\nStackTrace : " + ex.StackTrace);
            }
            finally
            {
                CloseConnection();
            }

            return columnsViewTable;
        }


        #endregion
    }
}
