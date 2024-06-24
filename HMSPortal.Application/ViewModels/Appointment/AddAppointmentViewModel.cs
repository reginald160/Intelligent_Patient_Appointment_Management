using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels.Appointment
{
	public class AddAppointmentViewModel
	{
		public DateTime Date { get; set; }
		public string DoctorId { get; set; }
		
		public string PatientId { get; set; }
	
		public string? TimeSlot { get; set; }

		public string? ProblemDescrion { get; set; }
		public string? Department { get; set; }
        public List<SelectListItem>? Patients { get; set; }
        public List<SelectListItem>? Doctors { get; set; }
    }
}
