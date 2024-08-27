using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicketSystem.Business.IServices;
using TicketSystem.Common.CustomAttributes;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketTypeController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;
        private readonly ILogger<TicketTypeController> _logger;

        public TicketTypeController(ITicketTypeService ticketTypeService, ILogger<TicketTypeController> logger)
        {
            _ticketTypeService = ticketTypeService;
            _logger = logger;
        }

        [HttpGet("GetTicketTypesByCategoryId/{categoryId}")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support, UserType.Manager])]
        public async Task<IActionResult> GetTypesByCategoryId(int categoryId)
        {
            var response = await _ticketTypeService.GetTypesByCategoryIdAsync(categoryId);
            _logger.LogDebug($"TicketTypeController-GetTypesByCategoryId Request={categoryId} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpGet("GetTicketTypeByTicketTypeId/{id}")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> GetTicketTypeByTicketTypeId(int id)
        {
            var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
            _logger.LogDebug($"TicketTypeController-GetTicketTypeById Request={id} / Response={JsonConvert.SerializeObject(ticketType)}");
            return Ok(ticketType);
        }

        [HttpPost("CreateTicketType")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> CreateTicketType([FromBody] PostTicketTypeDto ticketType)
        {
            var createdTicketType = await _ticketTypeService.CreateTicketTypeAsync(ticketType);
            _logger.LogDebug($"TicketTypeController-CreateTicketType Request={JsonConvert.SerializeObject(ticketType)} / Response={JsonConvert.SerializeObject(createdTicketType)}");
            return Ok(createdTicketType);
        }

        [HttpPost("UpdateTicketType")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> UpdateTicketType([FromBody] TicketTypeDto ticketType)
        {
            var response = await _ticketTypeService.UpdateTicketTypeAsync(ticketType);
            _logger.LogDebug($"TicketTypeController-UpdateTicketType Request={JsonConvert.SerializeObject(ticketType)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost("DeleteTicketType/{id}")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> DeleteTicketType(int id)
        {
            var response = await _ticketTypeService.DeleteTicketTypeAsync(id);
            _logger.LogDebug($"TicketTypeController-DeleteTicketType Request=TicketTypeId:{id} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
        [HttpGet]
        [Route("GetTicketTypesLookup")]
        public async Task<IActionResult> GetTicketTypesLookup()
        {
            var response = await _ticketTypeService.GetTicketTypes();
            _logger.LogDebug($"TicketController-GetTicketTypesLookup Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
    }
}
