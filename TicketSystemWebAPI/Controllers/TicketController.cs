using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketSystem.Business.IServices;
using TicketSystem.Common.CustomAttributes;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITicketService _ticketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketController> _logger;
        public TicketController(ITicketService ticketService, IMapper mapper, ILogger<TicketController> logger, 
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _ticketService = ticketService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("UserTickets")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support, UserType.Manager])]
        public async Task<IActionResult> GetAllTickets([FromQuery] GetAllTicketsDto getAllTickets)
        {
            var response = await _ticketService.GetAllTicketsAsync(getAllTickets.PageNumber, getAllTickets.PageSize, getAllTickets.Status);
            _logger.LogDebug($"TicketController-GetAllTickets Request={JsonConvert.SerializeObject(getAllTickets)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpGet("GetAllTicketsByUserId/{userId}")]
        [CustomAuthorizeAttribute([UserType.Manager, UserType.Support, UserType.Client])]
        public async Task<IActionResult> GetAllTicketsByUserId(string userId)
        {
            var response = await _ticketService.GetAllTicketsByUserIdAsync(userId);
            _logger.LogDebug($"TicketController-GetAllTickets UserId={userId} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpGet("GetTicketByTicketId/{ticketId}")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support, UserType.Manager])]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {
            var response = await _ticketService.GetTicketByIdAsync(ticketId);
            _logger.LogDebug($"TicketController-GetTicketById Request={{TicketId={ticketId}}} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost("AssignTicketToEmployee")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> AssignTicketToEmployee([FromBody] PostAssignTicketDto request)
        {
            var response = await _ticketService.AssignTicketToEmployeeAsync(request.TicketId, request.EmployeeId);
            _logger.LogDebug($"TicketController-AssignTicketToEmployee Request={JsonConvert.SerializeObject(request)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost("CreateTicket")]
        [CustomAuthorizeAttribute([UserType.Client])]
        public async Task<IActionResult> CreateTicket([FromBody] PostTicketDto ticketDto)
        {
            var response = await _ticketService.CreateTicketAsync(ticketDto);
            _logger.LogDebug($"TicketController-CreateTicket Request={JsonConvert.SerializeObject(ticketDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost("UpdateTicketStatus")]
        [CustomAuthorizeAttribute([UserType.Support, UserType.Client])]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] PutTicketStatusDto updateStatusDto)
        {
            var response = await _ticketService.UpdateTicketStatusAsync(updateStatusDto.TicketId, updateStatusDto.Status);
            _logger.LogDebug($"TicketController-UpdateTicketStatus Request={JsonConvert.SerializeObject(updateStatusDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost("UpdateTicket")]
        [CustomAuthorizeAttribute([UserType.Support, UserType.Client])]
        public async Task<IActionResult> UpdateTicket([FromBody] PutTicketDto updateTicketDto)
        {
            var response = await _ticketService.UpdateTicketAsync(updateTicketDto);
            _logger.LogDebug($"TicketController-UpdateTicket Request={JsonConvert.SerializeObject(updateTicketDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpGet]
        [Route("GetTicketLookup")]
        public async Task<IActionResult> GetTicketLookup()
        {
            var response =await _ticketService.GetTicketLookup();
            _logger.LogDebug($"TicketController-GetTicketLookup Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

    }
}
