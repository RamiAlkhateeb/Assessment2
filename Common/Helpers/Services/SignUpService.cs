using Assessment.Common.Helpers;
using Assessment.Common.Models.Request;
using Assessment.Common.Models.Response;
using AutoMapper;
using Clincs.Common.Helpers;
using Common.Authorization;
using Common.Context;
using Common.Models.Database;
using Common.Models.Database.API;
using Microsoft.Extensions.Options;
using BCryptNet = BCrypt.Net.BCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Schema.Teams;
using Assessment.Common.Models.Database;
using Assessment.Common.Models;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Common.Helpers.Services
{
    public class SignUpService : ISignUpService
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        private IJwtUtils _jwtUtils;
        private readonly IConfiguration _configuration;
        public SignUpService(AppDbContext context,
            IMapper mapper,
            IJwtUtils jwtUtils,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _configuration = configuration;
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        public void CreateUser(SignUpRequest model)
        {
            // validate
            if (_context.Users.Any(x => x.Email == model.Email))
                throw new AppException("User with the email '" + model.Email + "' already exists");

            // map model to new user object
            var user = _mapper.Map<User>(model);

            // hash password
            user.PasswordHash = BCryptNet.HashPassword(model.Password);

            // save user
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public ConversationReferenceEntity SaveConversationReference(ConversationReferenceEntity cr)
        {
            var test = _context.conversationReferenceEntities.FirstOrDefault(x => x.UserId == cr.UserId);
            if (test != null)
                return test;
            else
            {
                _context.conversationReferenceEntities.Add(cr);
                _context.SaveChanges();
                return cr;
            }


        }

        

        public AuthenticateResponse Login(LoginRequest loginInfo)
        {
            var user = _context.Users.FirstOrDefault(user => user.Email.Equals(loginInfo.Email));
            // validate
            if (user == null || !BCryptNet.Verify(loginInfo.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            return new AuthenticateResponse(user, jwtToken);
        }

        public IEnumerable<ConversationReferenceEntity> GetConversationReferences()
        {
            return _context.conversationReferenceEntities;
        }

        public List<MailLog> GetMailLogs()
        {
            return _context.MailLogs.ToList();
        }

        public MailLog SaveMailLog(MailLog mailLog)
        {
            _context.MailLogs.Add(mailLog);
            _context.SaveChanges();
            return mailLog;
        }

        public User GetUserByAadObjectId(string id)
        {
            return _context.Users.FirstOrDefault(c => c.AadObjectId == id);
        }

        public ConversationReferenceEntity GetReferenceEntity(string id)
        {
            return _context.conversationReferenceEntities.FirstOrDefault(c => c.ConversationId == id);
        }


        public void SendEmail(MailRequest mailData, string name)
        {
            string to = mailData.Email; //To address    
            string from = _configuration["SmtpSettings:SenderEmail"]; //From address
            MailMessage mail = new MailMessage(from, to);
            var receiver = String.IsNullOrEmpty(name) ? GetEmailName(to) : name;
            mail.Body = "Hi " + receiver + " from " + mailData.Department + ". Thank you for signing up!” ";
            mail.Subject = "Email from the Bot";


            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp 
            client.Credentials = new System.Net.NetworkCredential(from, _configuration["SmtpSettings:Password"]);
            client.UseDefaultCredentials = false;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            if (mailData.Email != null)
                try
                {
                    //client.Send(mail);

                }
                catch (Exception e)
                {
                    throw new AppException("Sending Email Failed, " + e.Message);
                }
        }

        public string GetEmailName(string mail)
        {
            string name = "";
            foreach (var letter in mail)
            {
                if (letter == '@')
                    break;
                name += letter;
            }
            return name;
        }

    }
}
