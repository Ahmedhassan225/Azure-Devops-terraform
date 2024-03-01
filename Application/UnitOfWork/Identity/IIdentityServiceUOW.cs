using Domain;
using Domain.DTOs.Identity;
using Domain.Models;

namespace Application.UnitOfWork
{
    public interface IIdentityServiceUOW
    {
        Task<AuthenticationReplyDTO> login(AuthenticationDTO authenticationpayload);
        ValidateTokenEnum ValidateToken(TokenAuthenticationDTO authenticationDTO);
        Task<ReplyEnum> Register(UserRegistrationDTO registrationDTO);
        ReplyEnum Update(UpdateUserDTO updateUserDTO);
        ReplyEnum Remove(EmailDTO  emailDTO);
        ReplyEnum AdminResetPassword(ChangePasswordDTO passwordDTO);
        Task<ReplyEnum> ForgetPassword(string email);
        Task<ReplyEnum> VerifyForgetPassword(ForgetPasswordVerificationDTO verificationDTO);
        ReplyEnum ChangePassword(ChangePasswordDTO passwordDTO);
        //Task SendEmailVerification(User user);
        ReplyEnum VerifyEmail(UsersActivation usersActivation);
        ReplyEnum CheckEmailExist(string email);
        UpdateUserDTO GetUserData(EmailDTO emailDTO);
        //Task SendMessage(Email messageBody);

    }
}
