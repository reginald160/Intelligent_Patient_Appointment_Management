using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Add()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Add()
        //{
        //    return View();
        //}

    }
}
