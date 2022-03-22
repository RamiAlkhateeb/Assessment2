
using Common.Helpers.Services;
using Common.Models.Database;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers.Bot
{
    public class ConversationReferencesHelper : IConversationReferencesHelper
    {
        private ISignUpService _signupService;

        public ConversationReferencesHelper(IConfiguration configuration, ISignUpService signupService) 
        {
            _signupService = signupService;
        }
        public void AddorUpdateConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member)
        {
            var entity = ConvertConversationReferanceForDB(reference, member);
            _signupService.SaveConversationReference(entity);
        }

        public async Task DeleteConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member)
        {
            var entity = ConvertConversationReferanceForDB(reference, member);
            //await DeleteAsync(entity);
        }

        public ConversationReference GetConversationRefrenceAsync(ConcurrentDictionary<string, ConversationReference> conversationReferences,string currentUserAadObjectId)
        {
            if (conversationReferences.Count == 0)
            {
                conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
                conversationReferences = GetConversationDectionary();
            }
            var conversationList = conversationReferences.Values;
            var conversation = conversationList.FirstOrDefault(c => c.Conversation.AadObjectId == currentUserAadObjectId);
            if (conversation == null)
            {
                conversation = conversationList.FirstOrDefault();
                conversation.Conversation.AadObjectId = _signupService.GetReferenceEntity(conversation.Conversation.Id).AadObjectId;
            }
            return conversation;
        }

        private ConversationReferenceEntity ConvertConversationReferanceForDB(ConversationReference reference, TeamsChannelAccount currentMember)
        {
            return new ConversationReferenceEntity
            {
                UPN = currentMember.UserPrincipalName,
                Name = currentMember.Name,
                AadObjectId = currentMember.AadObjectId,
                UserId = currentMember.Id,
                ActivityId = reference.ActivityId,
                BotId = reference.Bot.Id,
                ChannelId = reference.ChannelId,
                ConversationId = reference.Conversation.Id,
                Locale = reference.Locale,
                //RowKey = currentMember.UserPrincipalName,
                ServiceUrl = reference.ServiceUrl,
                //PartitionKey = ConversationReferences.PartitionKey

            };
        }

        private ConcurrentDictionary<string, ConversationReference> GetConversationDectionary()
        {
            var list = _signupService.GetConversationReferences();
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
