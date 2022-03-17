using Assessment.Common.Models.Request;
using Assessment.Common.Models.Response;
using Common.Models.Database;
using Common.Models.Database.API;
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


    }
}
