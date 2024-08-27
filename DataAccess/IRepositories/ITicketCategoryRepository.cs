using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.IRepositories
{
    public interface ITicketCategoryRepository : IRepository<TicketCategory>
    {
        Task<IEnumerable<TicketCategory>> GetAllAsync();
    }
}
