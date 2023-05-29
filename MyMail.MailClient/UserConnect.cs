using System;
using System.IO;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MyMail.MailClient.Entities;

namespace MailClient
{
    public static class UserConnect
    {
        public static void Init(User user)
        {
            Program.MailConfiguration.Alias = user.Alias;
            Program.MailConfiguration.Email = user.Email;
            Program.MailConfiguration.Password = user.Password;
        }

        public static bool Check()
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    // SMTP
                    smtp.Connect("smtp." + Program.MailConfiguration.ServerName, Program.MailConfiguration.SmtpPort, SecureSocketOptions.Auto);
                    smtp.Authenticate(Program.MailConfiguration.Email, Program.MailConfiguration.Password);
                }

                using (var pop3 = new Pop3Client())
                {
                    // POP3
                    pop3.Connect("pop." + Program.MailConfiguration.ServerName, Program.MailConfiguration.Pop3Port, SecureSocketOptions.Auto);
                    pop3.Authenticate(Program.MailConfiguration.Email, Program.MailConfiguration.Password);
                }

                using (var imap = new ImapClient())
                {
                    // IMAP
                    imap.Connect("imap." + Program.MailConfiguration.ServerName, Program.MailConfiguration.ImapPort, SecureSocketOptions.Auto);
                    imap.Authenticate(Program.MailConfiguration.Email, Program.MailConfiguration.Password);
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