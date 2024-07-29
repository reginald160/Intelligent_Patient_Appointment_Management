using AutoMapper;
using HMS.Infrastructure.Repositories.IRepository;
using HMS.Infrastructure.Repositories.Repository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Doctor;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HMSPortal.Controllers
{
	public class DoctorController : Controller
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IDoctorServices _DoctorServices;
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;

		
		public DoctorController(IWebHostEnvironment webHostEnvironment, IDoctorServices doctorServices, IMapper mapper, UserManager<ApplicationUser> userManager)
		{
			_webHostEnvironment = webHostEnvironment;
			_DoctorServices = doctorServices;
			_mapper = mapper;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
            var doctors = _DoctorServices.GetAllDoctors();
            var data = new GetAllDoctorsViewModel { Doctors = doctors };
            return View(data);

     
		}

        [HttpGet]
        public IActionResult Detail(Guid id)
        {
            var doctor= _DoctorServices.GetDoctorById(id);
            
            return View(doctor);


        }

		[Authorize]
		
		public async Task<IActionResult> Clocking()
		{
			var user = await _userManager.GetUserAsync(User);
			var result =  _DoctorServices.GetUserClockIn(user.Id);
			ViewBag.ClockInStatus = result ? "TRUE" : "FALSE";
			return View();
		}

		//[Authorize]
		//[ValidateAntiForgeryToken]
		[HttpGet]
		public async Task<IActionResult> ClockIn(string id)
		{
			var user = await _userManager.GetUserAsync(User);
			var result = await _DoctorServices.ClockInAsync(user.Id);

			var status = result.Split('@')[0];
			var message = result.Split('@')[1];

			if (status == "1")
			{
				return Json(new { success = true, message = message });

			}
			else
			{
				return Json(new { success = true, message = message });
			}

		}
		//[ValidateAntiForgeryToken]
		//[HttpPost("clockout")]
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> ClockOut(string id)
		{
			var user = await _userManager.GetUserAsync(User);
			var result = await _DoctorServices.ClockOutAsync(user.Id);
			var status = result.Split('@')[0];
			var message = result.Split('@')[1];

			if (status == "1")
			{
				return Json(new { success = true, message = message });

			}
			else
			{
				return Json(new { success = true, message = message });
			}
			
		}

		[HttpGet]
		public IActionResult Add()
		{
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public  async Task<IActionResult> Add(AddDoctorViewModel doctor)
		{
			if (!ModelState.IsValid)
			{
				List<string> errorMessages = new List<string>();

				foreach (var error in ModelState)
				{
					var sss = error.Value.Errors.Select(x => x.ErrorMessage).ToList();
					errorMessages.AddRange(sss);
				}

				// Combine all error messages into a single message
				string allErrorMessages = string.Join("; ", errorMessages);

				ModelState.AddModelError("error-V", allErrorMessages);
				return View(doctor);
			}

			if (_DoctorServices.CheckExistingDoctor(doctor.Email))
			{
				ModelState.AddModelError("error-V", "User with email " + doctor.Email+ " already exist");

				return View(doctor);
			}

			doctor.ImageUrl = FileUpload.UploadFile(doctor.Image);
			doctor.Password = RandomHelper.GeneratePassword();
			var result = await _DoctorServices.CreateDoctor(doctor);
			if (!result.IsSuccessful)
			{
				ModelState.AddModelError("error-V", result.Message);
				return View(doctor);
			}
			// Set a flag in TempData to indicate successful submission
			TempData["Success"] = "Doctor record has been created successfully.";
			var doctorId = Guid.Parse(result.Data.ToString());
			return RedirectToAction("Detail", "Doctor", new {id = doctorId });

		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var obj = _DoctorServices.GetDoctorById(id);
			var doctor = _mapper.Map<EditDoctorViewModel>(obj);

			return View(doctor);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditDoctorViewModel doctor)
		{
			if (ModelState.IsValid)
			{
				if (doctor.Image != null)
				{
					doctor.ImageUrl = FileUpload.UploadFile(doctor.Image);
				}
				
				 await _DoctorServices.UpdateDoctorAsync(doctor);
				//_unitOfWork.Patient.Add(patientModel);
				TempData["Success"] = "Doctor record has been updated successfully.";
				return RedirectToAction("Edit", "Doctor", new {id = doctor.Id });
			}

			return View(doctor);
		}

		[HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _DoctorServices.DeleteDoctor(id);

            if (response.IsSuccessful)
            {
                return Json(new { success = true });

            }
            else
            {
                return Json(new { success = false });
            }

        }
    }
}
