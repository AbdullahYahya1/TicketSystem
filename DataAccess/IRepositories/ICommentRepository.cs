using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.IRepositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllAsync();

    }
}
