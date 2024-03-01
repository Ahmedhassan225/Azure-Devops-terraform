using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User : BaseEntity
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobilePhone { get; set; }
        public int UserTypeID { get; set; }
        public Nullable<bool> Activated { get; set; }
        public Nullable<bool> EmailVerified { get; set; }
        public Nullable<bool> PhoneVerified { get; set; }
        public int Notifications { get; set; }
        public int CustomerID { get; set; }

    }
}
