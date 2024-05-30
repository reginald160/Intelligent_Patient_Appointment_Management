using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels
{
	public class AddPaymentViewModel
	{
		public Payment? Payment { get; set; }
		public IEnumerable<SelectListItem>? DoctorList { get; set; }
		public IEnumerable<SelectListItem>? PatientList { get; set; }
		public IEnumerable<SelectListItem>? Departments { get; set; }
		public IEnumerable<SelectListItem>? PaymentType { get; set; }
	}
}
