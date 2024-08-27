namespace TicketSystem.DataAccess.Models
{
    public class ResponseModel<T>
    {
        public T Result { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
