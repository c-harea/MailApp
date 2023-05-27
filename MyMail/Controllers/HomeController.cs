using Microsoft.AspNetCore.Mvc;
using MyMail.Models;
using System.Diagnostics;

namespace MyMail.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
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
        public IActionResult SendMail(MailModel model)
        {
            return View("Mail");
        }

        public IActionResult GetImap()
        {
            return View();
        }

        public IActionResult GetPop3()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		public IActionResult LogUser(UserModel model)
		{
			return View("Mail");
		}

		public IActionResult Server()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Server(ServerModel model)
		{
			return View("Login");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}