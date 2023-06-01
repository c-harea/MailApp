using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailClient;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MyMail.MailClient.Entities;

namespace MyMail.MailClient
{
    public class EmailFacade
    {

        private SmtpClient _smtpClient;
        private Pop3Client _pop3Client;
        private ImapClient _imapClient;
        private GMailClient _imapGClient;
        private GMailClient _pop3GClient;
        private Protocol _lastClient;

        public EmailFacade()
        {
            _smtpClient = new SmtpClient();
            _pop3Client = new Pop3Client();
            _imapClient = new ImapClient();

            var factory = new MailClientFactory();
            _imapGClient = factory.CreateMailClient(_imapClient);
            _pop3GClient = factory.CreateMailClient(_pop3Client);
        }

        public Response Connect(Server server)
        {
            ServerConnect.Init(server);
            return ServerConnect.Check();
        }

        public Response Authenticate(User user)
        {
            UserConnect.Init(user);
            return UserConnect.Check();
        }

        public Response Send(Mail mail)
        {
            return MySmtpClient.Send(mail);
        }

        public List<Mail> GetNextMails(int count, Protocol protocol)
        {
            if(protocol == Protocol.Pop3)
            {
                _lastClient = Protocol.Pop3;
                return _pop3GClient.GetNextMails(count);
            }
            else
            {
                _lastClient = Protocol.Imap;
                return _imapGClient.GetNextMails(count);
            }
        }

        public List<Mail> GetMailPage(int page, int pageSize, Protocol protocol)
        {
            if (protocol == Protocol.Pop3)
            {
                _lastClient = Protocol.Pop3;
                return _pop3GClient.GetMailPage(page, pageSize);
            }
            else
            {
                _lastClient = Protocol.Imap;
                return _imapGClient.GetMailPage(page, pageSize);
            }
        }

        public Mail GetMail(int id)
        {
            if(_lastClient == Protocol.Pop3)
            {
                return _pop3GClient.GetMail(id);

            }
            else
            {
                return _imapGClient.GetMail(id);
            }
        }

        public Response DownloadMail(int id)
        {
            if (_lastClient == Protocol.Pop3)
            {
                return _pop3GClient.DownloadMail(id);

            }
            else
            {
                return _imapGClient.DownloadMail(id);
            }
        }
    }

}
