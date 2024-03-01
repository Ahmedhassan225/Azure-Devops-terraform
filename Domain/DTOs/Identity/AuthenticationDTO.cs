using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Identity
{
    public class AuthenticationDTO
    {


        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
