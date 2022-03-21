

using Common.Helpers.Services;
using Common.Models.Database;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using System;
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

        public IEnumerable<ConversationReferenceEntity> GetConversationRefrenceAsync()
        {
            return _signupService.GetConversationReferences();
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
    }
}
