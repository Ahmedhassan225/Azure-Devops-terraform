﻿namespace Domain.DTOs.Identity
{
    public class ChangePasswordDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
