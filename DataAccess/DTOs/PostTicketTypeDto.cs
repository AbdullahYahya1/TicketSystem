using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystem.DataAccess.DTOs
{
    public class PostTicketTypeDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int TicketCategoryId { get; set; }
    }
}
