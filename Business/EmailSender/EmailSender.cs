using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TicketSystem.Business.Services;

namespace TicketSystem.Business.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _Configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration Configuration, ILogger<EmailSender> logger)
        {
            _Configuration = Configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailSettings = _Configuration.GetSection("EmailSettings");

                var mail = emailSettings["Username"];
                var pass = emailSettings["Password"];

                var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"]))
                {
                    Credentials = new NetworkCredential(mail, pass),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(mail),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending email to {email}.");
            }
        }
    }
}
