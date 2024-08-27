using AutoMapper;
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
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITicketService _ticketService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ITicketService ticketService, IMapper mapper, ILogger<CommentController> logger)
        {
            _mapper = mapper;
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpPost("AddComment")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support])]
        public async Task<IActionResult> AddCommentToTicket([FromBody] PostCommentDto commentDto)
        {
            var response = await _ticketService.AddCommentToTicketAsync(commentDto.TicketId, commentDto);
            _logger.LogDebug($"CommentController-AddCommentToTicket Request={JsonConvert.SerializeObject(commentDto)} / Response={JsonConvert.SerializeObject(response, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            return Ok(response);

        }

        [HttpPost("UpdateComment")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support])]
        public async Task<IActionResult> UpdateTicketComment([FromBody] PutCommentDto updateCommentDto)
        {
            var response = await _ticketService.UpdateTicketCommentAsync(updateCommentDto.TicketId, updateCommentDto.CommentId, updateCommentDto.NewContent);
            _logger.LogDebug($"CommentController-UpdateTicketComment Request={JsonConvert.SerializeObject(updateCommentDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost("DeleteComment")]
        [CustomAuthorizeAttribute([UserType.Client, UserType.Support])]
        public async Task<IActionResult> DeleteTicketComment([FromBody] DeleteTicketCommentDto deleteCommentDto)
        {
            var response = await _ticketService.DeleteTicketCommentAsync(deleteCommentDto.TicketId, deleteCommentDto.CommentId);
            _logger.LogDebug($"CommentController-DeleteTicketComment Request={JsonConvert.SerializeObject(deleteCommentDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
    }
}
