using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.DTOs
{
    public class GetAllUsersDto
    {
        public UserType? UserType { get; set; }
    }
}
