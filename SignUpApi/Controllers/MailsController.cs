using Assessment.Common.Helpers;
using Assessment.Common.Models;
using Assessment.Common.Models.Cards;
using Assessment.Common.Models.Database;
using Assessment.Common.Models.Request;
using Common.Authorization;
using Common.Helpers.Bot;
using Common.Helpers.Services;
using Common.Models.Database.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignUpApi.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private readonly IConversationReferencesHelper _conversationReferenceHelper;
        private readonly ISignUpService _signupService;
        private string _activityId;
        private IHttpContextAccessor _httpContextAccessor;

        public MailsController(IConversationReferencesHelper conversationReferencesHelper,
            IBotFrameworkHttpAdapter adapter,
            IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences,
            ISignUpService signupService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _adapter = adapter;
            _conversationReferenceHelper = conversationReferencesHelper;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _signupService = signupService;
            _httpContextAccessor = httpContextAccessor;

        }


        [Authorize]
        [HttpPost("api/card")]
        public async Task<IActionResult> PostCardToUser([FromHeader] string authorization)
        {
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];
            

            if (_conversationReferences.Count == 0)
            {
                _conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
                _conversationReferences = getConversationDectionary();
            }
            var conversationList = _conversationReferences.Values;
            var conversation = conversationList.FirstOrDefault(c => c.Conversation.AadObjectId == currentUser.AadObjectId);
            if(conversation == null)
            {
                conversation = conversationList.FirstOrDefault();
                conversation.Conversation.AadObjectId = _signupService.GetReferenceEntity(conversation.Conversation.Id).AadObjectId;
            }
            CardText text = new CardText();
            text.Text = "Enter Email and Department to send registration mail";
            text.Color = AdaptiveCards.AdaptiveTextColor.Accent;
            var card = SubmitCard.createCard(currentUser,text);
            IMessageActivity message = MessageFactory.Attachment(card);
            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversation,
               async (context, token) => await BotCallback(message, context, token),
               default(CancellationToken));
            

            if (conversation == null)
                throw new AppException("Conversation not found");
            return Ok("Sent to user");
               

        }


        [Authorize(Role.Admin)]
        [HttpPost]
        [Route("api/mail")]
        public IActionResult SendEmail([FromBody] MailRequest mail, [FromHeader] string authorization)
        {
            
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];
            MailValidator mailValidator = new MailValidator();
            var dataToSend = new DataToSend();
            dataToSend.email = mail.Email;
            dataToSend.dept = mail.Department;
            //return Ok(_userService.GetAll().AsQueryable());
            if (mailValidator.IsMailValid(mail.Email))
            {
                try
                {
                    _signupService.SendEmail(dataToSend,currentUser.FirstName);
                    var mailData = new MailLog();
                    mailData.AlternativeEmail = mail.Email;
                    mailData.Department = mail.Department;
                    mailData.SentAt = DateTime.Now;
                    mailData.UserId = currentUser.Id + "";

                    // mailData.UserId = turnContext.Activity.Conversation.AadObjectId;
                    _signupService.SaveMailLog(mailData);
                }
                catch (Exception e)
                {
                    throw new AppException("Mail not sent, " + e.Message);

                }


            }
            else
                throw new AppException("Mail Not Valid");
            return Ok("Email is sent");
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        [Route("api/mails")]
        public IActionResult GetMails()
        {
            return Ok(_signupService.GetMailLogs());
        }


        private async Task BotCallback(IMessageActivity message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            //await turnContext.SendActivityAsync("proactive hello " + message.Text);
            var turnContextResponse = await turnContext.SendActivityAsync(message, cancellationToken);
            _activityId = turnContextResponse.Id;
        }

        [NonAction]
        public ConcurrentDictionary<string, ConversationReference> getConversationDectionary()
        {
            var list = _conversationReferenceHelper.GetConversationRefrenceAsync();
            var dict = new ConcurrentDictionary<string, ConversationReference>();
            foreach (var item in list)
            {
                ConversationReference cr = new ConversationReference();
                cr.ActivityId = item.ActivityId;
                cr.ChannelId = item.ChannelId;
                ChannelAccount bot = new ChannelAccount();
                ChannelAccount user = new ChannelAccount();
                ConversationAccount ca = new ConversationAccount();
                ca.AadObjectId = item.AadObjectId;
                user.Id = item.UserId;
                bot.Id = item.BotId;
                cr.Bot = bot;
                cr.User = user;
                ca.Id = item.ConversationId;
                cr.Conversation = ca;
                cr.ServiceUrl = item.ServiceUrl;
                dict[item.UserId] = cr;
            }
            return dict;
        }

        
    }
}
