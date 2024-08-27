namespace TicketSystem.DataAccess.DTOs
{
    public class TicketCategoryDto
    {
        public int TicketCategoryId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public IEnumerable<TicketTypeDto> TicketTypes { get; set; }
    }
}
