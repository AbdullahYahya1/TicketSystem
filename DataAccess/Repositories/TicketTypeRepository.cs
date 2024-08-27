using Microsoft.EntityFrameworkCore;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class TicketTypeRepository : Repository<TicketType>, ITicketTypeRepository
    {
        private readonly TicketSystemDbContext _context;

        public TicketTypeRepository(TicketSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketType>> GetTypesByCategoryIdAsync(int categoryId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.TicketCategoryId == categoryId)
                .ToListAsync();
        }
    }
}
