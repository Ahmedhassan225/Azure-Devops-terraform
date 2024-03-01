using System;

namespace Domain.DTOs.Identity
{
    public class UpdateUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobilePhone { get; set; }
        public string UserType { get; set; }
    }
}
