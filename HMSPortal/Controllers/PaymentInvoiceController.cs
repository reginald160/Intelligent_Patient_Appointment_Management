using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HMSPortal.Controllers
{
    public class PaymentInvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PaymentInvoiceController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Detail(Guid id)
        {
            
                var payment = _unitOfWork.PaymentInvoice.Get(u => u.Id == id, includeProperties: "PaymentInvoice.Patient");
			var payments = new List<PaymentInvoice> { payment };

            if (payments == null)
            {
                return NotFound();
            }
			return View(payments);
        }

        [HttpPost]
        public IActionResult Detail(PaymentInvoice paymentDetail)
        {
            if (paymentDetail == null)
            {
                return NotFound();
            }

            _unitOfWork.PaymentInvoice.Add(paymentDetail);
            TempData["success"] = "Cart updated successfully";
            _unitOfWork.Save();
            return View();
        }
    }
}
