namespace Domain.DTOs.Identity
{
    public class ForgetPasswordVerificationDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ResetToken { get; set; }
    }
}
