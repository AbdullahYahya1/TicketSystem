using Microsoft.EntityFrameworkCore;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Context
{
    public class TicketSystemDbContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<TicketAttachment> Attachments { get; set; }

        public TicketSystemDbContext(DbContextOptions<TicketSystemDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure self-referencing relationship for AppUser
            builder.Entity<AppUser>()
                .HasOne(u => u.CreatedBy)
                .WithMany()
                .HasForeignKey(u => u.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<AppUser>()
                .HasOne(u => u.UpdateBy)
                .WithMany()
                .HasForeignKey(u => u.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for AppUser
            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedProducts)
                .WithOne(p => p.CreatedBy)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.UpdatedProducts)
                .WithOne(p => p.UpdateBy)
                .HasForeignKey(p => p.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedTickets)
                .WithOne(t => t.CreatedBy)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.AssignedTickets)
                .WithOne(t => t.AssignedTo)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.UpdatedTickets)
                .WithOne(t => t.UpdateBy)
                .HasForeignKey(t => t.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedComments)
                .WithOne(c => c.CreatedBy)
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.UpdatedComments)
                .WithOne(c => c.UpdateBy)
                .HasForeignKey(c => c.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.TicketDetails)
                .WithOne(td => td.CreateBy)
                .HasForeignKey(td => td.CreateById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedCategories)
                .WithOne(c => c.CreatedBy)
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.UpdatedCategories)
                .WithOne(c => c.UpdateBy)
                .HasForeignKey(c => c.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedTypes)
                .WithOne(t => t.CreatedBy)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.UpdatedTypes)
                .WithOne(t => t.UpdateBy)
                .HasForeignKey(t => t.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedAttachments)
                .WithOne(a => a.CreatedBy)
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>()
                .HasMany(u => u.UpdatedAttachments)
                .WithOne(a => a.UpdateBy)
                .HasForeignKey(a => a.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for Comment
            builder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Product
            builder.Entity<Product>()
                .HasMany(p => p.Tickets)
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Ticket
            builder.Entity<Ticket>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Ticket>()
                .HasMany(t => t.TicketDetails)
                .WithOne(td => td.Ticket)
                .HasForeignKey(td => td.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Ticket>()
                .HasMany(t => t.Attachments)
                .WithOne(a => a.Ticket)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Ticket>()
                .HasOne(t => t.TicketType)
                .WithMany(ty => ty.Tickets)
                .HasForeignKey(t => t.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for TicketDetail
            builder.Entity<TicketDetail>()
                .HasOne(td => td.Ticket)
                .WithMany(t => t.TicketDetails)
                .HasForeignKey(td => td.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for TicketType
            builder.Entity<TicketType>()
                .HasOne(t => t.TicketCategory)
                .WithMany(c => c.TicketTypes)
                .HasForeignKey(t => t.TicketCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for Attachment
            builder.Entity<TicketAttachment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TicketAttachment>()
                .HasOne(a => a.CreatedBy)
                .WithMany(u => u.CreatedAttachments)
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketAttachment>()
                .HasOne(a => a.UpdateBy)
                .WithMany(u => u.UpdatedAttachments)
                .HasForeignKey(a => a.UpdateById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
