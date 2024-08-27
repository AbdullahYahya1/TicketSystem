using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class TicketAttachmentRepository : Repository<Attachment>, ITicketAttachmentRepository
    {
        private readonly TicketSystemDbContext _context;

        public TicketAttachmentRepository(TicketSystemDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> Delete(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
