using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.DataAccess.Models
{
    public class TicketType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketTypeId { get; set; }

        [StringLength(100)]
        public string NameAr { get; set; }

        [StringLength(100)]
        public string NameEn { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public string CreatedById { get; set; }
        public virtual AppUser CreatedBy { get; set; }

        [ForeignKey("UpdatedByUser")]
        public string UpdateById { get; set; }
        public virtual AppUser UpdateBy { get; set; }

        [ForeignKey("TicketCategory")]
        public int TicketCategoryId { get; set; }
        public virtual TicketCategory TicketCategory { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
