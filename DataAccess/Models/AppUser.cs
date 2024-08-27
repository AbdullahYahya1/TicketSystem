using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.DataAccess.Models
{
    public enum UserType
    {
        Client = 0,
        Manager = 1,
        Support = 2,
    }
    public class AppUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserId { get; set; }
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [StringLength(100)]
        public string FullName { get; set; }
        [StringLength(500)]
        public string UserImageURL { get; set; }
        public DateTime DateOfBirth { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public virtual AppUser? CreatedBy { get; set; }
        public string? CreatedById { get; set; }

        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        public virtual AppUser? UpdateBy { get; set; }
        public string? UpdateById { get; set; }

        public UserType UserType { get; set; }

        [Phone]
        public string MobileNumber { get; set; }
        [Required]
        public string Password { get; set; }


        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<Product> CreatedProducts { get; set; } = new List<Product>();
        public virtual ICollection<Product> UpdatedProducts { get; set; } = new List<Product>();
        public virtual ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> UpdatedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Comment> CreatedComments { get; set; } = new List<Comment>();
        public virtual ICollection<Comment> UpdatedComments { get; set; } = new List<Comment>();
        public virtual ICollection<TicketDetail> TicketDetails { get; set; } = new List<TicketDetail>();
        public virtual ICollection<TicketCategory> CreatedCategories { get; set; } = new List<TicketCategory>();
        public virtual ICollection<TicketCategory> UpdatedCategories { get; set; } = new List<TicketCategory>();
        public virtual ICollection<TicketType> CreatedTypes { get; set; } = new List<TicketType>();
        public virtual ICollection<TicketType> UpdatedTypes { get; set; } = new List<TicketType>();
        public virtual ICollection<TicketAttachment> CreatedAttachments { get; set; } = new List<TicketAttachment>();
        public virtual ICollection<TicketAttachment> UpdatedAttachments { get; set; } = new List<TicketAttachment>();
    }

}
