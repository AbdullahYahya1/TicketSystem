using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.DataAccess.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [StringLength(100)]
        public string NameAr { get; set; }

        [StringLength(100)]
        public string NameEn { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public string CreatedById { get; set; }
        public virtual AppUser CreatedBy { get; set; }

        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UpdatedByUser")]
        public string UpdateById { get; set; }
        public virtual AppUser UpdateBy { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
