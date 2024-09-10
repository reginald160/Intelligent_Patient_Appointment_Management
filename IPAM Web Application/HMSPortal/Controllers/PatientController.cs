using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.Model;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Numerics;

namespace HMSPortal.Controllers
{
	//[Authorize]
	[DisplayControllerName( displayName:"Patient")]
	public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IPatientServices _patientServices;

		private readonly IWebHostEnvironment _webHostEnvironment;

		public PatientController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IPatientServices patientServices)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
			_patientServices=patientServices;
		}



		public  async Task<IActionResult> Index()
        {
		
            var obj = await _patientServices.GetAllPatient();
            var data = new GetAllPatientViewModel { Patient = obj };

            return View(data);
        }


		
		public  async Task<IActionResult> Add()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public  async Task<IActionResult> Add(AddPatientViewModel patient)
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
				return View(patient);
			}

			if (_patientServices.CheckExistingPatient(patient.Email))
			{
				ModelState.AddModelError("error-V", "User with email " + patient.Email+ " already exist");

				return View(patient);
			}

			patient.ImageUrl = FileUpload.UploadFile(patient.Image);
			patient.Password = RandomHelper.GeneratePassword();
			var result = await _patientServices.CreatePatient(patient);
			//_unitOfWork.Patient.Add(patientModel);
			var patientId = Guid.Parse(result.Data.ToString());
			// Set a flag in TempData to indicate successful submission
			TempData["Success"] = "Patient record has been created successfully.";
			return RedirectToAction("Detail", "Patient", new { id = patientId });
		}

		public async Task<IActionResult> Edit(Guid id)
		{
			var obj = _patientServices.GetPatientById(id);
			obj.Countries = await GetCountriesAsync();
			return View(obj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(GetPatientViewModel patient)
		{
			if (ModelState.IsValid)
			{
				if (patient.Image != null)
				{
					patient.ImageUrl = FileUpload.UploadFile(patient.Image);
				}
				patient.Password = RandomHelper.GeneratePassword();
				var result = await _patientServices.UpdatePatient(patient);
				//_unitOfWork.Patient.Add(patientModel);
				TempData["Success"] = "Doctor record has been updated successfully.";
				return RedirectToAction(nameof(Detail), "Patient", new {id = patient.Id});
			}

			return View(patient);
		}
		public async Task<IActionResult> Detail(Guid id)
		{
			var obj = _patientServices.GetPatientById(id);
			return View(obj);
	
		}



		[HttpPost]
		public async Task<IActionResult> Detail(GetPatientViewModel patient)
		{
			if (ModelState.IsValid)
			{
				if(patient.Image != null )
				{
					patient.ImageUrl = FileUpload.UploadFile(patient.Image);
				}
				patient.Password = RandomHelper.GeneratePassword();
				var result = await _patientServices.UpdatePatient(patient);
				//_unitOfWork.Patient.Add(patientModel);
			}

			return View(patient);
		}



		[HttpDelete]
      
        public async Task<IActionResult> Delete(Guid id)
		{
			var response =  await _patientServices.DeletePatient(id);

			if(response.IsSuccessful)
			{
				return Json(new { success = true });

            }
			else
			{
                return Json(new { success = false });
            }

		}

		
		public async Task<List<SelectListItem>>  getCountries()
		{
			var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "country.json");

			using (var fs = System.IO.File.OpenText(filePath))
			{
				var data = await fs.ReadToEndAsync();
				var countries = data.Replace("\"", " ").Replace("{", " ").Replace("}", " ").Split(",");
				var selectedItems = new List<SelectListItem>();
				foreach (var item in countries)
				{
					var splitItem = item.Split(":");

					selectedItems.Add(new SelectListItem { Text = splitItem[1].Trim(), Value = splitItem[1].Trim() });
				}
				return selectedItems;
	
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetCountries()
		{
			try
			{
				var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "country.json");
				var json = await System.IO.File.ReadAllTextAsync(filePath);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		
		}

		[NonAction]
		public async  Task<List<Country>> GetCountriesAsync()
		{
			try
			{
				var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "country.json");
				var json = await System.IO.File.ReadAllTextAsync(filePath);
				List<Country> countries = JsonConvert.DeserializeObject<List<Country>>(json);
				return countries ;
			}
			catch (Exception ex)
			{
				return null;
			}

		}

	}
}
