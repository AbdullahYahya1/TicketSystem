using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.IRepositories;

namespace TicketSystem.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TicketSystemDbContext _db;
        public ITicketRepository tickets { get; }
        public IUserRepository users { get; }
        public ITicketDetailRepository ticketDetails { get; private set; }
        public ICommentRepository comments { get; private set; }
        public IProductRepository products { get; private set; }
        public ITicketCategoryRepository ticketCategories { get; private set; }
        public ITicketTypeRepository ticketTypes { get; private set; }
        public ITicketAttachmentRepository ticketAttachments { get; private set; }
        public UnitOfWork(TicketSystemDbContext context, ITicketRepository ticketRepository, IUserRepository userRepository,
            ITicketDetailRepository ticketDetailRepository, ITicketAttachmentRepository ticketAttachmentsRepository, ICommentRepository commentRepository, IProductRepository productsRepository, ITicketCategoryRepository ticketCategoriesRepository, ITicketTypeRepository ticketTypeRepository)
        {
            _db = context;
            tickets = ticketRepository;
            users = userRepository;
            ticketDetails = ticketDetailRepository;
            comments = commentRepository;
            products = productsRepository;
            ticketCategories = ticketCategoriesRepository;
            ticketTypes = ticketTypeRepository;
            ticketAttachments = ticketAttachmentsRepository;
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
