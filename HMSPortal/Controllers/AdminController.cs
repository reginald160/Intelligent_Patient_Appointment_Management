using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Admin;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
	public class AdminController : AdminBaseController
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IAdminServices _adminServices;

		public AdminController(UserManager<ApplicationUser> userManager, IAdminServices adminServices)
		{
			_userManager=userManager;
			_adminServices=adminServices;
		}

		[HttpGet]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> All()
        {

            var obj =  _adminServices.GetAllAdmin();
            

            return View(obj);
        }

        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Add()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(AddAdminViewModel admin)
		{


			if (_userManager.Users.Any(x=> x.Email == admin.Email))
			{
				ModelState.AddModelError("error-V", "User with email " + admin.Email+ " already exist");

				return View(admin);
			}

			admin.ImageUrl = FileUpload.UploadFile(admin.Image);
			var result = await _adminServices.CreateAdmin (admin);
			//_unitOfWork.Patient.Add(adminModel);
			var adminId = Guid.Parse(result.Data.ToString());
			// Set a flag in TempData to indicate successful submission
			TempData["Success"] = "Patient record has been created successfully.";
			return RedirectToAction("Detail", "Admin", new { id = adminId });
		}

		public async Task<IActionResult> Detail(Guid id)
		{
			var obj = _adminServices.GetAdminById(id);
			return View(obj);

		}

		[HttpPost]
		public async Task<IActionResult> Detail(GetPatientViewModel patient)
		{
			if (ModelState.IsValid)
			{
				if (patient.Image != null)
				{
					patient.ImageUrl = FileUpload.UploadFile(patient.Image);
				}
				patient.Password = RandomHelper.GeneratePassword();
				//var result = await _patientServices.UpdatePatient(patient);
				//_unitOfWork.Patient.Add(patientModel);
			}

			return View(patient);
		}

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(Guid id)
		{
			var obj = _adminServices.GetAdminById(id);
			return View(obj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(GetAdminViewModel admin)
		{
			if (ModelState.IsValid)
			{
				if (admin.Image != null)
				{
					admin.ImageUrl = FileUpload.UploadFile(admin.Image);
				}
				//patient.Password = RandomHelper.GeneratePassword();
				var result = await _adminServices.UpdateAdmin(admin);
				//_unitOfWork.Patient.Add(patientModel);
				TempData["Success"] = "Admin record has been updated successfully.";
				return RedirectToAction(nameof(Detail), "Admin", new { id = admin.Id });
			}

			return View(admin);
		}

        [HttpDelete]
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _adminServices.DeleteAdmin(id);

            if (response.IsSuccessful)
            {
                return Json(new { success = true });

            }
            else
            {
                return Json(new { success = false });
            }

        }
		[HttpDelete]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UnLock(Guid id)
        {
            var response = await _adminServices.UnLockAdmin(id);

            if (response.IsSuccessful)
            {
                return Json(new { success = true });

            }
            else
            {
                return Json(new { success = false });
            }

        }

        [HttpDelete]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Lock(Guid id)
        {
            var response = await _adminServices.LockAdmin(id);

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
