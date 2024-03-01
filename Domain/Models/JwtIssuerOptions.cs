using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class JwtIssuerOptions
    {
        public string TokenIssuer { get; set; }
        public string TokenSubject { get; set; }
        public DateTime TokenNotBefore { get; set; } = DateTime.UtcNow;
        public DateTime TokenIssuedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan TokenValidFor { get; set; } = TimeSpan.FromMinutes(30);
        public DateTime TokenExpiration => TokenIssuedAt.Add(TokenValidFor);
        public string TokenAudience { get; set; }
        public Func<Task<string>> TokenJtiGenerator =>
          () => Task.FromResult(Guid.NewGuid().ToString());
        public SigningCredentials TokenSigningCredentials { get; set; }
    }
}
