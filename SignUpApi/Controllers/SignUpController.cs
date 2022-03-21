using Assessment.Common.Helpers;
using Assessment.Common.Models;
using Assessment.Common.Models.Database;
using Assessment.Common.Models.Request;
using Common.Authorization;
using Common.Helpers.Services;
using Common.Models.Database.API;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SignUpApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignUpController : ControllerBase
    {
        private ISignUpService _signUpService;
        private IJwtUtils _jwtUtils;
        public SignUpController(ISignUpService signUpService, IJwtUtils jwtUtils)
        {
            _signUpService = signUpService;
            _jwtUtils = jwtUtils;
        }


        [Authorize(Role.Admin)]
        [HttpPost]
        [Route("sendmail")]
        public IActionResult SendEmail([FromBody] MailRequest mail, [FromHeader] string authorization)
        {
            var userInfo = new TokenUser();
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                userInfo = _jwtUtils.ValidateJwtToken(parameter);
                // scheme will be "Bearer"
                // parmameter will be the token itself.
            }
            var user = _signUpService.GetUserByAadObjectId(userInfo.AadObjectId);
            MailValidator mailValidator = new MailValidator();
            var dataToSend = new DataToSend();
            dataToSend.email = mail.Email;
            dataToSend.dept = mail.Department;
            //return Ok(_userService.GetAll().AsQueryable());
            if (mailValidator.IsMailValid(mail.Email))
            {
                try
                {
                    _signUpService.SendEmail(dataToSend, user.FirstName);
                    var mailData = new MailLog();
                    mailData.AlternativeEmail = mail.Email;
                    mailData.Department = mail.Department;
                    mailData.SentAt = DateTime.Now;
                    mailData.UserId = user.Id + "";

                    // mailData.UserId = turnContext.Activity.Conversation.AadObjectId;
                    _signUpService.SaveMailLog(mailData);
                }
                catch(Exception e)
                {
                    throw new AppException("Mail not sent, "+ e.Message);

                }


            }
            else
                throw new AppException("Mail Not Valid");
            return Ok("Email is sent");
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        [Route("getdetails")]
        public IActionResult GetUserDetails()
        {
            return Ok(_signUpService.GetMailLogs());
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Create([FromBody] SignUpRequest model)
        {
            _signUpService.CreateUser(model);
          
            return Ok(new { message = "User created" });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            var user = _signUpService.Login(model);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });


            return Ok(user);
        }
    }
}
