using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TicketSystem.DataAccess.Models;

namespace TicketSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<CommentController> _logger;

        public HealthCheckController(HealthCheckService healthCheckService, ILogger<CommentController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("Check")]
        public IActionResult HealthyCheckAsync()
        {
            var result = _healthCheckService.CheckHealthAsync().Result.Entries.ToDictionary(e => e.Key, e => new ResponseModel<string>()
            {
                IsSuccess = e.Value.Status == HealthStatus.Healthy,
                Result = e.Value.Status.ToString(),
                Message = e.Value.Exception?.Message ?? e.Value.Description,
            });

            _logger.LogDebug($"HealthCheckController-Check Request=None / Response={JsonConvert.SerializeObject(result)}");

            return Ok(result);

        }
    }
}
