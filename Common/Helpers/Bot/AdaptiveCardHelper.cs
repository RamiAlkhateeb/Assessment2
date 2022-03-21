using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers.Bot
{
    public class AdaptiveCardHelper
    {
        public class SubmitActionCardPayload
        {
            public string Url { get; set; }

            [JsonProperty("msteams")]
            public CardAction MsTeams { get; set; }
        }


    }
}
