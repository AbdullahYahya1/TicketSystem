using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.DataAccess.Models
{
    public enum TicketStatus
    {
        New = 0,
        Assigned = 1,
        InProgress = 2,
        Resolved = 3,
        Confirmed = 4,
        Closed = 5,
        Canceled = 6
    }
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketId { get; set; }

        [ForeignKey("CreatedByUser")]
        public string CreatedById { get; set; }
        public virtual AppUser CreatedBy { get; set; }

        [ForeignKey("AssignedToUser")]
        public string? AssignedToId { get; set; }
        public virtual AppUser AssignedTo { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("TicketType")]
        public int TicketTypeId { get; set; }
        public virtual TicketType TicketType { get; set; }

        [ForeignKey("UpdatedByUser")]
        public string UpdateById { get; set; }
        public virtual AppUser UpdateBy { get; set; }


        [StringLength(500)]
        public string ProblemDescription { get; set; }
        public TicketStatus Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<TicketDetail> TicketDetails { get; set; } = new List<TicketDetail>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    }
}
