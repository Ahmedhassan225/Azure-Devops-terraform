using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class UsersActivation : BaseEntity
    {
        [Key]
        public string Email { get; set; }

        public string FirstTimeToken { get; set; }

        public string? PhoneCode { get; set; }

        public User user { get; }
    }
}
