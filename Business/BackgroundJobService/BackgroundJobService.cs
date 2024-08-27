using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.DataAccess.DTOs;

namespace TicketSystem.Business.BackgroundJobService
{
    public class BackgroundJobService
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public BackgroundJobService(HealthCheckService healthCheckService, ILogger<BackgroundJobService> logger, IEmailSender emailSender, IConfiguration configuration)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task HealthCheck()
        {
            var healthCheckResult = await _healthCheckService.CheckHealthAsync();
            var result = healthCheckResult.Entries.ToDictionary(e => e.Key, e => new
            {
                Result = e.Value.Status.ToString(),
                IsSuccess = e.Value.Status == HealthStatus.Healthy,
                Message = e.Value.Exception?.Message ?? e.Value.Description
            });

            StringBuilder emailContent = new StringBuilder();
            bool hasUnhealthyServices = false;

            foreach (var entry in result)
            {
                if (!entry.Value.IsSuccess)
                {
                    hasUnhealthyServices = true;
                    string unhealthyMessage = $"Unhealthy Service Detected: {entry.Key}<br>Result: {entry.Value.Result}<br>IsSuccess: {entry.Value.IsSuccess}<br>Message: {entry.Value.Message}<br><br>";
                    emailContent.AppendLine(unhealthyMessage);
                    _logger.LogError(unhealthyMessage);
                }
                else
                {
                    string healthyMessage = $"Healthy Service Detected: {entry.Key}<br>Result: {entry.Value.Result}<br>IsSuccess: {entry.Value.IsSuccess}<br>Message: {entry.Value.Message}<br><br>";
                    _logger.LogInformation(healthyMessage);
                }
            }

            if (hasUnhealthyServices)
            {
                string emailBody = emailContent.ToString();
                await _emailSender.SendEmailAsync(_configuration["EmailSettings:SupportEmail"], "Health Check Report", emailBody);
            }

        }
    }
}
