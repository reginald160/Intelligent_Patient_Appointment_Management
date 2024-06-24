using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.AppSettings
{
	public class SMTPSettings
	{
		public string? Host { get; set; }
		public int Post { get; set; }
		public string? Password { get; set; }
		public string? EmailAddress { get; set; }
		public bool? EnableSSl { get; set; }
		public string? sendgridKey { get; set; }
		public string? sendgridSender { get; set; }
		public string sendgridName { get; set; }
	}
}
