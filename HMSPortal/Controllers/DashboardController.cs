using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.Core.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;
using HMSPortal.Application.ViewModels.Dashboard;

namespace HMSPortal.Controllers
{
	
	public class DashboardController : Controller
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ApplicationDbContext _dbContext;
		private const string PatientCountCacheKey = "PatientCount";
		private readonly ICacheService _cacheService;


        public DashboardController(IMemoryCache memoryCache, ApplicationDbContext dbContext, ICacheService cacheService)
        {
            _memoryCache=memoryCache;
            _dbContext=dbContext;
            _cacheService=cacheService;
        }

		//[Authorize(Roles = "Administrator")]
        [Authorize]
        public async Task<IActionResult> Index()
		{
            //await Email();


            ViewBag.PatientCount = _cacheService.GetPatientCount();
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

        public IActionResult Message(string title, string message,  string action, string controller, string button)
        {
            var model = new MessageViewModel
            {
                Title = title,
                Message = message,
            
                Controller = controller,
                Action = action,
                ButtonTitle = button
            };
            return View(model);
        }
        //[HttpPost]
        //public IActionResult Message(MessageViewModel model)
        //{
        //    return RedirectToAction(actionName: model.Action , model.Controller);
        //}
        public static async Task Email()
        {

            var key = "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";
            using var message = new MailMessage();
            message.From = new MailAddress("reginald.ozougwu@yorksj.ac.uk", "Ifeanyi");

            message.IsBodyHtml = true;
            message.To.Add(new MailAddress("reginald1149@gmail.com", "Easy Medicare"));
            message.Body = "hello";

            using var client = new SmtpClient(host: "smtp.sendgrid.net", port: 587);
            client.Credentials = new NetworkCredential(
                userName: "apikey",
                password: key
                );


            client.Send(message);

        }

    }
}
