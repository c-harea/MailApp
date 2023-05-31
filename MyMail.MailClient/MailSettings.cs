using MyMail.MailClient.Entities;

namespace MailClient
{
    public class MailSettings
    {
        public string ServerName { get; private set; }
        public int SmtpPort { get; private set; }
        public int Pop3Port { get; private set; }
        public int ImapPort { get; private set; }
        public string Alias { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        private static MailSettings _instance;

        private MailSettings()
        {
        }

        public void SetServer(Server server)
        {
            ServerName = server.ServerName;
            SmtpPort = server.SmtpPort;
            Pop3Port = server.Pop3Port;
            ImapPort = server.ImapPort;
        }

        public void SetUser(User user)
        {
            Alias = user.Alias;
            Email = user.Email;
            Password = user.Password;
        }

        public static MailSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MailSettings();
                }
                return _instance;
            }
        }
    }
}
