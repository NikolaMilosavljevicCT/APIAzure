using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Views;
using API.DataManagement.Interfaces.OracleInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Controllers
{
    public class ViewOracleController : BaseApiController
    {
        private readonly IOracleRepository oracleRepository;

        public ViewOracleController(IOracleRepository oracleRepository)
        {
            this.oracleRepository = oracleRepository;
        }

        [HttpPost("postView")]
        public async Task<IActionResult> PostView(CreateViewDTO data)
        {
            if (data.ListOfFieldsTable2.Count > 0)
            {
                if (await oracleRepository.CreateViewTwoTablesAsync(data) == -1)
                {
                    return Ok("View created!");
                }
            }
            else
            {
                if (await oracleRepository.CreateViewOneTableAsync(data) == -1)
                {
                    return Ok("View created!");
                }
            }

            return BadRequest("View not created!");
        }

        [HttpPost("alterView")]
        public async Task<IActionResult> AlterView(AlterViewDTO data)
        {
            //Ovo je za sada ovako, menjace se logika ovde kako da se provali da li je view nad jednom tabelom ili vise
            if (data.ListOfFieldsTable2.Count > 0)
            {
                if (await oracleRepository.AlterViewTwoTablesAsync(data) == -1)
                {
                    return Ok("View updated!");
                }
            }
            else
            {
                if (await oracleRepository.AlterViewOneTableAsync(data) == -1)
                {
                    return Ok("View updated!");
                }
            }

            return BadRequest("View is not updated!");
        }

        [HttpPost("viewDetails")]
        public async Task<ActionResult<DetailsViewDTO>> GetViewDetailsAsync(StringDTO data)
        {
            return Ok(await oracleRepository.GetViewDetailsAsync(data));
        }

        [HttpGet("allViews")]
        public async Task<ActionResult<IEnumerable<StringDTO>>> GetAllViews()
        {
            return Ok(await oracleRepository.GetAllViewsAsync());
        }
    }
}
