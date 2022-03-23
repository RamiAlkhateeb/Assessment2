using Common.Models.Database;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Helpers.Bot
{
    public interface IConversationReferencesHelper
    {
        void AddorUpdateConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member);
        ConversationReferenceEntity DeleteConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member);
        ConversationReference GetConversationRefrenceAsync(ConcurrentDictionary<string, ConversationReference> conversationReferences, string currentUserAadObjectId);
    }
}
