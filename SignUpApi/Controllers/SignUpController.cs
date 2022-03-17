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
        [EnableQuery]
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            //return Ok(_userService.GetAll().AsQueryable());
            return Ok();
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
