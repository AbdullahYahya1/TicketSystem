namespace TicketSystem.DataAccess.DTOs
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public string CreatedBy { get; set; }  
        public int ProductId { get; set; }
        public int TicketTypeId { get; set; }
        public string UpdatedBy { get; set; }  
        public string ProblemDescription { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<TicketAttachmentDto> Attachments { get; set; } = new List<TicketAttachmentDto>();
    }
}
