using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.IRepositories
{
    public interface ITicketTypeRepository : IRepository<TicketType>
    {
        Task<IEnumerable<TicketType>> GetTypesByCategoryIdAsync(int categoryId);

    }
}
