using Assessment.Common.Models;
using Assessment.Common.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Common.Helpers.Services
{
    public class EmailSenderService
    {
        public void SendEmail(DataToSend data, string name)
        {
            string to = data.email; //To address    
            string from = "rami13195@gmail.com"; //From address
            MailMessage mail = new MailMessage(from, to);
            var receiver = String.IsNullOrEmpty(name) ? GetEmailName(data.email) : name;
            mail.Body = "Hi "+ receiver + " from "+data.dept+". Thank you for signing up!” ";
            mail.Subject = "Email from the Bot";


            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp 
            client.Credentials = new System.Net.NetworkCredential("rami13195@gmail.com", "acmpform");
            client.UseDefaultCredentials = false;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            if (data.email != null)
                try
                {
                    //client.Send(mail);

                }
                catch (Exception e)
                {
                    throw new AppException("Sending Email Failed");
                }
        }

        public string GetEmailName(string mail)
        {
            string name = "";
            foreach(var letter in mail)
            {
                if (letter == '@')
                    break;
                name += letter;
            }
            return name;
        }
    }
}
