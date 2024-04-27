using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
