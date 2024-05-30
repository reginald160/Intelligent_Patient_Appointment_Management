using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMSPortal.Controllers
{
    public class PaymentInvoiceDetailController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IWebHostEnvironment _webHostEnvironment;
        //public PaymentInvoiceDetailController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        //{
        //    _unitOfWork = unitOfWork;
        //    _webHostEnvironment = webHostEnvironment;
        //}

        //public IActionResult Detail(Guid id)
        //{
        //    PaymentInvoice invoiceDetail = new PaymentInvoice()
        //    {
        //        Payment = _unitOfWork.Payment.Get(u => u.Id == id, includeProperties: "Patient" ),
        //    };

        //    return View(invoiceDetail);
        //}

        //[HttpPost]
        //public IActionResult Detail(PaymentInvoice paymentDetail)
        //{
        //    if (paymentDetail == null)
        //    {
        //        return NotFound();  
        //    }

        //    _unitOfWork.PaymentInvoice.Add(paymentDetail);
        //    TempData["success"] = "Cart updated successfully";
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));
        //}
        
    }
}
