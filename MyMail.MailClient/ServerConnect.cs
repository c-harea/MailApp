using System;
using System.IO;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MyMail.MailClient.Entities;

namespace MailClient
{
    public static class ServerConnect
    {

        public static void Init(Server server)
        {
            Program.MailConfiguration.ServerName = server.ServerName;
            Program.MailConfiguration.SmtpPort = server.SmtpPort;
            Program.MailConfiguration.Pop3Port = server.Pop3Port;
            Program.MailConfiguration.ImapPort = server.ImapPort;
        }

        public static bool Check()
        {
            // Try connecting to the SMTP, POP3, and IMAP servers
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect("smtp." + Program.MailConfiguration.ServerName, Program.MailConfiguration.SmtpPort, SecureSocketOptions.StartTls);
                }

                using (var pop3Client = new Pop3Client())
                {
                    pop3Client.Connect("pop." + Program.MailConfiguration.ServerName, Program.MailConfiguration.Pop3Port, SecureSocketOptions.Auto);
                }

                using (var imapClient = new ImapClient())
                {
                    imapClient.Connect("imap." + Program.MailConfiguration.ServerName, Program.MailConfiguration.ImapPort, SecureSocketOptions.SslOnConnect);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        
    }
}

