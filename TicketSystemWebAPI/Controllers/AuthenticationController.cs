using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketSystem.Business.IServices;
using TicketSystem.DataAccess.DTOs;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] PostUserDto userDto)
    {
        var res = await _userService.CreateUser(userDto);
        _logger.LogDebug($"AuthenticationController-Register Request={JsonConvert.SerializeObject(userDto)} / Response={JsonConvert.SerializeObject(res)}");
        return Ok(res);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateModel model)
    {
        var tokens = await _userService.Authenticate(model.EmailORUserName, model.Password);
        _logger.LogDebug($"AuthenticationController-Login Request={JsonConvert.SerializeObject(model)} / Response={JsonConvert.SerializeObject(tokens)}");
        return Ok(tokens);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        var response = await _userService.RefreshToken(tokenRequest);
        _logger.LogDebug($"AuthenticationController-RefreshToken Request={JsonConvert.SerializeObject(tokenRequest)} / Response={JsonConvert.SerializeObject(response)}");
        return Ok(response);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var response = await _userService.ResetPassword(resetPasswordDto);
        _logger.LogDebug($"AuthenticationController-ResetPassword Request={JsonConvert.SerializeObject(resetPasswordDto)} / Response={JsonConvert.SerializeObject(response)}");
        return Ok(response);
    }

    [HttpPost("ForgetPassword")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
    {
        var response = await _userService.ForgetPassword(forgetPasswordDto);
        _logger.LogDebug($"AuthenticationController-ForgetPassword Request={JsonConvert.SerializeObject(forgetPasswordDto)} / Response={JsonConvert.SerializeObject(response)}");
        return Ok(response);
    }
}
