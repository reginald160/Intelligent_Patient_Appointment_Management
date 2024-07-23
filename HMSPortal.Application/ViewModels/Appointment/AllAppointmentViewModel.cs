using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels.Appointment
{
	public class AllAppointmentViewModel
	{
        public Guid	Id { get; set; }
		public DateTime Date { get; set; }
		public Guid? DoctorId { get; set; }
		public string DoctorName { get; set; }
		public string UserId { get; set; }
		public Guid? PatientId { get; set; }
		public string PatientName { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime Endtime { get; set; }
		public string ReferenceNumber { get; set; }
		public string PatientRef { get; set; }
		public string TimeSlot { get; set; }
		public string AppointmentType { get; set; }
		public string? ProblemDescrion { get; set; }
		public string? Prescriptions { get; set; }
		public string? Status { get; set; }
		public string? File { get; set; }
		public string? Rating { get; set; }
		public string? DoctorComment { get; set; }
		public string? PatientComment { get; set; }
		public string? Department { get; set; }
	}
}
