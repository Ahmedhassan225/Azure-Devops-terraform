namespace Domain.DTOs.Identity
{
    public class TokenAuthenticationDTO
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
    }
}
