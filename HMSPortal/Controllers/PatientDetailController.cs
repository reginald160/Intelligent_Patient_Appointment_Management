//using HMS.Infrastructure.Repositories.IRepository;
//using HMSPortal.Application.ViewModels;
//using Microsoft.AspNetCore.Mvc;

//namespace HMSPortal.Controllers
//{
//	public class PatientDetailController : Controller
//	{
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IWebHostEnvironment _webHostEnvironment;
//        public PatientDetailController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
//        {
//            _unitOfWork = unitOfWork;
//            _webHostEnvironment = webHostEnvironment;
//        }
//        public IActionResult Detail()
//        {
//            AddPaymentViewModel payment = new AddPaymentViewModel()
//            {
//                var obj = _unitOfWork.Patient.GetAll();

//                return View(obj);
//            };
            
//        }
//    }
//}
