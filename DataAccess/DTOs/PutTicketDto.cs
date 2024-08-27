using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystem.DataAccess.DTOs
{
    public class PutTicketDto
    {
        [Required(ErrorMessage = "TicketId is required.")]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "ProblemDescription is required.")]
        public string ProblemDescription { get; set; }

        [Required(ErrorMessage = "ProductId is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "TicketTypeId is required.")]
        public int TicketTypeId { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
