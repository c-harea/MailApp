using System;
using System.IO;
using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;

namespace MailClient
{
    class ImapOptions
    {
        public static void Execute()
        {
            const int SIZE_OF_PAGE = 10;
            byte option;

            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap." + Program.Configuration.ServerName, Program.Configuration.ImapPort, SecureSocketOptions.SslOnConnect);
                    client.Authenticate(Program.Configuration.Username, Program.Configuration.Password);
                    client.Inbox.Open(FolderAccess.ReadOnly);

                    int messageCount = client.Inbox.Count;
                    int start = Math.Max(0, messageCount - SIZE_OF_PAGE);

                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Recent messages:");
                        for (int i = messageCount - 1; i >= start; i--)
                        {
                            var message = client.Inbox.GetMessage(i);
                            Console.WriteLine("[{0}] {1} [{2}]", i, message.Subject, message.Date.ToString("dd.MM.yyyy HH:mm:ss"));
                        }

                        Console.WriteLine("\nMenu options:");
                        Console.WriteLine("1: Download");
                        Console.WriteLine("2: Open");
                        Console.WriteLine("3: Reply");
                        Console.WriteLine("4: Next 10 items");
                        Console.WriteLine("5: Exit");

                        Console.Write("\nOption: ");
                        option = byte.Parse(Console.ReadLine());
                        if (option == 5)
                        {
                            break;
                        }
                        else if (option == 1 || option == 2 || option == 3)
                        {
                            Console.Write("Mail number: ");
                            int mailNumber;
                            if (int.TryParse(Console.ReadLine(), out mailNumber) && mailNumber >= start && mailNumber < messageCount)
                            {
                                var message = client.Inbox.GetMessage(mailNumber);

                                switch (option)
                                {
                                    case 1:
                                        Console.WriteLine("Downloading...");
                                        DownloadMail.Run(client, mailNumber);
                                        break;
                                    case 2:
                                        Console.WriteLine("Opening...");
                                        OpenMail.Run(client, mailNumber);
                                        break;
                                    case 3:
                                        Console.WriteLine("Replying...");
                                        ReplyToEmail.Run(client, mailNumber);
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid mail number.");
                            }
                        }
                        else if (option == 4)
                        {
                            start = Math.Max(0, start - SIZE_OF_PAGE);
                            messageCount = Math.Max(0, messageCount - SIZE_OF_PAGE);
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error! Couldn't retrieve messages: {0}", ex.Message);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
