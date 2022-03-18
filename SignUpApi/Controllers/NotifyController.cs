using Assessment.Common.Helpers;
using Assessment.Common.Models.Cards;
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
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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
        private IJwtUtils _jwtUtils;


        public NotifyController(IConversationReferencesHelper conversationReferencesHelper,
            IBotFrameworkHttpAdapter adapter,
            IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences,
            ISignUpService signupService,
            ILogger<NotifyController> logger,
             IJwtUtils jwtUtils
            )
        {
            _adapter = adapter;
            _conversationReferenceHelper = conversationReferencesHelper;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _signupService = signupService;
            _logger = logger;
            _jwtUtils =  jwtUtils;

        }


        [Authorize]
        [HttpPost("api/signup/card")]
        public async Task<IActionResult> PostCardToUser([FromHeader] string authorization)
        {
            var UserInfo = new TokenUser();
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // we have a valid AuthenticationHeaderValue that has the following details:

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                UserInfo = _jwtUtils.ValidateJwtToken(parameter);
                // scheme will be "Bearer"
                // parmameter will be the token itself.
            }

            if (_conversationReferences.Count == 0)
            {
                _conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
                _conversationReferences = getConversationDectionary();
            }
            var conversationList = _conversationReferences.Values;
            var conversation = conversationList.FirstOrDefault(c => c.Conversation.AadObjectId == UserInfo.AadObjectId);
            if(conversation == null)
            {
                conversation = conversationList.FirstOrDefault();
                conversation.Conversation.AadObjectId = _signupService.GetReferenceEntity(conversation.Conversation.Id).AadObjectId;
            }
            var user = _signupService.GetById(UserInfo.userId);
            var card = SubmitCard.createCard(user);
            IMessageActivity message = MessageFactory.Attachment(card);
            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversation,
               async (context, token) => await BotCallback(message, context, token),
               default(CancellationToken));
            #region comments
            //foreach (var conversationReference in _conversationReferences.Values)
            //{
            //    if(conversationReference.Conversation.AadObjectId == null)
            //    {
            //        conversationReference.Conversation.AadObjectId = _signupService.GetReferenceEntity(conversationReference.Conversation.Id).AadObjectId;
            //    }
            //    if(conversationReference.Conversation.AadObjectId == UserInfo.AadObjectId)
            //    {
            //        var user = _signupService.GetById(UserInfo.userId);
            //        var card = SubmitCard.createCard(user);
            //        IMessageActivity message = MessageFactory.Attachment(card);
            //        await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference,
            //           async (context, token) => await BotCallback(message, context, token),
            //           default(CancellationToken));
            //        conversationFound = true;
            //    }


            //}
            #endregion

            if (conversation == null)
                throw new AppException("Conversation not found");
            return Ok("sent to user");
               

        }

        #region graphCode

        [HttpGet("api/user")]
        public async Task<IActionResult> GetUser()
        {

            //var user = _graphServiceClient.Users.Request().GetAsync().Result;

            //var me = _graphServiceClient.Me.Request().GetAsync().Result;

            //// var photo = await _graphApiClientDirect.GetGraphApiProfilePhoto();
            //// var file = await _graphApiClientDirect.GetSharepointFile();
            return Ok();


        }

        [HttpGet("api/postuser")]
        public async Task<IActionResult> PosstUser()
        {

            //var user = new Microsoft.Graph.User
            //{
            //    AccountEnabled = true,
            //    DisplayName = "Aya Mansour",
            //    MailNickname = "AyaM",
            //    UserPrincipalName = "Aya@rami13195gmail.onmicrosoft.com",
            //    PasswordProfile = new PasswordProfile
            //    {
            //        ForceChangePasswordNextSignIn = false,
            //        Password = "Rami@603"
            //    }
            //};

            //var userr = await _graphServiceClient.Users
            //    .Request()
            //    .AddAsync(user);
            return Ok();



        }
        #endregion



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
