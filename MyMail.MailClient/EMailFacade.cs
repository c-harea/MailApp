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
        private GMailClient _gClientStrategy;
        private Protocol _lastClient;

        public EmailFacade()
        {
            _smtpClient = new SmtpClient();
            _pop3Client = new Pop3Client();
            _imapClient = new ImapClient();
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
            var factory = new MailClientFactory();

            if (protocol == Protocol.Pop3)
            {                           
                _gClientStrategy = factory.CreateMailClient(_pop3Client);
                _lastClient = Protocol.Pop3;
                return _gClientStrategy.GetNextMails(count);
            }
            else
            {
                _gClientStrategy = factory.CreateMailClient(_imapClient);
                _lastClient = Protocol.Imap;
                return _gClientStrategy.GetNextMails(count);
            }
        }

        public List<Mail> GetMailPage(int page, int pageSize, Protocol protocol)
        {
            var factory = new MailClientFactory();

            if (protocol == Protocol.Pop3)
            {
                _gClientStrategy = factory.CreateMailClient(_pop3Client);
                _lastClient = Protocol.Pop3;
                return _gClientStrategy.GetMailPage(page, pageSize);
            }
            else
            {
                _gClientStrategy = factory.CreateMailClient(_imapClient);
                _lastClient = Protocol.Imap;
                return _gClientStrategy.GetMailPage(page, pageSize);
            }
        }

        public Mail GetMail(int id)
        {
            var factory = new MailClientFactory();

            if (_lastClient == Protocol.Pop3)
            {
                _gClientStrategy = factory.CreateMailClient(_pop3Client);
                return _gClientStrategy.GetMail(id);

            }
            else
            {
                _gClientStrategy = factory.CreateMailClient(_imapClient);
                return _gClientStrategy.GetMail(id);
            }
        }

        public Response DownloadMail(int id)
        {
            var factory = new MailClientFactory();

            if (_lastClient == Protocol.Pop3)
            {
                _gClientStrategy = factory.CreateMailClient(_pop3Client);
                return _gClientStrategy.DownloadMail(id);

            }
            else
            {
                _gClientStrategy = factory.CreateMailClient(_imapClient);
                return _gClientStrategy.DownloadMail(id);
            }
        }
    }

}
