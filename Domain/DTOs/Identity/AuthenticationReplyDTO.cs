using System.Collections.Generic;

namespace Domain.DTOs.Identity
{
    public class AuthenticationReplyDTO
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public int Notifications { get; set; }
    }
}
