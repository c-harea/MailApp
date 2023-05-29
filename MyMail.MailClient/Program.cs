namespace MailClient
{
    class Program
    {
        public static class MailConfiguration
        {
            public static string ServerName { get; set; } 
            public static int SmtpPort { get; set; } 
            public static int Pop3Port { get; set; } 
            public static int ImapPort { get; set; }
            public static string Alias { get; set; }
            public static string Email { get; set; }
            public static string Password { get; set; } 

        }
    }
}
