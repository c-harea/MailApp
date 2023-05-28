using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using MailClient;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MyMail.MailClient;
using Org.BouncyCastle.Tls;

namespace MyMail.MailClient
{
    public static class MySmtpClient
    {
        public static bool Send(Mail mail)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = mail.Body;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mail.SenderName, Program.MailConfiguration.Username));
            message.To.Add(new MailboxAddress(mail.RecipientName, mail.RecipientEmail));
            message.Subject = mail.Subject;

            // Create a copy of the attachments collection

            var attachments = new List<MimePart>();

            foreach (var item in mail.AttachmentPaths)
            {
                var stream = File.OpenRead(item);
                //using (var stream = model.Attachment.OpenReadStream())

                var mimePart = new MimePart(new MimeKit.ContentType("application", "octet-stream"))
                {
                    Content = new MimeContent(stream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    FileName = Path.GetFileName(item)
                };

                attachments.Add(mimePart);
            }
            

            // Add the attachments to the message body
            var multipart = new Multipart("mixed");
            foreach (var attachment in attachments)
            {
                multipart.Add(attachment);
            }
            multipart.Add(bodyBuilder.ToMessageBody());
            message.Body = multipart;

            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    client.Connect(("smtp." + Program.MailConfiguration.ServerName), Program.MailConfiguration.SmtpPort, SecureSocketOptions.StartTls);
                    client.Authenticate(Program.MailConfiguration.Username, Program.MailConfiguration.Password);
                    client.Send(message);

           

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

    }
}