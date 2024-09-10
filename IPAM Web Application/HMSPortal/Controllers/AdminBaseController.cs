using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
    public class AdminBaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
