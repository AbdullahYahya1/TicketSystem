namespace TicketSystem.DataAccess.DTOs
{
    public class PutCommentDto
    {
        public int TicketId { get; set; }
        public int CommentId { get; set; }
        public string NewContent { get; set; }
    }
}
