using System;

namespace MailClient
{
    class MainMenu
    {
        public static void Execute()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Mail Client Menu");
                Console.WriteLine("---------------------");
                Console.Write("Select an option: ");
                Console.WriteLine("---------------------");
                Console.WriteLine("1. Get mails with IMAP");
                Console.WriteLine("2. Get mails with POP3");
                Console.WriteLine("3. Send mail");
                Console.WriteLine("4. Exit program");
                

                byte option = byte.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        ImapOptions.Execute();
                        break;
                    case 2:
                        Pop3Options.Execute();
                        break;
                    case 3:
                        SendMailOptions.Execute();
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            }
        }
    }
}
