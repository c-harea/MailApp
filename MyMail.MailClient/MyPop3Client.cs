using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MailClient;
using MailKit.Net.Pop3;
using MailKit.Security;
using MimeKit;

namespace MyMail.MailClient
{
    public static class MyPop3Client
    {
        public static List<Mail> DownloadMails(int count)
        {
            var mails = new List<Mail>();

            using (var client = new Pop3Client())
            {
                try
                {
                    client.Connect("pop." + Program.MailConfiguration.ServerName, Program.MailConfiguration.Pop3Port, true);
                    client.Authenticate(Program.MailConfiguration.Email, Program.MailConfiguration.Password);

                    int start = client.GetMessageCount();
                    int finish = Math.Max(0, start - count);

                    for (int i = start - 1; i >= finish; i--)
                    {
                        var message = client.GetMessage(i);

                        var mail = new Mail
                        {
                            SenderName = message.From.Mailboxes.FirstOrDefault()?.Name,
                            SenderEmail = message.From.Mailboxes.FirstOrDefault()?.Address,
                            RecipientName = message.To.Mailboxes.FirstOrDefault()?.Name,
                            RecipientEmail = message.To.Mailboxes.FirstOrDefault()?.Address,
                            Subject = message.Subject,
                            Body = message.TextBody,
                            AttachmenMime = new List<MimePart>()
                        };

                        // Save attachments
                        foreach (var attachment in message.Attachments.OfType<MimePart>())
                        {
                            mail.AttachmenMime.Add(attachment);

                            // Save the attachment to disk
                            var sanitizedSender = SanitizeFileName(mail.SenderEmail);
                            var sanitizedSubject = SanitizeFileName(mail.Subject);
                            var sanitizedReceiver = SanitizeFileName(mail.RecipientEmail);

                            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "DownloadedMails", sanitizedReceiver, $"{sanitizedSender}[{sanitizedSubject}]");

                            // Create the directory if it doesn't exist
                            Directory.CreateDirectory(folderPath);

                            var attachmentFileName = SanitizeFileName(attachment.FileName);
                            var attachmentSavePath = Path.Combine(folderPath, "Attachments", attachmentFileName);

                            using (var stream = File.Create(attachmentSavePath))
                            {
                                attachment.Content.DecodeTo(stream);
                            }
                        }

                        /// Save the email message as .eml file
                        var emlSanitizedSender = SanitizeFileName(mail.SenderEmail);
                        var emlSanitizedSubject = SanitizeFileName(mail.Subject);
                        var emlSanitizedReceiver = SanitizeFileName(mail.RecipientEmail);

                        var emlFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "DownloadedMails", emlSanitizedReceiver, $"{emlSanitizedSender}[{emlSanitizedSubject}]");

                        // Create the directory if it doesn't exist
                        Directory.CreateDirectory(emlFolderPath);

                        var emlSavePath = Path.Combine(emlFolderPath, $"{emlSanitizedSender}[{emlSanitizedSubject}].eml");

                        using (var stream = File.Create(emlSavePath))
                        {
                            message.WriteTo(stream);
                        }

                        mails.Add(mail);
                    }

                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    client.Disconnect(true);
                    return null;
                }
            }

            return mails;
        }


        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var sanitizedFileName = string.Join("_", fileName.Split(invalidChars.ToCharArray()));
            return sanitizedFileName;
        }
    }
}
