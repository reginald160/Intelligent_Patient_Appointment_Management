using AutoMapper;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Application.Core.MessageBrocker.KafkaBus;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
	public class AppointmentController : Controller
	{
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAppointmentServices _appointmentServices;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;


       
        public AppointmentController(IWebHostEnvironment webHostEnvironment,
            IAppointmentServices appointmentServices, IMapper mapper, ICacheService cacheService)
        {
            _webHostEnvironment=webHostEnvironment;
            _appointmentServices=appointmentServices;
            _mapper=mapper;
            _cacheService=cacheService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var response =  await _appointmentServices.GetAllAppointment();
            var apponitments = response.Data as List<AllAppointmentViewModel>;

            return View(apponitments);
        }
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
        public async Task<IActionResult> BotAppointment()
        {
            //var userId = _cacheService.GetCachedUser().Id;
            var model = new BotMessage
            {
                Messages =  await _appointmentServices.GetRecentMessagesAsync(100),
                //UserId =  userId
            };

            return View(model);
        }

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

        [Authorize]
        public async Task<IActionResult> SelfAppointment()
        {
            //var userId = _cacheService.GetCachedUser().Id;
            var model = new BotMessage
            {
                Messages =  await _appointmentServices.GetRecentMessagesAsync(100),
                //UserId =  userId
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> SelfBotAppointment()
        {
            //var userId = _cacheService.GetCachedUser().Id;
            var model = new BotMessage
            {
                Messages =  await _appointmentServices.GetRecentMessagesAsync(100),
                //UserId =  userId
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Add(AddAppointmentViewModel viewModel)
        {
            await _appointmentServices.CreateAppointmentByAdmin(viewModel);
            return View(viewModel);
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
    }
}
