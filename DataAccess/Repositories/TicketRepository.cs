using Microsoft.EntityFrameworkCore;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository

    {
        private readonly TicketSystemDbContext _context;

        public TicketRepository(TicketSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Ticket> GetTicketWithDetailsAsync(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.UpdateBy)
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.TicketCategory)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedBy)
                .Include(t => t.TicketDetails)
                    .ThenInclude(td => td.CreateBy)
                .Include(t => t.Product)
                .Include(t => t.Attachments) 
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }


        public async Task<IEnumerable<Ticket>> GetAllTicketsWithDetailsAsync()
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.UpdateBy)
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.TicketCategory)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedBy)
                .Include(t => t.TicketDetails)
                    .ThenInclude(td => td.CreateBy)
                .Include(t => t.Product)
                .Include(t => t.Attachments)
                .ToListAsync();
        }

        public async Task UpdateTicketCommentAsync(int ticketId, int commentId, string newContent)
        {
            var ticket = await _context.Tickets.Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket != null)
            {
                var comment = ticket.Comments.FirstOrDefault(c => c.CommentId == commentId);
                if (comment != null)
                {
                    comment.Text = newContent;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteTicketCommentAsync(int ticketId, int commentId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket != null)
            {
                var comment = ticket.Comments.FirstOrDefault(c => c.CommentId == commentId);
                if (comment != null)
                {
                    ticket.Comments.Remove(comment);
                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}
