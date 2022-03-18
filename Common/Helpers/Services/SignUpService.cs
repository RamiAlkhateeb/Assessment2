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

namespace Common.Helpers.Services
{
    public class SignUpService : ISignUpService
    {
        private AppDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private IJwtUtils _jwtUtils;
        
        public SignUpService(AppDbContext context,
            IMapper mapper,
            IJwtUtils jwtUtils,
            
                             IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }

        public Models.Database.API.User GetById(int id)
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
            var user = _mapper.Map<Models.Database.API.User>(model);

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

        public Models.Database.API.User SaveUserToAD(TeamsChannelAccount userData)
        {
            var user = new Models.Database.API.User();
            string s = " ";
            string userdatamail = userData.Email;
            if (!_context.Users.Any(x => x.Email == userData.Email))
                foreach (var a in _context.Users.ToList())
                {
                    s = a.Email;
                    if (s == userdatamail)
                        s = " ";
                }
            
            else
            {
                user = _context.Users.FirstOrDefault(u => u.Email == userData.Email);

            }

            //var graphUser = new Microsoft.Graph.User
            //{
            //    AccountEnabled = true,
            //    DisplayName = user.FirstName + " " + user.LastName,
            //    MailNickname = "",
            //    UserPrincipalName = user.FirstName+"@rami13195gmail.onmicrosoft.com",
            //    PasswordProfile = new PasswordProfile
            //    {
            //        ForceChangePasswordNextSignIn = false,
            //        Password = user.PasswordHash
            //    },
            //    Department = user.Department,
            //    Mail = userData.Email
                
            //};

            //var users = _graphServiceClient.Users.Request().GetAsync().Result;
            //var returnedUser = new Microsoft.Graph.User();
            //bool isUserFound = false;
            //foreach( var graphuser in users)
            //{
            //    if (graphuser.Mail == graphUser.Mail)
            //    {
            //        isUserFound = true;
            //        returnedUser = graphuser;
            //    }

            //}
            //if (isUserFound)
            //    return returnedUser;
            
            //var createdUser = _graphServiceClient.Users
            //    .Request()
            //    .AddAsync(graphUser).Result;
            user.AadObjectId = userData.AadObjectId;
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
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

        public IEnumerable<MailLog> GetMailLogs()
        {
            return _context.MailLogs;
        }

        public MailLog SaveMailLog(MailLog mailLog)
        {
            _context.MailLogs.Add(mailLog);
            _context.SaveChanges();
            return mailLog;
        }

        public ConversationReferenceEntity GetReferenceEntity(string id)
        {
            return _context.conversationReferenceEntities.FirstOrDefault(c => c.ConversationId == id);
        }

        public User GetUserByAadObjectId(string id)
        {
            return _context.Users.FirstOrDefault(c => c.AadObjectId == id);
        }

    }
}
