using HMSPortal.Application.ViewModels.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.DataBank
{
	public static class AppointmentBank
	{
		public static List<AllAppointmentViewModel> GenerateRandomAppointments(int count)
		{
			var random = new Random();
			var doctorNames = new[] { "Dr. Smith", "Dr. Johnson", "Dr. Williams", "Dr. Brown", "Dr. Jones" };
			var patientNames = new[] { "John Doe", "Jane Doe", "Alice Smith", "Bob Johnson", "Charlie Brown" };
			var appointmentTypes = new[] { "Consultation", "Follow-up", "Surgery", "Check-up" };
			var statuses = new[] { "Scheduled", "Completed", "Cancelled", "No-show" };
			var departments = new[] { "Cardiology", "Neurology", "Orthopedics", "Pediatrics", "General Medicine" };

			DateTime RandomDate(DateTime start, DateTime end)
			{
				int range = (end - start).Days;
				return start.AddDays(random.Next(range));
			}

			DateTime RandomTime()
			{
				return DateTime.Today.AddHours(random.Next(8, 18)).AddMinutes(random.Next(0, 60));
			}

			var appointments = new List<AllAppointmentViewModel>();

			for (int i = 0; i < count; i++)
			{
				var appointment = new AllAppointmentViewModel
				{
					Id = Guid.NewGuid(),
					Date = RandomDate(new DateTime(2023, 1, 1), new DateTime(2024, 12, 31)),
					DoctorId = random.Next(2) == 0 ? Guid.NewGuid() : (Guid?)null,
					DoctorName = doctorNames[random.Next(doctorNames.Length)],
					UserId = Guid.NewGuid().ToString(),
					PatientId = random.Next(2) == 0 ? Guid.NewGuid() : (Guid?)null,
					PatientName = patientNames[random.Next(patientNames.Length)],
					StartTime = RandomTime(),
					Endtime = RandomTime(),
					ReferenceNumber = Guid.NewGuid().ToString().Substring(0, 10).Replace("-", ""),
					PatientRef = Guid.NewGuid().ToString().Substring(0, 10).Replace("-", ""),
					AppointmentType = appointmentTypes[random.Next(appointmentTypes.Length)],
					ProblemDescrion = random.Next(2) == 0 ? "Problem description here" : null,
					Prescriptions = random.Next(2) == 0 ? "Prescription details here" : null,
					Status = statuses[random.Next(statuses.Length)],
					File = random.Next(2) == 0 ? "File path here" : null,
					Rating = random.Next(2) == 0 ? random.Next(1, 6).ToString() : null,
					DoctorComment = random.Next(2) == 0 ? "Doctor comment here" : null,
					PatientComment = random.Next(2) == 0 ? "Patient comment here" : null,
					Department = departments[random.Next(departments.Length)]
				};

				appointments.Add(appointment);
			}

			return appointments;
		}
	}
}
