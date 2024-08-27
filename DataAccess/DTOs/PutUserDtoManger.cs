using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.DTOs
{
    public class PutUserDtoManger
    {
        public string ToUpdateId { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Base64Image { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
    }
}
