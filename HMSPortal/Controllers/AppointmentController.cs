using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
	public class AppointmentController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
