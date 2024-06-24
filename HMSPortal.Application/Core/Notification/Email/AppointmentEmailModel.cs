using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Notification.Email
{
	public class AppointmentEmailModel
	{
		public string PatientName { get; set; }
        public string location { get; set; }
        public DateTime Date { get; set; }
		public string Time { get; set; }
		public string LogoUrl { get; set; }
		public string BGImageUrl { get; set; }
	}
}
