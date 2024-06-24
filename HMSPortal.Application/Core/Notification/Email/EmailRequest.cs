using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Notification.Email
{
	public class EmailRequest
	{
		public EmailRequest()
		{
			Attachments = new List<IFormFile>();
		}
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public List<IFormFile> Attachments { get; set; }
	}
}
