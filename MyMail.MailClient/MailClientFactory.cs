using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;

namespace MyMail.MailClient
{
    public class MailClientFactory
    {
        public GMailClient CreateMailClient(string clientType)
        {
            if (clientType == "imap")
            {
                var imapClient = new ImapClient();
                var imapClientAdapter =  new ImapClientAdapter(imapClient);
                return new GMailClient(imapClientAdapter);
            }
            else if (clientType == "pop3")
            {
                var pop3Client = new Pop3Client();
                var pop3ClientAdapter = new Pop3ClientAdapter(pop3Client);
                return new GMailClient(pop3ClientAdapter);
            }
            else
            {
                throw new ArgumentException("Invalid client type");
            }
        }
    }
}
