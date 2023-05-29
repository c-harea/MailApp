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
                SenderName = model.SenderName,
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
           MyImapClient.Initialize();

           var mails = MyImapClient.GetNextMails(4);
           var downloadedMails = new List<DownloadedMail>();

            foreach (var mail in mails)
            {
                downloadedMails.Add(new DownloadedMail
                {
                    Id = mail.Id,
                    Subject = mail.Subject,
                    SenderName= mail.SenderName,
                    SenderEmail= mail.SenderEmail,
                });
            }

            return View(downloadedMails);
        }
        
        public IActionResult DownloadMail(int id)
        {
            

            if (MyImapClient.DownloadMail(id) == false)
            {
                TempData["error"] = "Failed to download mail!";
                return RedirectToAction("Login");
            }

            else
            {
                TempData["success"] = "Success!";

            }

            return RedirectToAction("Mail");
        }


        public IActionResult GetPop3()
        {
            var mails = MyPop3Client.DownloadMails(4);
            var downloadedMails = new List<DownloadedMail>();

            foreach (var mail in mails)
            {
                downloadedMails.Add(new DownloadedMail
                {
                    Subject = mail.Subject,
                    SenderName = mail.SenderName,
                    SenderEmail = mail.SenderEmail,
                });
            }

            return View(downloadedMails);
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
                Email= model.Email,
                Password= model.Password,
            };

            UserConnect.Init(user);

            if (UserConnect.Check() == false)
            {
                TempData["error"] = "Failed to login!";
                return RedirectToAction("Login");
            }

            else
            {
                TempData["success"] = "Success!";
                
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

            if (ServerConnect.Check() == false)
            {
                TempData["error"] = "Failed to connect!";
                return View("Server");
            }

            else
            {
                TempData["success"] = "Success!";
            }

            return View("Login");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}