using API.DataManagement.DTOs;
using API.DataManagement.Interfaces;
using API.DataManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Controllers
{
    public class MSSQLController : BaseApiController
    {
        private readonly IMSSQLRepository mSSQLRepository;

        public MSSQLController(IMSSQLRepository mSSQLRepository)
        {
            this.mSSQLRepository = mSSQLRepository;
        }

        //[HttpPost("fullColumnsData")]
        //public List<ColumnsList> FullColumnsDataForSelectedTable(TableNamesDTO data)
        //{
        //    return mSSQLRepository.FullColumnsDataForSelectedTableFromColList(data.TableName);
        //}

        [HttpGet("tables")]
        public async Task<ActionResult<IEnumerable<TableNamesDTO>>> GetAllTableNames()
        {
            List<TableNamesDTO> tables = new List<TableNamesDTO>();

            foreach (var item in await mSSQLRepository.GetAllTableNamesAsync())
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
            if(await mSSQLRepository.CreateTableAsync(data) == -1)
            {
                if(await mSSQLRepository.InsertIntoTblListAsync(data) == 1)
                {
                    return Ok("Table is created.");
                }
            }
            return BadRequest("Table is not created.");
        }

        [HttpPost("postColumn")]
        public async Task<IActionResult> CreateColumn(DataTransferObjectBase data)
        {
            if(await mSSQLRepository.IfColumnExistInTableAsync(data) == false)
            {
                if(await mSSQLRepository.InsertIntoColListAsync(data) == 1 &&
                    await mSSQLRepository.CreateColumnAsync(data) == -1)
                {
                    return Ok("Column is created.");
                }
                else
                {
                    return BadRequest("Column is not created.");
                }
            }
            return BadRequest("Column is not created.");

        }

        [HttpPost("putTable")]
        public async Task<IActionResult> UpdateTable(UpdateTableDTO data)
        {
            if(await mSSQLRepository.IsTableEmptyAsync(data.OldTableName) == false)
            {
                if(await mSSQLRepository.ChangeTableNameAsync(data.OldTableName, data.NewTableName) != 0)
                {
                    if(await mSSQLRepository.ChangeTableNameInTblListAsync(data.OldTableName, data.NewTableName) != 0 && 
                        await mSSQLRepository.ChangeTableNameInColListAsync(data.OldTableName, data.NewTableName) != 0 )
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
            return Ok(await mSSQLRepository.GetAllColumnsFromSelectedTableFromColListAsync(data));
        }

        //[HttpPost("getTypeForSelectedColumn")]
        //public IActionResult getColumnTypeForSelectedColumn(UpdateColumnDTO data)
        //{
        //    StringDTO help = new StringDTO();
        //    help = mSSQLRepository.GetColumnTypeForSelectedColumn(data.TableName, data.OldColumnName);
        //    if ( help.text == "")
        //    {
        //        return BadRequest("Please contact your system administrator.");
        //    }
        //    return Ok(help);
        //}

        [HttpPost("putColumn")]
        public async Task<IActionResult> UpdateColumn(UpdateColumnDTO data)
        {
           if(await mSSQLRepository.IsColumnEmptyAsync(data.OldColumnName, data.TableName) == false)
           {
                if(data.NewColumnType != null)
                {
                    if(await mSSQLRepository.ChangeColumnTypeInTableAsync(data.TableName, data.OldColumnName, data.OldColumnType, data.NewColumnName, data.NewColumnType) != 0 &&
                        await mSSQLRepository.ChangeColumnTypeInColListAsync(data.TableName, data.OldColumnName, data.OldColumnType, data.NewColumnName, data.NewColumnType) != 0)
                    {
                        if (await mSSQLRepository.ChangeColumnNameInColListAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0 &&
                       await mSSQLRepository.ChangeColumnNameInTableAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0)
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
                    if(await mSSQLRepository.ChangeColumnNameInColListAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0 &&
                        await mSSQLRepository.ChangeColumnNameInTableAsync(data.TableName, data.OldColumnName, data.NewColumnName) != 0)
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
