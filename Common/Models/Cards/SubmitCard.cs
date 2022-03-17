using AdaptiveCards;
using Common.Models.Database.API;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Common.Models.Cards
{
    public class SubmitCard
    {

        public static Attachment createCard(User user)
        {
            AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion("1.1"));
            List<AdaptiveColumn> cols = new List<AdaptiveColumn>();
            var col2 = new AdaptiveColumn();
            col2.Type = "Column";
            col2.Spacing = AdaptiveSpacing.Medium;
            col2.Width = AdaptiveColumnWidth.Auto;
            Random rand = new Random();
            col2.Items.Add(new AdaptiveImage()
            {
                Url = new Uri("https://picsum.photos/100/100?image=" + rand.Next(1, 1000)),
                Size = AdaptiveImageSize.Medium,
                Style = AdaptiveImageStyle.Person,
                //Spacing = AdaptiveSpacing.Padding


            });
            var col = new AdaptiveColumn();
            col.Type = "Column";
            col.Width = AdaptiveColumnWidth.Auto;
            col.Items.Add(new AdaptiveTextBlock()
            {
                Weight = AdaptiveTextWeight.Bolder,
                Text = "test",
                //Wrap= true
            });
            col.Items.Add(new AdaptiveTextBlock()
            {
                IsSubtle = true,
                Text = "test",
                Weight = AdaptiveTextWeight.Lighter,
                Spacing = AdaptiveSpacing.None,
                //Wrap = true
            });

            cols.Add(col2);
            cols.Add(col);
            var mailFieldCol = new AdaptiveColumn();
            mailFieldCol.Type = "Column";
            mailFieldCol.Width = AdaptiveColumnWidth.Auto;
            mailFieldCol.Items.Add(new AdaptiveTextInput()
            {
                Id = "email",
                Label = "enter email",
                Type = "Input.Text",
                Style = AdaptiveTextInputStyle.Text
            });
            var deptFieldCol = new AdaptiveColumn();
            deptFieldCol.Type = "Column";
            deptFieldCol.Width = AdaptiveColumnWidth.Auto;
            deptFieldCol.Items.Add(new AdaptiveTextInput()
            {
                Id = "dept",
                Label = "enter dept",
                Type = "Input.Text",
                Style = AdaptiveTextInputStyle.Text
            });
            cols.Add(mailFieldCol);
            cols.Add(deptFieldCol);

            card.Body.Add(new AdaptiveColumnSet()
            {
                Spacing = AdaptiveSpacing.Medium,

                Columns = cols
            });

            card.Actions = new List<AdaptiveAction>()
        {
            new AdaptiveSubmitAction()
            {
                Type = "Action.Submit",
                Title = "Erstellen",
                Data = new DataToSend
                {
                    email = "email"
                }
            }
        };
            // serialize the card to JSON

            string json = card.ToJson();


            var adaptiveCardAttachment = CreateAdaptiveCardAttachment(json);
            return adaptiveCardAttachment;
        }

        public AdaptiveColumn GetImageCol()
        {
            var col2 = new AdaptiveColumn();
            col2.Type = "Column";
            col2.Spacing = AdaptiveSpacing.Medium;
            col2.Width = AdaptiveColumnWidth.Auto;
            Random rand = new Random();

            col2.Items.Add(new AdaptiveImage()
            {
                Url = new Uri("https://picsum.photos/100/100?image=" + rand.Next(1, 1000)),
                Size = AdaptiveImageSize.Medium,
                Style = AdaptiveImageStyle.Person,
                //Spacing = AdaptiveSpacing.Padding


            });
            return col2;
        }

        private AdaptiveColumn GetNameCol(User user)
        {
            var col = new AdaptiveColumn();
            col.Type = "Column";
            col.Width = AdaptiveColumnWidth.Auto;
            col.Items.Add(new AdaptiveTextBlock()
            {
                Weight = AdaptiveTextWeight.Bolder,
                //Text = user.Title + " " + user.FirstName + " " + user.LastName,
                //Wrap= true
            });
            col.Items.Add(new AdaptiveTextBlock()
            {
                IsSubtle = true,
                Text = user.Email,
                Weight = AdaptiveTextWeight.Lighter,
                Spacing = AdaptiveSpacing.None,
                //Wrap = true
            });
            return col;
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

    public class DataToSend
    {
        public string email { get; set; }
        public string dept { get; set; }
    }
    
}
