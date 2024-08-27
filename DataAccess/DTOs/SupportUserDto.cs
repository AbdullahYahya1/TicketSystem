using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystem.DataAccess.DTOs
{
    public class SupportUserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TicketsHandled { get; set; }
    }
}
