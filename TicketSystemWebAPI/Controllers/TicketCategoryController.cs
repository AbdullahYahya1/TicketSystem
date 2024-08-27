using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class TicketCategoryController : ControllerBase
    {
        private readonly ITicketCategoryService _ticketCategoryService;
        private readonly ILogger<TicketCategoryController> _logger;

        public TicketCategoryController(ITicketCategoryService ticketCategoryService, ILogger<TicketCategoryController> logger)
        {
            _ticketCategoryService = ticketCategoryService;
            _logger = logger;
        }

        [HttpGet("GetAllCategories")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support, UserType.Manager])]
        public async Task<IActionResult> GetCategories()
        {
            var response = await _ticketCategoryService.GetAllTicketCategoriesAsync();
            _logger.LogDebug($"TicketCategoryController-GetCategories Request=None / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpGet("GetTicketCategoryByCategoryId/{id}")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> GetTicketCategoryById(int id)
        {
            var ticketCategory = await _ticketCategoryService.GetTicketCategoryByIdAsync(id);
            _logger.LogDebug($"TicketCategoryController-GetTicketCategoryById Request={id} / Response={JsonConvert.SerializeObject(ticketCategory)}");
            return Ok(ticketCategory);
        }

        [HttpPost("CreateTicketCategory")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> CreateTicketCategory([FromBody] PostTicketCategoryDto ticketCategoryDto)
        {
            var createdTicketCategory = await _ticketCategoryService.CreateTicketCategoryAsync(ticketCategoryDto);
            _logger.LogDebug($"TicketCategoryController-CreateTicketCategory Request={JsonConvert.SerializeObject(ticketCategoryDto)} / Response={JsonConvert.SerializeObject(createdTicketCategory)}");
            return Ok(createdTicketCategory);
        }

        [HttpPost("UpdateTicketCategory")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> UpdateTicketCategory([FromBody] TicketCategoryDto ticketCategoryDto)
        {
            var updatedTicketCategory = await _ticketCategoryService.UpdateTicketCategoryAsync(ticketCategoryDto);
            _logger.LogDebug($"TicketCategoryController-UpdateTicketCategory Request={JsonConvert.SerializeObject(ticketCategoryDto)} / Response={JsonConvert.SerializeObject(updatedTicketCategory)}");
            return Ok(updatedTicketCategory);
        }

        [HttpPost("DeleteTicketCategory/{id}")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> DeleteTicketCategory(int id)
        {
            var deleteResult = await _ticketCategoryService.DeleteTicketCategoryAsync(id);
            _logger.LogDebug($"TicketCategoryController-DeleteTicketCategory Request=TicketCategoryId:{id} / Response={JsonConvert.SerializeObject(deleteResult)}");
            return Ok(deleteResult);
        }
        [HttpGet]
        [Route("GetCategoryLookup")]
        public async Task<IActionResult> GetCategoryLookup()
        {
            var response = await _ticketCategoryService.GetTicketCategoriesLookupAsync();
            _logger.LogDebug($"TicketCategoryController-GetCategoryLookup Request=None / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
    }
}
