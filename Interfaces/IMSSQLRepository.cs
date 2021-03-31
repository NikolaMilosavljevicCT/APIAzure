using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Views;
using API.DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Interfaces
{
    public interface IMSSQLRepository
    {
        #region Create
        public Task<int> GetMaxIDAsync();
        public Task<int> InsertIntoTblListAsync(CreateTableDto newTable);
        public Task<int> CreateTableAsync(CreateTableDto newTable);
        public Task<List<TableList>> GetAllTableNamesAsync();
        public Task<List<string>> GetAllColumnsAsync();
        public Task<List<DataTypes>> GetAllDataTypesAsync();
        public Task<bool> IfColumnExistInTableAsync(DataTransferObjectBase data);
        public Task<int> InsertIntoColListAsync(DataTransferObjectBase data);
        public Task<int> CreateColumnAsync(DataTransferObjectBase data);
        public Task<int> CreateViewOneTableAsync(CreateViewDTO data);
        public Task<int> CreateViewTwoTablesAsync(CreateViewDTO data);
        #endregion

        #region Update
        public Task<bool> IsTableEmptyAsync(string tableName);
        public Task<int> ChangeTableNameAsync(string tableName, string newTableName);
        public Task<int> ChangeTableNameInTblListAsync(string tableName, string newTableName);
        public Task<int> ChangeTableNameInColListAsync(string tableName, string newTableName);
        public Task<List<string>> GetAllColumnsFromSelectedTableAsync(string tableName);
        public Task<List<string>> GetAllColumnsFromSelectedTableFromColListAsync(TableNamesDTO data);
        public Task<List<ColumnsList>> GetAllColumnsAsync(string tableName);
        public Task<bool> IsColumnEmptyAsync(string columnName, string tableName);
        public Task<int> ChangeColumnNameInColListAsync(string tableName, string columnName, string newColumnName);
        public Task<int> ChangeColumnNameInTableAsync(string tableName, string columnName, string newColumnName);
        public Task<string> GetColumnTypeForSelectedColumnAsync(string tableName, string columnName);
        public Task<int> ChangeColumnTypeInColListAsync(string tableName, string columnName, string oldDataType, string newColumnName, string newDataType);
        public Task<int> ChangeColumnTypeInTableAsync(string tableName, string columnName, string oldDataType, string newColumnName, string newDataType);
        public Task<List<ColumnsList>> GetAllColumnsDataFromSelectedTableFromColListAsync(string tableName);
        public Task<List<StringDTO>> GetAllViewsAsync();
        public Task<DetailsViewDTO> GetViewDetailsAsync(StringDTO data);
        public Task<int> AlterViewOneTableAsync(AlterViewDTO data);
        public Task<int> AlterViewTwoTablesAsync(AlterViewDTO data);
        #endregion
    }
}
