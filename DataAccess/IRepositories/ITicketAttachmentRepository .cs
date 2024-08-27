using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.IRepositories
{
    public interface ITicketAttachmentRepository : IRepository<Attachment>
    {
        Task<bool> Delete(int id);
    }
}
