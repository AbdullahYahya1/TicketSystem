using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystem.DataAccess.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } 
        public string? VerificationCode { get; set; } = null;
        public string? OldPassword { get; set; } = null; 
        public string NewPassword { get; set; } 
        public string ConfirmPassword { get; set; } 
    }
}
