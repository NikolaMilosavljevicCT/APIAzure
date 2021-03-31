using API.DataManagement.DTOs;
using API.DataManagement.DTOs.Events;
using API.DataManagement.Interfaces.EventsInterface;
using API.DataManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.DataManagement.Controllers
{
    public class EventsController : BaseApiController
    {
        private readonly IEventsRepository eventsController;

        public EventsController(IEventsRepository eventsController)
        {
            this.eventsController = eventsController;
        }

        [HttpGet("getEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            return Ok(await eventsController.GetAllEventsAsync());
        }

        [HttpPost("createEventInterval")]
        public async Task<IActionResult> CreateEventInterval(EventsIntervalDTO data)
        {
            
                if (await eventsController.InsertIntoEventsIntervalAsync(data) == 1)
                {
                    return Ok("Event is created.");
                }
                else
                {
                    return BadRequest("Event is not created.");
                }
        }

        [HttpPost("createEventTimer")]
        public async Task<IActionResult> CreateEventTimer(EventsTimerDTO data)
        {
            if (await eventsController.InsertIntoEventsTimerAsync(data) == 1)
            {
                return Ok("Event is created.");
            }
            else
            {
                return BadRequest("Event is not created.");
            }
        }

    }
}
