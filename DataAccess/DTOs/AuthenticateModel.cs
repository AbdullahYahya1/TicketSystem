namespace TicketSystem.DataAccess.DTOs
{
    public class AuthenticateModel
    {
        public string EmailORUserName { get; set; }
        public string Password { get; set; }
    }
}
