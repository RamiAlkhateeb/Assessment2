using AdaptiveCards;
using Assessment.Common.Models.Request;
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

        public static Attachment createCard(User user,CardText text)
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
                Url = new Uri("https://picsum.photos/100/100?image=" + user.Id),//rand.Next(1, 1000)),
                Size = AdaptiveImageSize.Medium,
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
                Wrap= true
            });
            

            TextColumns.Add(NameColumn);
            TextColumns.Add(TitleColumn);
            List<AdaptiveColumn> InputColumns = new List<AdaptiveColumn>();
            var mailFieldCol = new AdaptiveColumn();
            mailFieldCol.Type = "Column";
            mailFieldCol.Width = AdaptiveColumnWidth.Auto;
            mailFieldCol.Items.Add(new AdaptiveTextInput()
            {
                Id = "Email",
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
                Id = "Department",
                //Label = "enter dept",
                Placeholder = "Department",
                Type = "Input.Text",
                Style = AdaptiveTextInputStyle.Text
            });
            InputColumns.Add(mailFieldCol);
            InputColumns.Add(deptFieldCol);

            List<AdaptiveColumn> Warnings = new List<AdaptiveColumn>();

            var WarningTextColumn = new AdaptiveColumn();
            WarningTextColumn.Type = "Column";
            WarningTextColumn.Width = AdaptiveColumnWidth.Auto;
            WarningTextColumn.Items.Add(new AdaptiveTextBlock()
            {
                IsSubtle = true,
                Text = text.Text,
                Color = text.Color,
                Weight = AdaptiveTextWeight.Lighter,
                Spacing = AdaptiveSpacing.None,
                Wrap = true
            });
            Warnings.Add(WarningTextColumn);
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
            card.Body.Add(new AdaptiveColumnSet()
            {
                Spacing = AdaptiveSpacing.Medium,

                Columns = Warnings
            });

            card.Actions = new List<AdaptiveAction>()
        {
            new AdaptiveSubmitAction()
            {
                Type = "Action.Submit",
                Title = "Submit",
                Data = new MailRequest
                {
                    Department = "Department",
                    Email = "Email"
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
