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

        //public static Attachment GetNotificationAdaptiveCard(NotificationEntity entity, string taskModuleUri)
        //{
        //    AdaptiveCard card = new("1.2");
        //    card.Body.Add(new AdaptiveTextBlock
        //    {
        //        Text = entity.Body,
        //        Wrap = true,
        //        Size = AdaptiveTextSize.Large,
        //        Height = AdaptiveHeight.Stretch,
        //        FontType = AdaptiveFontType.Default,
        //        Weight = AdaptiveTextWeight.Bolder,
        //        Color = AdaptiveTextColor.Accent,
        //    });

        //    card.Body.Add(new AdaptiveTextBlock
        //    {
        //        Text = entity.ShortDescription,
        //        //Wrap = true,
        //        //Size = AdaptiveTextSize.Large,
        //        //Height = AdaptiveHeight.Stretch,
        //        //FontType = AdaptiveFontType.Default,
        //        //Weight = AdaptiveTextWeight.Bolder,
        //        //Color = AdaptiveTextColor.Accent
        //    });

        //    //card.Actions.Add(new AdaptiveSubmitAction()
        //    //{
        //    //    Title = "View More",
        //    //    Data = new SubmitActionCardPayload
        //    //    {
        //    //        Url = taskModuleUri,
        //    //        MsTeams = new CardAction
        //    //        {
        //    //            Type = "task/fetch",
        //    //            Text = "viewmore"
        //    //        }
        //    //    }
        //    //});

        //    return new Attachment
        //    {
        //        ContentType = AdaptiveCard.ContentType,
        //        Content = card,
        //    };
        //}

        #region comments
        //public static Attachment GetRequestAdaptiveCard(Models.Responses.BMCAPI.Values value, string taskModuleUri)
        //{
        //    AdaptiveCard card = new("1.2");
        //    card.Body.Add(new AdaptiveTextBlock
        //    {
        //        Text = $"{value.RequestNumber} : {value.Status}",
        //        Wrap = true,
        //        Size = AdaptiveTextSize.Large,
        //        Height = AdaptiveHeight.Stretch,
        //        FontType = AdaptiveFontType.Default,
        //        Weight = AdaptiveTextWeight.Bolder,
        //        Color = AdaptiveTextColor.Accent
        //    });

        //    card.Body.Add(new AdaptiveTextBlock
        //    {
        //        Text = value.Summary,
        //    });

        //    card.Actions.Add(new AdaptiveSubmitAction()
        //    {
        //        Title = "View More",
        //        Data = new SubmitActionCardPayload
        //        {
        //            Url = taskModuleUri,
        //            MsTeams = new CardAction
        //            {
        //                Type = "task/fetch",
        //                Text = "viewmore"
        //            }
        //        }
        //    });

        //    return new Attachment
        //    {
        //        ContentType = AdaptiveCard.ContentType,
        //        Content = card,
        //    };
        //}
        //public static List<Attachment> GetApprovalsHeroCardsActivity(List<Models.Responses.BMCAPI.Entry> input)
        //{
        //    List<Attachment> attachments = new();
        //    foreach (var item in input.Take(5))
        //    {
        //        HeroCard heroCard = new()
        //        {
        //            Title = $"{item.values.RequestNumber} : {item.values.ApprovalStatus}",
        //            Subtitle = item.values.Summary,
        //        };
        //        Attachment Attachment = heroCard.ToAttachment();
        //        attachments.Add(Attachment);
        //    }
        //    return attachments;
        //}
        #endregion

    }
}
