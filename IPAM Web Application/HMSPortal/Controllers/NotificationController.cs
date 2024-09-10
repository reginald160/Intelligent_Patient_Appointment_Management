using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.ViewModels.Notification;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
	public class NotificationController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly INotificatioServices notificatioServices;
        private readonly IJobScheduleService _jobScheduleService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ApplicationDbContext context, INotificatioServices notificatioServices, IJobScheduleService jobScheduleService, UserManager<ApplicationUser> userManager)
        {
            _context=context;
            this.notificatioServices=notificatioServices;
            _jobScheduleService=jobScheduleService;
            _userManager=userManager;
        }

        [Authorize]
        public async Task <IActionResult> Index()
        {
            var user =  await _userManager.GetUserAsync(User);
            var notifications = _context.Notifications.Where(x => x.UserId == user.Id)
                .Select(x=> new AllNotificationViewModel
                {
                    Id = x.Id,
                    Message = x.Message,
                    Date = DateTime.Now,
                    Status = x.Status
                }).ToList();
            return View(notifications);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        
        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Add(CreateNotificationViewmodel model)
		{
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime selectedDate = model.Date;
                    int selectedHour = model.Hour;
                    int selectedMinute = model.Minute;

                    // Combine date, hour, and minute to create a DateTime object
                    DateTime notificationDateTime = new DateTime(
                        selectedDate.Year,
                        selectedDate.Month,
                        selectedDate.Day,
                        selectedHour,
                        selectedMinute,
                        0 // seconds
                    );

                    // Ensure the datetime is in the future
                    if (notificationDateTime <= DateTime.Now)
                    {
                        ModelState.AddModelError("error-V", "The selected time must be in the future.");
                        return View(model);
                    }
					var user = await _userManager.GetUserAsync(User);
                    var notifications = _context.Notifications.Where(x => x.UserId == user.Id);

					model.Date = notificationDateTime;
                    model.NotificationTime = notificationDateTime;
                    model.UserId = user.Id;
                    await _jobScheduleService.ScheduleNotification(model);

                    TempData["Success"] = "Notification created successfully.";
                    return RedirectToAction(nameof(Index), "Notification");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("error-V", $"An error occurred while creating the notification: {ex.Message}");
                }
            }
			

			return View(model);
        }
    }
}
