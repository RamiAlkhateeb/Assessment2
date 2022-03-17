using Assessment.Common.Helpers;
using Assessment.Common.Models.Cards;
using Common.Helpers.Bot;
using Common.Helpers.Services;
using Common.Models.Database.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SignUpApi.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private readonly IConversationReferencesHelper _conversationReferenceHelper;
        private readonly ISignUpService _signupService;
        private string _activityId;
        private readonly ILogger<NotifyController> _logger;
        private readonly GraphServiceClient _graphServiceClient;


        public NotifyController(IConversationReferencesHelper conversationReferencesHelper,
            IBotFrameworkHttpAdapter adapter,
            IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences,
            ISignUpService signupService,
            GraphServiceClient graphServiceClient,
            ILogger<NotifyController> logger
            )
        {
            _adapter = adapter;
            _conversationReferenceHelper = conversationReferencesHelper;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _signupService = signupService;
            _logger = logger;
            _graphServiceClient = graphServiceClient;

        }


        [HttpGet("api/notify")]
        public async Task<IActionResult> Get()
        {

            if (_conversationReferences.Count == 0)
            {
                _conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
                _conversationReferences = getConversationDectionary();
            }

            foreach (var conversationReference in _conversationReferences.Values)
            {
                //var conversationNotification = notifications.Where(n => n.UserId == conversationReference.User.Id && n.IsSent == false).ToList();
                //foreach (var notification in conversationNotification)
                //{
                //    IMessageActivity message = MessageFactory.Text(notification.Body);
                //    await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference,
                //       async (context, token) => await BotCallback(message, context, token),
                //       default(CancellationToken));
                //}


            }

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        [HttpPost("api/signup/{userid}")]
        public async Task<IActionResult> PostUser([FromRoute] string userId)
        {
            
             if (_conversationReferences.Count == 0)
            {
                _conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
                _conversationReferences = getConversationDectionary();
            }
            foreach (var conversationReference in _conversationReferences.Values)
            {
                if(conversationReference.User.Id == userId)
                {
                    var user = new Common.Models.Database.API.User();
                    var card = SubmitCard.createCard(user);
                    IMessageActivity message = MessageFactory.Attachment(card);
                    await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference,
                       async (context, token) => await BotCallback(message, context, token),
                       default(CancellationToken));
                }
                

            }
            return Ok("sent to user");
               

        }

        [HttpGet("api/user")]
        public async Task<IActionResult> GetUser()
        {

            var user = _graphServiceClient.Users.Request().GetAsync().Result;

            var me = _graphServiceClient.Me.Request().GetAsync().Result;

            // var photo = await _graphApiClientDirect.GetGraphApiProfilePhoto();
            // var file = await _graphApiClientDirect.GetSharepointFile();
            return Ok(user);


        }

        [HttpGet("api/postuser")]
        public async Task<IActionResult> PostUser()
        {

            var user = new Microsoft.Graph.User
            {
                AccountEnabled = true,
                DisplayName = "Aya Mansour",
                MailNickname = "AyaM",
                UserPrincipalName = "Aya@rami13195gmail.onmicrosoft.com",
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = "Rami@603"
                }
            };

            var userr = await _graphServiceClient.Users
                .Request()
                .AddAsync(user);
            return Ok(userr);


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
