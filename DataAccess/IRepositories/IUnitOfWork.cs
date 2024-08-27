namespace TicketSystem.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        ITicketRepository tickets { get; }
        ITicketDetailRepository ticketDetails { get; }
        IUserRepository users { get; }
        ICommentRepository comments { get; }
        IProductRepository products { get; }
        ITicketCategoryRepository ticketCategories { get; }
        ITicketTypeRepository ticketTypes { get; }
        ITicketAttachmentRepository ticketAttachments { get; }
        Task<int> SaveChangesAsync();

    }
}
