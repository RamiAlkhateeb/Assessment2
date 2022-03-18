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
            List<AdaptiveColumn> TextColumns = new List<AdaptiveColumn>();
            var NameColumn = new AdaptiveColumn();
            NameColumn.Type = "Column";
            NameColumn.Spacing = AdaptiveSpacing.Medium;
            NameColumn.Width = AdaptiveColumnWidth.Auto;
            Random rand = new Random();
            NameColumn.Items.Add(new AdaptiveImage()
            {
                Url = new Uri("https://picsum.photos/100/100?image=" + rand.Next(1, 1000)),
                Size = AdaptiveImageSize.Large,
                Style = AdaptiveImageStyle.Person,
                //Spacing = AdaptiveSpacing.Padding


            });
            var TitleColumn = new AdaptiveColumn();
            TitleColumn.Type = "Column";
            TitleColumn.Width = AdaptiveColumnWidth.Auto;
            TitleColumn.Items.Add(new AdaptiveTextBlock()
            {
                Weight = AdaptiveTextWeight.Bolder,
                Text = "Hi "+user.FirstName + " " + user.LastName,
                //Wrap= true
            });
            TitleColumn.Items.Add(new AdaptiveTextBlock()
            {
                IsSubtle = true,
                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Weight = AdaptiveTextWeight.Lighter,
                Spacing = AdaptiveSpacing.None,
                Wrap = true
            });

            TextColumns.Add(NameColumn);
            TextColumns.Add(TitleColumn);
            List<AdaptiveColumn> InputColumns = new List<AdaptiveColumn>();
            var mailFieldCol = new AdaptiveColumn();
            mailFieldCol.Type = "Column";
            mailFieldCol.Width = AdaptiveColumnWidth.Auto;
            mailFieldCol.Items.Add(new AdaptiveTextInput()
            {
                Id = "email",
                //Label = "enter email",
                Type = "Input.Text",
                Placeholder = "Alternative email address",
                Style = AdaptiveTextInputStyle.Text
            });
            var deptFieldCol = new AdaptiveColumn();
            deptFieldCol.Type = "Column";
            deptFieldCol.Width = AdaptiveColumnWidth.Auto;
            deptFieldCol.Items.Add(new AdaptiveTextInput()
            {
                Id = "dept",
                //Label = "enter dept",
                Placeholder = "Department",
                Type = "Input.Text",
                Style = AdaptiveTextInputStyle.Text
            });
            InputColumns.Add(mailFieldCol);
            InputColumns.Add(deptFieldCol);

            card.Body.Add(new AdaptiveColumnSet()
            {
                Spacing = AdaptiveSpacing.Medium,

                Columns = TextColumns
            });
            card.Body.Add(new AdaptiveColumnSet()
            {
                Spacing = AdaptiveSpacing.Medium,

                Columns = InputColumns
            });

            card.Actions = new List<AdaptiveAction>()
        {
            new AdaptiveSubmitAction()
            {
                Type = "Action.Submit",
                Title = "Submit",
                Data = new DataToSend
                {
                    dept = "dept",
                    email = "email"
                }
            }
        };
            // serialize the card to JSON

            string json = card.ToJson();


            var adaptiveCardAttachment = CreateAdaptiveCardAttachment(json);
            return adaptiveCardAttachment;
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
