using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.DataAccess.Models
{
    public class TicketAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttachmentId { get; set; }


        [StringLength(500)]
        public string FilePath { get; set; }
        [StringLength(200)]
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public string CreatedById { get; set; }
        public virtual AppUser CreatedBy { get; set; }

        [ForeignKey("UpdatedByUser")]
        public string UpdateById { get; set; }
        public virtual AppUser UpdateBy { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
