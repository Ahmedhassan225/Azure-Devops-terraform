using Application.Helpers.Encryption;
using Application.Repository.Interfaces;
using Domain;
using Domain.DTOs.Identity;
using Domain.Models;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.UnitOfWork
{
    public class IdentityServiceUOW : IIdentityServiceUOW
    {
        #region Data Members 
        /// <summary>
        /// repositories
        /// </summary>
        protected IRepositoryProvider RepositoryProvider { get; set; }
        protected IRepository<User> UsersRepo { get { return GetStandardRepo<User>(); } }
        protected IRepository<UserToken> UserTokensRepo { get { return GetStandardRepo<UserToken>(); } }
        protected IRepository<UsersActivation> UsersActivationRepo { get { return GetStandardRepo<UsersActivation>(); } }
        protected IRepository<ResetPasswordToken> ResetPasswordTokenRepo { get { return GetStandardRepo<ResetPasswordToken>(); } }


        Encryption encryption = new Encryption();


        private ILogger<IdentityServiceUOW> _logger;

        #endregion

        #region Constructor
        public IdentityServiceUOW(IRepositoryProvider repositoryProvider, ILogger<IdentityServiceUOW> logger)
        {
            if (repositoryProvider.DbContext == null) throw new ArgumentNullException("dbContext is null"); /// if Database context not initalized Through Exception
            this.RepositoryProvider = repositoryProvider;
            _logger = logger;

        }
        #endregion

        #region Public Methods

        public async Task<AuthenticationReplyDTO> login(AuthenticationDTO payload)
        {
            try
            {
                string encreptedPass = encryption.EncryptData(payload.Password);
                AuthenticationReplyDTO authenticationreply = new AuthenticationReplyDTO();

                var checkUser = UsersRepo.Where(us => us.Email == payload.Email && us.Password == encreptedPass).FirstOrDefault();
                if (checkUser != null)
                {
                    if (checkUser.Activated == true)
                    {

                        JwtIssuerOptions _jwtOptions = new JwtIssuerOptions();

                        var claims = new[] {
                               new Claim(JwtRegisteredClaimNames.UniqueName,payload.Email),
                               new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.TokenJtiGenerator()),
                               new Claim(JwtRegisteredClaimNames.GivenName, checkUser.FirstName),
                               new Claim(JwtRegisteredClaimNames.Typ, (checkUser.UserTypeID==1)? "Admin" : "Customer")
                        };

                        _jwtOptions.TokenIssuer = "Backend";
                        _jwtOptions.TokenAudience = "Backend-Ahmed";

                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TDsC!03(F^$s]_>(*i=S"));
                        _jwtOptions.TokenSigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


                        var jwtSecToken = new JwtSecurityToken(
                         issuer: _jwtOptions.TokenIssuer,
                         audience: _jwtOptions.TokenAudience,
                         claims: claims,
                         notBefore: _jwtOptions.TokenNotBefore,
                         expires: _jwtOptions.TokenExpiration,
                         signingCredentials: _jwtOptions.TokenSigningCredentials);

                        var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwtSecToken);

                        var userToken = UserTokensRepo.Where(t => t.Email == checkUser.Email).FirstOrDefault();
                        if(userToken != null)
                        {
                            UserTokensRepo.Delete(userToken);
                            UserTokensRepo.SaveChanges();
                        }

                        UserToken token = new UserToken()
                        {
                            Email = checkUser.Email,
                            Token = encodedToken,
                            Expiry = DateTime.Now.AddDays(1)
                        };

                        UserTokensRepo.Add(token);
                        UserTokensRepo.SaveChanges();


                        authenticationreply.Token = encodedToken;
                        authenticationreply.FullName = checkUser.FirstName + " " + checkUser.LastName;
                        authenticationreply.Type = (checkUser.UserTypeID == '1')? "Admin" : "Customer";
                        authenticationreply.Notifications = checkUser.Notifications;

                        return authenticationreply;

                    }
                    else
                    {
                        authenticationreply.FullName = "User Not Activated";

                        return authenticationreply;
                    }
                }
                else
                {
                    authenticationreply.FullName = "User Not Found";

                    _logger.LogDebug("User Not Found " + payload.Email);

                    return authenticationreply;
                }
            }catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw;
            }


        }

        public ValidateTokenEnum ValidateToken(TokenAuthenticationDTO authenticationDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<ReplyEnum> Register(UserRegistrationDTO registrationDTO)
        {
            var User = UsersRepo.Where(us => us.Email == registrationDTO.Email).FirstOrDefault();

            if (User == null)
            {
                try
                {
                    User newUser = new User();

                    newUser.Email = registrationDTO.Email;
                    newUser.FirstName = registrationDTO.Firstname;
                    newUser.LastName = registrationDTO.Lastname;
                    newUser.MobilePhone = registrationDTO.MobilePhone;
                    newUser.Password = encryption.EncryptData(registrationDTO.Password);
                    newUser.CreatedBy = registrationDTO.Email;
                    newUser.ModifiedBy = "";
                    newUser.UserTypeID = 2;
                    newUser.Activated = true; //Untill Email Verification is Up 
                    newUser.EmailVerified = false;


                    //await SendEmailVerification(newUser);

                    UsersRepo.Add(newUser);
                    UsersRepo.SaveChanges();

                    return ReplyEnum.Valid;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                return ReplyEnum.EmailExist;
            }

        }

        public ReplyEnum Update(UpdateUserDTO updateUserDTO)
        {
            var User = UsersRepo.Where(c => c.Email == updateUserDTO.Email).FirstOrDefault();
            if (User != null)
            {
                try
                {

                    User.MobilePhone = updateUserDTO.MobilePhone;
                    User.FirstName =updateUserDTO.FirstName;
                    User.LastName = updateUserDTO.LastName;                      

                    UsersRepo.Update(User);
                    UsersRepo.SaveChanges();

                    return ReplyEnum.Valid;
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            else
            {
                return ReplyEnum.AccountNotFound;
            }
        }

        public ReplyEnum Remove(EmailDTO emailDTO)
        {
            try
            {
                var User = UsersRepo.Where(us => us.Email == emailDTO.Email).FirstOrDefault();

                if (User != null)
                {

                    UsersRepo.Delete(User);
                    UsersRepo.SaveChanges();
                    return ReplyEnum.Valid;
                }
                else
                {
                    return ReplyEnum.InvalidEmail;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw;
            }
        } //ok

        public ReplyEnum AdminResetPassword(ChangePasswordDTO passwordDTO)
        {
            var User = UsersRepo.Where(c => c.Email == passwordDTO.Email).FirstOrDefault();
            if(User != null)
            {
                try
                {
                    var newPass = encryption.EncryptData(passwordDTO.NewPassword);

                    User.Password = newPass;
                    UsersRepo.Update(User);
                    UsersRepo.SaveChanges();

                    return ReplyEnum.Valid;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return ReplyEnum.InvalidEmail;
            }
        } //ok

        public async Task<ReplyEnum> ForgetPassword(string email)
        {
            var user = UsersRepo.Where(us => us.Email == email).FirstOrDefault();

            if (user != null)
            {
                try
                {

                    throw new NotImplementedException(); 

                    //var passwordToken = ResetPasswordTokenRepo.Where(reset => reset.Email == email).FirstOrDefault();
                    //if (passwordToken != null)
                    //{
                    //    ResetPasswordTokenRepo.Delete(passwordToken);
                    //    ResetPasswordTokenRepo.SaveChanges();
                    //}
                    //ResetPasswordToken newresetToken = new ResetPasswordToken();
                    //newresetToken.Token = Guid.NewGuid().ToString();
                    //newresetToken.Email = user.Email;
                    //newresetToken.Expiry = DateTime.Now.AddMinutes(60);
                    //newresetToken.CreatedBy = user.Email;

                    //ResetPasswordTokenRepo.Add(newresetToken);
                    //ResetPasswordTokenRepo.SaveChanges();

                    //Email emailmessage = new Email()
                    //{

                    //    To = new List<EmailRecipient> { new EmailRecipient() { Email = user.Email, Name = $"{user.FirstName} {user.LastName}" } },
                    //    From = "Sender@Shickoo.com",
                    //    Subject = "Your password has been reset successfully"
                    //};

                    //List<EmailBuilder> emailBuilder = new List<EmailBuilder>
                    //{
                    //    new EmailBuilder { Name = "{FullName}", Value = $"{user.FirstName} {user.LastName}"},
                    //    new EmailBuilder { Name = "{Link}", Value = _config.PlatformURL  + "auth/resetpassword?T=" + newresetToken.Token}
                    //};


                    //emailmessage.EmailBuilder = emailBuilder;
                    //emailmessage.EmailTemplate = "ForgetPassword";

                    ////await SendMessage(emailmessage);

                    //return ReplyEnum.Valid ;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {
                return ReplyEnum.AccountNotFound;
            }
        }
        public async Task<ReplyEnum> VerifyForgetPassword(ForgetPasswordVerificationDTO verificationDTO)
        {
            var passwordToken = ResetPasswordTokenRepo.Where(reset => reset.Token == verificationDTO.ResetToken).FirstOrDefault();
            try
            {
                if (passwordToken != null)
                {
                    var user = UsersRepo.Where(us => us.Email == passwordToken.Email).FirstOrDefault();

                    if (user != null)
                    {
                        if (passwordToken != null)
                        {
                            if (passwordToken.Expiry < DateTime.Now)
                            {
                                return ReplyEnum.ExpiredToken;
                            }
                            else
                            {

                                throw new NotImplementedException();

                                //user.Password = encryption.EncryptData(verificationDTO.NewPassword); ;
                                //user.ModifiedBy = verificationDTO.Email;

                                //UsersRepo.Update(user);
                                //UsersRepo.SaveChanges();

                                //Email emailmessage = new Email()
                                //{

                                //    To = new List<EmailRecipient> { new EmailRecipient() { Email = user.Email, Name = $"{user.FirstName} {user.LastName}" } },
                                //    From = "Sender@Shickoo.com",
                                //    Subject = "Your password has been reset successfully"
                                //};

                                //List<EmailBuilder> emailBuilder = new List<EmailBuilder>
                                //{
                                //    new EmailBuilder { Name = "{FullName}", Value = $"{user.FirstName} {user.LastName}" },
                                //    new EmailBuilder { Name = "{Link}", Value = _config.PlatformURL + "auth/forgetpassword" }
                                //};

                                //emailmessage.EmailBuilder = emailBuilder;
                                //emailmessage.EmailTemplate = "PasswordChange";

                                ////await SendMessage(emailmessage);

                                //ResetPasswordTokenRepo.Delete(passwordToken);
                                //ResetPasswordTokenRepo.SaveChanges();

                                //return ReplyEnum.Valid;
                            }
                        }
                        else
                        {
                            return ReplyEnum.InvalidToken;
                        }
                    }
                    else
                    {
                        return ReplyEnum.AccountNotFound;
                    }


                }
                else

                {
                    return ReplyEnum.AccountNotFound;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        } //ok

        public ReplyEnum ChangePassword(ChangePasswordDTO passwordDTO)
        {
            try
            {
                var encryptedPassword = encryption.EncryptData(passwordDTO.Password);
                var User = UsersRepo.Where(us => us.Email == passwordDTO.Email && us.Password == encryptedPassword).FirstOrDefault();

                if (User != null)
                {
                    User.Password = encryption.EncryptData(passwordDTO.NewPassword);
                    User.ModifiedBy = passwordDTO.Email;
                    UsersRepo.SaveChanges();

                    return ReplyEnum.Valid;

                }
                else
                {
                    return ReplyEnum.AccountNotFound;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        } //ok

        //public async Task SendEmailVerification(User user)
        //{
            //UsersActivation usersActivation = new UsersActivation();
            //usersActivation.Email = user.Email;
            //usersActivation.FirstTimeToken = Guid.NewGuid().ToString();

            //UsersActivationRepo.Add(usersActivation);
            //UsersActivationRepo.SaveChanges();

            //Email emailmessage = new Email(){

            //    To = new List<EmailRecipient> { new EmailRecipient() { Email = user.Email, Name = $"{user.FirstName} {user.LastName}" } },
            //    From = "Sender@Shickoo.com",
            //    Subject = "Verify your email"
            //};

            //List<EmailBuilder> emailBuilder = new List<EmailBuilder>{
                
            //    new EmailBuilder { Name = "{FullName}", Value = $"{user.FirstName} {user.LastName}"},
            //    new EmailBuilder { Name = "{Link}", Value = _config.PlatformURL + "auth/emailverification?T=" + usersActivation.FirstTimeToken }
            //};

            //emailmessage.EmailBuilder = emailBuilder;
            //emailmessage.EmailTemplate = "EmailVerification";

            //await SendMessage(emailmessage);

        //} //ok

        public ReplyEnum CheckEmailExist(string email)
        {
            var User = UsersRepo.Where(us => us.Email == email).FirstOrDefault();

            if (User != null)
            {
                return ReplyEnum.EmailExist;
            }
            else
            {
                return ReplyEnum.AccountNotFound;
            }
        } //ok

        public UpdateUserDTO GetUserData(EmailDTO emailDTO)
        {
            var User = UsersRepo.Where(us => us.Email == emailDTO.Email).FirstOrDefault();
            
            UpdateUserDTO updateUserDTO = new UpdateUserDTO();
            if (User != null)
            {




                updateUserDTO.Email = User.Email;
                updateUserDTO.FirstName = User.FirstName;
                updateUserDTO.LastName = User.LastName;
                updateUserDTO.MobilePhone = User.MobilePhone;
                updateUserDTO.UserType = (User.UserTypeID == '1')? "Admin" : "Customer";

                return updateUserDTO;

            }
            else
            {
                updateUserDTO.Email = "Account Not Found";
                return updateUserDTO;
            }
        } //ok

        ReplyEnum IIdentityServiceUOW.VerifyEmail(UsersActivation usersActivation)
        {
            var activationexist = UsersActivationRepo.Where(UA => UA.FirstTimeToken == usersActivation.FirstTimeToken).FirstOrDefault();

            if (activationexist != null)
            {
                var User = UsersRepo.Where(us => us.Email == activationexist.Email).FirstOrDefault();

                if (User != null)
                {
                    try
                    {
                        if (User.Activated == true)
                        {
                            return ReplyEnum.AlreadyActivated;
                        }
                        else
                        {
                            User.EmailVerified = true;
                            User.Activated = true;
                            UsersRepo.Update(User);
                            UsersRepo.SaveChanges();

                            return ReplyEnum.Valid;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
                else
                {
                    return ReplyEnum.AccountNotFound;
                }

            }
            else
            {
                return ReplyEnum.InvalidToken;
            }

        } //ok
    

        #endregion

        #region Private Methods

        private IRepository<T> GetStandardRepo<T>() where T : BaseEntity
        {
            try
            {
                var repo = RepositoryProvider.GetRepositoryForEntityType<T>();
                return repo;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw ex;
            }
        }
        private T GetRepo<T>() where T : BaseEntity
        {
            try
            {
                return RepositoryProvider.GetRepository<T>();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Disposing 

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        //public async Task SendMessage(Email messageBody)
        //{
        //    var credentails = new BasicAWSCredentials("", "");
        //    var _sqsClient = new AmazonSQSClient(credentails);
        //    var qUrl = "";

        //    SendMessageResponse responseSendMsg = await _sqsClient.SendMessageAsync(qUrl, JsonConvert.SerializeObject(messageBody));
        //    Console.WriteLine($"Message added to queue\n  {qUrl}");
        //    Console.WriteLine($"HttpStatusCode: {responseSendMsg.HttpStatusCode}");
        //}
        #endregion

    }
}
