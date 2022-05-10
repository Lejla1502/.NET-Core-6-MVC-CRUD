using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //configuring the properties of an email that we need to send
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("bulkybookof2022@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){ Text=htmlMessage};
            //setting text format to be html


            //sending an email
            using(var emailClient = new SmtpClient())
			{
                emailClient.CheckCertificateRevocation = false;
                //establishing the connection to SMTP server, 587 is default port for gmail
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("bulkybookof2022@gmail.com", "Bukly1234!");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
			}

            return Task.CompletedTask;
        }
    }
}
