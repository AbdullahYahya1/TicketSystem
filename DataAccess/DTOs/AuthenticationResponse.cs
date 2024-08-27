namespace TicketSystem.DataAccess.DTOs
{
    public class AuthenticationResponse
    {
        public TokenResponse Tokens { get; set; }
        public GetOneUserDto User { get; set; }
    }
}
