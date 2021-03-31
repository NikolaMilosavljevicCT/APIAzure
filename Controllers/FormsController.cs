using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Forms;
using API.DataManagement.Interfaces;
using API.DataManagement.Models.Forms;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataManagement.Controllers
{
    public class FormsController : BaseApiController
    {
        private readonly IFormDefaultDisplayRepository formDefaultDisplayRepository;

        public FormsController(IFormDefaultDisplayRepository formDefaultDisplayRepository)
        {
            this.formDefaultDisplayRepository = formDefaultDisplayRepository;
        }

        [HttpPost("addForm")]
        public async Task<IActionResult> AddNewForm(DefaultFormDisplayDTO data)
        {
            if(await formDefaultDisplayRepository.FormExistAsync(data.FormName))
            {
                return BadRequest("Form with that name already exist.");
            }
            if (await formDefaultDisplayRepository.InsertDefaultFormDisplayAsync(data) == 1)
            {
                return Ok("New default form: " + data.FormName + " added.");
            }
            return BadRequest("Form is not inserted into DB.");
        }

        [HttpGet("allForms")]
        public async Task<ActionResult<IEnumerable<DefaultFormDisplay>>> GetAllForms()
        {
            return Ok(await formDefaultDisplayRepository.GetAllDefaultFormDisplaysAsync());
        }

        [HttpPost("updateForm")]
        public async Task<IActionResult> UpdateForm(UpdateDefaultFormDisplayDTO data)
        {
            if (!(await formDefaultDisplayRepository.FormExistAsync(data.FormName)))
            {
                return BadRequest("Form can`t be found.");
            }
            if (await formDefaultDisplayRepository.UpdateDefaultFormDisplayAsync(data) != 0)
            {
                return Ok();
            }
            return BadRequest("Form is not updated.");
        }

        [HttpPost("getForm")]
        public async Task<ActionResult<DefaultFormDisplay>> GetForm(StringDTO data)
        {
            if (!(await formDefaultDisplayRepository.FormExistAsync(data.text)))
            {
                return BadRequest("Form can`t be found.");
            }
            var form = await formDefaultDisplayRepository.GetDefaultFormDisplayAsync(data);
            return Ok( form );
        }

        [HttpPost("deleteForm")]
        public async Task<IActionResult> DeleteForm(StringDTO data)
        {
            if (!(await formDefaultDisplayRepository.FormExistAsync(data.text)))
            {
                return BadRequest("Form not deleted. Form can`t be found.");
            }
            if (await formDefaultDisplayRepository.DeleteDefaultFormDisplayAsync(data) == 1)
            {
                return Ok("Default form: " + data.text + " DELETED.");
            }
            return BadRequest("Something went wrong. Reguest not DELETED");
        }
    }
}
