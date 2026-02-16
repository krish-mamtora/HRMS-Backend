using MimeKit;
using System.Net;
using System.Net.Mail;

namespace HRMS_Backend.Services.Email
{
    public class EmailService 
    {
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string smtpUsername;
        private readonly string smtpPassword;
        public EmailService(IConfiguration configuration)
        {
            smtpServer = configuration.GetValue<string>("SmtpSettings.SmtpServer", "");
            smtpPort = configuration.GetValue<int>("SmtpSettings.SmtpPort", 0);
            smtpUsername = configuration.GetValue<string>("SmtpSettings.SmtpUsername", "");
            smtpPassword = configuration.GetValue<string>("SmtpSettings.SmtpPassword", "");
        }


            public void SendEmail (string senderName , string senderEmail 
                , string toName  , string toEmail  , string subject , string textContent)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = textContent
            };

            using (var client = new SmtpClient())
            {
                //client.Connect(smtpServer, smtpPort, false);

                //client.Authenticate(smtpUsername, smtpPassword);
                //try
                //{
                //    var result = client.Send(message);
                //    Console.WriteLine("Email Sender Ok : \n" + result);
                //    client.Disconnect(true);
                //}
                //catch (ex)
                //{
                //    Console.WriteLine("Email Sender Fail : \n" + ex.ToString());
                //}
            }
        } 
        }
    }
//}
