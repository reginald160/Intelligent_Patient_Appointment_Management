using HMSPortal.Domain.Models.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
	public class AppointmentEvents: AuditableEntity
	{
		public Guid? ObjectId { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
		public string? Actuator { get; set; }
        public string JobId { get; set; }

        public DateTime JobDate { get; set; }
        public DateTime JobNotificationTime { get; set; }
        public bool Status { get; set; }



    }
}
