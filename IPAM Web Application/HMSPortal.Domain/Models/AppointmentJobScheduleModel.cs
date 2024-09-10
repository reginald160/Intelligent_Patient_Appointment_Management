using HMSPortal.Domain.Models.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
	public class AppointmentJobScheduleModel: AuditableEntity
	{
        public Guid? AppointmentId { get; set; }
		[ForeignKey(nameof(AppointmentId))]
        public  AppointmentModel? Appointment { get; set; }
		public string? Title { get; set; }

		public DateTime AppointmentDate { get; set; }
		public bool IsNotifiedOneDayBefore { get; set; }
		public bool IsNotifiedTwentyMinutesBefore { get; set; }
		public bool NT1 { get; set; }
		public bool NT2 { get; set; }
		public bool NT3 { get; set; }
		public bool NT4 { get; set; }
		public bool NT5 { get; set; }

	}
}
