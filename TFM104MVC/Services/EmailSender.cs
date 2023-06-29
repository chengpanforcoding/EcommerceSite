using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Services
{
    public class EmailSender : ISender
    {
        void ISender.Sender(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("網站開發者", "jpan424@gmail.com")); //寄件者
            emailMessage.To.Add(new MailboxAddress("親愛的用戶", email)); //收件者
            emailMessage.Subject = subject; //郵件標題
            emailMessage.Body = new TextPart("html") { Text = message }; //郵件內容
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("jpan424@gmail.com", "ukfkzcdxlukbsvxq");
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}
