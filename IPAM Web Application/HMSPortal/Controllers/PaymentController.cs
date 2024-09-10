using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.AppServices.IServices;
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
		private readonly IDoctorServices _doctorServices;

        public PaymentController(IDoctorServices doctorServices)
        {
            _doctorServices=doctorServices;
        }

        public async Task<IActionResult> Index()
		{
            await _doctorServices.CreateDoctor(GenerateFakeDoctor());
			return View();
		}


        private static IFormFile CreateFakeImage()
        {
            var content = "This is a fake file content";
            var fileName = "doctor-image.jpg";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "image", fileName);
        }
        public static AddDoctorViewModel GenerateFakeDoctor()
        {
            var random = new Random();

            return new AddDoctorViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = DateTime.Now.Ticks.ToString()+ ".doe@example.com",
                Phone = "1234567890",
                PostalCode = "12345",
                HouseNumber = "42",
                Address = "123 Main St, Anytown, USA",
                DateOfBirth = DateTime.Now.AddYears(-35).AddDays(-random.Next(365)),
                Gender = random.Next(2) == 0 ? "Male" : "Female",
                DoctorCode = "DOC" + random.Next(1000, 9999).ToString(),
                BackgroundHistory = "Graduated from Medical University with honors. Residency at City Hospital.",
                Specialty = "Cardiology",
                YearsOfExperience = random.Next(1, 20),
                Age = 35,
                DoctorDetails = "Specializes in non-invasive cardiac procedures. Published research on heart disease prevention.",
                Image = CreateFakeImage(),
                Password = "P@ssw0rd123!",
                ImageUrl = "https://example.com/doctor-image.jpg"
            };
        }

        // Assuming you have the necessary using statements for your project

  

}


}
