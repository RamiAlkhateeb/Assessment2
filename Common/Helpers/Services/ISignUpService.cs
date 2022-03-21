using Assessment.Common.Models;
using Assessment.Common.Models.Database;
using Assessment.Common.Models.Request;
using Assessment.Common.Models.Response;
using Common.Models.Database;
using Common.Models.Database.API;
using Microsoft.Bot.Schema.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers.Services
{
    public interface ISignUpService
    {
        ConversationReferenceEntity SaveConversationReference(ConversationReferenceEntity cr);

        IEnumerable<ConversationReferenceEntity> GetConversationReferences();

        User GetById(int id);
        AuthenticateResponse Login(LoginRequest user);
        void CreateUser(SignUpRequest model);
        ConversationReferenceEntity GetReferenceEntity(string id);
        //Models.Database.API.User SaveUserToAD(TeamsChannelAccount userData);
        MailLog SaveMailLog(MailLog mailLog);
        User GetUserByAadObjectId(string id);
        List<MailLog> GetMailLogs();

        void SendEmail(DataToSend data, string name);

    }
}
