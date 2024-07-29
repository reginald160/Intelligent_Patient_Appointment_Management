using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.Core.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;
using HMSPortal.Application.ViewModels.Dashboard;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Identity;
using HMSPortal.Domain.Enums;
using HMSPortal.Application.ViewModels;
using HMS.Infrastructure.DataBank;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.ViewModels.Appointment;

namespace HMSPortal.Controllers
{
	
	public class DashboardController : Controller
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ApplicationDbContext _dbContext;
		private const string PatientCountCacheKey = "PatientCount";
		private readonly ICacheService _cacheService;
        private  readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentServices _appointmentServices;


        public DashboardController(IMemoryCache memoryCache, ApplicationDbContext dbContext, ICacheService cacheService, UserManager<ApplicationUser> userManager, IAppointmentServices appointmentServices)
        {
            _memoryCache = memoryCache;
            _dbContext = dbContext;
            _cacheService = cacheService;
            _userManager = userManager;
            _appointmentServices = appointmentServices;
        }

        //[Authorize(Roles = "Administrator")]
        [Authorize]
        public async Task<IActionResult> Index()
		{
            //await Email();
            var currentUser = await _userManager.GetUserAsync(User);
            var role = await _userManager.GetRolesAsync(currentUser);
            var dashboard = new DashBoardViewModel();

			if (role.Contains("Admin") || role.Contains("SuperAdmin"))
            {
                ViewBag.PatialView = "_AdminMiniPartial";
                ViewBag.PatientCount = _cacheService.GetPatientCount();
				ViewBag.AppointmentCount = _cacheService.GetAppointmentCount();
				ViewBag.DoctorCount = _cacheService.GetDoctorCount();
                var response = await _appointmentServices.GetAllAppointment();
                var apponitments = response.Data as List<AllAppointmentViewModel>;
                dashboard.AllAppointments = apponitments; //AppointmentBank.GenerateRandomAppointments(50);
				return View(dashboard);
            }
            else if (role.Contains("Patient"))
            {
				ViewBag.PatialView = "_AdminMiniPartial";
				ViewBag.PatientCount = _cacheService.GetPatientCount();
				ViewBag.AppointmentCount = _cacheService.GetAppointmentCount();
				ViewBag.DoctorCount = _cacheService.GetDoctorCount();
				return View();
            }
            else
            {
				ViewBag.PatialView = "_AdminMiniPartial";
				ViewBag.PatientCount = _cacheService.GetPatientCount();
				ViewBag.AppointmentCount = _cacheService.GetAppointmentCount();
				ViewBag.DoctorCount = _cacheService.GetDoctorCount();
				return View();
            }



           
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
