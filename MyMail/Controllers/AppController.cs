using Microsoft.AspNetCore.Mvc;
using MyMail.Models;
using System.Diagnostics;
using MyMail.MailClient;
using MyMail.MailClient.Entities;
using MailClient;
using MimeKit;
using System;
using System.IO;

namespace MyMail.Controllers
{
    public class AppController : Controller
    {
        private readonly ILogger<AppController> _logger;
        private static GMailClient _imapClient;
        private static GMailClient _pop3Client;

        private static List<DownloadedMail> _imapMails = new List<DownloadedMail>();
        private static List<DownloadedMail> _pop3Mails = new List<DownloadedMail>();

        public AppController(ILogger<AppController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Mail()
        {
            return View();
        }

        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMail(SendMailModel model)
        {
            Mail mail = new Mail()
            {
                RecipientName = model.RecipientName,
                RecipientEmail = model.RecipientEmail,
                Subject = model.Subject,
                Body = model.Body,
                AttachmentPaths = new List<string>()
            };

            foreach (var attachment in model.Attachment)
            {
                string fileName = Path.GetFileName(attachment.FileName);
                string filePath = Path.Combine("D:\\Docs\\UTM Folder\\Anul 3\\TMPS\\Proiect Curs\\MyMail\\temp\\", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    attachment.CopyTo(stream);
                    mail.AttachmentPaths.Add(filePath);
                }
  
            }
            
            if(MySmtpClient.Send(mail) == false)
            {
                TempData["error"] = "Failed to send!";
                return RedirectToAction("SendMail");
            }


            return View("Mail");
        }

        public IActionResult GetImap()
        {

           var mails = _imapClient.GetNextMails(10);
            
            foreach (var mail in mails)
            {
                _imapMails.Add(new DownloadedMail
                {
                    Id = mail.Id,
                    Subject = mail.Subject,
                    SenderName= mail.SenderName,
                    SenderEmail= mail.SenderEmail,
                });
            }

            return View(_imapMails);
        }
        
        public IActionResult DownloadMail(int id)
        {
            
            var response = GMailClient.DownloadMail(id);
            if (response.Status == false)
            {
                TempData["error"] = response.Message;
            }

            else
            {
                TempData["success"] = "Success!";
            }

            return RedirectToAction("Mail");
        }


        public IActionResult GetPop3()
        {
            var mails = _pop3Client.GetNextMails(10);

            foreach (var mail in mails)
            {
                _pop3Mails.Add(new DownloadedMail
                {
                    Id=mail.Id,
                    Subject = mail.Subject,
                    SenderName = mail.SenderName,
                    SenderEmail = mail.SenderEmail,
                });
            }

            return View(_pop3Mails);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		public IActionResult LogUser(UserModel model)
		{
            User user = new User()
            {
                Email = model.Email,
                Password = model.Password,
                Alias = model.Alias
            };

            UserConnect.Init(user);

            Response response = UserConnect.Check();
            if (response.Status == false)
            {
                TempData["error"] = response.Message;
                return RedirectToAction("Login");
            }

            else
            {
                TempData["success"] = "Success!";
                var factory = new MailClientFactory();
                _imapClient = factory.CreateMailClient("imap");
                _pop3Client = factory.CreateMailClient("pop3");
            }

            return RedirectToAction("Mail");
		}

		public IActionResult Server()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Server(ServerModel model)
		{
            if (model.ServerName == null)
            {
                model.ServerName = "gmail.com";
            }

            Server server = new Server()
            {
                ServerName = model.ServerName,
                SmtpPort = model.SmtpPort,
                ImapPort = model.ImapPort,
                Pop3Port = model.Pop3Port,
            };

            ServerConnect.Init(server);

            Response response = ServerConnect.Check();
            if (response.Status == false)
            {
                TempData["error"] = response.Message;
                return View("Server");
            }

            else
            {
                TempData["success"] = "Success!";
            }

            return View("Login");
		}

        public IActionResult ViewMail(int id) {

            var message = GMailClient.GetMail(id);
            DownloadedMail mail = new DownloadedMail
            {
                SenderEmail = message.SenderEmail,
                SenderName = message.SenderName,
                Body = message.Body,
                Subject = message.Subject
            };

            return View(mail);
        }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}