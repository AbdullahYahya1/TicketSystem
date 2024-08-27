namespace TicketSystem.DataAccess.DTOs
{
    public class UserWithTicketStatsDto
    {
        public GetOneUserDto User { get; set; }
        public int TotalTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int NewTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ConfirmedTickets { get; set; }
        public int CanceledTickets { get; set; }
        public List<SupportUserDto> TopSupportUsers { get; set; }

    }

}
