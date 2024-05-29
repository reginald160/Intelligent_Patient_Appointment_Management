using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.ViewModels;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
    public class DoctorController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public DoctorController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			var obj = _unitOfWork.Doctor.GetAll().ToList();
			return View(obj);
		}


		[HttpGet]
		public IActionResult Upsert(AddDoctorViewModel patient, Guid id)
		{

			if (id == Guid.Empty)
			{
				//Create
				return View(patient);
			}
			else
			{
				//Edit
				var obj = _unitOfWork.Doctor.Get(x => x.Id == id);

				return View(obj);
			}
		}

		[HttpPost]
		public IActionResult Upsert(AddDoctorViewModel? prodVM, IFormFile file, Guid id)
		{
			//prodVM.Patient.Id = Guid.NewGuid();
			//if (ModelState.IsValid)
			//{
			string wwwRoothPath = _webHostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				string rootPath = Path.Combine(wwwRoothPath, @"images/patient");

				if (!string.IsNullOrEmpty(prodVM.Doctor.ImageUrl))
				{
					var oldImg = Path.Combine(prodVM.Doctor.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImg))
					{
						System.IO.File.Delete(oldImg);
					}
				}

				using (var fileStream = new FileStream(Path.Combine(rootPath, fileName), FileMode.Create))
				{
					file.CopyTo(fileStream);
				}
				prodVM.Doctor.ImageUrl = @"images/patient" + fileName;
			}

			

			if (prodVM.Doctor.Id == Guid.Empty)
			{
				_unitOfWork.Doctor.Add(prodVM.Doctor);
				_unitOfWork.Save();

				TempData["Success"] = "Patient created successfully";
				return RedirectToAction("Index");
			}
			else
			{

				_unitOfWork.Doctor.Update(prodVM.Doctor);
				_unitOfWork.Save();

				TempData["Success"] = "Patient updated successfully";
				return RedirectToAction("Index");
			}

		}

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Doctor> prodList = _unitOfWork.Doctor.GetAll().ToList();

			return Json(new { data = prodList });
		}

		public IActionResult Delete(Guid id)
		{
			var prodToDelete = _unitOfWork.Doctor.Get(x => x.Id == id);
			if (prodToDelete == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}
			var oldImagPath = Path.Combine(_webHostEnvironment.WebRootPath, prodToDelete.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagPath))
			{
				System.IO.File.Delete(oldImagPath);
			}
			_unitOfWork.Doctor.Remove(prodToDelete);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}

		public IActionResult Detail()
		{
			var obj = _unitOfWork.Doctor.GetAll();

			return View(obj);
		}
	}
}
