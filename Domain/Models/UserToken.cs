using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class UserToken : BaseEntity
    {
        [Key]
        public string Email { get; set; }
        public string Token { get; set; }
        public Nullable<System.DateTime> Expiry { get; set; }
        public User user { get; }
    }
}
