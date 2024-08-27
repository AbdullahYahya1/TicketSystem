using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class TicketDetailRepository : Repository<TicketDetail>, ITicketDetailRepository
    {
        public TicketDetailRepository(TicketSystemDbContext context) : base(context)
        {
        }
    }
}
