﻿using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.DTOs
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public string UserName { get; set; }
        public string UserImageURL { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string MobileNumber { get; set; }

    }
}
