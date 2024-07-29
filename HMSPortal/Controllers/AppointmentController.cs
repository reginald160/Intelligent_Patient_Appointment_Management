using AutoMapper;
using HMS.Infrastructure.DataBank;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Attributes;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Application.Core.MessageBrocker.KafkaBus;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMSPortal.Controllers
{
	[Authorize]
	public class AppointmentController : Controller
	{
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppointmentServices _appointmentServices;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        public  string Patient = Roles.Patient.ToString();
		private readonly UserManager<ApplicationUser> _userManager;

        



		public AppointmentController(IWebHostEnvironment webHostEnvironment,
			IAppointmentServices appointmentServices, IMapper mapper, ICacheService cacheService,
            UserManager<ApplicationUser> userManager)
		{
			_webHostEnvironment=webHostEnvironment;
			_appointmentServices=appointmentServices;
			_mapper=mapper;
			_cacheService=cacheService;
			_userManager=userManager;
		}
		[Authorize]
        public async Task<IActionResult> Index()
        {
            var response =  await _appointmentServices.GetAllAppointment();
            var apponitments = response.Data as List<AllAppointmentViewModel>;
			//var apponitments = AppointmentBank.GenerateRandomAppointments(50);

			return View(apponitments);
        }

		[Authorize]
		public async Task<IActionResult> All()
		{

            var apponitments = AppointmentBank.GenerateRandomAppointments(50);

			return View(apponitments);
		}
		[Authorize(Roles = $"SuperAdmin, Admin")]
		public async Task<IActionResult> Add()
		{
            var (patients, doctors) = await _appointmentServices.GetPatientAndDoctor();
            var appointment = new AddAppointmentViewModel
            {
                Patients = patients,
                Doctors = doctors
            };
            return View(appointment);
		}
		[HttpPost]
		public async Task<IActionResult> Add(AddAppointmentViewModel viewModel)
		{
			await _appointmentServices.CreateAppointmentByAdmin(viewModel);
			TempData["Success"] = "Patient record has been created successfully.";
			return RedirectToAction(nameof(Index),"Appointment");
		}

		[Authorize]
		public async Task<IActionResult> Reschedule(string Id)
		{
			
			var appointment = new AddAppointmentViewModel
			{
				AppointmentType = Id,
			};
			return View(appointment);
		}
		[Authorize]
		public async Task<IActionResult> View(string Id)
		{

			var appointment = new AddAppointmentViewModel
			{
				AppointmentType = Id,
			};
			return View(appointment);
		}

		[ValidateAntiForgeryToken]
		[Authorize]
        [HttpPost]
		public async Task<IActionResult> Reschedule(AddAppointmentViewModel model)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			var roles =  await _userManager.GetRolesAsync(currentUser);
			var schedule = new AddAppointmentViewModel
			{
				TimeSlot = model.TimeSlot,
				Date = model.Date,
			

			};

            await _appointmentServices.RescheduleAppointmentByPatient(schedule, model.AppointmentType);
            if(roles.Contains(RoleNames.Patient))
            {
				return RedirectToAction("MyAppointments", "Appointment");
            }
            else
            {
				return RedirectToAction("Index", "Appointment");

			}

		}

		[Authorize(Roles = RoleNames.Patient)]
		public async Task<IActionResult> PatientAppointment()
        {
            //var userId = _cacheService.GetCachedUser().Id;
            var model = new BotMessage
            {
                Messages =  await _appointmentServices.GetRecentMessagesAsync(100),
                //UserId =  userId
            };

            return View(model);
        }
        //public IActionResult All(int userId)
        //{
        //    return View();
        //}


		[Authorize(Roles = RoleNames.Patient)]
		//[CustomAuthorize("Patient")]
		public async Task<IActionResult> MyAppointments()
        {
			var response = await _appointmentServices.GetAllAppointmentByUser(User.GetUserId());
			var apponitments = response.Data as List<AllAppointmentViewModel>;
			return View(apponitments);
        }
       


        [HttpGet]
        public JsonResult GetAvailableTimeSlots(string date)
        {
            // Logic to fetch available time slots based on the date.
            // This is just a sample. Replace it with your actual logic.
            var dateObject = DateTime.Parse(date);
            var timeSlots = _appointmentServices.GetAvailableSlotsForDateToString(dateObject);

            return Json(timeSlots);
        }

        [NonAction]
        public List<BotMessageViewModel> GenerateFakeMessages(int count)
        {
            List<BotMessageViewModel> messages = new List<BotMessageViewModel>();
            Random random = new Random();

            string[] senders = { "User", "Bot", "Admin" };
            string[] contents = { "Hello!", "How are you?", "What's up?", "Nice to meet you.", "Goodbye!" };

            for (int i = 1; i <= count; i++)
            {
                BotMessageViewModel message = new BotMessageViewModel
                {
                
                    Sender = senders[random.Next(senders.Length)], // Random sender from the senders array
                    Content = contents[random.Next(contents.Length)], // Random content from the contents array
                    SentAt = DateTime.Now.AddDays(-random.Next(1, 30)) // Random date within the last 30 days
                };
                messages.Add(message);
            }

            return messages;
        }

		[HttpDelete]

		public async Task<IActionResult> Cancel(string id)
		{
			
			var response = await _appointmentServices.CancelAppointmentById(id);
			
			if (response.IsSuccessful)
			{
				return Json(new { success = true, message = response });

			}
			else
			{
				return Json(new { success = false });
			}

		}
	}

	public static class RoleNames
	{
		public const string Patient = "Patient";
	}
}
