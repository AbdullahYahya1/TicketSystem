namespace TicketSystem.DataAccess.DTOs
{
    public class GetCommentDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
