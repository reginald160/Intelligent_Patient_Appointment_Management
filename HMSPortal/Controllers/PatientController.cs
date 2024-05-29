using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.ViewModels;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMSPortal.Controllers
{
    //[Authorize]
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public PatientController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        

        public IActionResult Index()
        {
            var obj = _unitOfWork.Patient.GetAll().ToList();
            return View(obj);
        }


		[HttpGet]
		public IActionResult Upsert(AddPatientViewModel patient, Guid id)
		{
			
			if (id == Guid.Empty)
			{
				//Create
				return View(patient);
			}
			else
			{
				//Edit
				var obj = _unitOfWork.Patient.Get(x => x.Id == id);

				return View(obj);
			}
		}

		[HttpPost]
		public IActionResult Upsert(AddPatientViewModel? prodVM, IFormFile file, Guid id)
		{
			//prodVM.Patient.Id = Guid.NewGuid();
			//if (ModelState.IsValid)
			//{
				string wwwRoothPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string rootPath = Path.Combine(wwwRoothPath, @"images/patient");

					if (!string.IsNullOrEmpty(prodVM.Patient.ImageUrl))
					{
						var oldImg = Path.Combine(prodVM.Patient.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImg))
						{
							System.IO.File.Delete(oldImg);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(rootPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					prodVM.Patient.ImageUrl = @"images/patient" + fileName;
				}

				//var patientModel = new Patient
				//{
				//	FirstName = prodVM.FirstName,
				//	LastName = prodVM.LastName,
				//	PatientCode = prodVM.PostalCode,
				//	Phone = prodVM.PostalCode,
				//	DateOfBirth = prodVM.DateOfBirth,
				//	Address = prodVM.Address,
				//	Gender = prodVM.Gender,
				//	Email = prodVM.Email,
				//	PostalCode = prodVM.PostalCode,
				//	HouseNumber = prodVM.HouseNumber,
				//	ImageUrl = prodVM.ImageUrl,
				//};

				if (prodVM.Patient.Id == Guid.Empty)
				{
					_unitOfWork.Patient.Add(prodVM.Patient);
					_unitOfWork.Save();

					TempData["Success"] = "Patient created successfully";
					return RedirectToAction("Index");
				}
				else
				{

					_unitOfWork.Patient.Update(prodVM.Patient);
					_unitOfWork.Save();

					TempData["Success"] = "Patient updated successfully";
					return RedirectToAction("Index");
				}

			//}
			//else
			//{
			//	return View(prodVM);
			//}
		}






		//     public IActionResult Create()
		//     {
		//         return View();
		//     }

		//     [HttpPost]
		//     public IActionResult Create(AddPatientViewModel patientVM, IFormFile file)
		//     {
		//patientVM.Id = Guid.NewGuid();


		//if (patientVM != null && ModelState.IsValid)
		//         {
		//	string wwwRoothPath = _webHostEnvironment.WebRootPath;
		//	if (file != null)
		//	{
		//		string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
		//		string rootPath = Path.Combine(wwwRoothPath, @"images/patient");

		//		if (!string.IsNullOrEmpty(patientVM.ImageUrl))
		//		{
		//			var oldImg = Path.Combine(patientVM.ImageUrl.TrimStart('\\'));

		//			if (System.IO.File.Exists(oldImg))
		//			{
		//				System.IO.File.Delete(oldImg);
		//			}
		//		}

		//		using (var fileStream = new FileStream(Path.Combine(rootPath, fileName), FileMode.Create))
		//		{
		//			file.CopyTo(fileStream);
		//		}
		//		patientVM.ImageUrl = @"images/patient" + fileName;
		//	}

		//	var patientModel = new Patient
		//	{
		//		FirstName = patientVM.FirstName,
		//                 LastName = patientVM.LastName,
		//                 PatientCode = patientVM.PostalCode,
		//                 Phone = patientVM.PostalCode,
		//                 DateOfBirth = patientVM.DateOfBirth,
		//                 Address = patientVM.Address,
		//                 Gender = patientVM.Gender,
		//                 Email = patientVM.Email,
		//                 PostalCode = patientVM.PostalCode,
		//                 HouseNumber = patientVM.HouseNumber,
		//                 ImageUrl = patientVM.ImageUrl,  

		//	};

		//	_unitOfWork.Patient.Add(patientModel);
		//             _unitOfWork.Save();
		//             TempData["Success"] = "Patient created successfully";
		//             return RedirectToAction("Index");
		//         }
		//         return View(patientVM);
		//     }


		//     public IActionResult Edit(Guid? id)
		//     {
		//         if (id == null)
		//         {
		//             return NotFound();
		//         }
		//         var obj = _unitOfWork.Patient.Get(x => x.Id == id);

		//         if (obj == null)
		//         {
		//             return NotFound();
		//         }
		//         return View(obj);
		//     }

		//     [HttpPost]
		//     public IActionResult Edit(Patient obj)
		//     {

		//         if (ModelState.IsValid)
		//         {
		//             _unitOfWork.Patient.Update(obj);
		//             _unitOfWork.Save();
		//             TempData["Success"] = "Patient updated successfully";
		//             return RedirectToAction("Index");
		//         }
		//         return View(obj);
		//     }

		//public IActionResult Delete(Guid? id)
		//{
		//    if (id == null)
		//    {
		//        return NotFound();
		//    }
		//    var obj = _unitOfWork.Patient.Get(u => u.Id == id);
		//    if (obj == null)
		//    {
		//        return NotFound();
		//    }
		//    return View(obj);
		//}

		//[HttpPost, ActionName("Delete")]
		//public IActionResult DeletePost(Guid? id)
		//{
		//    var obj = _unitOfWork.Patient.Get(x => x.Id == id);
		//    if (obj == null)
		//    {
		//        return NotFound(id);
		//    }
		//    _unitOfWork.Patient.Remove(obj);
		//    _unitOfWork.Save();
		//    TempData["Success"] = "Patient deleted successfully";
		//    return RedirectToAction("Index");
		//}


		[HttpGet]
		public IActionResult GetAll()
		{
			List<Patient> prodList = _unitOfWork.Patient.GetAll().ToList();

			return Json(new { data = prodList });
		}

		
		public IActionResult Delete(Guid id)
		{
            var prodToDelete = _unitOfWork.Patient.Get(x => x.Id == id);
            if(prodToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagPath = Path.Combine(_webHostEnvironment.WebRootPath, prodToDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagPath))
            {
                System.IO.File.Delete(oldImagPath);
            }
            _unitOfWork.Patient.Remove(prodToDelete); 
            _unitOfWork.Save();

            return Json(new {success = true, message = "Delete Successful"});
		}

		public IActionResult Detail()
		{
			var obj = _unitOfWork.Patient.GetAll();

			return View(obj);
		}

	}
}
