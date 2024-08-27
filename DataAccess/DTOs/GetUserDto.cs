using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.DTOs
{
    public class GetUserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserImageURL { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string MobileNumber { get; set; }
    }
}
