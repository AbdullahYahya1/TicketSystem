using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystem.DataAccess.DTOs
{
    public class GetOneTicketDto
    {
        public int TicketId { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string ProblemDescription { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<GetCommentDto> Comments { get; set; } = new List<GetCommentDto>();
        public List<TicketDetailDto> TicketDetails { get; set; } = new List<TicketDetailDto>();
        public string TicketCategoryAr { get; set; }
        public string TicketCategoryEn { get; set; }
        public string TicketTypeAr { get; set; }
        public string TicketTypeEn { get; set; }
        public string ProductNameAr { get; set; }
        public string ProductNameEn { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
