using API.DataManagement.DTOs;
using API.DataManagement.Interfaces.OracleInterface;
using API.DataManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Controllers
{
    public class OracleController : BaseApiController
    {
        private readonly IOracleRepository oracleRepository;
        public OracleController(IOracleRepository oracleRepository)
        {
            this.oracleRepository = oracleRepository;
        }

        [HttpPost("fullColumnsData")]
        public async Task<ActionResult<IEnumerable<ColumnsList>>> FullColumnsDataForSelectedTable(TableNamesDTO data)
        {
            return await oracleRepository.FullColumnsDataForSelectedTableFromColListAsync(data.TableName);
        }

        [HttpGet("tables")]
        public async Task<ActionResult<IEnumerable<TableNamesDTO>>> GetAllTableNames()
        {
            List<TableNamesDTO> tables = new List<TableNamesDTO>();

            foreach (var item in await oracleRepository.GetAllTableNamesAsync())
            {
                tables.Add(new TableNamesDTO
                {
                    TableName = item.TableName
                });
            }

            return tables;
        }

        [HttpPost("postTable")]
        public async Task<IActionResult> CreateTable(CreateTableDto data)
        {
            //provera za naziv tabele
            if (await oracleRepository.CreateTableAsync(data) == -1)
            {
                if (await oracleRepository.InsertIntoTblListAsync(data) == 1)
                {
                    return Ok("Table is created.");
                }
            }
            return BadRequest("Table is not created.");
        }

        [HttpPost("postColumn")]
        public async Task<IActionResult> CreateColumn(DataTransferObjectBase data)
        {
            if (await oracleRepository.IfColumnExistInTableAsync(data) == false)
            {
                if (await oracleRepository.InsertIntoColListAsync(data) == 1 &&
                    await oracleRepository.CreateColumnAsync(data) == -1)
                {
                    return Ok("Column is created.");
                }
            }
            return BadRequest("Column is not created.");

        }

        [HttpPost("putTable")]
        public async Task<IActionResult> UpdateTable(UpdateTableDTO data)
        {
            if (await oracleRepository.IsTableEmptyAsync(data.OldTableName) == false)
            {
                if (await oracleRepository.ChangeTableNameAsync(data.OldTableName, data.NewTableName) != 0)
                {
                    if (await oracleRepository.ChangeTableNameInTblListAsync(data.OldTableName, data.NewTableName) != 0 &&
                        await oracleRepository.ChangeTableNameInColListAsync(data.OldTableName, data.NewTableName) != 0)
                    {
                        return Ok("Table is updated.");
                    }
                }
                else
                {
                    return BadRequest("Table is not updated.");
                }
            }
            else
            {
                return BadRequest("Table is not empty.");
            }
            return BadRequest("Table is not updated.");
        }

        [HttpPost("columnsForSelectedTable")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllColumnsForSelectedTable(TableNamesDTO data)
        {
            return Ok(await oracleRepository.GetAllColumnsFromSelectedTableFromColListAsync(data));
        }

        [HttpPost("getTypeForSelectedColumn")]
        public async Task<ActionResult<string>> GetColumnTypeForSelectedColumn(UpdateColumnDTO data)
        {
            string help = await oracleRepository.GetColumnTypeForSelectedColumnAsync(data.TableName, data.OldColumnName);
            if (help == "")
            {
                return BadRequest("Please contact your system administrator.");
            }
            return Ok(help);
        }

        [HttpPost("putColumn")]
        public async Task<IActionResult> UpdateColumn(UpdateColumnDTO data)
        {
            if (await oracleRepository.IsColumnEmptyAsync(data.OldColumnName, data.TableName) == false)
            {
                if (data.NewColumnType != null)
                {
                    if (await oracleRepository.ChangeColumnTypeInTableAsync(data.TableName, data.OldColumnName, data.OldColumnType, data.NewColumnName, data.NewColumnType) != 0 &&
                        await oracleRepository.ChangeColumnTypeInColListAsync(data.TableName, data.OldColumnName, data.OldColumnType, data.NewColumnName, data.NewColumnType) != 0)
                    {
                        if (await oracleRepository.ChangeColumnNameInColListAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0 &&
                       await oracleRepository.ChangeColumnNameInTableAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0)
                        {
                            return Ok("Selected column is updated.");
                        }
                        else
                        {
                            return BadRequest("Selected column is not updated.");
                        }
                    }
                    else
                    {
                        return BadRequest("Column type for selected column is not updated.");
                    }
                }
                else
                {
                    if (await oracleRepository.ChangeColumnNameInColListAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0 &&
                        await oracleRepository.ChangeColumnNameInTableAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0)
                    {
                        return Ok("Selected column is updated.");
                    }
                    else
                    {
                        return BadRequest("Selected column is not updated.");
                    }
                }
            }
            return BadRequest("Selected column is not updated.");
        }


    }
}
