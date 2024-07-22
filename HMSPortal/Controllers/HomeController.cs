using HMSPortal.Application.Core.Chat.Message;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HMSPortal.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			var sssss = SystemContent.GetSymptonFilterpath();

            return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


    }
}
