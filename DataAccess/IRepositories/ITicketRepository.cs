using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.IRepositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task AddAsync(Ticket ticket);
        Task<Ticket> GetTicketWithDetailsAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetAllTicketsWithDetailsAsync();

    }
}
