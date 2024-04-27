using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Adds()
        {
            return View();
        }
    }
}
