using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.DTOs
{
    public class PutTicketStatusDto
    {
        public int TicketId { get; set; }
        public TicketStatus Status { get; set; }
    }
}
