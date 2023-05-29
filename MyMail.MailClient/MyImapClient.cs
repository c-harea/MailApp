using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailClient;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;

namespace MyMail.MailClient
{
    public static class MyImapClient
    {
        private static int start;
        private static int finish;

        public static bool Initialize()
        {
            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap." + Program.MailConfiguration.ServerName, Program.MailConfiguration.ImapPort, SecureSocketOptions.SslOnConnect);
                    client.Authenticate(Program.MailConfiguration.Username, Program.MailConfiguration.Password);
                    client.Inbox.Open(FolderAccess.ReadOnly);

                    finish = client.Inbox.Count;
                    start = client.Inbox.Count;
                    client.Disconnect(true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static List<Mail> GetNextMails(int count)
        {
            var mails = new List<Mail>();

            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap." + Program.MailConfiguration.ServerName, Program.MailConfiguration.ImapPort, SecureSocketOptions.SslOnConnect);
                    client.Authenticate(Program.MailConfiguration.Username, Program.MailConfiguration.Password);
                    client.Inbox.Open(FolderAccess.ReadOnly);

                    finish -= count;

                    for (int i = start - 1; i >= finish; i--)
                    {
                        var message = client.Inbox.GetMessage(i);

                        var mail = new Mail
                        {
                            Id = i,
                            SenderName = message.From.FirstOrDefault()?.Name,
                            SenderEmail = message.From.FirstOrDefault()?.ToString(),
                            RecipientName = message.To.FirstOrDefault()?.Name,
                            RecipientEmail = message.To.FirstOrDefault()?.ToString(),
                            Subject = message.Subject,
                            Body = message.TextBody,
                            AttachmenMime = message.Attachments.OfType<MimePart>().ToList()
                        };

                        mails.Add(mail);
                    }
                    start -= count;

                    client.Disconnect(true);

                    return mails;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static bool DownloadMail(int index)
        {
            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap." + Program.MailConfiguration.ServerName, Program.MailConfiguration.ImapPort, SecureSocketOptions.SslOnConnect);
                    client.Authenticate(Program.MailConfiguration.Username, Program.MailConfiguration.Password);
                    client.Inbox.Open(FolderAccess.ReadOnly);

                    var message = client.Inbox.GetMessage(index);

                    var senderEmail = message.From.FirstOrDefault()?.ToString();
                    var subject = message.Subject;

                    // Sanitize sender and subject to remove invalid characters
                    var sanitizedSender = SanitizeFileName(senderEmail);
                    var sanitizedSubject = SanitizeFileName(subject);

                    var receiverEmail = message.To.FirstOrDefault()?.ToString();
                    var sanitizedReceiver = SanitizeFileName(receiverEmail);

                    var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "DownloadedMails", sanitizedReceiver, $"{sanitizedSender}[{sanitizedSubject}]");

                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(folderPath);

                    var savePath = Path.Combine(folderPath, $"{sanitizedSender}[{sanitizedSubject}].eml");

                    using (var stream = File.Create(savePath))
                    {
                        message.WriteTo(stream);
                    }

                    // Download attachments
                    var attachmentsFolder = Path.Combine(folderPath, "Attachments");
                    Directory.CreateDirectory(attachmentsFolder);

                    foreach (var attachment in message.Attachments)
                    {
                        var attachmentFileName = SanitizeFileName(attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name);
                        var attachmentSavePath = Path.Combine(attachmentsFolder, attachmentFileName);

                        using (var stream = File.Create(attachmentSavePath))
                        {
                            if (attachment is MimePart mimePart)
                                mimePart.Content.DecodeTo(stream);
                        }
                    }

                    client.Disconnect(true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var sanitizedFileName = string.Join("_", fileName.Split(invalidChars.ToCharArray()));
            return sanitizedFileName;
        }

    }
}
