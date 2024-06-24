
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSPortal.Domain.Models
{
    public class Patient : BaseIndividual
    {
		[ValidateNever]
		public string? ImageUrl { get; set; }
		public string? PatientCode { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        [NotMapped]
        public double BMI { get; set; }
		public ICollection<AppointmentModel>? Appointments { get; set; } 

	}
}
