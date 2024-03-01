using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Domain.DTOs.Identity;
using Application.UnitOfWork;
using Domain;
using Domain.Models;
using Infrastructure.Middleware.Exceptions.Common;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {

        private readonly ILogger<IdentityController> _logger;
        private IIdentityServiceUOW _uow;

        public IdentityController(ILogger<IdentityController> logger, IIdentityServiceUOW uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> login(AuthenticationDTO authenticationDTO)
        {

            AuthenticationReplyDTO result = await _uow.login(authenticationDTO);

            if (result.FullName == "User Not Found")
            {
                throw new NotFoundException("User Not Found");
            }
            else if (result.FullName == "User Not Activated")
            {
                throw new UnauthorizedException("User Not Activated");
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Registration(UserRegistrationDTO user)
        {
            var result = await _uow.Register(user);

            if (result == ReplyEnum.Valid)
                return Ok(result);
            else
                throw new CustomException(result.ToString(), statusCode: System.Net.HttpStatusCode.BadRequest);

        }

        [HttpPost]
        [Route("TokenValidation")]
        public ActionResult TokenValidation(TokenAuthenticationDTO tokenAuthenticationDTO)
        {
            try
            {
                return Ok(new TokenValidationReturnDTO { TokenValidationResult = _uow.ValidateToken(tokenAuthenticationDTO).ToString() });
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost]
        [Route("GetUserNameByToken")]
        public ActionResult GetUserNameByToken(TokenAuthenticationDTO tokenAuthenticationDTO)
        {
            try
            {
                return Ok(new TokenValidationReturnDTO { TokenValidationResult = _uow.ValidateToken(tokenAuthenticationDTO).ToString() });
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword(EmailDTO emailDTO)
        {

            try
            {
                var result = _uow.ForgetPassword(emailDTO.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }
        }

        [HttpPost("VerifyForgetPassword")]
        public IActionResult VerifyForgetPassword(ForgetPasswordVerificationDTO verificationDTO)
        {
            try
            {
                return Ok(_uow.VerifyForgetPassword(verificationDTO));

            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost("AdminResetPassword")]
        public IActionResult AdminResetPassword(ChangePasswordDTO passwordDTO)
        {
            try
            {
                return Ok(_uow.AdminResetPassword(passwordDTO));
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost("RemoveUser")]
        public IActionResult RemoveUser(EmailDTO emailDTO)
        {
            try
            {
                return Ok(_uow.Remove(emailDTO));
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost("CheckEmailExist")]
        public IActionResult CheckEmailExist(EmailDTO emailDTO)
        {
            try
            {
                return Ok(_uow.CheckEmailExist(emailDTO.Email));
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost("VerifyEmail")]
        public IActionResult VerifyEmail(UsersActivation usersActivation)
        {
            try
            {
                return Ok(_uow.VerifyEmail(usersActivation));
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }

        }

        [HttpPost("GetUserData")]
        public UpdateUserDTO GetUserData(EmailDTO emailDTO)
        {
            try
            {
                return _uow.GetUserData(emailDTO);
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex.Message);
                throw new InvalidOperationException("Internal Server Error!");
            }
        }
    }
}