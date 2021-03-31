using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Views;
using API.DataManagement.Interfaces;
using API.DataManagement.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Controllers
{
    public class ViewController : BaseApiController
    {
        private readonly IMSSQLRepository mSSQLRepository;

        public ViewController(IMSSQLRepository mSSQLRepository)
        {
            this.mSSQLRepository = mSSQLRepository;
        }

        [HttpPost("postView")]
        public async Task<IActionResult> PostView(CreateViewDTO data)
        {
            if(data.ListOfFieldsTable2.Count > 0)
            {
                if (await mSSQLRepository.CreateViewTwoTablesAsync(data) == -1)
                {
                    return Ok("View created!");
                }
            }
            else
            {
                if (await mSSQLRepository.CreateViewOneTableAsync(data) == -1)
                {
                    return Ok("View created!");
                }
            }
            return BadRequest("View not created!");
        }

        [HttpPost("alterView")]
        public async Task<IActionResult> AlterView(AlterViewDTO data)
        {
            if (data.ListOfFieldsTable2.Count > 0)
            {
                if (await mSSQLRepository.AlterViewTwoTablesAsync(data) == -1)
                {
                    return Ok("View updated!");
                }
            }
            else
            {
                if (await mSSQLRepository.AlterViewOneTableAsync(data) == -1)
                {
                    return Ok("View updated!");
                }
            }

            return BadRequest("View is not updated!");
        }

        [HttpPost("viewDetails")]
        public async Task<ActionResult<DetailsViewDTO>> GetViewDetails(StringDTO data)
        {
            return Ok(await mSSQLRepository.GetViewDetailsAsync(data));
        }

        [HttpGet("allViews")]
        public async Task<ActionResult<IEnumerable<StringDTO>>> GetAllViews()
        {
            return Ok(await mSSQLRepository.GetAllViewsAsync());
        }
    }
}
