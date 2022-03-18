using AdaptiveCards;
using Common.Helpers.Bot;
using Common.Helpers.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Attachment = Microsoft.Bot.Schema.Attachment;
using Common.Models.Database.API;
using Assessment.Common.Models.Cards;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using Assessment.Common.Helpers.Services;
using Assessment.Common.Models.Database;
using Assessment.Common.Helpers;
using Assessment.Common.Models;

namespace Assessment.Bot
{
    public class Bot : ActivityHandler
    {
        // Messages sent to the user.
        private const string WelcomeMessage = "Welcom";


        private const string PatternMessage = " the bot " +
                                              "handles 'hello', 'hi', 'help' and 'intro'. Try it now, type 'hi'";

        //private readonly string[] _cards = { ".//Resources//DoctorCard.json" };
        private readonly IConversationReferencesHelper _conversationReferenceHelper;
        private readonly BotState _userState;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private readonly ISignUpService _signupService;

        public Bot(IConversationReferencesHelper conversationReferencesHelper,
           UserState userState,
           ConcurrentDictionary<string, ConversationReference> conversationReferences,
           ISignUpService signupservice
           )
        {
            _userState = userState;
            _conversationReferenceHelper = conversationReferencesHelper;
            _conversationReferences = conversationReferences;
            _signupService = signupservice;

        }

        protected override async Task OnInstallationUpdateActivityAsync(ITurnContext<IInstallationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var activity = turnContext.Activity;
            ConversationReference botConRef = turnContext.Activity.GetConversationReference();
            await turnContext.SendActivityAsync(MessageFactory.Text("installing"), cancellationToken);

            var currentMember = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
            if (activity.Action.Equals("add"))
            {
                //var createUser = _signupService.SaveUserToAD(currentMember);
                _conversationReferenceHelper.AddorUpdateConversationRefrenceAsync(botConRef, currentMember);

            }
            else if (activity.Action.Equals("remove"))
            {
                
                //var createUser = _signupService.SaveUserToAD(currentMember);
                _conversationReferenceHelper.AddorUpdateConversationRefrenceAsync(botConRef, currentMember);
            }


        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            //var conversationReferenceString = conversationReference.Conversation.Id;


            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }


        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        // Greet when users are added to the conversation.
        // Note that all channels do not send the conversation update activity.
        // If you find that this bot works in the emulator, but does not in
        // another channel the reason is most likely that the channel does not
        // send this activity.
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"Hi there - {member.Name}. {WelcomeMessage}", cancellationToken: cancellationToken);
                    //await turnContext.SendActivityAsync(InfoMessage, cancellationToken: cancellationToken);
                    //await turnContext.SendActivityAsync($"{LocaleMessage} Current locale is '{turnContext.Activity.GetLocale()}'.", cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync(PatternMessage, cancellationToken: cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
      
            //await turnContext.SendActivityAsync(MessageFactory.Text("sending email"), cancellationToken);

            var activityId = turnContext.Activity.Id;

            //var welcomeUserStateAccessor = _userState.CreateProperty<WelcomeUserState>(nameof(WelcomeUserState));
            //var didBotWelcomeUser = await welcomeUserStateAccessor.GetAsync(turnContext, () => new WelcomeUserState(), cancellationToken);
            //AddConversationReference(turnContext.Activity as Activity);
          
            try
            {
                SendMail(turnContext);
                //create nre message activity
                var messageActivity = new Activity
                {
                    Text = "Mail Sent",
                    ReplyToId = activityId
                };
                //await turnContext.UpdateActivityAsync(messageActivity);
                await turnContext.SendActivityAsync(MessageFactory.Text("Email Sent"), cancellationToken);

                //update card 
            }
            catch (Exception e)
            {
                var messageActivity = new Activity
                {
                    Text = e.Message,
                    ReplyToId = activityId
                };
                //await turnContext.UpdateActivityAsync(messageActivity);
                await turnContext.SendActivityAsync(MessageFactory.Text("Email Not sent " + e.Message), cancellationToken);

                //update error card
            }



            #region comments
            //string to = data.email; //To address    
            //string from = "rami13195@gmail.com"; //From address
            //MailMessage mail = new MailMessage(from,to);
            //mail.Body = "teeeeeest";
            //mail.Subject = "subjeect" + data.dept;


            //SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp 
            //client.Credentials = new System.Net.NetworkCredential("rami13195@gmail.com", "");
            //client.UseDefaultCredentials = false;

            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.EnableSsl = true;

            //if (data.email != null)
            //    try
            //    {
            //        client.Send(mail);

            //    }
            //    catch (Exception e)
            //    {

            //    }

            #endregion

            //await turnContext.SendActivityAsync(MessageFactory.Text("Done"), cancellationToken);


            await _userState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }

        private void SendMail(ITurnContext<IMessageActivity> turnContext)
        {
            var message = turnContext.Activity;
            EmailSenderService emailSenderService = new EmailSenderService();
            MailValidator mailValidator = new MailValidator();
            DataToSend submitedData = ((JObject)message.Value).ToObject<DataToSend>();
            var user = _signupService.GetUserByAadObjectId(turnContext.Activity.From.AadObjectId);

            if (mailValidator.IsMailValid(submitedData.email))
            {
                emailSenderService.SendEmail(submitedData, user.FirstName);
                var mailData = new MailLog();
                mailData.AlternativeEmail = submitedData.email;
                mailData.Department = submitedData.dept;
                mailData.UserId = user.Id + "";
                _signupService.SaveMailLog(mailData);

            }
            else
                throw new Exception("Email not valid");
        }

        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Title = "Welcome to Bot Framework!",
                Text = @"Welcome to Welcome Users bot sample! This Introduction card
                         is a great way to introduce your Bot to the user and suggest
                         some things to get them started. We use this opportunity to
                         recommend a few next steps for learning more creating and deploying bots.",
                Images = new List<CardImage>() { new CardImage("https://aka.ms/bf-welcome-card-image") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.OpenUrl, "Get an overview", null, "Get an overview", "Get an overview", "https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0"),
                    new CardAction(ActionTypes.OpenUrl, "Ask a question", null, "Ask a question", "Ask a question", "https://stackoverflow.com/questions/tagged/botframework"),
                    new CardAction(ActionTypes.OpenUrl, "Learn how to deploy", null, "Learn how to deploy", "Learn how to deploy", "https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0"),
                }
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        private static Attachment CreateAdaptiveCardAttachment(string adaptiveCardJson)
        {
            //var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }

        




    }
}
