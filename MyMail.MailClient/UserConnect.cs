using System;
using System.IO;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MyMail.MailClient;
using MyMail.MailClient.Entities;

namespace MailClient
{
    public static class UserConnect
    {
        static private Handler _hander;
        private static SmtpClient _smtpClient;
        private static Pop3Client _pop3Client;
        private static ImapClient _imapClient;
        public static void Init(User user)
        {
            Program.MailConfiguration.Alias = user.Alias;
            Program.MailConfiguration.Email = user.Email;
            Program.MailConfiguration.Password = user.Password;

            _smtpClient = new SmtpClient();
            _pop3Client = new Pop3Client();
            _imapClient = new ImapClient();

            // Create Connect chain
            Handler smtpConnect = new SmtpConnect(_smtpClient);
            Handler pop3Connect = new Pop3Connect(_pop3Client);
            Handler imapConnect = new ImapConnect(_imapClient);

            // Create Login chain
            Handler smtpLogin = new SmtpLogin(_smtpClient);
            Handler pop3Login = new Pop3Login(_pop3Client);
            Handler imapLogin = new ImapLogin(_imapClient);

            // Set chain order for Connect
            smtpConnect.SetNextHandler(smtpLogin);
            smtpLogin.SetNextHandler(pop3Connect);
            pop3Connect.SetNextHandler(pop3Login);
            pop3Login.SetNextHandler(imapConnect);
            imapConnect.SetNextHandler(imapLogin);

            _hander = smtpConnect;
        }

        public static Response Check()
        {
            return _hander.HandleRequest();
        }
    }
}