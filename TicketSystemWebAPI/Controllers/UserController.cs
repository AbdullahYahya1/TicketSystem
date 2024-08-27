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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;

        public UserController(IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetUserTypesLookup")]
        public async Task<IActionResult> GetUserTypesLookup()
        {
            var response = await _userService.GetUserTypesLookup();
            _logger.LogDebug($"UserController-GetUserTypesLookup Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
        [HttpGet]
        [Route("GetUsersLookup")]
        public async Task<IActionResult> GetUsersLookup(UserType? type = null)
        {
            var response = await _userService.GetUsersLookup(type);
            _logger.LogDebug($"UserController-GetUsersLookup Request={JsonConvert.SerializeObject(type)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
        [HttpPost("EditUserAsManager")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> EditUserAsManager([FromBody] PutUserDtoManger userDto)
        {
            var result = await _userService.UpdateUserAsManager(userDto, userDto.ToUpdateId);
            _logger.LogDebug($"UserController-EditUserAsManager Request={JsonConvert.SerializeObject(userDto)} /  Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);

        }

        [HttpPost("EditUserSelf")]
        public async Task<IActionResult> EditUser([FromBody] PutUserDto userDto)
        {
            var result = await _userService.UpdateUser(userDto);
            _logger.LogDebug($"UserController-EditUser Request={JsonConvert.SerializeObject(userDto)} / Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }

        [HttpPost("ActivateDeactivateUser/{id}")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> ActivateDeactivateUser(string id)
        {
            var result = await _userService.ActivateDeactivateUser(id);
            _logger.LogDebug($"UserController-ActivateDeactivateUser Request={id} / Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }

        [HttpGet("GetAllUsers")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersDto getAllDto)
        {
            var result =  await _userService.GetAllUsersAsync( getAllDto.UserType);
            _logger.LogDebug($"UserController-GetAllUsers Request={JsonConvert.SerializeObject(getAllDto)} / Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }

        [HttpGet("GetUserStats")]
        public async Task<IActionResult> GetUserStats()
        {
            var result = await _userService.GetCurrentUserTicketStats();
            _logger.LogDebug($"UserController-GetUserStats Request=None / Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }

        [HttpGet("GetUser/{id}")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _userService.GetUserById(id);
            _logger.LogDebug($"UserController-GetUser Request={id} / Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }

        [HttpGet("GetAllClientsWithTickets")]
        [CustomAuthorizeAttribute([UserType.Manager])]
        public async Task<IActionResult> GetAllClientsWithTickets()
        {
            var result = await _userService.GetClientsWithTickets(); 
            _logger.LogDebug($"UserController-GetAllClientsWithTickets Request=none / Response={JsonConvert.SerializeObject(result)}");
            return Ok(result);
        }
    }
}
