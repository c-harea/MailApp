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
        private static EmailFacade client = new EmailFacade();

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
            Mail mail = SendMailModel.ToMail(model);

            var response = client.Send(mail);
            if(response.Status == false)
            {
                TempData["error"] = response.Message;
                return RedirectToAction("SendMail");
            }


            return View("Mail");
        }

        public IActionResult GetImap()
        {

           var mails = client.GetNextMails(10, Protocol.Imap);
            
            foreach (var mail in mails)
            {
                _imapMails.Add(DownloadedMail.FromMail(mail));
            }

            return View(_imapMails);
        }
        
        public IActionResult DownloadMail(int id)
        {
            
            var response = client.DownloadMail(id);
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
            var mails = client.GetNextMails(10, Protocol.Pop3);

            foreach (var mail in mails)
            {
                _pop3Mails.Add(DownloadedMail.FromMail(mail));
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

            Response response = client.Authenticate(user);
            if (response.Status == false)
            {
                TempData["error"] = response.Message;
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

            Response response = client.Connect(server);
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

            var message = client.GetMail(id);
            DownloadedMail mail = DownloadedMail.FromMail(message);

            return View(mail);
        }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}