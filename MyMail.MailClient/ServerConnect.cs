using System;
using System.IO;
using MailClient;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MyMail.MailClient.Entities;

namespace MyMail.MailClient
{
    public static class ServerConnect
    {
        private static IRequestHandler _handlerChain;

        public static void Init(Server server)
        {
            Program.MailConfiguration.ServerName = server.ServerName;
            Program.MailConfiguration.SmtpPort = server.SmtpPort;
            Program.MailConfiguration.Pop3Port = server.Pop3Port;
            Program.MailConfiguration.ImapPort = server.ImapPort;
        }

        public static bool Check()
        {
            // Create the handler chain
            BuildHandlerChain();

            // Create a request object
            var request = new ServerConnectRequest();

            // Handle the request using the chain of handlers
            _handlerChain.HandleRequest(request);

            return request.IsConnected;
        }

        private static void BuildHandlerChain()
        {
            // Create the handlers
            var smtpHandler = new SmtpHandler();
            var pop3Handler = new Pop3Handler();
            var imapHandler = new ImapHandler();

            // Build the chain
            smtpHandler.SetNext(pop3Handler);
            pop3Handler.SetNext(imapHandler);

            // Set the chain as the handler chain
            _handlerChain = smtpHandler;
        }
    }
}


