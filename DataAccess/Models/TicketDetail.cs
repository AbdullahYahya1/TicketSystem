using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.DataAccess.Models
{
    public class TicketDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketDetailId { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [ForeignKey("CreateBy")]
        public string CreateById { get; set; }
        public virtual AppUser CreateBy { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
