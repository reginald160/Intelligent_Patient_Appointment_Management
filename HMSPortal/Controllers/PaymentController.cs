using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.ViewModels;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using HMSPortal.Domain.Enums;

namespace HMSPortal.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public PaymentController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			var obj = _unitOfWork.Payment.GetAll().ToList();
			return View(obj);
		}


		[HttpGet]
		public IActionResult Upsert( Guid id)
		{
			AddPaymentViewModel payment = new AddPaymentViewModel()
			{
				DoctorList = _unitOfWork.Doctor.GetAll().Select(c => new SelectListItem
				{
					Text = c.FirstName,
					Value = c.Id.ToString(),
				}),

                PatientList = _unitOfWork.Patient.GetAll().Select(c => new SelectListItem
                {
                    Text = c.FirstName,
                    Value = c.Id.ToString(),
                }),
                Payment = new Payment()

            };

            var dept = Enum.GetValues(typeof(Department)).Cast<Department>();
            var deptList = dept.Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            }).ToList();

            var PaymtType = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>();
            var PaymtList = PaymtType.Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            }).ToList();

            if (id == Guid.Empty)
			{

				//Create
				return View(payment);
			}
			else
			{
				//Edit
				var obj = _unitOfWork.Payment.Get(x => x.Id == id);

				return View(obj);
			}
		}
		
		[HttpPost]
		public IActionResult Upsert(AddPaymentViewModel? paymentVM, IFormFile file)
		{
			//prodVM.Patient.Id = Guid.NewGuid();
			//if (ModelState.IsValid)
			//{
			string wwwRoothPath = _webHostEnvironment.WebRootPath;
			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				string rootPath = Path.Combine(wwwRoothPath, @"images/patient");

				if (!string.IsNullOrEmpty(paymentVM.Payment.ImageUrl))
				{
					var oldImg = Path.Combine(paymentVM.Payment.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImg))
					{
						System.IO.File.Delete(oldImg);
					}
				}

				using (var fileStream = new FileStream(Path.Combine(rootPath, fileName), FileMode.Create))
				{
					file.CopyTo(fileStream);
				}
				paymentVM.Payment.ImageUrl = @"images/patient" + fileName;
			}



			if (paymentVM.Payment.Id == Guid.Empty)
			{
				_unitOfWork.Payment.Add(paymentVM.Payment);
				_unitOfWork.Save();

				TempData["Success"] = "Patient created successfully";
				return RedirectToAction("Index");
			}
			else
			{

				_unitOfWork.Payment.Update(paymentVM.Payment);
				_unitOfWork.Save();

				TempData["Success"] = "Patient updated successfully";
				return RedirectToAction("Index");
			}

		}

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Payment> prodList = _unitOfWork.Payment.GetAll().ToList();
			return Json(new { data = prodList });
		}


		public IActionResult Delete(Guid id)
		{
			var prodToDelete = _unitOfWork.Payment.Get(x => x.Id == id);
			if (prodToDelete == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}
			var oldImagPath = Path.Combine(_webHostEnvironment.WebRootPath, prodToDelete.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagPath))
			{
				System.IO.File.Delete(oldImagPath);
			}
			_unitOfWork.Payment.Remove(prodToDelete);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}

		public IActionResult Detail()
		{
			var obj = _unitOfWork.Payment.GetAll();

			return View(obj);
		}
	}
}
