using Microsoft.EntityFrameworkCore;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class TicketCategoryRepository : Repository<TicketCategory>, ITicketCategoryRepository
    {
        private readonly TicketSystemDbContext _context;

        public TicketCategoryRepository(TicketSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketCategory>> GetAllAsync()
        {
            return await _context.TicketCategories.Include(tc => tc.TicketTypes).ToListAsync();
        }
    }
}
