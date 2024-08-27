using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TicketSystem.DataAccess.DTOs
{
    public class PostTicketDto
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "TicketTypeId is required.")]
        public int TicketTypeId { get; set; }
        [Required(ErrorMessage = "ProblemDescription is required.")]
        public string ProblemDescription { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
