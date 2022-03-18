using Assessment.Common.Helpers;
using Assessment.Common.Helpers.Services;
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
using System.Threading.Tasks;

namespace SignUpApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignUpController : ControllerBase
    {
        private ISignUpService _signUpService;
        public SignUpController(ISignUpService signUpService)
        {
            _signUpService = signUpService;
        }


        [Authorize(Role.Admin)]
        //[EnableQuery]
        [HttpPost]
        [Route("sendmail")]
        public IActionResult SendEmail([FromBody] MailRequest mail)
        {
            MailValidator mailValidator = new MailValidator();
            EmailSenderService emailSenderService = new EmailSenderService();
            var dataToSend = new DataToSend();
            dataToSend.email = mail.Email;
            dataToSend.dept = mail.Department;
            //return Ok(_userService.GetAll().AsQueryable());
            if (mailValidator.IsMailValid(mail.Email))
            {
                emailSenderService.SendEmail(dataToSend , "");
                var mailData = new MailLog();
                mailData.AlternativeEmail = mail.Email;
                mailData.Department = mail.Department;
                mailData.SentAt = DateTime.Now;
               // mailData.UserId = turnContext.Activity.Conversation.AadObjectId;
                _signUpService.SaveMailLog(mailData);

            }
            else
                throw new AppException("Mail Not Valid");
            return Ok("Email is sent");
        }

        [Authorize(Role.Admin)]
        //[EnableQuery]
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
