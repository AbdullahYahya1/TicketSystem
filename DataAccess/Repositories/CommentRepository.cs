using Microsoft.EntityFrameworkCore;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(TicketSystemDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _dbSet.Include(c => c.CreatedBy)
                               .Include(c => c.Ticket)
                               .ToListAsync();
        }
    }
}
